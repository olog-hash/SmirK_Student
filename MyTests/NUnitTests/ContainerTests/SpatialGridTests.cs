using SmirK_Student.Map.Containers;
using SmirK_Student.Map.Core;

namespace MyTests.NUnitTests.ContainerTests
{

    [TestFixture]
    public sealed class SpatialGridTests : BaseContainerTests
    {
        private const int CellSize = 10;

        protected override DriversContainer CreateContainer(int width, int height)
        {
            return new SpatialGrid(width, height, CellSize);
        }

        [Test]
        public void SpatialGrid_ContentGrid_IsNotNull()
        {
            var grid = (SpatialGrid)_container;
            Assert.That(grid.GridCells, Is.Not.Null);
        }
    }
}