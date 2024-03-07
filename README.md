# Mongo Utils

This is a work in progress and has not had any production exposure.  It was made as a tool that I thought might be useful for me.

### Thanks

This utility uses the MongoDBMigrations project - https://bitbucket.org/i_am_a_kernel/mongodbmigrations

### Install Mongo Utils Tool

To install the Mongo Utils tool run `dotnet tool restore`

Verify by running `dotnet mongo-utils -?`

Mongo Utils comes with help:

```console
dotnet mongo-utils -h
```

```console
Description:
  MongoDB Utilities

Usage:
  mongo-utils [command] [options]

Options:
  -c, --connection <connection> (REQUIRED)  MongoDB connection String
  --version                                 Show version information
  -?, -h, --help                            Show help and usage information

Commands:
  migrate   Run MongoDB migration
  drop      Drop a MongoDB database
  reset     Drop database and run all migrations
  health    Runs a Health Check against a MongoDB instance
  scenario  Scenario Commands
```

### Run Migration Example

```console
dotnet mongo-utils migrate -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 --databaseName "OrgNamespace_API_Core" -ma  .\OrgNamespace.Domain.MongoDb.Migrations\bin\Debug\net6.0\OrgNamespace.Domain.MongoDb.Migrations.dll
```

### Drop a MongoDB database example

```console
dotnet mongo-utils drop -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "OrgNamespace_API_Core"
```

### Drop database and run all migrations

```console
 dotnet mongo-utils reset -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 --db "OrgNamespace_API_Core" -ma  .\OrgNamespace.Domain.MongoDb.Migrations\bin\Debug\net6.0\OrgNamespace.Domain.MongoDb.Migrations.dll
```

### Run a Health Check agaisnt a MongoDB Database example

```console
dotnet mongo-utils health -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "OrgNamespace_API_Core"
```

### Save the current DB state into a Scenario file example

```console
dotnet mongo-utils scenario save -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db OrgNamespace_API_Core --out .\ModelData\BasicScenario.json
```

### Load a saved scenario back into the DB example

```console
dotnet mongo-utils scenario load -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db OrgNamespace_API_Core --in .\ModelData\BasicScenario.json
```
