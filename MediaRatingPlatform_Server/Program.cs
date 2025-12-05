using Npgsql;
using System.Data.Common;
using MediaRatingPlatform_BusinessLogicLayer;
using MediaRatingPlatform_Server;
using MediaRatingPlatform_DataAccessLayer.Repositories;
using MediaRatingPlatform_BusinessLogicLayer.Repositories;

DBConnection dbConnection = new DBConnection("Host=localhost;Port=5432;Username=mrpdatabase;Password=mysecretpassword;Database=mrpdatabase");

HttpServer httpServer = new HttpServer("http://localhost:8080/");
await httpServer.Start();



