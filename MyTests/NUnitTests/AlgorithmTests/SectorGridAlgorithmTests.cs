using SmirK_Student.Domain.Drivers.Algorithms;
using SmirK_Student.Domain.Drivers.Algorithms.Core;
using SmirK_Student.Domain.Drivers.Map.Containers;

namespace MyTests.NUnitTests.AlgorithmTests
{

    public sealed class SectorGridAlgorithmTests : BaseAlgorithmTests<SpatialGrid>
    {
        private const int CellSize = 10;
        
        protected override SpatialGrid CreateContainer(int width, int height)
        {
            return new SpatialGrid(width, height, cellSize: CellSize);
        }

        protected override BaseDriverSearchStrategy<SpatialGrid> CreateAlgorithm()
        {
            return new SectorGridAlgorithm();
        }

        // Работа с секторами немного специфична, поэтому отдельный тест для него,
        // чтобы показать его работоспособность.
        [Test]
        public void FindNearestDrivers_DifferentCellSizes_WorksCorrectly()
        {
            // Arrange
            var smallCellGrid = new SpatialGrid(20, 20, cellSize: 2);
            var largeCellGrid = new SpatialGrid(20, 20, cellSize: 10);

            smallCellGrid.AddOrUpdateDriver(1, 10, 10);
            largeCellGrid.AddOrUpdateDriver(1, 10, 10);

            var algorithm = new SectorGridAlgorithm();

            // Act
            var resultSmall = algorithm.FindNearestDrivers(smallCellGrid, 5, 5, 5);
            var resultLarge = algorithm.FindNearestDrivers(largeCellGrid, 5, 5, 5);

            // Assert
            Assert.That(resultSmall, Has.Count.EqualTo(1));
            Assert.That(resultLarge, Has.Count.EqualTo(1));
            Assert.That(resultSmall[0].Distance, Is.EqualTo(resultLarge[0].Distance));
        }
    }
}