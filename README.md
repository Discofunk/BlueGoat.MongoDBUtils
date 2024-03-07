# BlueGoat Mongo Utils

This .NET Tool provides some utilities that can be useful during a project's development phase when working with MongoDB and can do the following:

- Dropping a DB
- Checking the health of a DB
- Running a [MongoDBMigrations](https://bitbucket.org/i_am_a_kernel/mongodbmigrations) migration
- Saving/Loading scenario data

As testing aid, you can save the current state of the DB as a scenario JSON file and load it back in again. This is useful for quickly testing against various data states.

> ⚠ Warning: This is not intended as a way to back up / restore large amounts of data but rather manage various light-weight scenarios for a small amount of data. The scenario commands will attempt to dump / restore the entire DB using an inefficient JSON file format.

This is a work in progress and has not had any production exposure but I thought I would share it as it has been useful for me.

### Thanks

This utility uses the excellent MongoDBMigrations project - https://bitbucket.org/i_am_a_kernel/mongodbmigrations

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

> Note: Local installation must use the `dotnet` command first

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

#### Drop a MongoDB database example

```console
dotnet mongo-utils drop -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB"
```

#### Drop database and run all migrations

```console
 dotnet mongo-utils reset -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB" -ma  .\MongoDb.Migrations.dll
```

#### Run a Health Check agaisnt a MongoDB Database example

```console
dotnet mongo-utils health -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB"
```

#### Save the current DB state into a Scenario file example

```console
dotnet mongo-utils scenario save -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB" --out .\ModelData\BasicScenario.json
```

#### Load a saved scenario back into the DB example

```console
dotnet mongo-utils scenario load -c mongodb://root:BlueGoatsFlyFaster@localhost:27017 -db "MyMongoDB" --in .\ModelData\BasicScenario.json
```
