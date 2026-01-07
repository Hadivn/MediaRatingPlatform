using MediaRatingPlatform_BusinessLogicLayer;
using MediaRatingPlatform_DataAccessLayer.Repositories.Interface;
using MediaRatingPlatform_Domain.DTO;
using MediaRatingPlatform_Domain.Entities;
using Moq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaRatingPlatform.Test
{
    public class MediaServiceTest
    {
        private MediaService _mediaService;
        private Mock<IMediaRepository> _mediaRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;

        public MediaServiceTest()
        {
            _mediaRepositoryMock = new Mock<IMediaRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mediaService = new MediaService(_mediaRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateMedia_EmptyMediaType_ThrowsException()
        {
            var mediaDto = new MediaDTO
            {
                title = "The Matrix",
                mediaType = null,
                genres = "Drama"
            };

            await Assert.ThrowsAsync<Exception>(() =>
                _mediaService.CreateMediaAsync(mediaDto, 1)
            );
        }

        [Fact]
        public async Task DeleteMedia_MediaDoesNotExist_ThrowException()
        {
            _mediaRepositoryMock.Setup(m => m.GetCreatedByUserId(It.IsAny<string>()))
                .ThrowsAsync(new NpgsqlException());

            await Assert.ThrowsAsync<NpgsqlException>(async () =>
            {
                await _mediaService.DeleteMediaByTitleAsync("notExist", 1);
            });
        }

        [Fact]
        public async Task DeleteMedia_Failed_ThrowException()
        {
            string title = "Fantastic Mr. Fox";
            int userId = 1;

            _mediaRepositoryMock.Setup(m => m.GetCreatedByUserId(It.IsAny<string>()))
                .ReturnsAsync(userId);

            _mediaRepositoryMock.Setup(m => m.DeleteMediaByTitle(It.IsAny<string>()))
                .ThrowsAsync(new Exception("failed to delete"));

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _mediaService.DeleteMediaByTitleAsync(title, userId);
            });
        }

        //[Fact]
        //public async Task CreateMedia_Successfull()
        //{
        //    var mediaEntity = new MediaEntity
        //    {

        //        title = "Close Up",
        //        description = "This fiction-documentary hybrid uses a sensational real-life event—the arrest of a young" +
        //        " man on charges that he fraudulently impersonated the well-known filmmaker Mohsen Makhmalbaf—as" +
        //        " the basis for a stunning, multilayered investigation into movies, identity, artistic creation," +
        //        " and existence, in which the real people from the case play themselves.",
        //        mediaType = 0,
        //        releaseYear = 1990,
        //        genres = "Sci-Fi, Thriller",
        //        ageRestriction = 16,
        //        userId = 1


        //    };

        //    var mediaDto = new MediaDTO
        //    {
        //        title = "Close Up",
        //        ageRestriction = 16,
        //        description = "This fiction-documentary hybrid uses a sensational real-life event—the arrest of a young";
        //        mediaType = 0,
        //        genres = "Sci-Fi, Thriller",
        //        releaseYear = 1990
        //    };

        //    _mediaRepositoryMock.Setup(m => m.CreateMediaAsync(It.IsAny<MediaEntity>()))
        //    .ReturnsAsync(Task.CompletedTask);
        //    await _mediaService.CreateMediaAsync(mediaDto, 1);
        //    _mediaRepositoryMock.Verify(m => m.CreateMediaAsync(It.IsAny<MediaEntity>()), Times.Once);

        //}

        [Fact]
        public async Task RateMedia_MediaDoesNotExist()
        {
            MediaRatingDTO mediaRatingDTO = new MediaRatingDTO
            {
                isConfirmed = true,
                comment = "Great movie!",
                star = 5

            };
            _mediaRepositoryMock.Setup(m => m.RateMediaAsync(It.IsAny<MediaRatingEntity>()))
                .ThrowsAsync(new NpgsqlException() );
            await Assert.ThrowsAsync<NpgsqlException>(async () =>
            {
                await _mediaService.RateMediaAsync(mediaRatingDTO, "this movie doesnt exist", 1);
            });
        }

       
    }
}
