using MyTests.NUnitTests.MathAlgorithmTest.Core;
using SmirK_Student.Domain.Drivers.Algorithms;
using SmirK_Student.Domain.Drivers.Algorithms.Core;
using SmirK_Student.Domain.Drivers.Map.Containers;

namespace MyTests.NUnitTests.MathAlgorithmTest
{
    [TestFixture]
    public class BruteForceAlgorithmMathTests : BaseAlgorithmMathTests<ListGrid>
    {
        protected override ListGrid CreateContainer(int width, int height)
        {
            return new ListGrid(width, height);
        }

        protected override BaseDriverSearchStrategy<ListGrid> CreateAlgorithm()
        {
            return new BruteForceAlgorithm();
        }
    }
}