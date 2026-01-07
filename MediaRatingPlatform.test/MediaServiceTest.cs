using MediaRatingPlatform_BusinessLogicLayer;
using MediaRatingPlatform_DataAccessLayer.Repositories.Interface;
using MediaRatingPlatform_Domain.DTO;
using Moq;
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
        public async Task CreateMedia_EmptyMediaType_ThrowsArgumentException()
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

    }
}
