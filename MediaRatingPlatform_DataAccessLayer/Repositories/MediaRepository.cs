using MediaRatingPlatform_Domain.Entities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_DataAccessLayer.Repositories
{
    public class MediaRepository
    {
        private string _connectionString = "Host=localhost;Port=5432;Username=mrpdatabase;Password=user;Database=mrpdatabase";

        // CRUD - Media create
        public async Task CreateMediaAsync(MediaEntity mediaEntity)
        {

            if (await MediaExists(mediaEntity.title))
            {
                throw new Exception("Media existiert bereits!");
            }
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var cmd = new NpgsqlCommand(@"
                INSERT INTO media (title, description, media_type, release_year, genres, age_restriction, created_at, updated_at)
                VALUES (@title, @description, @mediaType, @releaseYear, @genres, @ageRestriction, @createdAt, @updatedAt);
            ", connection);

            cmd.Parameters.AddWithValue("@title", mediaEntity.title);
            cmd.Parameters.AddWithValue("@description", mediaEntity.description);
            cmd.Parameters.AddWithValue("@mediaType", mediaEntity.mediaType ?? (object)DBNull.Value); 
            cmd.Parameters.AddWithValue("@genres", mediaEntity.genres);
            cmd.Parameters.AddWithValue("@releaseYear", mediaEntity.releaseYear);
            cmd.Parameters.AddWithValue("@ageRestriction", mediaEntity.ageRestriction);
            cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@updatedAt", mediaEntity.updatedAt);

            await cmd.ExecuteNonQueryAsync();

        }

        // CRUD - Media read
        public async Task ReadAllMediaAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var cmd = new NpgsqlCommand("SELECT * FROM media", connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"Title: {reader["title"]}, Description: {reader["description"]}");
            }
        }

        public async Task UpdateMedia

        // CRUD - Media delete
        public async Task DeleteMediaByTitle(string title)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                var deleteCmd = new NpgsqlCommand("DELETE FROM media WHERE title = @t", connection);
                deleteCmd.Parameters.AddWithValue("t", title);
                await deleteCmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("-------------------------------------------------------");
                throw new Exception($"Error while trying to delete {title}" +
                    $"exception Layer: DataAccessLayer " +
                    $"exception: {ex.Message}");
                Console.WriteLine("-------------------------------------------------------");
            }

        }

        public async Task<bool> MediaExists(string title)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var mediaExistsCmd = new NpgsqlCommand("SELECT COUNT(*) FROM media WHERE title = @t", connection);
            mediaExistsCmd.Parameters.AddWithValue("t", title);

            var count = (long)await mediaExistsCmd.ExecuteScalarAsync();
            return count > 0;
        }

       


      
    }
}
