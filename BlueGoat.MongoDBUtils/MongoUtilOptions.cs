using System.CommandLine;
using MongoDB.Bson;

namespace BlueGoat.MongoDBUtils
{
    public static  class MongoUtilOptions
    {
        internal static Option<string> Connection = new Option<string>("--connection", "MongoDB connection String") { IsRequired = true };
        internal static Option<string> DatabaseName = new Option<string>("--databaseName", "MongoDB Database Name") {IsRequired = true};
        internal static Option<string> Version = new Option<string>("--version", "Optional Target Version");
        internal static Option<FileInfo> OutFilePath = new Option<FileInfo>("--out", description: "Output File Path") { IsRequired = true };
        internal static Option<FileInfo> InFilePath = new Option<FileInfo>("--in", description: "Input File Path") { IsRequired = true };
        internal static Option<bool> ForceOption = new Option<bool>("--force", description: "Skip Prompts");
        internal static Option<FileInfo> MigrationAssembly = new Option<FileInfo>("--migration-assembly") { IsRequired = true};
        internal static Option<GuidRepresentation?> GuidRepresentation = new Option<GuidRepresentation?>("--guid", 
            description: "Sets the desired MongoDB Guid Representation") { IsRequired = false };
        internal static Option<GuidRepresentationMode?> GuidMode = new Option<GuidRepresentationMode?>("--guid-mode", 
            description: "Sets the desired MongoDB Guid Representation Mode") { IsRequired = false};

        static MongoUtilOptions()
        {
            Connection.AddAlias("-c");
            DatabaseName.AddAlias("-db");
            Version.AddAlias("-v");
            OutFilePath.AddAlias("-out");
            InFilePath.AddAlias("-in");
            ForceOption.AddAlias("-f");
            MigrationAssembly.AddAlias("-ma");
        }

    }
}
