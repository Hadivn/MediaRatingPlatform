using MediaRatingPlatform_BusinessLogicLayer;
using MediaRatingPlatform_Domain.DTO;
using MediaRatingPlatform_Domain.Entities;

namespace MediaRatingPlatfrom.Test
{
    [TestFixture]
    public class UserServiceTest
    {
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userService = new UserService(new TokenService());
        }

        [Test]
        public async Task RegisterUser_UserDoesNotExist()
        {
            UserRegisterDTO userRegisterDTO = new UserRegisterDTO
            {
                username = "Test3",
                password = "Test"
            };

            await _userService.RegisterUserAsync(userRegisterDTO);

            UserEntity userEntity = await _userService.GetUserByUsernameAsync(userRegisterDTO.username);

            Assert.That(userEntity, Is.Not.Null);
        }
    }
}
