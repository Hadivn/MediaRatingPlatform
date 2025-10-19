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


        public MediaRepository()
        {
            // should fix this, no await in constructor
            InitializeDatabase();       
        }

        public async Task CreateMediaAsync(MediaEntity mediaEntity)
        {
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
    }
}
