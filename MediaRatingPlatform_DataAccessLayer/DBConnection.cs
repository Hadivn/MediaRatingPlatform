using Npgsql;

namespace MediaRatingPlatform_DataAccessLayer
{
    public class DBConnection
    {
        private string _connectionString;
        public DBConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<NpgsqlConnection> ConnectToDatabaseAsync()
        {
            var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            Console.WriteLine("Connected to PostgreSQL!");
            return conn;

        }


        // changes later, add userId to track which has which entry
        public async Task InitializeDatabase()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // TEXT --> no limit, VarChar --> Limited
            var createMediaTableCmd = new NpgsqlCommand(
                @"CREATE TABLE IF NOT EXISTS media (
                id SERIAL PRIMARY KEY,
                title VARCHAR(200) NOT NULL,
                description TEXT NOT NULL,
                media_type VARCHAR(50) NOT NULL,
                release_year INT NOT NULL,
                age_restriction INT NOT NULL,
                genres VARCHAR(200) NOT NULL,
                created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
                updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
            )", connection);

            await createMediaTableCmd.ExecuteNonQueryAsync();

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
