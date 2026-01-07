using MediaRatingPlatform_DataAccessLayer.Repositories.Interface;
using MediaRatingPlatform_Domain.Entities;
 
using Npgsql;

namespace MediaRatingPlatform_BusinessLogicLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private string _connectionString = "Host=192.168.0.53;Port=5432;Username=mrpdatabase;Password=user;Database=mrpdatabase";
        

        // CRUD - User create
        public async Task CreateUser(UserEntity userEntity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // userRegisterEntity.password = BCrypt.Net.BCrypt.HashPassword(userRegisterEntity.password);

            var insertCmd = new NpgsqlCommand(
                "INSERT INTO users (username, password, is_active, created_at) VALUES (@u, @p, @active, @created)",
                connection);

            // lastLoginAt is null at creation
            insertCmd.Parameters.AddWithValue("u", userEntity.username);
            insertCmd.Parameters.AddWithValue("p", userEntity.password);
            insertCmd.Parameters.AddWithValue("active", false);
            insertCmd.Parameters.AddWithValue("created", DateTime.UtcNow);

            await insertCmd.ExecuteNonQueryAsync();
        }

        // CRUD - User read own data
        public async Task<UserEntity> GetUserByIdAsync(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "SELECT id, username, password, is_active, created_at FROM users WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", userId);
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null; // user not found
            
            return new UserEntity(reader.GetString(1), reader.GetString(2))
            {
                id = reader.GetInt32(0),
                isActive = reader.GetBoolean(3),
                createdAt = reader.GetDateTime(4)
            };
        }

        // CRUD - User read by username
        public async Task<UserEntity> GetUserByUsernameAsync(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "SELECT id, username, password, is_active, created_at FROM users WHERE username = @u";
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@u", username);
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null; // user not found
            
            return new UserEntity(reader.GetString(1), reader.GetString(2))
            {
                id = reader.GetInt32(0),
                isActive = reader.GetBoolean(3),
                createdAt = reader.GetDateTime(4)
            };
        }

        // User update last login
        public async Task UpdateLastLogin(int userId, DateTime time)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = "UPDATE users SET last_login_at = @t WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@t", time);
            cmd.Parameters.AddWithValue("@id", userId);

            await cmd.ExecuteNonQueryAsync();
        }


        public async Task<bool> GetByUsernameAsync(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var userExistsCmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @u", connection);
            userExistsCmd.Parameters.AddWithValue("u", username);

            // sollte ich genauer anschauen
            var count = (long)await userExistsCmd.ExecuteScalarAsync();
            return count > 0;
        }


        public async Task<UserEntity> GetFullUserByUsername(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT id, username, password, is_active, created_at FROM users WHERE username = @u";
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@u", username);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null; // user not found
           
            // need a closer inspection
            return new UserEntity(reader.GetString(1), reader.GetString(2))
            {
                id = reader.GetInt32(0),
                isActive = reader.GetBoolean(3),
                createdAt = reader.GetDateTime(4)
            };
        }

        public async Task<bool> DoesUserExist(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var userExistsCmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @u", connection);
            userExistsCmd.Parameters.AddWithValue("u", username);
            var count = (long)await userExistsCmd.ExecuteScalarAsync();
            return count > 0;


        }


       
    }
}
