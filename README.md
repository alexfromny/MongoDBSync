# MongoDB two-way database synchronization solution

This is a single ASP.NET Core Web Application where implemented client(offline server) and server(cloud) and synchronization process.
Current solution help to solve the sync data problem beetwen client and cloud when they don't have a permanent internet connection. 

## How it works

The idea of synchronization is to log all change commands to MongoDB. They are insert, update and delete commands. In this approach we should implement a soft delete to avoid conflicts while synchronization. Soft delete is not deleting a row from a table, but only marking it as Deleted(status = deleted). Synchronization is done merging differences in CloudCommandJournal and ClientCommandJournal, then ordering them by timestamp and executing one by one. If command was executed in cloud and on client we can mark this command as Synced.
Client application is responsible for synchronization. Cloud knows nothing about a client.


### Command schema sample

```

{
   insert: <collection>,
   documents: [ <document>, <document>, ... ]
}

```

## How to start

Copy 2 times this repository, one for client and one for cloud.

Cloud settings:

- MongoDBSync.WebAPI\Properties\launchSettings.json

-- "applicationUrl": "https://localhost:5001;http://localhost:5000"

- MongoDBSync.WebAPI\appsettings.json

-- "RunSyncService": false

-- "CloudUrl": "https://localhost:5001"

Client settings:
- MongoDBSync.WebAPI\Properties\launchSettings.json

-- "applicationUrl": "https://localhost:5003;http://localhost:5002"

- MongoDBSync.WebAPI\appsettings.json

-- "RunSyncService": true

-- "CloudUrl": "https://localhost:5001"


To start application go to the project folder in cmd and run next command.


```
dotnet run
```

It will start the application in a console mode. 

## How to use

There are 2 endpoint:
* 'api/user/add' - adds a new user
* 'api/user/getall' - returns all users from db.

By opening url: https://localhost:5001/api/user/getall , cloud will return all users from a cloud's db.


## Summary

Pros:
-Full synchronization

-Stable and scalable architecture

-MongoDB


Cons:
-Item should have 2 Ids

-Soft delete

-Command journal can be big, if synchronization process is not frequent.


## License

[![License](http://img.shields.io/:license-mit-blue.svg?style=flat-square)](http://badges.mit-license.org)

- **[MIT license](http://opensource.org/licenses/mit-license.php)**
