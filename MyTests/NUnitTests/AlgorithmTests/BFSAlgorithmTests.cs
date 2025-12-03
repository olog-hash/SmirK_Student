using SmirK_Student.Algorithms;
using SmirK_Student.Algorithms.Core;
using SmirK_Student.Map.Containers;

namespace MyTests.NUnitTests.AlgorithmTests
{

    [TestFixture]
    public sealed class BFSAlgorithmTests : BaseAlgorithmTests<ClassicGrid>
    {
        protected override ClassicGrid CreateContainer(int width, int height)
        {
            return new ClassicGrid(width, height);
        }

        protected override BaseDriverSearchStrategy<ClassicGrid> CreateAlgorithm()
        {
            return new BFSAlgorithm();
        }
    }
}