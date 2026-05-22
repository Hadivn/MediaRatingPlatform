using Npgsql;
using System.Data.Common;
using MediaRatingPlatform_BusinessLogicLayer;
using MediaRatingPlatform_Server;
using MediaRatingPlatform_DataAccessLayer;
using MediaRatingPlatform_BusinessLogicLayer.Repositories;


string connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
DBConnection dbConnection = new DBConnection(connectionString);
await dbConnection.ConnectToDatabaseAsync();
await dbConnection.InitializeDatabase();

HttpServer httpServer = new HttpServer("http://localhost:8080/");
await httpServer.Start();


// devops test 1
// devops test 2
// devops test 3
// dev test
