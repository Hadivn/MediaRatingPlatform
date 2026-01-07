using MediaRatingPlatform_DataAccessLayer.Repositories.Interface;
using MediaRatingPlatform_Domain.DTO;
using MediaRatingPlatform_Domain.Entities;
using MediaRatingPlatform_Domain.ENUM;
using Npgsql;
using NpgsqlTypes;

namespace MediaRatingPlatform_DataAccessLayer.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        // ExecuteNonQueryAsync für Insert, Update, Delete
        // ExecuteReaderAsync für Select
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
        public async Task<List<MediaDTO>> ReadAllMediaAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM media", connection);
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            List<MediaDTO> mediaList = new List<MediaDTO>();
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"Id: {reader["id"]}");
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

                mediaList.Add(new MediaDTO
                {
                    title = reader["title"].ToString(),
                    description = reader["description"].ToString(),
                    mediaType = Enum.Parse<EMediaType>(reader["media_type"].ToString()),
                    releaseYear = Convert.ToInt32(reader["release_year"]),
                    ageRestriction = Convert.ToInt32(reader["age_restriction"]),
                    genres = reader["genres"].ToString()
                });
            }

            return mediaList;


        }

        public async Task<MediaDTO> ReadMediaByTitle(string title)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM media WHERE title = @t", connection);
            cmd.Parameters.AddWithValue("t", title);
            MediaDTO mediaDTO = new MediaDTO();
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            while(await reader.ReadAsync())
            {
                mediaDTO.title = reader["title"].ToString();
                mediaDTO.description = reader["description"].ToString();
                mediaDTO.mediaType = Enum.Parse<EMediaType>(reader["media_type"].ToString());
                mediaDTO.releaseYear = Convert.ToInt32(reader["release_year"]);
                mediaDTO.ageRestriction = Convert.ToInt32(reader["age_restriction"]);
                mediaDTO.genres = reader["genres"].ToString();
            }

            return mediaDTO;

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

        public async Task<List<MediaAllRatingsDTO>> ReadAllMediaRatingsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            NpgsqlCommand cmd;
            cmd = new NpgsqlCommand("SELECT r.star as star, r.comment as comment, r.is_confirmed as is_confirmed," +
                " m.title as title FROM ratings r left join media m on r.media_id = m.id", connection);
            NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            List<MediaAllRatingsDTO> mediaRatings = new List<MediaAllRatingsDTO>();
            while (await reader.ReadAsync())
            {
                if (reader["is_confirmed"].Equals(true))
                {
                    
                    mediaRatings.Add(new MediaAllRatingsDTO
                    {
                        star = (int)reader["star"],
                        comment = reader["comment"].ToString(),
                        isConfirmed = (bool)reader["is_confirmed"],
                        title = reader["title"].ToString()

                    });
                    
                }
            }

            return mediaRatings;


        }

        public async Task UpdateMediaRatingAsync(MediaRatingUpdateDTO mediaRatingUpdateDTO, int ratingId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var updateFields = new List<string>();
            using var updateCmd = connection.CreateCommand();
            if (mediaRatingUpdateDTO.star != null)
            {
                updateFields.Add("star = @star");
                updateCmd.Parameters.AddWithValue("star", mediaRatingUpdateDTO.star);
            }
            if (!string.IsNullOrEmpty(mediaRatingUpdateDTO.comment))
            {
                updateFields.Add("comment = @comment");
                updateCmd.Parameters.Add("comment", NpgsqlDbType.Text)
                                    .Value = mediaRatingUpdateDTO.comment;
            }
            if (mediaRatingUpdateDTO.isConfirmed != null)
            {
                updateFields.Add("is_confirmed = @isConfirmed");
                updateCmd.Parameters.AddWithValue("isConfirmed", mediaRatingUpdateDTO.isConfirmed);
            }
            if (updateFields.Count == 0)
                return; // nothing to update
            updateCmd.CommandText = $"UPDATE ratings SET {String.Join(',', updateFields)} where id = @id";
            updateCmd.Parameters.AddWithValue("id", ratingId);
            await updateCmd.ExecuteNonQueryAsync();
        }

        public async Task LikeRatingAsync(LikeRatingEntity likeRatingEntity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var cmd = new NpgsqlCommand(@"
                INSERT INTO likes (rating_id, user_id, created_at)
                VALUES (@ratingId, @userId, @createdAt);
            ", connection);

            cmd.Parameters.AddWithValue("@ratingId", likeRatingEntity.ratingId);
            cmd.Parameters.AddWithValue("@userId", likeRatingEntity.userId);
            cmd.Parameters.AddWithValue("@createdAt", likeRatingEntity.createdAt);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<MediaDeleteDTO> DeleteRatingAsync(int ratingId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var deleteCmd = new NpgsqlCommand("DELETE FROM ratings WHERE id = @id", connection);
            deleteCmd.Parameters.AddWithValue("id", ratingId);
            await deleteCmd.ExecuteNonQueryAsync();
            MediaDeleteDTO mediaDeleteDTO = new MediaDeleteDTO
            {
                Id = ratingId,
                DeletedAt = DateTime.UtcNow,
                IsDeleted = true
            };

            return mediaDeleteDTO;
        }


        // CRUD - Media Favorite 

        public async Task FavoriteMediaAsync(MediaFavoriteEntity mediaFavoriteEntity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var cmd = new NpgsqlCommand(@"
                INSERT INTO favorites (media_id, user_id, created_at)
                VALUES (@mediaId, @userId, @createdAt);", connection);
            cmd.Parameters.AddWithValue("@mediaId", mediaFavoriteEntity.mediaId);
            cmd.Parameters.AddWithValue("@userId", mediaFavoriteEntity.userId);
            cmd.Parameters.AddWithValue("@createdAt", mediaFavoriteEntity.createdAt);
            await cmd.ExecuteNonQueryAsync();

        }

        public async Task<MediaFavoriteEntity> GetFavoritesAsync(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var cmd = new NpgsqlCommand("SELECT * FROM favorites where user_id = @id", connection);
            cmd.Parameters.AddWithValue("id", userId);
            using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            
            
            MediaFavoriteEntity mediaFavoriteEntity = new MediaFavoriteEntity(mediaId: (int)reader["media_id"],
                userId: (int)reader["user_id"])
            {
                createdAt = (DateTime)reader["created_at"]
            };

            return mediaFavoriteEntity;
        }

        public async Task UnfavoriteMediaAsync(int mediaId, int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var deleteCmd = new NpgsqlCommand("DELETE FROM favorites WHERE media_id = @mediaId AND user_id = @userId", connection);
            deleteCmd.Parameters.AddWithValue("mediaId", mediaId);
            deleteCmd.Parameters.AddWithValue("userId", userId);
            await deleteCmd.ExecuteNonQueryAsync();
        }


        // -------------------------------- Personal statistics methods ----------------------------

        public async Task<PersonalStatsDTO> GetPersonalStatsAsync(int id)
        {
            Console.WriteLine("-------------------- personal statistics --------------------------");
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            string favoriteGenre = new string("(SELECT media.genres FROM ratings r" +
                " join media on r.media_id = media.id" +
                " where r.user_id = @id" +
                " group by media.genres" +
                " order by SUM(r.star) DESC" +
                " Limit 1)");

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT Count(r.id) as total_ratings," +
                " AVG(r.star) as star_avg," +
                $" {favoriteGenre} as favorite_media FROM ratings r" +
                " where r.user_id = @id", connection);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            long ratingCount = reader.GetInt64(0);
            double starAvg = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);
            string favoriteMedia = reader.IsDBNull(2) ? "N/A" : reader.GetString(2);
            Console.WriteLine($"Total Ratings: {ratingCount}");
            Console.WriteLine($"Star Average: {starAvg}");
            Console.WriteLine($"Favorite Media Genre: {favoriteMedia}");

            PersonalStatsDTO personalStatsDTO = new PersonalStatsDTO
            {
                ratingCount = (int)ratingCount,
                averageRating = starAvg,
                favoriteGenre = favoriteMedia
            }; 

            return personalStatsDTO;
        }

        public async Task<MediaStatsDTO> GetMediaStatsAsync(int mediaId)
        {
            Console.WriteLine("-------------------- media statistics --------------------------");
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT Count(r.id) as total_ratings," +
                " AVG(r.star) as star_avg" +
                " FROM ratings r" +
                " where r.media_id = @mediaId", connection);
            cmd.Parameters.AddWithValue("mediaId", mediaId);
            using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            long ratingCount = reader.GetInt64(0);
            double starAvg = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);
            Console.WriteLine($"Total Ratings: {ratingCount}");
            Console.WriteLine($"Star Average: {Math.Round(starAvg, 2)}");

            MediaStatsDTO mediaStatsDTO = new MediaStatsDTO
            {
                totalRatings = (int)ratingCount,
                averageRating = Math.Round(starAvg, 2)
            };

            return mediaStatsDTO;
        }


        public async Task<List<MediaRatingEntity>> GetRatingHistoryAsync(int userId)
        {
            Console.WriteLine("-------------------- rating history --------------------------");
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM ratings where user_id = @id", connection);
            cmd.Parameters.AddWithValue("id", userId);
            using var reader = await cmd.ExecuteReaderAsync();
            List<MediaRatingEntity> ratings = new List<MediaRatingEntity>();
            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);
                int star = reader.GetInt32(1);
                string comment = reader.IsDBNull(2) ? "N/A" : reader.GetString(2);
                DateTime createdAt = reader.GetDateTime(3);
                bool isConfirmed = reader.GetBoolean(4);
                int userIdRead = reader.GetInt32(5);
                int mediaId = reader.GetInt32(6);

                Console.WriteLine($"Id: {id}");
                Console.WriteLine($"Star: {star}");
                if (reader["is_confirmed"].Equals(true))
                {
                    Console.WriteLine($"IsConfirmed: {reader["is_confirmed"]}");
                    Console.WriteLine($"Comment: {reader["comment"]}");
                }
                else
                {
                    Console.WriteLine($"IsConfirmed : {reader["is_confirmed"]}");
                }
                Console.WriteLine($"CreatedAt: {createdAt}");
                Console.WriteLine($"UserId: {userIdRead}");
                Console.WriteLine($"MediaId: {mediaId}");
                Console.WriteLine("----------------------------------------");

                ratings.Add(new MediaRatingEntity(star, comment, userId, mediaId, isConfirmed)
                {
                    id = id,
                    createdAt = createdAt,
                });
            }

            return ratings;
        }

        public async Task<List<LeaderboardDTO>> GetLeaderboardAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            NpgsqlCommand cmd = new NpgsqlCommand(
                "SELECT user_id, count(id) as rating_count from ratings " +
                "GROUP BY user_id order by count(id) DESC"
                , connection);
            using var reader = await cmd.ExecuteReaderAsync();
            List<LeaderboardDTO> leaderboardList = new List<LeaderboardDTO>();
            Console.WriteLine("-------------------- Leaderboard --------------------------");
            while (await reader.ReadAsync())
            {
                int userId = reader.GetInt32(0);
                long ratingCount = reader.GetInt64(1);
                Console.WriteLine($"UserId: {userId} - Ratings given: {ratingCount}");
                leaderboardList.Add(new LeaderboardDTO
                {
                    userId = userId,
                    ratingCount = (int)ratingCount
                });
            }

            return leaderboardList;
        }


        // --------------------------------- Search + Filter --------------------------------

        public async Task<string> SearchMediaAsync(string title)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            for (int i = title.Length; i >= 3; i--)
            {
                string subTitle = title.Substring(0, i);
                using NpgsqlCommand cmdTest = new NpgsqlCommand("SELECT title FROM media WHERE title ILIKE @title", connection);
                cmdTest.Parameters.AddWithValue("title", $"%{subTitle}%");
                using NpgsqlDataReader readerTest = await cmdTest.ExecuteReaderAsync();
                if (await readerTest.ReadAsync())
                {
                    Console.WriteLine($"Found matching title: {readerTest["title"]}");
                    string foundTitle = readerTest["title"].ToString()!;
                    return foundTitle;
                }


            }


            return "no title has been found!";
        }

        public async Task<List<MediaFilterDTO>> FilterMediaAsync(string? genre, string? type, int? releaseYear, int? ageRestriction, int? star, string? sortBy)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var filterConditions = new List<string>();
            string sortCondition = "";
            using var cmd = connection.CreateCommand();
            if (genre != null)
            {
                filterConditions.Add("m.genres = @genre");
                cmd.Parameters.AddWithValue("genre", genre);
            }
            if (type != null)
            {
                filterConditions.Add("m.media_type = @type");
                cmd.Parameters.AddWithValue("type", type);
            }
            if (releaseYear != null)
            {
                filterConditions.Add("m.release_year = @releaseYear");
                cmd.Parameters.AddWithValue("releaseYear", releaseYear);
            }
            if (ageRestriction != null)
            {
                filterConditions.Add("m.age_restriction <= @ageRestriction");
                cmd.Parameters.AddWithValue("ageRestriction", ageRestriction);
            }
            if (star != null)
            {
                filterConditions.Add("r.star >= @star");
                cmd.Parameters.AddWithValue("star", star);
            }
            if (sortBy != null)
            {
                if (sortBy.Equals("title"))
                {
                    sortCondition = "ORDER BY title";
                }
                else if (sortBy.Equals("release_year"))
                {
                    sortCondition = "ORDER BY release_year DESC";
                }
                else if (sortBy.Equals("star"))
                {
                    sortCondition = "ORDER BY star DESC";
                }
            }


            if (filterConditions.Count == 0)
                throw new Exception("keine filter conditions"); 
            cmd.CommandText = $"Select m.id, m.title, m.media_type, m.genres, m.release_year, m.age_restriction, ROUND(AVG(r.star), 2) as avg, Count(r.id) as count from media m " +
                "LEFT JOIN ratings r ON m.id = r.media_id " +
                $"where {String.Join(" AND ", filterConditions)} " +
                "GROUP BY m.id " +
                $"{sortCondition}";
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            List<MediaFilterDTO> filteredMedia = new List<MediaFilterDTO>();
            while (await reader.ReadAsync())
            {
                Console.WriteLine("----------------------------------------");
                Console.WriteLine($"Id: {reader["id"]}");
                Console.WriteLine($"Title: {reader["title"]}");
                Console.WriteLine($"mediaType: {reader["media_type"]}");
                Console.WriteLine($"genres: {reader["genres"]}");
                Console.WriteLine($"releaseYear: {reader["release_year"]}");
                Console.WriteLine($"ageRestriction: {reader["age_restriction"]}");
                Console.WriteLine($"Average Star: {reader["avg"]}");
                Console.WriteLine($"Total Ratings: {reader["count"]}");
                Console.WriteLine("----------------------------------------");
                filteredMedia.Add(new MediaFilterDTO
                {
                    id = (int)reader["id"],
                    title = reader["title"].ToString(),
                    mediaType = reader["media_type"].ToString(),
                    genres = reader["genres"].ToString(),
                    releaseYear = (int)reader["release_year"],
                    ageRestriction = (int)reader["age_restriction"],
                    averageStar = reader.IsDBNull(6) ? null : (decimal?)reader["avg"],
                    totalRatings = reader.IsDBNull(7) ? null : (long?)reader["count"]
                });
            }

            return filteredMedia;

        }

        public async Task<MediaRecommendationDTO> GetRecommendationAsync(int userId, string? genres, string? type, int? ageRestriction)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var recommendationConditions = new List<string>();
            using var cmd = connection.CreateCommand();

            if (genres != null)
            {
                recommendationConditions.Add("m.genres = @genres");
                cmd.Parameters.AddWithValue("genres", genres);
            }
            if (type != null)
            {
                recommendationConditions.Add("m.media_type = @type");
                cmd.Parameters.AddWithValue("type", type);
            }
            if (ageRestriction != null)
            {
                recommendationConditions.Add("m.age_restriction <= @ageRestriction");
                cmd.Parameters.AddWithValue("ageRestriction", ageRestriction);
            }
            recommendationConditions.Add("m.user_id != @userId");
            cmd.Parameters.AddWithValue("userId", userId);
            if (recommendationConditions.Count == 0)
                throw new Exception("recommendation conditions sind 0");

            cmd.CommandText = $"Select m.id, m.title, m.media_type, m.genres, m.release_year, m.age_restriction," +
                $" ROUND(AVG(r.star), 2) as avg from media m " +
                  "LEFT JOIN ratings r ON m.id = r.media_id " +
                  $"where {String.Join(" AND ", recommendationConditions)} " +
                  $"group by m.id " +
                  $"having ROUND(AVG(r.star), 2) >= 4";
            


            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            MediaRecommendationDTO mediaRecommendationDTO = new MediaRecommendationDTO();
            await reader.ReadAsync();
            mediaRecommendationDTO.id = (int)reader["id"];
            mediaRecommendationDTO.title = reader["title"].ToString()!;
            mediaRecommendationDTO.mediaType = reader["media_type"].ToString()!;
            mediaRecommendationDTO.genres = reader["genres"].ToString()!;
            mediaRecommendationDTO.releaseYear = (int)reader["release_year"];
            mediaRecommendationDTO.ageRestriction = (int)reader["age_restriction"];
            mediaRecommendationDTO.star = reader.IsDBNull(6) ? null : (decimal?)reader["avg"];

            return mediaRecommendationDTO;



        }


        //----------------------------------- hilfsmethoden ---------------------------------
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

        public async Task<int?> GetMediaIdByTitle(string title)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var mediaIdCmd = new NpgsqlCommand("SELECT id FROM media WHERE title = @t", connection);
            mediaIdCmd.Parameters.AddWithValue("t", title);
            int? mediaId = (int)await mediaIdCmd.ExecuteScalarAsync();
            return mediaId;
        }

        public async Task<bool> IsRatingPublic(int ratingId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var isRatingPublicCmd = new NpgsqlCommand("SELECT is_confirmed FROM ratings WHERE id = @id", connection);
            isRatingPublicCmd.Parameters.AddWithValue("id", ratingId);

            bool isPublic = (bool)await isRatingPublicCmd.ExecuteScalarAsync();

            return isPublic;
        }

        public async Task<int> GetUserIdByRatingId(int ratingId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var getUserIdCmd = new NpgsqlCommand("SELECT user_id FROM ratings WHERE id = @id", connection);
            getUserIdCmd.Parameters.AddWithValue("id", ratingId);
            int userId = (int)await getUserIdCmd.ExecuteScalarAsync();
            return userId;
        }

        public async Task<bool> DoesMediaExistById(int mediaId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var mediaExistsCmd = new NpgsqlCommand("SELECT COUNT(*) FROM media WHERE id = @id", connection);
            mediaExistsCmd.Parameters.AddWithValue("id", mediaId);
            var count = (long)await mediaExistsCmd.ExecuteScalarAsync();
            return count > 0;
        }

        public async Task<int> GetMediaId(string title)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var getMediaCmd = new NpgsqlCommand("SELECT id FROM media WHERE title = @t", connection);
            getMediaCmd.Parameters.AddWithValue("t", title);
            return (int)await getMediaCmd.ExecuteScalarAsync();
        }



    }
}
