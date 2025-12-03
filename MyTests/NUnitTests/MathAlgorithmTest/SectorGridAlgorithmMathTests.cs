using MyTests.NUnitTests.MathAlgorithmTest.Core;
using SmirK_Student.Algorithms;
using SmirK_Student.Algorithms.Core;
using SmirK_Student.Map.Containers;

namespace MyTests.NUnitTests.MathAlgorithmTest
{
    [TestFixture]
    public class SectorGridAlgorithmMathTests : BaseAlgorithmMathTests<SpatialGrid>
    {
        private const int CellSize = 10;

        protected override SpatialGrid CreateContainer(int width, int height)
        {
            // Предполагаю что SpatialGrid требует размер ячейки
            // Если конструктор другой - адаптируйте
            return new SpatialGrid(width, height, cellSize: CellSize);
        }

        protected override BaseDriverSearchStrategy<SpatialGrid> CreateAlgorithm()
        {
            return new SectorGridAlgorithm();
        }

        /// <summary>
        /// Особая проверка алгоритма для секторной сетки.
        /// </summary>
        [Test]
        public void FindNearestDrivers_FartherLayerCloserDistance_FindsCorrectDriver()
        {
            // Изначально, во время анимания алгоритма для секторной сетки  я исходил из того, что если искать водителя
            // по слоям (либо вокруг, либо в виде ромба) от центральной ячейки - то водители из дальних слоев точно будут дальше, чем водители из ближних.
            // Я... был не прав. Позвольте продемонстрировать почему.

            // Допустим есть некоторая карта 30x15 и размеро ячеек ~10 клеток.
            // Точка заказа находиться на позиции (5,5) и два водителя с координатами (20,5) и (19,9) 
            // Если набросать схему/рисунок, отчетливо видно что (20,5) будет находиться в 3-ем слое, а (19,9) во 2-м.
            // Следовательно, по моей изначальной задумке, это должен быть ближайший водитель. Но как только я прорешал этот пример
            // то обнаружил, что математический манхэтаннское расстояние будет ближе к первому.

            // "Дальний водитель" ~ |20 - 5| + |5 - 5| = 15
            // "Ближний водитель" ~ |19 - 5| + |9 - 5| = 18 <- будет дальше.

            // Я учел этот момент в своем алгоритме добавив вычисление минимального расстояния до следующего слоя.
            // "Если расстояние самого дальнего найденного водителя больше чем расстояние до следующего слоя - то есть шанс, что там может быть кто-то ближе"
            // Это один из немногих вариантов, что я смог найти. Уж больно мне не хотелось ограничиваться простым перебором всех возможных слоев.

            // === Arrange ===
            _container = CreateContainer(width: 30, height: 15);

            int orderX = 5, orderY = 5; // Позиция заказа

            // Водитель в ячейке первого слоя (ближе по секторам)
            _container.AddOrUpdateDriver(driverID: 1, x: 19, y: 9);

            // Водитель в ячейке второго слоя (дальше по секторам)
            _container.AddOrUpdateDriver(driverID: 2, x: 20, y: 5);

            // Act - просим найти одного ближайшего водителя
            var result = _algorithm.FindNearestDrivers(_container, orderX, orderY, maxDrivers: 1);

            // === Assert ===

            // Должен быть только один водитель
            Assert.That(result, Has.Count.EqualTo(1));

            // Критическая проверка: должен найтись водитель 2, а не водитель 1
            Assert.That(result[0].DriverOnMap.Driver.ID, Is.EqualTo(2));

            // Проверяем корректность расчета расстояния
            Assert.That(result[0].Distance, Is.EqualTo(15));

            // Дополнительная проверка: если запросим обоих водителей
            var resultBoth = _algorithm.FindNearestDrivers(_container, orderX, orderY, maxDrivers: 2);
            Assert.That(resultBoth, Has.Count.EqualTo(2));

            // Первым должен быть водитель 2 (ближе)
            Assert.That(resultBoth[0].DriverOnMap.Driver.ID, Is.EqualTo(2));
            Assert.That(resultBoth[0].Distance, Is.EqualTo(15));

            // Вторым должен быть водитель 1 (дальше)
            Assert.That(resultBoth[1].DriverOnMap.Driver.ID, Is.EqualTo(1));
            Assert.That(resultBoth[1].Distance, Is.EqualTo(18));
        }
    }
}