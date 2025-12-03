using MyTests.NUnitTests.MathAlgorithmTest.Core;
using SmirK_Student.Algorithms;
using SmirK_Student.Algorithms.Core;
using SmirK_Student.Map.Containers;

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