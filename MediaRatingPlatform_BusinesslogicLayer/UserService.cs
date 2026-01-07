    using MediaRatingPlatform_BusinessLogicLayer.Repositories;
    using MediaRatingPlatform_DataAccessLayer.Repositories.Interface;
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
            private IUserRepository _userRepository;
            private TokenService _tokenService;

            public UserService(IUserRepository userRepository, TokenService tokenService)
            {
                _userRepository = userRepository;
                _tokenService = tokenService;
            }

            // Register + Create User
            public async Task<UserEntity> RegisterUserAsync(UserRegisterDTO userRegisterDTO)
            {
                // map DTO into entity
                if (string.IsNullOrWhiteSpace(userRegisterDTO.username) || string.IsNullOrEmpty(userRegisterDTO.password))
                {
                    throw new ArgumentException("No Username or Password given!");
                }

                if (await UserExistsAsync(userRegisterDTO.username))
                {
                    throw new UnauthorizedAccessException("Username existiert bereits!");
                }

                UserEntity userEntity = new UserEntity(userRegisterDTO.username, userRegisterDTO.password);

                await _userRepository.CreateUser(userEntity);

                return userEntity;
       
       
            }


            // Login User
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
                        Console.WriteLine($"User ID: {userLogin.id}");
                        string token = _tokenService.GenerateToken(userLogin.id, userLogin.username);
                        return token;
                    }
                    else
                    {
                        throw new UnauthorizedAccessException($"Tried to loginIn but the password for {username} was wrong");
                    }
                }
                else
                {
                    throw new Exception("Tried to loginIn but no user was found");
                }
            }

            // Get User by Id
            public async Task<UserEntity> GetUserByIdAsync(int userId)
            {
                return await _userRepository.GetUserByIdAsync(userId);
            }

            public async Task<UserEntity> GetUserByUsernameAsync(string username)
            {
                return await _userRepository.GetUserByUsernameAsync(username);
            }

            public async Task<bool> UserExistsAsync(string username)
            {
                return await _userRepository.DoesUserExist(username);
            }


        }
    }
