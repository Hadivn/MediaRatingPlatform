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

        public async Task TestConnectionAsync()
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                Console.WriteLine("Connected to PostgreSQL");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }



    }
}
