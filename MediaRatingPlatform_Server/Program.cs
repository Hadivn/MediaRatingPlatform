using Npgsql;
using System.Data.Common;
using MediaRatingPlatform_BusinessLogicLayer;
using MediaRatingPlatform_Server;




HttpServer httpServer = new HttpServer("http://localhost:8080/");
await httpServer.Start();



