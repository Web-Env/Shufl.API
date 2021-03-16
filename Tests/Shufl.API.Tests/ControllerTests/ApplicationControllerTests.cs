using Shufl.API.Controllers;
using Xunit;

namespace Shufl.API.Tests.ControllerTests
{
    [Trait("Category", "Unit")]
    public class ApplicationControllerTests
    {
        private readonly ApplicationController _applicationController;

        public ApplicationControllerTests()
        {
            _applicationController = new ApplicationController();
        }

        [Fact]
        public void GetVersion_ShouldReturnAPIVersion()
        {
            //Arrange
            var version = typeof(Startup).Assembly.GetName().Version.ToString();

            //Act
            var receivedVersion = _applicationController.GetVersion().Value;

            //Assert
            Assert.Equal(version, receivedVersion);
        }
    }
}
