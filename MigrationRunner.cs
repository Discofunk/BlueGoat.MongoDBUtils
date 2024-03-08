using System.Reflection;
using MongoDBMigrations;
using MongoDBMigrations.Core;

namespace BlueGoat.MongoDBUtils
{
    public interface IMigrationRunner
    {
        void RunMigrations(Assembly migrationsAssembly, string connectionString, string databaseName, string? version = null, Action<InterimMigrationResult>? progressAction = null);
    }


    public class MigrationRunner : IMigrationRunner
    {
        public void RunMigrations(Assembly migrationsAssembly, string connectionString, string databaseName, string? version = null, Action<InterimMigrationResult>? progressAction = null)
        {
            var runner = new MigrationEngine()
                .UseDatabase(connectionString, databaseName)
                .UseAssembly(migrationsAssembly)
                .UseSchemeValidation(false, "");

            if (progressAction != null)
            {
                runner.UseProgressHandler(progressAction);
            }

            if (version != null)
            {
                runner.Run(version);
            }
            else
            {
                runner.Run();
            }
        }
    }
}
