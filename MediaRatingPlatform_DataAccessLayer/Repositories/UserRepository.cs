using MediaRatingPlatform_BusinessLogicLayer.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_BusinessLogicLayer.Repositories
{
    internal class UserRepository
    {
        private string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task CreateUser(User user)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var cmd = new NpgsqlCommand(
                "INSERT INTO users (username, password) VALUES (@u, @p)",
                connection);
            cmd.Parameters.AddWithValue("u", user.username);
            cmd.Parameters.AddWithValue("p", user.password);

            cmd.ExecuteNonQuery();
        }

    }
}
