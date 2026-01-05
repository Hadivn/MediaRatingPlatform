using MediaRatingPlatform_BusinessLogicLayer;
using MediaRatingPlatform_Domain.DTO;
using MediaRatingPlatform_Domain.Entities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Server
{
    internal class HttpServer
    {
        private UserService _userService;
        private MediaService _mediaService;
        private TokenService _tokenService = new TokenService();
        private HttpListener _listener;
        // method depending on endpoint.
        private Dictionary<(string path, string method), Func<HttpListenerContext, Task>> _routes;
        public HttpServer(string prefix)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            _userService = new UserService(_tokenService);
            _mediaService = new MediaService();

            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);

            _routes = new Dictionary<(string path, string method), Func<HttpListenerContext, Task>>
            {
                // Login and Register endpoints
                { ("/register", "POST"), RegisterHandlerAsync },
                { ("/login", "POST"), LoginHandlerAsync },
                // CRUD-Media endpoints
                // ich hätte nur einmal /media machen sollen und dann POST, GET, PUT, DELETE
                { ("/Media", "POST"), CreateMediaHandlerAsync },
                { ("/Media", "GET"), ReadAllMediaHandlerAsync },
                { ("/Media", "PUT"), UpdateMediaHandlerAsync },
                { ("/Media", "DELETE"), DeleteMediaHandlerAsync},
                // Media endpoints
                { ("/rateMedia", "POST"), RateMediaHandlerAsync},
                { ("/rateMedia", "GET"), ReadAllRateMediaHandlerAsync},
                { ("/rateMedia", "PUT"), UpdateRateMediaHandlerAsync},
                { ("/rateMedia", "DELETE"), DeleteRateMediaHandlerAsync},
                { ("/likeRating", "POST"), LikeCommentHandlerAsync},
                // Favorite endpoints
                {  ("/favoriteMedia", "POST"), FavoriteMediaHandlerAsync },
                { ("/favoriteMedia", "GET"), GetFavoritesMediaHandlerAsync},
                { ("/favoriteMedia", "DELETE"), UnfavoriteMediaHandlerAsync},
                // statistics endpoints
                {  ("/personalStats", "GET"), PersonalStatsHandlerAsync },
                {  ("/mediaStats", "GET"), MediaStatsHandlerAsync },
                {  ("/ratingHistory", "GET"), RatingHistoryHandlerAsync },
                // User endpoints
                 { ("/getUserById", "GET"), GetUserByIdHandlerAsync},
                 { ("/getUserByUsername", "GET"), GetUserByUsernameHandlerAsync}

            };


        }

        public async Task Start()
        {
            try
            {
                _listener.Start();
                Console.WriteLine("Server started");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Server failed to start: " + ex.Message);
            }


            while (true)
            {
                // waits for HTTP-Request from client.
                HttpListenerContext context = await _listener.GetContextAsync();
                // represents the incoming HTTP-Request from client to server
                HttpListenerRequest request = context.Request;
                // represents the outgoing HTTP-Answer from server to client
                HttpListenerResponse response = context.Response;

                string path = request.Url.AbsolutePath;
                string method = request.HttpMethod.ToUpper();
                Console.WriteLine($"[{DateTime.Now}] {request.HttpMethod} {path}");

                var key = (path, method);
                // check if route exists then call handler
                // what does the if mean here?
                // it means: if the key exists in the dictionary, assign the corresponding value to handler
                if (_routes.TryGetValue(key, out var handler))
                {
                    await handler(context);
                }
                else
                {
                    WriteResponse(response, "404 Not Found", "text/plain");
                }

            }
        }

        public void Stop()
        {
            _listener.Stop();
            Console.WriteLine("Server stopped");
        }
        public void Close() { _listener.Close(); }

        // should include response code as 4th argument??
        private void WriteResponse(HttpListenerResponse response, string text, string contentType = "text/plain")
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            response.ContentType = contentType;
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();

        }



        /*--------------------------------- Login & Register Handlers ---------------------------------
         ------------------------------------------------------------------------------------*/
        private async Task RegisterHandlerAsync(HttpListenerContext context)
        {
            /*
            Console.WriteLine("Raw Url: " + context.Request.RawUrl);
            Console.WriteLine("UserHostName: " + context.Request.UserHostName);
            Console.WriteLine("Headers: " + context.Request.Headers);
            */

            // hier weiter, System.IO.Stream+NullStream fehlermeldung ---> Body von Anfrage einlesen.
            // dann user registrieren sowie excistence checks
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string body = await stream.ReadToEndAsync();
            // stream beenden
            stream.Dispose();
            UserRegisterDTO userRegisterDTO = JsonSerializer.Deserialize<UserRegisterDTO>(body);
            Console.WriteLine("------------------ USER REGISTER INFO ------------------");
            Console.WriteLine("User register: " + body);
            Console.WriteLine("User Register DTO username: " + userRegisterDTO.username);
            Console.WriteLine("User Register DTO password: " + userRegisterDTO.password);
            Console.WriteLine("-----------------------------------------------------");
            WriteResponse(context.Response, "RegisterHandler", "text/plain");
            await _userService.RegisterUserAsync(userRegisterDTO);

        }

        private async Task LoginHandlerAsync(HttpListenerContext context)
        {
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string body = await stream.ReadToEndAsync();
            // stream beenden
            stream.Dispose();
            UserLoginDTO userLoginDTO = JsonSerializer.Deserialize<UserLoginDTO>(body);
            Console.WriteLine("------------------ USER LOGIN INFO ------------------");
            Console.WriteLine("User login: " + body);
            Console.WriteLine("-----------------------------------------------------");


            string validatedToken = await _userService.LoginUserAsync(userLoginDTO.username, userLoginDTO.password);

            var responseObj = new LoginResponseDTO
            {
                token = validatedToken
            };

            string json = JsonSerializer.Serialize(responseObj);
            WriteResponse(context.Response, json, "application/json");
        }

        /*--------------------------------- CRUD-Media Handlers ---------------------------------
         ------------------------------------------------------------------------------------*/

        // CRUD - Create, Read, Update, Delete
        // Create Media
        private async Task CreateMediaHandlerAsync(HttpListenerContext context)
        {
            int userId = await UserAuthorizationAsync(context);



            StreamReader stream = new StreamReader(context.Request.InputStream);
            string body = await stream.ReadToEndAsync();
            // stream beenden
            stream.Dispose();

            Console.WriteLine("------------------ USER MEDIA INFO + CREATE ------------------");
            MediaDTO mediaDTO = JsonSerializer.Deserialize<MediaDTO>(body);
            Console.WriteLine($"user ID: {userId}");
            Console.WriteLine("Media DTO: " + body + "\n");
            Console.WriteLine("------------------ USER TOKEN INFO ------------------");
            Console.WriteLine($"active token count: {_tokenService.getActiveTokens().Count}");
            foreach (string token in _tokenService.getActiveTokens())
            {
                Console.WriteLine("active token: " + token);
            }
            Console.WriteLine("-----------------------------------------------------");
            await _mediaService.CreateMediaAsync(mediaDTO, userId);
            WriteResponse(context.Response, "Successfull", "text/plain");

        }

        // Read All Media
        private async Task ReadAllMediaHandlerAsync(HttpListenerContext context)
        {
            //   int userId = await UserAuthorizationAsync(context);
            await UserAuthorizationAsync(context);

            await _mediaService.ReadAllMediaAsync();
            WriteResponse(context.Response, "Read Media Successfull", "text/plain");
        }

        // Update Media
        // to be implemented
        private async Task UpdateMediaHandlerAsync(HttpListenerContext context)
        {
            int userId = await UserAuthorizationAsync(context);
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string body = await stream.ReadToEndAsync();
            // stream beenden
            stream.Dispose();

            // title that gets searched
            string title = context.Request.QueryString.Get("title");
            Console.WriteLine("query: "+ context.Request.QueryString);
            Console.WriteLine("\n");
            // update body
            MediaUpdateDTO mediaUpdateDTO = JsonSerializer.Deserialize<MediaUpdateDTO>(body);
            await _mediaService.UpdateMediaAsync(mediaUpdateDTO, title, userId);
            WriteResponse(context.Response, "Successfull", "text/plain");
        }



        // Delete Media
        private async Task DeleteMediaHandlerAsync(HttpListenerContext context)
        {
            int userId = await UserAuthorizationAsync(context);

            StreamReader stream = new StreamReader(context.Request.InputStream);
            string body = await stream.ReadToEndAsync();
            // stream beenden
            stream.Dispose();

            Console.WriteLine("------------------ USER MEDIA INFO + DELETE ------------------");
            MediaDTO mediaDTO = JsonSerializer.Deserialize<MediaDTO>(body);
            Console.WriteLine($"user ID: {userId}");
            Console.WriteLine("Media DTO: " + body + "\n");
            Console.WriteLine("------------------ USER TOKEN INFO ------------------");
            Console.WriteLine($"active token count: {_tokenService.getActiveTokens().Count}");
            foreach (string token in _tokenService.getActiveTokens())
            {
                Console.WriteLine("active token: " + token);
            }
            Console.WriteLine("-----------------------------------------------------");
            await _mediaService.DeleteMediaByTitleAsync(mediaDTO.title, userId);
            WriteResponse(context.Response, "Successfull", "text/plain");
        }

        /*--------------------------------- Media Rating Handlers ---------------------------------
         ------------------------------------------------------------------------------------*/
        private async Task RateMediaHandlerAsync(HttpListenerContext context)
        {
            int userId = await UserAuthorizationAsync(context);
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string body = await stream.ReadToEndAsync();
            // stream beenden
            stream.Dispose();

            MediaRatingDTO mediaRatingDTO = JsonSerializer.Deserialize<MediaRatingDTO>(body);

            // title that gets searched
            string title = context.Request.QueryString.Get("title");
            Console.WriteLine("query: " + context.Request.QueryString);
            Console.WriteLine("\n");
            
            await _mediaService.RateMediaAsync(mediaRatingDTO, title, userId);
            WriteResponse(context.Response, "Successfull", "text/plain");

        }

        public async Task ReadAllRateMediaHandlerAsync(HttpListenerContext context)
        {
            //   int userId = await UserAuthorizationAsync(context);
            await UserAuthorizationAsync(context);
            await _mediaService.ReadAllMediaRatingsAsync();
            WriteResponse(context.Response, "Read Media Ratings Successfull", "text/plain");
        }

        private async Task UpdateRateMediaHandlerAsync(HttpListenerContext context)
        {
            int userId = await UserAuthorizationAsync(context);
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string body = await stream.ReadToEndAsync();
            // stream beenden
            stream.Dispose();
            MediaRatingUpdateDTO mediaRatingUpdateDTO = JsonSerializer.Deserialize<MediaRatingUpdateDTO>(body);
            int ratingId = int.Parse(context.Request.QueryString.Get("ratingId"));
            Console.WriteLine("ratingId: " + ratingId);
            await _mediaService.UpdateMediaRatingAsync(mediaRatingUpdateDTO, ratingId, userId);
            WriteResponse(context.Response, "Successfull", "text/plain");
        }

        private async Task DeleteRateMediaHandlerAsync(HttpListenerContext context)
        {
            int userId = await UserAuthorizationAsync(context);
            // using to auto dispose stream
            using StreamReader stream = new StreamReader(context.Request.InputStream);
            string body = await stream.ReadToEndAsync();
            int ratingId = int.Parse(context.Request.QueryString.Get("ratingId"));
            Console.WriteLine("ratingId: " + ratingId);
      

            await _mediaService.DeleteRatingAsync(ratingId, userId);
            WriteResponse(context.Response, "Successfull", "text/plain");
        }

        private async Task LikeCommentHandlerAsync(HttpListenerContext context)
        {
            int userId = await UserAuthorizationAsync(context);
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string body = await stream.ReadToEndAsync();
            // stream beenden
            stream.Dispose();

            LikeRatingDTO mediaLikeDTO = JsonSerializer.Deserialize<LikeRatingDTO>(body);

            await _mediaService.LikeRatingAsync(mediaLikeDTO, userId);
            WriteResponse(context.Response, "Successfull", "text/plain");

        }


        /*--------------------------------- Media Favorite Handlers ---------------------------------
         ------------------------------------------------------------------------------------*/

        private async Task FavoriteMediaHandlerAsync(HttpListenerContext context)
        {
            int userId = await UserAuthorizationAsync(context);
            using StreamReader stream = new StreamReader(context.Request.InputStream);
            string body = await stream.ReadToEndAsync();
            int mediaId = int.Parse(context.Request.QueryString.Get("mediaId"));
            await _mediaService.FavoriteMediaAsync(mediaId, userId);
            WriteResponse(context.Response, "Successfull", "text/plain");
        }

        private async Task GetFavoritesMediaHandlerAsync(HttpListenerContext context)
        {
            int userId = await UserAuthorizationAsync(context);
            await _mediaService.GetFavoritesAsync(userId);
            WriteResponse(context.Response, "Read Media Favorites Successfull", "application/json");
        }

        private async Task UnfavoriteMediaHandlerAsync(HttpListenerContext context)
        {
            int userId = await UserAuthorizationAsync(context);
            using StreamReader stream = new StreamReader(context.Request.InputStream);
            string body = await stream.ReadToEndAsync();
            int mediaId = int.Parse(context.Request.QueryString.Get("mediaId"));
            await _mediaService.UnfavoriteMediaAsync(mediaId, userId);
            WriteResponse(context.Response, "Successfull", "text/plain");
        }

        /*--------------------------------- Statistics ---------------------------------
         ------------------------------------------------------------------------------------*/

        private async Task PersonalStatsHandlerAsync(HttpListenerContext context)
        {
            int userId = await UserAuthorizationAsync(context);
            using StreamReader stream = new StreamReader(context.Request.InputStream);
            string body = await stream.ReadToEndAsync();
            
            await _mediaService.GetPersonalStatsAsync(userId);
            WriteResponse(context.Response, "Read Personal Stats Successfull", "application/json");
        }

        private async Task MediaStatsHandlerAsync(HttpListenerContext context)
        {
            int userId = await UserAuthorizationAsync(context);
            using StreamReader stream = new StreamReader(context.Request.InputStream);
            string body = await stream.ReadToEndAsync();
            int mediaId = int.Parse(context.Request.QueryString.Get("mediaId"));

            await _mediaService.GetMediaStatsAsync(mediaId);
            WriteResponse(context.Response, "Read Media Stats Successfull", "application/json");
        }

        private async Task RatingHistoryHandlerAsync(HttpListenerContext context)
        {
            int userId = await UserAuthorizationAsync(context);
            await _mediaService.GetRatingHistoryAsync(userId);
            WriteResponse(context.Response, "Read Rating History Successfull", "application/json");
        }


        /*--------------------------------- User Handlers ---------------------------------
         ------------------------------------------------------------------------------------*/

        private async Task GetUserByIdHandlerAsync(HttpListenerContext context)
        {
            try
            {
                await UserAuthorizationAsync(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

            int userId = Int32.Parse(context.Request.QueryString["id"]);


            await _userService.GetUserByIdAsync(userId);

            var user = await _userService.GetUserByIdAsync(userId);

            string json = JsonSerializer.Serialize(user);
            WriteResponse(context.Response, json, "application/json");
        }

        private async Task GetUserByUsernameHandlerAsync(HttpListenerContext context)
        {
            try
            {
                await UserAuthorizationAsync(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            string username = context.Request.QueryString.Get("username");


            var user = await _userService.GetUserByUsernameAsync(username);

            string json = JsonSerializer.Serialize(user);
            WriteResponse(context.Response, json, "application/json");
        }

        // Get My User Info
        // private async Task Get

        private async Task<int> UserAuthorizationAsync(HttpListenerContext context)
        {
            // get token
            string authHeader = context.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                WriteResponse(context.Response, "Missing Authorization header", "text/plain");
                Console.WriteLine("Missing Authorization header");
                throw new Exception();
            }

            // get userId
            int userId = _tokenService.GetUserIdFromToken(authHeader);

            if (userId == 0)
            {
                WriteResponse(context.Response, "Unauthorized", "text/plain");
                Console.WriteLine("------------------ USER TOKEN INFO ------------------");
                foreach (string tokenParts in authHeader.Split('.'))
                {
                    Console.WriteLine("Token Part: " + tokenParts);
                }
                Console.WriteLine("\nAuth Header: " + authHeader + "\n");
                foreach (string token in _tokenService.getActiveTokens())
                {
                    Console.WriteLine("active token: " + token);
                }
                Console.WriteLine("active tokens count: " + _tokenService.getActiveTokens().Count);
                Console.WriteLine("-----------------------------------------------------");
                throw new Exception();
            }

            return userId;

        }
    }
}




