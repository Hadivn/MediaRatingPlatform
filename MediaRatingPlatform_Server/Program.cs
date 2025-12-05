using Npgsql;
using System.Data.Common;
using MediaRatingPlatform_BusinessLogicLayer;
using MediaRatingPlatform_Server;
using MediaRatingPlatform_DataAccessLayer;
using MediaRatingPlatform_BusinessLogicLayer.Repositories;

DBConnection dbConnection = new DBConnection("Host=localhost;Port=5432;Username=mrpdatabase;Password=user;Database=mrpdatabase");
await dbConnection.ConnectToDatabaseAsync();
await dbConnection.InitializeDatabase();

HttpServer httpServer = new HttpServer("http://localhost:8080/");
await httpServer.Start();



