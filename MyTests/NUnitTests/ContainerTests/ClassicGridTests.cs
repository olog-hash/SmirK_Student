using SmirK_Student.Map.Containers;
using SmirK_Student.Map.Core;

namespace MyTests.NUnitTests.ContainerTests
{
    [TestFixture]
    public sealed class ClassicGridTests : BaseContainerTests
    {
        protected override DriversContainer CreateContainer(int width, int height)
        {
            return new ClassicGrid(width, height);
        }
        
        [Test]
        public void ClassicGrid_ContentGrid_IsNotNull()
        {
            var grid = (ClassicGrid)_container;
            Assert.That(grid.ContentGrid, Is.Not.Null);
        }
    }
}