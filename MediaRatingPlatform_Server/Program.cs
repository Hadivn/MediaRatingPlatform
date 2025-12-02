using Npgsql;
using System.Data.Common;
using MediaRatingPlatform_BusinessLogicLayer;
using MediaRatingPlatform_Server;
using MediaRatingPlatform_DataAccessLayer.Repositories;
using MediaRatingPlatform_BusinessLogicLayer.Repositories;

HttpServer httpServer = new HttpServer("http://localhost:8080/");
await httpServer.Start();



