using MediaRatingPlatform_BusinessLogicLayer.Repositories;
using MediaRatingPlatform_DataAccessLayer.Repositories;
using MediaRatingPlatform_Domain.DTO;
using MediaRatingPlatform_Domain.Entities;
using MediaRatingPlatform_Domain.ENUM;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_BusinessLogicLayer
{
    public class MediaService
    {
        private MediaRepository _mediaRepository;

        public MediaService()
        {
            _mediaRepository = new MediaRepository();
        }

        // CRUD - Media create
        public async Task CreateMediaAsync(MediaDTO mediaDTO, int userId)
        {

            // ENUM, from int to MediaType
            EMediaType eMediaType = (EMediaType)mediaDTO.mediaType;
            MediaEntity mediaEntity = new MediaEntity(mediaDTO.title, mediaDTO.description, eMediaType,
                mediaDTO.releaseYear, mediaDTO.genres, mediaDTO.ageRestriction, userId);

            try
            {
                await _mediaRepository.CreateMediaAsync(mediaEntity);
                Console.WriteLine($"Creating Media {mediaEntity.title} successfull");
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------ MEDIA CREATION FAILED ------------------");
                Console.WriteLine($"mediaType = {eMediaType}");
                Console.WriteLine($"releaseYear = {mediaEntity.releaseYear}");
                Console.WriteLine($"age restriction = {mediaEntity.ageRestriction}");
                Console.WriteLine($"title = {mediaEntity.title}");
                Console.WriteLine($"id = {mediaEntity.id}");
                Console.WriteLine($"userId = {mediaEntity.userId}");
                Console.WriteLine($"created at = {mediaEntity.createdAt}");
                Console.WriteLine($"description = {mediaEntity.description}");
                Console.WriteLine($"updated at = {mediaEntity.updatedAt}");
                Console.WriteLine($"genres = {mediaEntity.genres}");
                Console.WriteLine($"userId = {userId}");

                Console.WriteLine($"Creating Media {mediaEntity.title} failed: *{ex.Message}*");
                Console.WriteLine("------------------------------------------------------------");
            }


        }

        // CRUD - Media read
        public async Task ReadAllMediaAsync()
        {
            await _mediaRepository.ReadAllMediaAsync();
        }

        // CRUD - Media update
        public async Task UpdateMediaAsync(MediaUpdateDTO mediaUpdateDTO, string title, int userid)
        {
            int createdByUserId = await _mediaRepository.GetCreatedByUserId(title);
            if (createdByUserId != userid)
            {
                Console.WriteLine("Not allowed because of wrong userId\n----------------------------------");
                return;
            }
            try
            {
                await _mediaRepository.UpdateMediaAsync(mediaUpdateDTO, title);
                Console.WriteLine($"Updating Media {title} successfull");
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------ MEDIA UPDATE FAILED ------------------");
                Console.WriteLine($"Updating Media {title} failed: *{ex.Message}*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
        }

        // CRUD - Media delete
        public async Task DeleteMediaByTitleAsync(string title, int userid)
        {
            int createdByUserId = await _mediaRepository.GetCreatedByUserId(title);
            if (createdByUserId != userid)
            {
                Console.WriteLine("Not allowed because of wrong userId\n----------------------------------");
                return;
            }
            try
            {
                await _mediaRepository.DeleteMediaByTitle(title);
                Console.WriteLine($"Deleting Media {title} successfull");
            }
            catch (NpgsqlException npgsqlEx) when (npgsqlEx.SqlState == "P0001")
            {
                Console.WriteLine("------------------ MEDIA DELETION FAILED ------------------");
                Console.WriteLine($"Deleting Media {title} failed: *Media does not exist.*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------ MEDIA DELETION FAILED ------------------");
                Console.WriteLine($"Deleting Media {title} failed: *{ex.Message}*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
        }

        // --------------------------------- Media Rating --------------------------------
        public async Task RateMediaAsync(MediaRatingDTO mediaRatingDTO, string title, int userId)
        {

            int mediaId = await _mediaRepository.GetMediaIdByTitle(title);
            MediaRatingEntity mediaRatingEntity = new MediaRatingEntity(mediaRatingDTO.star, mediaRatingDTO.comment, userId, mediaId, mediaRatingDTO.isConfirmed);

            try
            {
                await _mediaRepository.RateMediaAsync(mediaRatingEntity);
                Console.WriteLine($"rating Media {title} successfull");
            }
            catch (NpgsqlException npgsqlEx) when (npgsqlEx.SqlState == "23503")
            {
                Console.WriteLine("------------------ CREATING MEDIA RATING FAILED ------------------");
                Console.WriteLine($"rating Media {title} failed: *Media does not exist.*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
            catch (NpgsqlException npgsqlEx) when (npgsqlEx.SqlState == "23505")
            {
                Console.WriteLine("------------------ CREATING MEDIA RATING FAILED ------------------");
                Console.WriteLine($"rating Media {title} failed: *User has already rated this media.*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------ CREATING MEDIA RATING FAILED ------------------");
                Console.WriteLine($"rating Media {title} failed: *{ex.Message}*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }

        }

        public async Task ReadAllMediaRatingsAsync()
        {
            await _mediaRepository.ReadAllMediaRatingsAsync();
        }

        public async Task UpdateMediaRatingAsync(MediaRatingUpdateDTO mediaRatingUpdateDTO, int ratingId, int userId)
        {
            int createdByUserId = await _mediaRepository.GetUserIdByRatingId(ratingId);
            if (createdByUserId != userId)
            {
                Console.WriteLine("Not allowed because of wrong userId\n----------------------------------");
                return;
            }
            try
            {
                await _mediaRepository.UpdateMediaRatingAsync(mediaRatingUpdateDTO, ratingId);
                Console.WriteLine($"Updating Media Rating {ratingId} successfull");
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------ UPDATING MEDIA RATING FAILED ------------------");
                Console.WriteLine($"Updating Media Rating {ratingId} failed: *{ex.Message}*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
        }

        public async Task LikeRatingAsync(LikeRatingDTO likeRatingDTO, int userId)
        {
            if (!await _mediaRepository.IsRatingPublic(likeRatingDTO.ratingId))
            {
                throw new Exception("Cannot like a private rating.");
            }
            LikeRatingEntity likeRatingEntity = new LikeRatingEntity(likeRatingDTO.ratingId, userId);
            try
            {
                await _mediaRepository.LikeRatingAsync(likeRatingEntity);
            }
            catch (NpgsqlException npgsqlEx) when (npgsqlEx.SqlState == "23505")
            {
                Console.WriteLine("------------------ LIKING RATING FAILED ------------------");
                Console.WriteLine($"Liking Rating {likeRatingDTO.ratingId} failed: *Rating already liked by this user.*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");


            }
            catch (NpgsqlException npgsqlEx) when (npgsqlEx.SqlState == "23503")
            {
                Console.WriteLine("------------------ LIKING RATING FAILED ------------------");
                Console.WriteLine($"Liking Rating {likeRatingDTO.ratingId} failed: *Rating does not exist.*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------ LIKING RATING FAILED ------------------");
                Console.WriteLine($"Liking Rating {likeRatingDTO.ratingId} failed: *{ex.Message}*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }



        }

        public async Task DeleteRatingAsync(int ratingId, int userId)
        {

            int createdByUserId = await _mediaRepository.GetUserIdByRatingId(ratingId);
            if (createdByUserId != userId)
            {
                Console.WriteLine("Not allowed because of wrong userId\n----------------------------------");
                return;
            }

            try
            {
                await _mediaRepository.DeleteRatingAsync(ratingId);
                Console.WriteLine($"Deleting Rating {ratingId} successfull");
            }
            catch (NpgsqlException npgsqlEx) when (npgsqlEx.SqlState == "P0001")
            {
                Console.WriteLine("------------------ DELETING RATING FAILED ------------------");
                Console.WriteLine($"Deleting Rating {ratingId} failed: *Rating does not exist.*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
            catch (NpgsqlException npgsqlEx) when (npgsqlEx.SqlState == "23503")
            {
                Console.WriteLine("------------------ DELETING RATING FAILED ------------------");
                Console.WriteLine($"Deleting Rating {ratingId} failed: *Rating is referenced by other entities.*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------ DELETING RATING FAILED ------------------");
                Console.WriteLine($"Deleting Rating {ratingId} failed: *{ex.Message}*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");

            }
        }


        // --------------------------------- Media Favorite --------------------------------

        public async Task FavoriteMediaAsync(int mediaId, int userId)
        {
            MediaFavoriteEntity mediaFavoriteEntity = new MediaFavoriteEntity(mediaId, userId);
            try
            {
                await _mediaRepository.FavoriteMediaAsync(mediaFavoriteEntity);
                Console.WriteLine($"Favoriting Media {mediaId} successfull");
            }
            catch (NpgsqlException npgsqlEx) when (npgsqlEx.SqlState == "23505")
            {
                Console.WriteLine("------------------ FAVORITING MEDIA FAILED ------------------");
                Console.WriteLine($"Favoriting Media {mediaId} failed: *Media already favorited by this user.*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------ FAVORITING MEDIA FAILED ------------------");
                Console.WriteLine($"Favoriting Media {mediaId} failed: *{ex.Message}*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
        }

        public async Task ReadAllFavoriteMediaAsync()
        {
            await _mediaRepository.ReadAllMediaFavoritesAsync();
        }

        public async Task UnfavoriteMediaAsync(int mediaId, int userId)
        {
            try
            {
                await _mediaRepository.UnfavoriteMediaAsync(mediaId, userId);
                Console.WriteLine($"Deleting Favorite Media {mediaId} successfull");
            }
            catch (NpgsqlException npgsqlEx) when (npgsqlEx.SqlState == "P0001")
            {
                Console.WriteLine("------------------ DELETING FAVORITE MEDIA FAILED ------------------");
                Console.WriteLine($"Deleting Favorite Media {mediaId} failed: *Favorite does not exist.*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------ DELETING FAVORITE MEDIA FAILED ------------------");
                Console.WriteLine($"Deleting Favorite Media {mediaId} failed: *{ex.Message}*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
        }

        // --------------------------------- Personal Stats --------------------------------

        public async Task GetPersonalStatsAsync(int userId)
        {
            await _mediaRepository.GetPersonalStatsAsync(userId);

        }
    }
}
