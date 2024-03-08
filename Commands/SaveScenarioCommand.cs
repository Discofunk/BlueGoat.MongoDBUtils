﻿using System.CommandLine;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace BlueGoat.MongoDBUtils.Commands;

public class SaveScenarioCommand : Command
{
    public SaveScenarioCommand() : base("save", "Export current DB state as a scenario to disk")
    {
        AddOption(MongoUtilOptions.DatabaseName);
        AddOption(MongoUtilOptions.OutFilePath);
        this.SetHandler(SaveScenario, MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName, MongoUtilOptions.OutFilePath, MongoUtilOptions.ForceOption);
    }

    private void SaveScenario(string connection, string databaseName, FileInfo filePath, bool force)
    {
        ConsoleEx.WriteLine("Save Scenario Started");

        var dbSize = HealthService.GetSize(connection, databaseName);
        if (!force && dbSize > Parameters.LargeFileSizeWarningThresholdBytes)
        {
            ConsoleEx.WriteWarn($"The database size is larger than {Parameters.LargeFileSizeWarningThresholdInMegaBytes}MB and may result in long save times or timeouts.  Continue? [Y]es / [N]o: ");
            var response = Console.ReadLine()?.ToUpper();
            if (response != "Y") return;
        }

        if (!force && filePath.Exists)
        {
            ConsoleEx.WriteWarn($"File {filePath} already exists. Overwrite? [Y]es /[N]o: ");
            var overWriteInput = Console.ReadLine()?.ToUpper();
            if (overWriteInput != "Y") return;
            filePath.Delete();
        }

        var client = new MongoClient(connection);
        var db = client.GetDatabase(databaseName);
        var options = new ListCollectionNamesOptions()
        {
            Filter = new JsonFilterDefinition<BsonDocument>("{ $and: [ { type: {$ne: 'view'}}, { name: {$regex: '^(?!_)'}}, { name: {$ne: 'system.views' }} ] }")
        };
        var collectionNames = db.ListCollectionNames(options).ToList();
        var root = new BsonDocument();
        foreach (var collectionName in collectionNames)
        {
            var collection = db.GetCollection<BsonDocument>(collectionName);
            var collectionData = collection.Find(new BsonDocument()).ToList();
            var array = new BsonArray(collectionData);
            var e = new BsonElement(collectionName, array);
            root.Add(e);
        }

        filePath.Directory?.Create();
        var json = root.ToJson(new JsonWriterSettings() { Indent = true, OutputMode = JsonOutputMode.Shell });
        using (var fileStream = filePath.Open(FileMode.OpenOrCreate, FileAccess.Write))
        {
            using (var sw = new StreamWriter(fileStream))
            {
                sw.WriteLine(json);
            }
        }

        ConsoleEx.WriteLineOk($"Scenario Saved");
        ConsoleEx.WriteLine(filePath.ToString());
    }
}