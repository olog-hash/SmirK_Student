using SmirK_Student.Domain.Drivers.Map.Containers;
using SmirK_Student.Domain.Drivers.Map.Containers.Core;

namespace MyTests.NUnitTests.ContainerTests
{
    
    [TestFixture]
    public sealed class ListGrisTests : BaseContainerTests
    {
        private const int CellSize = 10;

        protected override DriversContainer CreateContainer(int width, int height)
        {
            return new ListGrid(width, height);
        }
    }
}