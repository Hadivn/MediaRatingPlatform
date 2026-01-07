using MediaRatingPlatform_BusinessLogicLayer;
using MediaRatingPlatform_BusinessLogicLayer.Repositories;
using MediaRatingPlatform_DataAccessLayer.Repositories.Interface;
using MediaRatingPlatform_Domain.DTO;
using MediaRatingPlatform_Domain.Entities;
using Moq;
using Xunit;

namespace MediaRatingPlatform.test
{
    // AAA, Arrange, Act, Assert
    public class UserServiceTest
    {

        private UserService _userService;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<TokenService> _tokenServiceMock;

        public UserServiceTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<TokenService>();
            _userService = new UserService(_userRepositoryMock.Object, _tokenServiceMock.Object);
        }

       

        [Fact]
        public async Task RegisterUser_Exists_ThrowException()
        {
            var dto = new UserRegisterDTO
            {
                username = "Test1",
                password = "Test"
            };

            _userRepositoryMock
                .Setup(repo => repo.DoesUserExist("Test1"))
                .ReturnsAsync(true); 

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _userService.RegisterUserAsync(dto);
            });
        }

        [Fact]
        public async Task RegisterUser_ValidUser_Success()
        {
            var dto = new UserRegisterDTO
            {
                username = "newuser",
                password = "password123"
            };

            _userRepositoryMock
                .Setup(repo => repo.DoesUserExist("newuser"))
                .ReturnsAsync(false);

            _userRepositoryMock
                .Setup(repo => repo.CreateUser(It.IsAny<UserEntity>()))
                .Returns(Task.CompletedTask);

            var result = await _userService.RegisterUserAsync(dto);
           
            Assert.NotNull(result);
            Assert.Equal("newuser", result.username);
            Assert.Equal("password123", result.password);

            _userRepositoryMock.Verify(repo =>
                repo.CreateUser(It.IsAny<UserEntity>()),
                Times.Once);
        }

        [Fact]
        public async Task RegisterUser_IsNull()
        {
           var dto = new UserRegisterDTO
            {
                username = null,
                password = null
            };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _userService.RegisterUserAsync(dto);
            });
        }

        [Fact]
        public async Task LoginUser_UserDoesNotExists_ThrowException()
        {
            _userRepositoryMock
                .Setup(repo => repo.GetByUsernameAsync("Test1"))
                .ReturnsAsync(false);
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _userService.LoginUserAsync("Test1", "pass");
            });
        }

       





    }
}
