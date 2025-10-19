using MediaRatingPlatform_BusinessLogicLayer.Repositories;
using MediaRatingPlatform_DataAccessLayer.Repositories;
using MediaRatingPlatform_Domain.DTO;
using MediaRatingPlatform_Domain.Entities;
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

        public async Task CreateMediaAsync(MediaDTO mediaDTO, int userId)
        {
            MediaEntity mediaEntity = new MediaEntity(mediaDTO.title, mediaDTO.description, mediaDTO.mediaType,
                mediaDTO.releaseYear, mediaDTO.genres, mediaDTO.ageRestriction, userId);

            try
            {
                Console.WriteLine($"Creating Media {mediaEntity.title} successfull");
                await _mediaRepository.CreateMediaAsync(mediaEntity);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"mediaType = {mediaEntity.mediaType}");
                Console.WriteLine($"releaseYear = {mediaEntity.releaseYear}");
                Console.WriteLine($"age restriction = {mediaEntity.ageRestriction}");
                Console.WriteLine($"title = {mediaEntity.title}");
                Console.WriteLine($"id = {mediaEntity.id}");
                Console.WriteLine($"userId = {mediaEntity.userId}");
                Console.WriteLine($"created at = {mediaEntity.createdAt}");
                Console.WriteLine($"description = {mediaEntity.description}");
                Console.WriteLine($"updated at = {mediaEntity.updatedAt}");
                Console.WriteLine($"genres = {mediaEntity.genres}");

                Console.WriteLine($"Creating Media {mediaEntity.title} failed: " + ex.Message);
            }


        }
    }
}
