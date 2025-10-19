using MediaRatingPlatform_Domain.Entities;
using Npgsql;

namespace MediaRatingPlatform_BusinessLogicLayer.Repositories
{
    public class UserRepository
    {
        private string _connectionString = "Host=localhost;Port=5432;Username=mrpdatabase;Password=user;Database=mrpdatabase";
        

        // done
        public async Task CreateUser(UserEntity userEntity)
        {
            await InitializeDatabaseAsync();

            // check if users exists already
            if (await GetByUsernameAsync(userEntity.username))
            {
                throw new Exception("Username existiert bereits!");
            }

            // insert user into database
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // userRegisterEntity.password = BCrypt.Net.BCrypt.HashPassword(userRegisterEntity.password);

            var insertCmd = new NpgsqlCommand(
                "INSERT INTO users (username, password, is_active, created_at) VALUES (@u, @p, @active, @created)",
                connection);

            insertCmd.Parameters.AddWithValue("u", userEntity.username);
            insertCmd.Parameters.AddWithValue("p", userEntity.password);
            insertCmd.Parameters.AddWithValue("active", false);
            insertCmd.Parameters.AddWithValue("created", DateTime.UtcNow);

            await insertCmd.ExecuteNonQueryAsync();
        }


        // sollte ich genauer anschauen
        public async Task<bool> GetByUsernameAsync(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var userExistsCmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @u", connection);
            userExistsCmd.Parameters.AddWithValue("u", username);

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

            return new UserEntity(reader.GetString(1), reader.GetString(2))
            {
                id = reader.GetInt32(0),
                isActive = reader.GetBoolean(3),
                createdAt = reader.GetDateTime(4)
            };
        }

        // should be done
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


        // done
        public async Task InitializeDatabaseAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // check if table exists
            var createTableCmd = new NpgsqlCommand(
                @"CREATE TABLE IF NOT EXISTS users (
                id SERIAL PRIMARY KEY,
                username VARCHAR(100) NOT NULL,
                password VARCHAR(100) NOT NULL,
                is_active BOOLEAN NOT NULL DEFAULT FALSE,
                created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
                last_login_at TIMESTAMPTZ
            )", connection);

            

            await createTableCmd.ExecuteNonQueryAsync();

        }
    }
}
