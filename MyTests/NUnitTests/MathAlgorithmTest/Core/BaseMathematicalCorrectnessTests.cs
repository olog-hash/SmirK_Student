using SmirK_Student.Algorithms.Core;
using SmirK_Student.Map.Core;

namespace MyTests.NUnitTests.MathAlgorithmTest.Core
{
    /// <summary>
    /// Базовый класс для тестирования математической корректности алгоритмов поиска водителей.
    /// Проверяет что алгоритмы находят именно ближайших водителей и корректно вычисляют расстояния.
    /// (Хотя стоит отметить, что результат не всегда должен быть одинаков для алгоритмов ибо ожет быть несколько ближайших водителей).
    /// </summary>
    public abstract class BaseAlgorithmMathTests<TContainer>
        where TContainer : DriversContainer
    {
        protected TContainer _container { get; set; } = null;
        protected BaseDriverSearchStrategy<TContainer> _algorithm { get; set; } = null;

        protected abstract TContainer CreateContainer(int width, int height);
        protected abstract BaseDriverSearchStrategy<TContainer> CreateAlgorithm();

        [SetUp]
        public void SetUp()
        {
            _algorithm = CreateAlgorithm();
        }

        #region Проверка корректности расчета расстояний

        // Проверяет, что водитель в точке заказа имеет расстояние 0
        [Test]
        public void FindNearestDrivers_DriverAtSearchPoint_DistanceIsZero()
        {
            // Arrange
            _container = CreateContainer(10, 10);
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);

            // Act
            var result = _algorithm.FindNearestDrivers(_container, 5, 5, 5);

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Distance, Is.EqualTo(0));
            Assert.That(result[0].DriverOnMap.Driver.ID, Is.EqualTo(1));
        }

        // Проверяет правильность вычисления манхэттенского расстояния для известных точек
        [Test]
        public void FindNearestDrivers_KnownDistances_CalculatedCorrectly()
        {
            // Arrange
            _container = CreateContainer(15, 15);
            _container.AddOrUpdateDriver(driverID: 1, 7, 7);   // расстояние = 0
            _container.AddOrUpdateDriver(driverID: 2, 8, 7);   // расстояние = 1
            _container.AddOrUpdateDriver(driverID: 3, 7, 8);   // расстояние = 1
            _container.AddOrUpdateDriver(driverID: 4, 9, 9);   // расстояние = 4
            _container.AddOrUpdateDriver(driverID: 5, 7, 10);  // расстояние = 3

            // Act
            var result = _algorithm.FindNearestDrivers(_container, 7, 7, 10);

            // Assert
            Assert.That(result, Has.Count.EqualTo(5));
            
            // Проверяем каждое расстояние вручную
            var driver1 = result.First(d => d.DriverOnMap.Driver.ID == 1);
            Assert.That(driver1.Distance, Is.EqualTo(0));
            
            var driver2 = result.First(d => d.DriverOnMap.Driver.ID == 2);
            Assert.That(driver2.Distance, Is.EqualTo(1));
            
            var driver3 = result.First(d => d.DriverOnMap.Driver.ID == 3);
            Assert.That(driver3.Distance, Is.EqualTo(1));
            
            var driver4 = result.First(d => d.DriverOnMap.Driver.ID == 4);
            Assert.That(driver4.Distance, Is.EqualTo(4));
            
            var driver5 = result.First(d => d.DriverOnMap.Driver.ID == 5);
            Assert.That(driver5.Distance, Is.EqualTo(3));
        }

        // Проверяет, что все найденные расстояния неотрицательные
        [Test]
        public void FindNearestDrivers_AllDistances_AreNonNegative()
        {
            // Arrange
            _container = CreateContainer(10, 10);
            for (int i = 0; i < 10; i++)
            {
                _container.AddOrUpdateDriver(driverID: i, i, i);
            }

            // Act
            var result = _algorithm.FindNearestDrivers(_container, 5, 5, 10);

            // Assert
            Assert.That(result.All(d => d.Distance >= 0), Is.True);
        }

        #endregion

        #region Проверка свойства "ближайших"

        // Проверяет, что если нашли водителя на расстоянии D, 
        // то все более близкие водители тоже должны быть найдены
        [Test]
        public void FindNearestDrivers_CloserDriversNotMissed()
        {
            // Arrange
            _container = CreateContainer(20, 20);
            _container.AddOrUpdateDriver(driverID: 1, 10, 10); // расстояние = 0
            _container.AddOrUpdateDriver(driverID: 2, 11, 10); // расстояние = 1
            _container.AddOrUpdateDriver(driverID: 3, 12, 10); // расстояние = 2
            _container.AddOrUpdateDriver(driverID: 4, 13, 10); // расстояние = 3
            _container.AddOrUpdateDriver(driverID: 5, 14, 10); // расстояние = 4
            _container.AddOrUpdateDriver(driverID: 6, 15, 10); // расстояние = 5

            // Act - просим найти 4 водителей
            var result = _algorithm.FindNearestDrivers(_container, 10, 10, 4);

            // Assert
            Assert.That(result, Has.Count.EqualTo(4));
            
            // Максимальное расстояние в результатах
            int maxDistance = result.Max(d => d.Distance);
            
            // Проверяем что нет "пропусков" - все водители с меньшими расстояниями найдены
            for (int expectedDistance = 0; expectedDistance < maxDistance; expectedDistance++)
            {
                bool hasDriverAtDistance = result.Any(d => d.Distance == expectedDistance);
                Assert.That(hasDriverAtDistance, Is.True);
            }
        }

        // Проверяет поиск водителей на концентрических "слоях" расстояний
        [Test]
        public void FindNearestDrivers_ConcentricLayers_FoundInCorrectOrder()
        {
            // Arrange
            _container = CreateContainer(20, 20);
            int centerX = 10, centerY = 10;
            
            // Слой 0 (центр)
            _container.AddOrUpdateDriver(driverID: 1, 10, 10); // расстояние = 0
            
            // Слой 1 (крест)
            _container.AddOrUpdateDriver(driverID: 2, 11, 10); // расстояние = 1
            _container.AddOrUpdateDriver(driverID: 3, 9, 10);  // расстояние = 1
            _container.AddOrUpdateDriver(driverID: 4, 10, 11); // расстояние = 1
            _container.AddOrUpdateDriver(driverID: 5, 10, 9);  // расстояние = 1
            
            // Слой 2
            _container.AddOrUpdateDriver(driverID: 6, 12, 10); // расстояние = 2
            _container.AddOrUpdateDriver(driverID: 7, 11, 11); // расстояние = 2
            
            // Слой 3
            _container.AddOrUpdateDriver(driverID: 8, 13, 10); // расстояние = 3

            // Act
            var result = _algorithm.FindNearestDrivers(_container, centerX, centerY, 10);

            // Assert
            Assert.That(result, Has.Count.EqualTo(8));
            
            // Проверяем что первый результат - это центральный водитель
            Assert.That(result[0].Distance, Is.EqualTo(0));
            
            // Следующие 4 - на расстоянии 1
            Assert.That(result.Skip(1).Take(4).All(d => d.Distance == 1), Is.True);
            
            // Следующие 2 - на расстоянии 2
            Assert.That(result.Skip(5).Take(2).All(d => d.Distance == 2), Is.True);
            
            // Последний - на расстоянии 3
            Assert.That(result[7].Distance, Is.EqualTo(3));
        }

        #endregion

        #region Проверка сортировки и математических инвариантов

        // Проверяет, что результаты строго отсортированы по возрастанию расстояния
        [Test]
        public void FindNearestDrivers_Results_StrictlySortedByDistance()
        {
            // Arrange
            _container = CreateContainer(20, 20);
            _container.AddOrUpdateDriver(driverID: 1, 15, 15); // далеко
            _container.AddOrUpdateDriver(driverID: 2, 10, 11); // близко
            _container.AddOrUpdateDriver(driverID: 3, 10, 10); // очень близко
            _container.AddOrUpdateDriver(driverID: 4, 12, 12); // средне
            _container.AddOrUpdateDriver(driverID: 5, 10, 13); // средне-близко

            // Act
            var result = _algorithm.FindNearestDrivers(_container, 10, 10, 10);

            // Assert
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.That(result[i].Distance, Is.LessThanOrEqualTo(result[i + 1].Distance));
            }
        }

        // Проверяет, что количество результатов не превышает maxDrivers
        [Test]
        public void FindNearestDrivers_ResultCount_NeverExceedsMaxDrivers()
        {
            // Arrange
            _container = CreateContainer(15, 15);
            for (int i = 0; i < 20; i++)
            {
                _container.AddOrUpdateDriver(driverID: i, i % 15, i / 15);
            }

            // Act & Assert для разных значений maxDrivers
            for (int maxDrivers = 1; maxDrivers <= 10; maxDrivers++)
            {
                var result = _algorithm.FindNearestDrivers(_container, 7, 7, maxDrivers);
                Assert.That(result.Count, Is.LessThanOrEqualTo(maxDrivers));
            }
        }

        // Проверяет, что количество результатов не превышает количество доступных водителей
        [Test]
        public void FindNearestDrivers_ResultCount_NeverExceedsAvailableDrivers()
        {
            // Arrange
            _container = CreateContainer(10, 10);
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);
            _container.AddOrUpdateDriver(driverID: 2, 6, 6);
            _container.AddOrUpdateDriver(driverID: 3, 7, 7);

            // Act - просим больше чем есть
            var result = _algorithm.FindNearestDrivers(_container, 5, 5, 100);

            // Assert
            Assert.That(result.Count, Is.EqualTo(3));
        }

        #endregion

        #region Проверка граничных позиций поиска

        // Проверяет поиск из левого верхнего угла
        [Test]
        public void FindNearestDrivers_SearchFromTopLeftCorner_FindsClosest()
        {
            // Arrange
            _container = CreateContainer(10, 10);
            _container.AddOrUpdateDriver(driverID: 1, 0, 9);   // в углу, расстояние = 0
            _container.AddOrUpdateDriver(driverID: 2, 1, 9);   // расстояние = 1
            _container.AddOrUpdateDriver(driverID: 3, 0, 8);   // расстояние = 1
            _container.AddOrUpdateDriver(driverID: 4, 5, 5);   // далеко

            // Act
            var result = _algorithm.FindNearestDrivers(_container, 0, 9, 3);

            // Assert
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0].Distance, Is.EqualTo(0));
            Assert.That(result[1].Distance, Is.EqualTo(1));
            Assert.That(result[2].Distance, Is.EqualTo(1));
        }

        // Проверяет поиск из правого нижнего угла
        [Test]
        public void FindNearestDrivers_SearchFromBottomRightCorner_FindsClosest()
        {
            // Arrange
            _container = CreateContainer(10, 10);
            _container.AddOrUpdateDriver(driverID: 1, 9, 0);   // в углу, расстояние = 0
            _container.AddOrUpdateDriver(driverID: 2, 8, 0);   // расстояние = 1
            _container.AddOrUpdateDriver(driverID: 3, 9, 1);   // расстояние = 1
            _container.AddOrUpdateDriver(driverID: 4, 0, 5);   // далеко

            // Act
            var result = _algorithm.FindNearestDrivers(_container, 9, 0, 3);

            // Assert
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0].Distance, Is.EqualTo(0));
            Assert.That(result[1].Distance, Is.EqualTo(1));
            Assert.That(result[2].Distance, Is.EqualTo(1));
        }

        // Проверяет поиск из центра сетки
        [Test]
        public void FindNearestDrivers_SearchFromCenter_FindsSymmetric()
        {
            // Arrange
            _container = CreateContainer(11, 11);
            int centerX = 5, centerY = 5;
            
            // Размещаем водителей симметрично вокруг центра
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);   // центр
            _container.AddOrUpdateDriver(driverID: 2, 6, 5);   // справа
            _container.AddOrUpdateDriver(driverID: 3, 4, 5);   // слева
            _container.AddOrUpdateDriver(driverID: 4, 5, 6);   // сверху
            _container.AddOrUpdateDriver(driverID: 5, 5, 4);   // снизу

            // Act
            var result = _algorithm.FindNearestDrivers(_container, centerX, centerY, 10);

            // Assert
            Assert.That(result, Has.Count.EqualTo(5));
            Assert.That(result[0].Distance, Is.EqualTo(0)); // центр
            // Остальные 4 на расстоянии 1
            Assert.That(result.Skip(1).All(d => d.Distance == 1), Is.True);
        }

        #endregion

        #region Проверка специальных конфигураций

        // Проверяет случай когда все водители на одинаковом расстоянии
        [Test]
        public void FindNearestDrivers_AllDriversSameDistance_AllFound()
        {
            // Arrange
            _container = CreateContainer(15, 15);
            int centerX = 7, centerY = 7;
            
            // Создаем 4 водителей на расстоянии 3
            _container.AddOrUpdateDriver(driverID: 1, 10, 7);  // +3 по X
            _container.AddOrUpdateDriver(driverID: 2, 4, 7);   // -3 по X
            _container.AddOrUpdateDriver(driverID: 3, 7, 10);  // +3 по Y
            _container.AddOrUpdateDriver(driverID: 4, 7, 4);   // -3 по Y

            // Act
            var result = _algorithm.FindNearestDrivers(_container, centerX, centerY, 10);

            // Assert
            Assert.That(result, Has.Count.EqualTo(4));
            Assert.That(result.All(d => d.Distance == 3), Is.True);
        }

        // Проверяет расстояний по диагонали (лестнице)
        [Test]
        public void FindNearestDrivers_StairDistances_FoundInOrder()
        {
            // Arrange
            _container = CreateContainer(20, 20);
            _container.AddOrUpdateDriver(driverID: 1, 10, 10); // расстояние = 0
            _container.AddOrUpdateDriver(driverID: 2, 11, 10); // расстояние = 1
            _container.AddOrUpdateDriver(driverID: 3, 12, 10); // расстояние = 2
            _container.AddOrUpdateDriver(driverID: 4, 13, 10); // расстояние = 3
            _container.AddOrUpdateDriver(driverID: 5, 14, 10); // расстояние = 4

            // Act
            var result = _algorithm.FindNearestDrivers(_container, 10, 10, 5);

            // Assert
            Assert.That(result, Has.Count.EqualTo(5));
            for (int i = 0; i < 5; i++)
            {
                Assert.That(result[i].Distance, Is.EqualTo(i));
            }
        }

        // Проверяет горизонтальную линию водителей
        [Test]
        public void FindNearestDrivers_HorizontalLine_CorrectDistances()
        {
            // Arrange
            _container = CreateContainer(15, 10);
            for (int x = 0; x < 15; x++)
            {
                _container.AddOrUpdateDriver(driverID: x, x, 5);
            }

            // Act - ищем из центра линии
            var result = _algorithm.FindNearestDrivers(_container, 7, 5, 5);

            // Assert
            Assert.That(result, Has.Count.EqualTo(5));
            Assert.That(result[0].Distance, Is.EqualTo(0)); // водитель 7
            Assert.That(result[1].Distance, Is.EqualTo(1)); // водитель 6 или 8
            Assert.That(result[2].Distance, Is.EqualTo(1)); // водитель 8 или 6
            Assert.That(result[3].Distance, Is.EqualTo(2)); // водитель 5 или 9
            Assert.That(result[4].Distance, Is.EqualTo(2)); // водитель 9 или 5
        }

        // Проверяет вертикальную линию водителей
        [Test]
        public void FindNearestDrivers_VerticalLine_CorrectDistances()
        {
            // Arrange
            _container = CreateContainer(10, 15);
            for (int y = 0; y < 15; y++)
            {
                _container.AddOrUpdateDriver(driverID: y, 5, y);
            }

            // Act - ищем из центра линии
            var result = _algorithm.FindNearestDrivers(_container, 5, 7, 5);

            // Assert
            Assert.That(result, Has.Count.EqualTo(5));
            Assert.That(result[0].Distance, Is.EqualTo(0)); // водитель 7
            Assert.That(result[1].Distance, Is.EqualTo(1)); // водитель 6 или 8
            Assert.That(result[2].Distance, Is.EqualTo(1)); // водитель 8 или 6
            Assert.That(result[3].Distance, Is.EqualTo(2)); // водитель 5 или 9
            Assert.That(result[4].Distance, Is.EqualTo(2)); // водитель 9 или 5
        }

        #endregion

        #region Комплексные сценарии

        // Проверяет сложный сценарий с плотным скоплением и одиноким близким водителем
        [Test]
        public void FindNearestDrivers_ClusterAndLoneDriver_LoneDriverFoundFirst()
        {
            // Arrange
            _container = CreateContainer(20, 20);
            
            // Одинокий близкий водитель
            _container.AddOrUpdateDriver(driverID: 1, 11, 10); // расстояние = 1
            
            // Плотное скопление дальше
            _container.AddOrUpdateDriver(driverID: 2, 15, 15); // расстояние = 10
            _container.AddOrUpdateDriver(driverID: 3, 15, 16); // расстояние = 11
            _container.AddOrUpdateDriver(driverID: 4, 16, 15); // расстояние = 11
            _container.AddOrUpdateDriver(driverID: 5, 16, 16); // расстояние = 12

            // Act
            var result = _algorithm.FindNearestDrivers(_container, 10, 10, 3);

            // Assert
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0].DriverOnMap.Driver.ID, Is.EqualTo(1));
            Assert.That(result[0].Distance, Is.EqualTo(1));
        }

        // Проверяет большой разброс расстояний
        [Test]
        public void FindNearestDrivers_WideDistanceRange_FindsClosest()
        {
            // Arrange
            _container = CreateContainer(30, 30);
            _container.AddOrUpdateDriver(driverID: 1, 15, 15); // расстояние = 0
            _container.AddOrUpdateDriver(driverID: 2, 16, 15); // расстояние = 1
            _container.AddOrUpdateDriver(driverID: 3, 20, 20); // расстояние = 10
            _container.AddOrUpdateDriver(driverID: 4, 25, 25); // расстояние = 20
            _container.AddOrUpdateDriver(driverID: 5, 29, 29); // расстояние = 28

            // Act
            var result = _algorithm.FindNearestDrivers(_container, 15, 15, 3);

            // Assert
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0].Distance, Is.EqualTo(0));
            Assert.That(result[1].Distance, Is.EqualTo(1));
            Assert.That(result[2].Distance, Is.EqualTo(10));
            // Водители 4 и 5 не должны попасть в результат
        }

        #endregion
    }
}