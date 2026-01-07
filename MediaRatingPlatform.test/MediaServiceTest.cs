using MediaRatingPlatform_BusinessLogicLayer;
using MediaRatingPlatform_DataAccessLayer.Repositories.Interface;
using MediaRatingPlatform_Domain.DTO;
using MediaRatingPlatform_Domain.Entities;
using Moq;
using Npgsql;

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

        //MediaEntity mediaEntity = new MediaEntity("Close Up", "This fiction-documentary", 0,
        //      1990, "Sci-Fi, Thriller", 16, 1);

        //    var mediaEntity = new MediaEntity
        //    {

        //        title = "Close Up",
        //        description = "This fiction-documentary",
        //      
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
                .ThrowsAsync(new NpgsqlException());
            await Assert.ThrowsAsync<NpgsqlException>(async () =>
            {
                await _mediaService.RateMediaAsync(mediaRatingDTO, "this movie doesnt exist", 1);
            });
        }


        [Fact]
        public async Task RateMedia_Failed_ThrowException()
        {
            MediaRatingDTO mediaRatingDTO = new MediaRatingDTO
            {
                isConfirmed = true,
                star = 5
            };
            _mediaRepositoryMock.Setup(m => m.RateMediaAsync(It.IsAny<MediaRatingEntity>()))
                .ThrowsAsync(new Exception("failed to rate"));
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _mediaService.RateMediaAsync(mediaRatingDTO, "The Thing", 1);
            });

        }

        [Fact]
        public async Task RateMedia_ScoreTooLow()
        {
            MediaRatingDTO mediaRatingDTO = new MediaRatingDTO
            {
                isConfirmed = true,
                star = 0
            };

            var ex = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _mediaService.RateMediaAsync(mediaRatingDTO, "The Thing", 1);
            });

            Assert.Contains("Star rating must be 1 or higher", ex.Message);
        }

        [Fact]
        public async Task RateMedia_ScoreTooHigh()
        {
            MediaRatingDTO mediaRatingDTO = new MediaRatingDTO
            {
                isConfirmed = true,
                star = 8
            };

            var ex = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _mediaService.RateMediaAsync(mediaRatingDTO, "The Thing", 1);
            });

            Assert.Contains("Star rating must be 5 or lower", ex.Message);
        }

        [Fact]
        public async Task FavoriteMedia_MediaDoesNotExist()
        {

            MediaFavoriteEntity mediaFavoriteEntity = new MediaFavoriteEntity(4143, 1);

            _mediaRepositoryMock.Setup(m => m.DoesMediaExistById(mediaFavoriteEntity.mediaId))
                .ReturnsAsync(false);


            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _mediaService.FavoriteMediaAsync(mediaFavoriteEntity.mediaId, mediaFavoriteEntity.userId);
            });
        }

        [Fact]
        public async Task FavoriteMedia_AlreadyFavorited()
        {
            MediaFavoriteEntity mediaFavoriteEntity = new MediaFavoriteEntity(1, 1);


            _mediaRepositoryMock.Setup(m => m.DoesMediaExistById(mediaFavoriteEntity.mediaId))
                .ReturnsAsync(true);

            _mediaRepositoryMock.Setup(m => m.FavoriteMediaAsync(It.IsAny<MediaFavoriteEntity>()))
             .ThrowsAsync(new NpgsqlException("media bereits favorisiert"));

            await Assert.ThrowsAsync<NpgsqlException>(async () =>
            {
                await _mediaService.FavoriteMediaAsync(mediaFavoriteEntity.mediaId, mediaFavoriteEntity.userId);
            });

        }

        [Fact]
        public async Task FavoriteMedia_Success()
        {
            MediaFavoriteEntity mediaFavoriteEntity = new MediaFavoriteEntity(2, 1);

            _mediaRepositoryMock.Setup(m => m.DoesMediaExistById(mediaFavoriteEntity.mediaId))
                .ReturnsAsync(true);

            _mediaRepositoryMock.Setup(m => m.FavoriteMediaAsync(It.IsAny<MediaFavoriteEntity>()))
             .Returns(Task.CompletedTask);

            await _mediaService.FavoriteMediaAsync(mediaFavoriteEntity.mediaId, mediaFavoriteEntity.userId);
            _mediaRepositoryMock.Verify(m => m.FavoriteMediaAsync(It.IsAny<MediaFavoriteEntity>()), Times.Once);
        }

        [Fact]
        public async Task SearchMedia_DoesNotExist()
        {
            string title = "NonExistentMediaTitle";
            _mediaRepositoryMock.Setup(m => m.GetMediaIdByTitle(title))
                .ReturnsAsync((int?)null);

            var ex = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _mediaService.SearchMediaAsync(title);
            });

            Assert.Contains("Media does not exist.", ex.Message);

        }

        [Fact]
        public async Task UnfavoriteMedia_MediaDoesNotExist()
        {
            int mediaId = 4143;
            int userId = 1;

            _mediaRepositoryMock.Setup(m => m.DoesMediaExistById(mediaId))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _mediaService.FavoriteMediaAsync(mediaId, userId);
            });

        }

        [Fact]
        public async Task UnfavoriteMedia_Success()
        {
            int mediaId = 2;
            int userId = 1;

            _mediaRepositoryMock.Setup(m => m.DoesMediaExistById(mediaId))
                .ReturnsAsync(true);

            _mediaRepositoryMock.Setup(m => m.UnfavoriteMediaAsync(mediaId, userId))
             .Returns(Task.CompletedTask);

            await _mediaService.UnfavoriteMediaAsync(mediaId, userId);
            _mediaRepositoryMock.Verify(m => m.UnfavoriteMediaAsync(mediaId, userId), Times.Once);
        }

        [Fact]
        public async Task SearchMedia_SortByIsNull()
        {
            await _mediaService.FilterMediaAsync(null, null, null, null, null, null);

            _mediaRepositoryMock.Verify(m => m.FilterMediaAsync(null, null, null, null, null, "title"), Times.Once);
        }

        [Fact]
        public async Task SearchMedia_Successfull()
        {
            string title = "Whiplash";

            _mediaRepositoryMock.Setup(m => m.GetMediaIdByTitle(title))
                .ReturnsAsync(1);
            _mediaRepositoryMock.Setup(m => m.SearchMediaAsync(title))
                .ReturnsAsync("Whiplash");

            await _mediaService.SearchMediaAsync(title);
            _mediaRepositoryMock.Verify(m => m.SearchMediaAsync(title), Times.Once);

        }


    }
}
