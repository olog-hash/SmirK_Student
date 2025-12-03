using MyTests.NUnitTests.MathAlgorithmTest.Core;
using SmirK_Student.Algorithms;
using SmirK_Student.Algorithms.Core;
using SmirK_Student.Map.Containers;

namespace MyTests.NUnitTests.MathAlgorithmTest
{
    [TestFixture]
    public class ManhattanRadialSearchAlgorithmMathTests : BaseAlgorithmMathTests<ClassicGrid>
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