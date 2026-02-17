using SmirK_Student.Domain.Drivers.Map.Containers.Core;

namespace MyTests.NUnitTests
{
    /// <summary>
    /// Абстрактный базовый класс для тестирования любых контейнеров
    /// </summary>
    public abstract class BaseContainerTests
    {
        protected DriversContainer _container { get; set; }
        
        /// <summary>
        /// Реализация контейнера определяется наследником
        /// </summary>
        protected abstract DriversContainer CreateContainer(int width, int height);

        [SetUp]
        public void SetUp()
        {
            // Создаем контейнер перед каждым тестом
            _container = CreateContainer(10, 10);
        }
        
        #region Тесты на выход за границы
        
        // Проверяет, что попытка добавить водителя с отрицательной координатой X возвращает InvalidPosition
        [Test]
        public void AddDriver_NegativeX_ReturnsInvalidPosition()
        {
            // Arrange & Act
            var result = _container.AddOrUpdateDriver(driverID: 1, -1, 5);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.InvalidPosition));
            Assert.That(_container.DriversCount, Is.EqualTo(0));
        }
        
        // Проверяет, что попытка добавить водителя с отрицательной координатой Y возвращает InvalidPosition
        [Test]
        public void AddDriver_NegativeY_ReturnsInvalidPosition()
        {
            // Arrange & Act
            var result = _container.AddOrUpdateDriver(driverID: 1, 5, -1);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.InvalidPosition));
            Assert.That(_container.DriversCount, Is.EqualTo(0));
        }
        
        // Проверяет, что попытка добавить водителя за пределами ширины сетки возвращает InvalidPosition
        [Test]
        public void AddDriver_XOutOfBounds_ReturnsInvalidPosition()
        {
            // Arrange & Act
            var result = _container.AddOrUpdateDriver(driverID: 1, 10, 5);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.InvalidPosition));
            Assert.That(_container.DriversCount, Is.EqualTo(0));
        }
        
        // Проверяет, что попытка добавить водителя за пределами высоты сетки возвращает InvalidPosition.
        [Test]
        public void AddDriver_YOutOfBounds_ReturnsInvalidPosition()
        {
            // Arrange & Act
            var result = _container.AddOrUpdateDriver(driverID: 1, 5, 10);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.InvalidPosition));
            Assert.That(_container.DriversCount, Is.EqualTo(0));
        }
        
        // Проверяет, что попытка переместить существующего водителя на отрицательные координаты возвращает InvalidPosition.
        [Test]
        public void UpdateDriver_NegativeCoordinates_ReturnsInvalidPosition()
        {
            // Arrange - добавляем водителя на валидные координаты
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);

            // Act - пытаемся переместить за границы
            var result = _container.AddOrUpdateDriver(driverID: 1, -1, 5);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.InvalidPosition));
        }
        
        // Проверяет, что попытка переместить существующего водителя за пределы сетки возвращает InvalidPosition.
        [Test]
        public void UpdateDriver_OutOfBounds_ReturnsInvalidPosition()
        {
            // Arrange - добавляем водителя
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);

            // Act - пытаемся переместить за границы
            var result = _container.AddOrUpdateDriver(driverID: 1, 15, 15);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.InvalidPosition));
        }

        #endregion

        #region Тесты на занятую позицию
        
        // Проверяет, что попытка добавить нового водителя на позицию, занятую другим водителем, возвращает OccupiedPosition.
        [Test]
        public void AddDriver_OccupiedPosition_ReturnsOccupiedPosition()
        {
            // Arrange - добавляем первого водителя
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);

            // Act - пытаемся поставить другого водителя на ту же позицию
            var result = _container.AddOrUpdateDriver(driverID: 2, 5, 5);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.OccupiedPosition));
            Assert.That(_container.DriversCount, Is.EqualTo(1));
        }
        
        // Проверяет, что попытка переместить водителя на позицию, занятую другим водителем, возвращает OccupiedPosition.
        [Test]
        public void UpdateDriver_MoveToOccupiedPosition_ReturnsOccupiedPosition()
        {
            // Arrange - добавляем двух водителей на разные позиции
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);
            _container.AddOrUpdateDriver(driverID: 2, 3, 3);

            // Act - пытаемся переместить второго на позицию первого
            var result = _container.AddOrUpdateDriver(driverID: 2, 5, 5);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.OccupiedPosition));
            Assert.That(_container.DriversCount, Is.EqualTo(2));
        }
        
        // Проверяет, что перемещение водителя на его собственные координаты возвращает Updated и не вызывает ошибок.
        [Test]
        public void UpdateDriver_MoveToOwnPosition_ReturnsUpdated()
        {
            // Arrange - добавляем водителя
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);

            // Act - перемещаем на свои же координаты
            var result = _container.AddOrUpdateDriver(driverID: 1, 5, 5);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.Updated));
            Assert.That(_container.DriversCount, Is.EqualTo(1));
        }

        #endregion

        #region Тесты на добавление
        
        // Проверяет, что добавление нового водителя на свободную валидную позицию возвращает Added.
        [Test]
        public void AddDriver_ValidPosition_ReturnsAdded()
        {
            // Act
            var result = _container.AddOrUpdateDriver(driverID: 1, 5, 5);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.Added));
            Assert.That(_container.DriversCount, Is.EqualTo(1));
        }
        
        // Проверяет, что можно добавить несколько водителей на разные позиции.
        // Все операции должны вернуть Added.
        [Test]
        public void AddDriver_MultipleDrivers_AllAdded()
        {
            // Act
            var result1 = _container.AddOrUpdateDriver(driverID: 1, 1, 1);
            var result2 = _container.AddOrUpdateDriver(driverID: 2, 2, 2);
            var result3 = _container.AddOrUpdateDriver(driverID: 3, 3, 3);

            // Assert
            Assert.That(result1, Is.EqualTo(EDriverLocationUpdateResult.Added));
            Assert.That(result2, Is.EqualTo(EDriverLocationUpdateResult.Added));
            Assert.That(result3, Is.EqualTo(EDriverLocationUpdateResult.Added));
            Assert.That(_container.DriversCount, Is.EqualTo(3));
        }
        
        // Проверяет, что перемещение существующего водителя на новую свободную позицию возвращает Updated
        [Test]
        public void UpdateDriver_ValidPosition_ReturnsUpdated()
        {
            // Arrange - добавляем водителя
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);

            // Act - перемещаем на новую позицию
            var result = _container.AddOrUpdateDriver(driverID: 1, 7, 7);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.Updated));
            Assert.That(_container.DriversCount, Is.EqualTo(1));
        }

        #endregion

        #region Тесты на удаление
        
        // Проверяет, что удаление существующего водителя возвращает Removed и уменьшает счетчик.
        [Test]
        public void RemoveDriver_ExistingDriver_ReturnsRemoved()
        {
            // Arrange - добавляем водителя
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);

            // Act
            var result = _container.RemoveDriver(1);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.Removed));
            Assert.That(_container.DriversCount, Is.EqualTo(0));
        }
        
        // Проверяет, что попытка удалить несуществующего водителя возвращает InvalidDriver
        [Test]
        public void RemoveDriver_NonExistingDriver_ReturnsInvalidDriver()
        {
            // Act
            var result = _container.RemoveDriver(999);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.InvalidDriver));
            Assert.That(_container.DriversCount, Is.EqualTo(0));
        }
        
        // Проверяет, что попытка удалить водителя, добавление которого завершилось с ошибкой, возвращает InvalidDriver.
        [Test]
        public void RemoveDriver_AfterFailedAdd_ReturnsInvalidDriver()
        {
            // Arrange - пытаемся добавить с ошибкой (невалидные координаты)
            _container.AddOrUpdateDriver(driverID: 1, -1, 5);

            // Act
            var result = _container.RemoveDriver(1);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.InvalidDriver));
        }
        
        // Проверяет, что последовательное удаление нескольких водителей работает корректно.
        [Test]
        public void RemoveDriver_MultipleDrivers_AllRemoved()
        {
            // Arrange - добавляем трех водителей
            _container.AddOrUpdateDriver(driverID: 1, 1, 1);
            _container.AddOrUpdateDriver(driverID: 2, 2, 2);
            _container.AddOrUpdateDriver(driverID: 3, 3, 3);

            // Act & Assert
            var result1 = _container.RemoveDriver(1);
            Assert.That(result1, Is.EqualTo(EDriverLocationUpdateResult.Removed));
            Assert.That(_container.DriversCount, Is.EqualTo(2));

            var result2 = _container.RemoveDriver(2);
            Assert.That(result2, Is.EqualTo(EDriverLocationUpdateResult.Removed));
            Assert.That(_container.DriversCount, Is.EqualTo(1));

            var result3 = _container.RemoveDriver(3);
            Assert.That(result3, Is.EqualTo(EDriverLocationUpdateResult.Removed));
            Assert.That(_container.DriversCount, Is.EqualTo(0));
        }

        #endregion

        #region Тесты счетчика водителей
        
        // Проверяет, что счетчик водителей корректно увеличивается при добавлении.
        [Test]
        public void DriversCount_AfterAdditions_IsCorrect()
        {
            // Arrange & Act & Assert
            Assert.That(_container.DriversCount, Is.EqualTo(0));
            
            _container.AddOrUpdateDriver(driverID: 1, 1, 1);
            Assert.That(_container.DriversCount, Is.EqualTo(1));
            
            _container.AddOrUpdateDriver(driverID: 2, 2, 2);
            Assert.That(_container.DriversCount, Is.EqualTo(2));
            
            _container.AddOrUpdateDriver(driverID: 3, 3, 3);
            Assert.That(_container.DriversCount, Is.EqualTo(3));
        }
        
        // Проверяет, что счетчик водителей корректно уменьшается при удалении.
        [Test]
        public void DriversCount_AfterRemovals_IsCorrect()
        {
            // Arrange - добавляем трех водителей
            _container.AddOrUpdateDriver(driverID: 1, 1, 1);
            _container.AddOrUpdateDriver(driverID: 2, 2, 2);
            _container.AddOrUpdateDriver(driverID: 3, 3, 3);

            // Act & Assert - последовательно удаляем
            _container.RemoveDriver(1);
            Assert.That(_container.DriversCount, Is.EqualTo(2));
            
            _container.RemoveDriver(2);
            Assert.That(_container.DriversCount, Is.EqualTo(1));
            
            _container.RemoveDriver(3);
            Assert.That(_container.DriversCount, Is.EqualTo(0));
        }
        
        // Проверяет, что счетчик не изменяется при неудачных операциях добавления.
        [Test]
        public void DriversCount_AfterFailedOperations_RemainsUnchanged()
        {
            // Arrange
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);
            int initialCount = _container.DriversCount;

            // Act - пытаемся добавить на занятую позицию
            _container.AddOrUpdateDriver(driverID: 2, 5, 5);

            // Assert
            Assert.That(_container.DriversCount, Is.EqualTo(initialCount));
        }

        #endregion

        #region Массовые операции
        
        // Проверяет, что можно успешно добавить 100 водителей на разные позиции.
        // Тестирует стабильность контейнера при массовых операциях.
        [Test]
        public void MassiveAddOperations_AllSuccessful()
        {
            // Act - добавляем 100 водителей
            for (int i = 0; i < 100; i++)
            {
                int x = i % 10;
                int y = i / 10;
                var result = _container.AddOrUpdateDriver(driverID: i, x, y);
                
                // Assert
                Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.Added), 
                    $"Неудалось добавить водителя {i} на ({x}, {y})");
            }

            Assert.That(_container.DriversCount, Is.EqualTo(100));
        }
        
        // Проверяет, что можно успешно удалить 100 водителей.
        // Тестирует стабильность контейнера при массовых операциях удаления.
        [Test]
        public void MassiveRemoveOperations_AllSuccessful()
        {
            // Arrange - добавляем 100 водителей
            for (int i = 0; i < 100; i++)
            {
                _container.AddOrUpdateDriver(driverID: i, i % 10, i / 10);
            }

            // Act & Assert - удаляем всех
            for (int i = 0; i < 100; i++)
            {
                var result = _container.RemoveDriver(i);
                Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.Removed),
                    $"Неудалось удалить водителя {i}");
            }

            Assert.That(_container.DriversCount, Is.EqualTo(0));
        }
        
        // Проверяет, что можно выполнить 100 операций перемещения водителей.
        // Тестирует стабильность при массовых обновлениях позиций.
        [Test]
        public void MassiveUpdateOperations_AllSuccessful()
        {
            // Arrange - добавляем 10 водителей
            for (int i = 0; i < 10; i++)
            {
                _container.AddOrUpdateDriver(driverID: i, i, 0);
            }

            // Act - перемещаем каждого водителя 10 раз
            for (int i = 0; i < 10; i++)
            {
                for (int y = 1; y < 10; y++)
                {
                    var result = _container.AddOrUpdateDriver(driverID: i, i, y);
                    Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.Updated),
                        $"Неудалось переместить водителя {i} на позицию ({i}, {y})");
                }
            }

            Assert.That(_container.DriversCount, Is.EqualTo(10));
        }

        #endregion

        #region Тесты повторного использования ID
        
        // Проверяет, что ID удаленного водителя можно использовать для создания нового водителя.
        [Test]
        public void RemovedDriverID_CanBeReused()
        {
            // Arrange - добавляем и удаляем водителя
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);
            _container.RemoveDriver(1);

            // Act - используем тот же ID для нового водителя
            var result = _container.AddOrUpdateDriver(driverID: 1, 3, 3);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.Added));
            Assert.That(_container.DriversCount, Is.EqualTo(1));
        }
        
        // Проверяет, что позицию удаленного водителя можно занять новым водителем.
        [Test]
        public void RemovedDriverPosition_CanBeOccupied()
        {
            // Arrange - добавляем и удаляем водителя
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);
            _container.RemoveDriver(1);

            // Act - создаем нового водителя на той же позиции
            var result = _container.AddOrUpdateDriver(driverID: 2, 5, 5);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.Added));
            Assert.That(_container.DriversCount, Is.EqualTo(1));
        }
        
        // Проверяет, что после повторного использования ID старые данные не сохраняются
        [Test]
        public void RemovedDriverID_NewDriverIsIndependent()
        {
            // Arrange - добавляем водителя, удаляем, создаем нового с тем же ID
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);
            _container.RemoveDriver(1);
            _container.AddOrUpdateDriver(driverID: 1, 7, 7);

            // Act - проверяем, что старая позиция свободна
            var result = _container.AddOrUpdateDriver(driverID: 2, 5, 5);

            // Assert
            Assert.That(result, Is.EqualTo(EDriverLocationUpdateResult.Added));
        }

        #endregion

        #region Тесты освобождения координат
        
        // Проверяет, что при перемещении водителя его старая позиция освобождается.
        // На освободившуюся позицию можно добавить нового водителя.
        [Test]
        public void MoveDriver_OldPositionIsFreed()
        {
            // Arrange - добавляем водителя
            _container.AddOrUpdateDriver(driverID: 1, 5, 5);

            // Act - перемещаем водителя на новую позицию
            var moveResult = _container.AddOrUpdateDriver(driverID: 1, 7, 7);
            
            // Assert - старая позиция должна быть свободна
            var addResult = _container.AddOrUpdateDriver(driverID: 2, 5, 5);
            
            Assert.That(moveResult, Is.EqualTo(EDriverLocationUpdateResult.Updated));
            Assert.That(addResult, Is.EqualTo(EDriverLocationUpdateResult.Added));
            Assert.That(_container.DriversCount, Is.EqualTo(2));
        }
        
        // Проверяет, что при перемещении нескольких водителей все старые позиции освобождаются корректно.
        [Test]
        public void MoveMultipleDrivers_AllOldPositionsFreed()
        {
            // Arrange - добавляем трех водителей
            _container.AddOrUpdateDriver(driverID: 1, 1, 1);
            _container.AddOrUpdateDriver(driverID: 2, 2, 2);
            _container.AddOrUpdateDriver(driverID: 3, 3, 3);

            // Act - перемещаем всех на новые позиции
            _container.AddOrUpdateDriver(driverID: 1, 4, 4);
            _container.AddOrUpdateDriver(driverID: 2, 5, 5);
            _container.AddOrUpdateDriver(driverID: 3, 6, 6);

            // Assert - старые позиции должны быть свободны
            var result1 = _container.AddOrUpdateDriver(driverID: 4, 1, 1);
            var result2 = _container.AddOrUpdateDriver(driverID: 5, 2, 2);
            var result3 = _container.AddOrUpdateDriver(driverID: 6, 3, 3);

            Assert.That(result1, Is.EqualTo(EDriverLocationUpdateResult.Added));
            Assert.That(result2, Is.EqualTo(EDriverLocationUpdateResult.Added));
            Assert.That(result3, Is.EqualTo(EDriverLocationUpdateResult.Added));
            Assert.That(_container.DriversCount, Is.EqualTo(6));
        }

        #endregion
    }
}