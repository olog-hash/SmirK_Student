using SmirK_Student.Algorithms.Core;
using SmirK_Student.Map.Containers;
using SmirK_Student.Map.Core.Data;

namespace SmirK_Student.Algorithms
{
    /// <summary>
    /// Основная идея алгоритма - проходить клетки карты кольцами вокруг точки заказа.
    /// </summary>
    public sealed class CircleSearchAlgorithm : BaseDriverSearchStrategy<ClassicGrid>
    {
        protected override List<DriverDistance> ExecuteAlgorithm(ClassicGrid classicGrid, int x, int y, int maxDrivers)
        {
            // Находим максимальное расстояние от точки заказа до любого угла карты
            // Это будет максимальным радиусом окружности, который нужно будет проверять.
            int leftDistance = x;
            int rightDistance = classicGrid.Width - 1 - x;
            int topDistance = y;
            int bottomDistance = classicGrid.Height - 1 - y;
            
            int maxRadius = Math.Max(Math.Max(leftDistance, rightDistance), Math.Max(topDistance, bottomDistance));
            
            // Ищем водителей в каждом кольце
            var foundDrivers = new List<DriverOnMap>();
            for (int radius = 0; radius <= maxRadius && foundDrivers.Count < maxDrivers; radius++)
            {
                CheckCircle(foundDrivers, classicGrid, x, y, radius);
            }
            
            // Если никого не нашли 
            if (foundDrivers.Count == 0)
            {
                return new List<DriverDistance>();
            }
            
            // Используем приоритетную очередь для автоматической сортировки
            // Инвертируем приоритет (-distance), чтобы удалять самых дальних
            var driversQueue = new PriorityQueue<DriverDistance, int>();
            
            // Перебираем найденных водителей
            foreach (var driverOnMap in foundDrivers)
            {
                // Используем манхэттенское расстояние
                int distance = Math.Abs(driverOnMap.X - x) + Math.Abs(driverOnMap.Y - y);

                // Добавляем в приоритетную очередь с инвертированным приоритетом
                driversQueue.Enqueue(new DriverDistance(driverOnMap, distance), -distance);

                // Если водителей больше - удаляем самого дальнего
                if (driversQueue.Count > maxDrivers)
                {
                    driversQueue.Dequeue();
                }
            }
            
            // Извлекаем найденных водителей
            var result = new List<DriverDistance>(driversQueue.Count);
            while (driversQueue.Count > 0)
            {
                result.Add(driversQueue.Dequeue());
            }

            // Переворачиваем, чтобы получить список по возрастанию
            result.Reverse();
            
            // Обрезаем список до maxDrivers
            if (result.Count > maxDrivers)
            {
                result.RemoveRange(maxDrivers, result.Count - maxDrivers);
            }
            
            return result;
        }

        private void CheckCircle(List<DriverOnMap> result, ClassicGrid classicGrid, int centerX, int centerY, int radius)
        {
            int width = classicGrid.Width;
            int height = classicGrid.Height;
            
            // Проверяем центральную клетку
            if (radius == 0)
            {
                ProcessPoint(result, classicGrid, centerX, centerY);
            }

            // Вычисляем границы слоя с учетом поля
            int minX = Math.Max(0, centerX - radius);
            int maxX = Math.Min(width - 1, centerX + radius);
            int minY = Math.Max(0, centerY - radius);
            int maxY = Math.Min(height - 1, centerY + radius);
            
            // Верхнее ребро (без углов)
            int topY = centerY + radius;
            if (topY >= 0 && topY < height)
            {
                for (int x = minX + 1; x < maxX; x++)
                {
                    ProcessPoint(result, classicGrid, x, topY);
                }
            }
            
            // Нижнее ребро (без углов)
            int bottomY = centerY - radius;
            if (bottomY >= 0 && bottomY < height)
            {
                for (int x = minX + 1; x < maxX; x++)
                {
                    ProcessPoint(result, classicGrid, x, bottomY);
                }
            }
            
            // Левое ребро (без углов)
            int leftX = centerX - radius;
            if (leftX >= 0 && leftX < width)
            {
                for (int y = minY + 1; y < maxY; y++)
                {
                    ProcessPoint(result, classicGrid, leftX, y);
                }
            }
            
            // Правое ребро (без углов)
            int rightX = centerX + radius;
            if (rightX >= 0 && rightX < width)
            {
                for (int y = minY + 1; y < maxY; y++)
                {
                    ProcessPoint(result, classicGrid, rightX, y);
                }
            }
            
            // Обрабатываем 4 угла отдельно
            TryProcessPoint(result, classicGrid, width, height, centerX - radius, centerY - radius);
            TryProcessPoint(result, classicGrid, width, height, centerX - radius, centerY + radius);
            TryProcessPoint(result, classicGrid, width, height, centerX + radius, centerY - radius);
            TryProcessPoint(result, classicGrid, width, height, centerX + radius, centerY + radius);
        }

        private void TryProcessPoint(List<DriverOnMap> result, ClassicGrid classicGrid, int width, int height, int x, int y)
        {
            if (MapUtilits.IsInBounds(width, height, x, y))
            {
                ProcessPoint(result, classicGrid, x, y);
            }
        }
        
        private void ProcessPoint(List<DriverOnMap> result, ClassicGrid classicGrid, int x, int y)
        {
            var driver = classicGrid.ContentGrid[x, y];
            if (driver != null && driver.IsAvaliable)
            {
                result.Add(new DriverOnMap(driver, x, y));
            }
        }
    }
}