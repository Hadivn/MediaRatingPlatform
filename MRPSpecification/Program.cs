using Npgsql;
using System.Data.Common;
using MediaRatingPlatform_BusinessLogicLayer;
using MediaRatingPlatform_Server;

string connString = "Host=localhost;Port=5432;Username=mrpdatabase;Password=user;Database=mrpdatabase";
DatabaseService dbService = new DatabaseService(connString);
await dbService.TestDatabaseAsync();

HttpServer httpServer = new HttpServer("http://localhost:8080/");
await httpServer.Start();



