using MyTests.NUnitTests.MathAlgorithmTest.Core;
using SmirK_Student.Domain.Drivers.Algorithms;
using SmirK_Student.Domain.Drivers.Algorithms.Core;
using SmirK_Student.Domain.Drivers.Map.Containers;

namespace MyTests.NUnitTests.MathAlgorithmTest
{
    [TestFixture]
    public class BFSAlgorithmMathTests : BaseAlgorithmMathTests<ClassicGrid>
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