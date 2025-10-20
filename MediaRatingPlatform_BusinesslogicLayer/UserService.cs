using MediaRatingPlatform_BusinessLogicLayer.Repositories;
using MediaRatingPlatform_Domain.DTO;
using MediaRatingPlatform_Domain.Entities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_BusinessLogicLayer
{
    public class UserService
    {
        private UserRepository _userRepository;
        private TokenService _tokenService;

        public UserService(TokenService tokenService)
        {
            _userRepository = new UserRepository();
            _tokenService = tokenService;
        }
        
        // done
        public async Task RegisterUserAsync(UserRegisterDTO userRegisterDTO)
        {
            // map DTO into entity
            if (string.IsNullOrWhiteSpace(userRegisterDTO.username) || string.IsNullOrEmpty(userRegisterDTO.password))
            {
                throw new ArgumentException("No Username or Password given!");
            }

            UserEntity userEntity = new UserEntity(userRegisterDTO.username, userRegisterDTO.password);

            try
            {
                await _userRepository.CreateUser(userEntity);
                Console.WriteLine("Creating user successfull");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Creating a user failed: "+ ex.Message);
            }


        }

    
        // testing if it works
        public async Task<string> LoginUserAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("No Username or Password given!");
            }

            if (await _userRepository.GetByUsernameAsync(username))
            {
                UserEntity userLogin = await _userRepository.GetFullUserByUsername(username);
                if (userLogin.password.Equals(password) && userLogin.username.Equals(username))
                {
                    await _userRepository.UpdateLastLogin(userLogin.id, DateTime.UtcNow);
                    Console.WriteLine("User successfully logged in!");
                    string token = _tokenService.GenerateToken(userLogin.id, userLogin.username);
                    return token;
                }
                else
                {
                    throw new Exception($"Tried to loginIn but the password for {username} was wrong");
                }
            }
            else
            {
                throw new Exception("Tried to loginIn but no user was found");
            }
        }


    }
}
