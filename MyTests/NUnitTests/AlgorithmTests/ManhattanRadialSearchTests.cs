using SmirK_Student.Domain.Drivers.Algorithms;
using SmirK_Student.Domain.Drivers.Algorithms.Core;
using SmirK_Student.Domain.Drivers.Map.Containers;

namespace MyTests.NUnitTests.AlgorithmTests
{

    [TestFixture]
    public sealed class ManhattanRadialSearchTests : BaseAlgorithmTests<ClassicGrid>
    {
        protected override ClassicGrid CreateContainer(int width, int height)
        {
            return new ClassicGrid(width, height);
        }

        protected override BaseDriverSearchStrategy<ClassicGrid> CreateAlgorithm()
        {
            return new ManhattanRadialSearch();
        }
    }
}