# BlueGoat Mongo Utils

This .NET Tool provides some utilities that can be useful during a project's development phase when working with MongoDB and can do the following:

- Dropping a DB
- Checking the health of a DB
- Running a [MongoDBMigrations](https://bitbucket.org/i_am_a_kernel/mongodbmigrations) migration
- Saving/Loading scenario data

As a testing aid, you can save the current state of the DB as a scenario JSON file and load it back in again. This is useful for quickly testing against various data states and sharing data states with the team.

> ⚠ Warning: This is not intended as a way to back up / restore large amounts of data but rather manage various light-weight scenarios for a small amount of data. The scenario commands will attempt to dump / restore the entire DB using an inefficient JSON file format.

This is a work in progress and has not had any production exposure but I thought I would share it as it has been useful for me.

### Thanks

This utility uses the excellent MongoDBMigrations project - https://bitbucket.org/i_am_a_kernel/mongodbmigrations

### Current Version and Dependency Versions

Mongo Utils (this): `0.0.10`
MongoDBMigrations: `2.2.0`
MongoDB.Driver: `2.24.0`

Release Notes: https://github.com/Discofunk/BlueGoat.MongoDBUtils/releases

### Limitations

Currently dependencies are packaged with this utility at a specific version which may cause issues if you are using an older MongoDBMigrations version or your MongoDB database is not compatible with the current driver being used.

### What's New?

Added option to select MongoDB `GuidRepresentationMode` and `GuidRepresentation` when loading & saving scenario data to maintain expected format after loading. For more information see https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/serialization/guid-serialization/

Examples:

```console
mongo-utils scenario load -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB" --in .\ModelData\BasicScenario.json --guid-mode V3
```

### Installing Mongo Utils Tool

#### Global Installation

To install the Mongo Utils tool run globally:

```
dotnet tool install --global BlueGoat.MongoUtils
```

Verify it is installed by running `mongo-utils -?`

#### Local Installation

To install the Mongo Utils locally for a given project:

```
dotnet new tool-manifest #creates a tool manifest that can be shared with the team
dotnet tool install --local BlueGoat.MongoUtils --version 0.0.3
```

Verify install by running `dotnet mongo-utils -?`

> Note: Local installations must prefix with the `dotnet` command to run

### Help

Mongo Utils comes with help:

```console
mongo-utils -h
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

### Examples

#### Run Migration Example

```console
mongo-utils migrate -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB" -ma  .\MongoDb.Migrations.dll
```

#### Run Migration To Given Version Example

```console
mongo-utils migrate -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB" -ma  .\MongoDb.Migrations.dll --version 1.0.2
```

#### Drop a MongoDB database example

```console
mongo-utils drop -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB"
```

#### Drop database and run all migrations

```console
 mongo-utils reset -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB" -ma  .\MongoDb.Migrations.dll
```

#### Run a Health Check agaisnt a MongoDB Database example

```console
mongo-utils health -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB"
```

#### Save the current DB state into a Scenario file example

```console
mongo-utils scenario save -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB" --out .\ModelData\BasicScenario.json
```

#### Load a saved scenario back into the DB example

```console
mongo-utils scenario load -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB" --in .\ModelData\BasicScenario.json
```

#### Save the current DB state into a Scenario file with selected Guid Mode example

```console
mongo-utils scenario save -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB" --out .\ModelData\BasicScenario.json --guid-mode V3
```

#### Load a saved scenario back into the DB with selected Guid Mode example

```console
mongo-utils scenario load -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB" --in .\ModelData\BasicScenario.json --guid-mode V3
```
