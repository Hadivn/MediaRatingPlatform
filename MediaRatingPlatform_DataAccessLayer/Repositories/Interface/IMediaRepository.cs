using MediaRatingPlatform_Domain.DTO;
using MediaRatingPlatform_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_DataAccessLayer.Repositories.Interface
{
    public interface IMediaRepository
    {
        Task CreateMediaAsync(MediaEntity mediaEntity);
        Task<List<MediaDTO>> ReadAllMediaAsync();
        Task UpdateMediaAsync(MediaUpdateDTO mediaUpdateDTO, string title);
        Task DeleteMediaByTitle(string title);
        Task RateMediaAsync(MediaRatingEntity mediaRatingEntity);
        Task<List<MediaAllRatingsDTO>> ReadAllMediaRatingsAsync();
        Task UpdateMediaRatingAsync(MediaRatingUpdateDTO mediaRatingUpdateDTO, int ratingId);
        Task LikeRatingAsync(LikeRatingEntity likeRatingEntity);
        Task<MediaDeleteDTO> DeleteRatingAsync(int ratingId);
        Task FavoriteMediaAsync(MediaFavoriteEntity mediaFavoriteEntity);
        Task<MediaFavoriteEntity> GetFavoritesAsync(int userId);
        Task UnfavoriteMediaAsync(int mediaId, int userId);
        Task<PersonalStatsDTO> GetPersonalStatsAsync(int id);

        Task<MediaStatsDTO> GetMediaStatsAsync(int mediaId);
        Task<List<MediaRatingEntity>> GetRatingHistoryAsync(int userId);
        Task<List<LeaderboardDTO>> GetLeaderboardAsync();
        Task<string> SearchMediaAsync(string title);
        Task<List<MediaFilterDTO>> FilterMediaAsync(string? genre, string? type, int? releaseYear, int? ageRestriction, int? star, string? sortBy);
        Task<MediaRecommendationDTO> GetRecommendationAsync(int userId, string? genres, string? type, int? ageRestriction);
        Task<bool> MediaExists(string title);
        Task<int> GetCreatedByUserId(string title);
        Task<int?> GetMediaIdByTitle(string title);
        Task<bool> DoesMediaExistById(int mediaId);
        Task<bool> IsRatingPublic(int ratingId);
        Task<int> GetUserIdByRatingId(int ratingId);
        Task<int> GetMediaId(string title);
        Task<MediaDTO> ReadMediaByTitle(string title);

    }
}
