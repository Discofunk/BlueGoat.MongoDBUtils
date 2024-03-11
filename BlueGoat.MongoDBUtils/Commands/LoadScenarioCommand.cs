﻿using System.CommandLine;
using MongoDB.Bson;

namespace BlueGoat.MongoDBUtils.Commands;

public class LoadScenarioCommand : Command
{
    private readonly IMongoClientFactory clientFactory;
    private readonly IConsole console;

    public LoadScenarioCommand(IMongoClientFactory clientFactory, IConsole console) : base("load", "Load scenario from disk")
    {
        this.clientFactory = clientFactory;
        this.console = console;
        AddOption(MongoUtilOptions.DatabaseName);
        AddOption(MongoUtilOptions.InFilePath);
        AddOption(MongoUtilOptions.ForceOption);
        this.SetHandler((connection, databaseName, filePath, force) => 
            LoadScenario(connection, databaseName, filePath, force), 
            MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName, MongoUtilOptions.InFilePath, MongoUtilOptions.ForceOption
        );
    }

    private Result LoadScenario(string connection, string databaseName, FileInfo filePath, bool force)
    {
        if (!filePath.Exists)
        {
            console.WriteLineError($"File {filePath} does not exist");
            return Result.Error;
        }

        if (!force && filePath.Length > Parameters.LargeFileSizeWarningThresholdBytes)
        {
            console.WriteWarn($"The selected file is larger than {Parameters.LargeFileSizeWarningThresholdInMegaBytes}MB and may result in long loading time or timeouts.  Continue? [Y]es / [N]o: ");
            var response = console.ReadLine()?.ToUpper();
            if (response != "Y") return Result.Cancelled;
        }

        var client = clientFactory.GetClient(connection);

        var db = client.GetDatabase(databaseName);
        var rawBsonAsJson = File.ReadAllText(filePath.FullName);
        var document = BsonDocument.Parse(rawBsonAsJson);
        foreach (var element in document.Elements)
        {
            var collectionName = element.Name;
            var data = element.Value;
            var collection = db.GetCollection<BsonDocument>(collectionName);
            if (!force)
            {
                var collectionCount = collection.EstimatedDocumentCount();
                if (collectionCount > 0)
                {
                    console.WriteWarn($"Collection \"{collectionName}\" contains existing data. Delete existing data first? [Y]es / [N]o / [A]ll: ");
                    var response = console.ReadLine()?.ToUpper();
                    if (response == "A")
                    {
                        force = true;
                    }
                    else if (response != "Y") continue;
                    collection.DeleteMany(new BsonDocument());
                }
            }
            
            if (data is BsonArray dataArray)
            {
                if (dataArray.Count == 0) continue;
                var documents = dataArray.ToList();
                collection.InsertMany(documents.ConvertAll(x => (BsonDocument)x));
                console.WriteLineInfo($"Loaded {documents.Count()} into \"{collectionName}\"");
            }
        }
        console.WriteLineOk("Scenario Load Completed");
        return Result.Success;
    }
}