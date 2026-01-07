using MediaRatingPlatform_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_DataAccessLayer.Repositories.Interface
{
    public interface IUserRepository
    {
        Task CreateUser(UserEntity user);
        Task<bool> GetByUsernameAsync(string username);
        Task UpdateLastLogin(int userId, DateTime lastLogin);
        Task<UserEntity> GetUserByIdAsync(int userId);
        Task<UserEntity> GetUserByUsernameAsync(string username);
        Task<bool> DoesUserExist(string username);
        Task<UserEntity> GetFullUserByUsername(string username);
    }
}
