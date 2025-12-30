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

        // CRUD - Media create
        public async Task CreateMediaAsync(MediaDTO mediaDTO, int userId)
        {
            MediaEntity mediaEntity = new MediaEntity(mediaDTO.title, mediaDTO.description, mediaDTO.mediaType,
                mediaDTO.releaseYear, mediaDTO.genres, mediaDTO.ageRestriction, userId);

            try
            {
                await _mediaRepository.CreateMediaAsync(mediaEntity);
                Console.WriteLine($"Creating Media {mediaEntity.title} successfull");
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------ MEDIA CREATION FAILED ------------------");
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
        public async Task UpdateMediaAsync(MediaUpdateDTO mediaUpdateDTO, string title)
        {
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
        public async Task DeleteMediaByTitleAsync(string title)
        {
            try
            {
                await _mediaRepository.DeleteMediaByTitle(title);
                Console.WriteLine($"Deleting Media {title} successfull");
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------ MEDIA DELETION FAILED ------------------");
                Console.WriteLine($"Deleting Media {title} failed: *{ex.Message}*");
                Console.WriteLine("Exception in BusinessLogic-Layer");
                Console.WriteLine("------------------------------------------------------------");
            }
        }

       
    }
}
