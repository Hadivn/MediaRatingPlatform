using MediaRatingPlatform_Domain.DTO;
using MediaRatingPlatform_Domain.Entities;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_DataAccessLayer.Repositories
{
    public class MediaRepository
    {
        private string _connectionString = "Host=192.168.0.53;Port=5432;Username=mrpdatabase;Password=user;Database=mrpdatabase";

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
                INSERT INTO media (title, description, media_type, release_year, genres, age_restriction, created_at, updated_at, user_id)
                VALUES (@title, @description, @mediaType, @releaseYear, @genres, @ageRestriction, @createdAt, @updatedAt, @userId);
            ", connection);

            cmd.Parameters.AddWithValue("@title", mediaEntity.title);
            cmd.Parameters.AddWithValue("@description", mediaEntity.description);
            cmd.Parameters.AddWithValue("@mediaType", mediaEntity.mediaType.ToString()); 
            cmd.Parameters.AddWithValue("@genres", mediaEntity.genres);
            cmd.Parameters.AddWithValue("@releaseYear", mediaEntity.releaseYear);
            cmd.Parameters.AddWithValue("@ageRestriction", mediaEntity.ageRestriction);
            cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@updatedAt", mediaEntity.updatedAt);
            cmd.Parameters.AddWithValue("@userId", mediaEntity.userId);

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
                Console.WriteLine($"Title: {reader["title"]}");
                Console.WriteLine($"description: {reader["description"]}");
                Console.WriteLine($"mediaType: {reader["media_type"]}");
                Console.WriteLine($"mediaType typeof name: {reader["media_type"].GetType().Name}");
                Console.WriteLine($"genres: {reader["genres"]}");
                Console.WriteLine($"releaseYear: {reader["release_year"]}");
                Console.WriteLine($"ageRestriction: {reader["age_restriction"]}");
                Console.WriteLine($"createdAt: {reader["created_at"]}");
                Console.WriteLine($"userId: {reader["user_id"]}");
                Console.WriteLine("----------------------------------------");
            }
        }

        // CRUD - Media update
        public async Task UpdateMediaAsync(MediaUpdateDTO mediaUpdateDTO, string title)
        {
            if (!await MediaExists(title))
                throw new Exception("Media existiert nicht!");

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var updateFields = new List<string>();
            using var updateCmd = connection.CreateCommand();

            if (!string.IsNullOrEmpty(mediaUpdateDTO.description))
            {
                updateFields.Add("description = @description");
                updateCmd.Parameters.Add("description", NpgsqlDbType.Text)
                                    .Value = mediaUpdateDTO.description;
            }

            if (!string.IsNullOrEmpty(mediaUpdateDTO.mediaType))
            {
                updateFields.Add("media_type = @mediaType");
                updateCmd.Parameters.Add("mediaType", NpgsqlDbType.Text)
                                    .Value = mediaUpdateDTO.mediaType;
            }

            if (mediaUpdateDTO.releaseYear != null)
            {
                updateFields.Add("release_year = @releaseYear");
                updateCmd.Parameters.AddWithValue("releaseYear", mediaUpdateDTO.releaseYear);
            }

            if (mediaUpdateDTO.ageRestriction != null)
            {
                updateFields.Add("age_restriction = @ageRestriction");
                updateCmd.Parameters.AddWithValue("ageRestriction", mediaUpdateDTO.ageRestriction);
            }

            if (!string.IsNullOrEmpty(mediaUpdateDTO.genres))
            {
                updateFields.Add("genres = @genres");
                updateCmd.Parameters.Add("genres", NpgsqlDbType.Text)
                                    .Value = mediaUpdateDTO.genres;
            }

            if (!string.IsNullOrEmpty(mediaUpdateDTO.title))
            {
                updateFields.Add("title = @newTitle");
                updateCmd.Parameters.Add("newTitle", NpgsqlDbType.Text)
                                    .Value = mediaUpdateDTO.title;
            }

            if (updateFields.Count == 0)
                return; // nothing to update

            updateCmd.CommandText = $"UPDATE media SET {String.Join(',', updateFields)} where title = @t";

            updateCmd.Parameters.Add("t", NpgsqlDbType.Text).Value = title;

            await updateCmd.ExecuteNonQueryAsync();
        }

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
                    $"exception: {ex.Message}" +
                    "-------------------------------------------------------");
               
            }

        }

        public async Task RateMediaAsync(MediaRatingEntity mediaRatingEntity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var cmd = new NpgsqlCommand(@"
                INSERT INTO ratings (star, comment, created_at, user_id, media_id, is_confirmed)
                VALUES (@star, @comment, @createdAt, @userId, @mediaId, @isConfirmed);
            ", connection);

            cmd.Parameters.AddWithValue("@star", mediaRatingEntity.star);
            cmd.Parameters.AddWithValue("@comment", mediaRatingEntity.comment ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@userId", mediaRatingEntity.userId);
            cmd.Parameters.AddWithValue("@mediaId", mediaRatingEntity.mediaId);
            // hier oder im konstruktor?
            cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@isConfirmed", mediaRatingEntity.isConfirmed);


            await cmd.ExecuteNonQueryAsync();
        }


        // hilfsmethoden
        public async Task<bool> MediaExists(string title)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var mediaExistsCmd = new NpgsqlCommand("SELECT COUNT(*) FROM media WHERE title = @t", connection);
            mediaExistsCmd.Parameters.AddWithValue("t", title);

            var count = (long)await mediaExistsCmd.ExecuteScalarAsync();
            return count > 0;
        }

        public async Task<int> GetCreatedByUserId(string title)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var mediaExistsCmd = new NpgsqlCommand("SELECT user_id FROM media WHERE title = @t", connection);
            mediaExistsCmd.Parameters.AddWithValue("t", title);

            int createdByUserId = (int)await mediaExistsCmd.ExecuteScalarAsync();
            return createdByUserId;
        }

        public async Task<int> GetMediaIdByTitle(string title)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var mediaIdCmd = new NpgsqlCommand("SELECT id FROM media WHERE title = @t", connection);
            mediaIdCmd.Parameters.AddWithValue("t", title);
            int mediaId = (int)await mediaIdCmd.ExecuteScalarAsync();
            return mediaId;
        }





    }
}
