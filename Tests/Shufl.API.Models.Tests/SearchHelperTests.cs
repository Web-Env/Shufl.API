using Shufl.API.Models.Helpers;
using Xunit;

namespace Shufl.API.Models.Tests
{
    [Trait("Category", "Unit")]
    public class SearchHelperTests
    {
        [Fact]
        public void RandInt_ShouldReturnIntInRange()
        {
            //Arrange
            var min = 0;
            var max = 100;

            //Act
            var randInt = SearchHelper.RandInt(min, max);

            //Assert
            Assert.InRange(randInt, min, max);
        }
    }
}
