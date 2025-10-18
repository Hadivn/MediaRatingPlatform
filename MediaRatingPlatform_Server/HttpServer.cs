using MediaRatingPlatform_Domain.DTO;
using MediaRatingPlatform_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MediaRatingPlatform_BusinessLogicLayer;

namespace MediaRatingPlatform_Server
{
    internal class HttpServer
    {
        private UserService _userService = new UserService();
        private HttpListener _listener;
        // method depending on endpoint.
        private Dictionary<(string path, string method), Func<HttpListenerContext, Task>> _routes;
        public HttpServer(string prefix)
        {
            if(prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);

            _routes = new Dictionary<(string path, string method), Func<HttpListenerContext, Task>>
            {
               // { ("/", "GET"), ctx => { RootHandler(ctx); return Task.CompletedTask; } },
                { ("/register", "POST"), RegisterHandlerAsync },
                { ("/login", "POST"), LoginHandlerAsync }
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
                // waits for HTTP-Anfrage from client.
                HttpListenerContext context = await _listener.GetContextAsync();
                // represents the incoming HTTP-Anfrage
                HttpListenerRequest request = context.Request;
                // represents the outgoing HTTP-Answer from server to client
                HttpListenerResponse response = context.Response;

                string path = request.Url.AbsolutePath;
                string method = request.HttpMethod.ToUpper();
                Console.WriteLine($"[{DateTime.Now}] {request.HttpMethod} {path}");

                var key = (path, method);
                if(_routes.TryGetValue(key, out var handler))
                {
                    await handler(context);
                }
                else
                {
                    WriteResponse(response, "404 Not Found", "text/plain");
                }

            }
        }

        public void Stop() {
            _listener.Stop();
            Console.WriteLine("Server stopped");
        }
        public void Close() { _listener.Close(); }

        private void WriteResponse(HttpListenerResponse response, string text, string contentType ="text/plain")
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            response.ContentType = contentType;
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
            
        }

        private void RootHandler(HttpListenerContext context)
        {
            WriteResponse(context.Response, "RootHandler", "text/plain");
        }

        // done
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
            Console.WriteLine("User register: " + body);
            Console.WriteLine("User Register DTO username: " + userRegisterDTO.username);
            Console.WriteLine("User Register DTO password: " + userRegisterDTO.password);
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
            Console.WriteLine("User login: " + body);
            Console.WriteLine("User Login DTO username: " + userLoginDTO.username);
            Console.WriteLine("User Login DTO password: " + userLoginDTO.password);

            
            string validatedToken = await _userService.LoginUserAsync(userLoginDTO.username, userLoginDTO.password);

            var responseObj = new LoginResponseDTO
            {
                token = validatedToken
            };

            string json = JsonSerializer.Serialize(responseObj);
            WriteResponse(context.Response, json, "application/json");
        }

    }
}




