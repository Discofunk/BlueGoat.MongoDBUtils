using System.CommandLine;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlueGoat.MongoDBUtils;

public class LoadScenarioCommand : Command
{
    public LoadScenarioCommand() : base("load", "Load scenario from disk")
    {
        AddOption(MongoUtilOptions.DatabaseName);
        AddOption(MongoUtilOptions.InFilePath);
        AddOption(MongoUtilOptions.ForceOption);
        this.SetHandler(LoadScenario, MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName, MongoUtilOptions.InFilePath, MongoUtilOptions.ForceOption);
    }

    private void LoadScenario(string connection, string databaseName, FileInfo filePath, bool forceDeleteCollectionData)
    {
        if (!filePath.Exists)
        {
            ConsoleEx.WriteLineError($"File {filePath} does not exist");
            return;
        }

        var client = new MongoClient(connection);
        
        var db = client.GetDatabase(databaseName);
        var rawBsonAsJson = File.ReadAllText(filePath.FullName);
        var document = BsonDocument.Parse(rawBsonAsJson);
        foreach (var element in document.Elements)
        {
            var collectionName = element.Name;
            var data = element.Value;
            var collection = db.GetCollection<BsonDocument>(collectionName);
            if (!forceDeleteCollectionData)
            {
                var collectionCount = collection.EstimatedDocumentCount();
                if (collectionCount > 0)
                {
                    ConsoleEx.WriteWarn($"Collection \"{collectionName}\" contains existing data. Delete existing data first? [Y]es / [N]o / [A]ll: ");
                    var response = Console.ReadLine()?.ToUpper();
                    if (response == "A")
                    {
                        forceDeleteCollectionData = true;
                    }
                    else if (response != "Y") continue;
                }
            }
            collection.DeleteMany(new BsonDocument());
            if (data is BsonArray dataArray)
            {
                if (dataArray.Count == 0) continue;
                var documents = dataArray.ToList();
                collection.InsertMany(documents.ConvertAll(x => (BsonDocument)x));
                ConsoleEx.WriteLineInfo($"Loaded {documents.Count()} into \"{collectionName}\"");
            }
        }
        ConsoleEx.WriteLineOk("Scenario Load Completed");
    }
}