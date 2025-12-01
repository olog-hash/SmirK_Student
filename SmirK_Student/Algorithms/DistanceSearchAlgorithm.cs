using SmirK_Student.Algorithms.Core;
using SmirK_Student.Map.Layers;

namespace SmirK_Student.Algorithms
{
    /// <summary>
    /// В основе алгоритма лежит идея поиска водителей уже в заранее определенных координатах, которые являются ближайшими к точке поиска
    /// </summary>
    public sealed class DistanceSearchAlgorithm : IDriverSearchStrategy
    {
        public List<DriverDistance> FindNearestDrivers(DriversLayer driversLayer, int x, int y, int maxDrivers = 5)
        {
            // Неверные координаты или отсутствие водителей
            if (!driversLayer.ContentGrid.IsValid(x, y) || driversLayer.DriversCount == 0)
            {
                return new List<DriverDistance>();
            }

            int width = driversLayer.Width;
            int height = driversLayer.Height;
            
            // Находим максимально возможное расстояние в рамках сетки
            // Для этого ищем расстояние от точки до всех углов
            int leftTopDistance = MapUtilits.CalculateDistance(0, height - 1, x, y);
            int rightTopDistance = MapUtilits.CalculateDistance(width - 1, height - 1, x, y);
            int leftBottomDistance = MapUtilits.CalculateDistance(0, 0, x, y);
            int rightBottomDistance = MapUtilits.CalculateDistance(width - 1, 0, x, y);

            int maxDistance = Math.Max(
                Math.Max(leftBottomDistance, rightBottomDistance),
                Math.Max(leftTopDistance, rightTopDistance));
            
            var foundDrivers = new List<DriverDistance>();
            for (int distance = 0; distance <= maxDistance && foundDrivers.Count < maxDrivers; distance++)
            {
                // Список всех водителей, что получилось найти на этой дистанции
                GetDriversOnDistance(foundDrivers, driversLayer, x, y, distance);
            }
            
            // Сортировку проводить не требуется, все равно искали по возрастанию от точки заказа
            // Делаем обрезание по максимальному показателю.
            if (foundDrivers.Count > maxDrivers)
            {
                foundDrivers.RemoveRange(maxDrivers, foundDrivers.Count - maxDrivers);
            }
            
            return foundDrivers;
        }

        private void GetDriversOnDistance(List<DriverDistance> driverOnMaps, DriversLayer driversLayer, int centerX, int centerY,
            int distance)
        {
            int width = driversLayer.Width;
            int height = driversLayer.Height;

            // Если точка центральная, просто возвращаем её
            if (distance == 0)
            {
                ProcessPoint(driverOnMaps, driversLayer, centerX, centerY, 0);
                return;
            }

            // Каждый слой для d > 0 содержит d*4 точек.
            // d=0 ~ 1, d=1 ~ 4, d=2 ~ 8, d=3 ~ 12.
            // Если составить простенькую зарисовку и посмотреть, то видно, что рисунок из точек каждого слоя будет напоминать ромб или перевернутый квадрат.
            // Следовательно, можно найти ключевые точки на вершинах и собрать все точки, что находятся на диагонали.

            // В силу того, что у карты N*M есть рамки, некоторые точки ребра на больших дистанциях могут выходить за ее пределы.
            // Можно конечно просто обойтись методом "InBounds" и проверять каждый раз, что точка принадлежит этой области и не включать ее.
            // Но... учитывая частоту вызовов и количество слоев, это может привести к издержкам при больших сетках.
            // Плюсом, было интересно поломать голову и попробовать оптимизировать это дело

            // 1) Левое верхнее ребро (X++ Y++)
            {
                // Клетки вычисляются по формуле
                // x = centerX - distance + i;
                // y = centerY + i;

                // x >= 0 и y >= 0
                int iMin = Math.Max(0, -centerX + distance); // i >= -centerX + distance
                iMin = Math.Max(iMin, -centerY); // i >= -centerY

                // x < width и y < height
                int iMax = Math.Min(distance,
                    (width - 1) - (centerX - distance)); // centerX - distance + i <= width - 1
                iMax = Math.Min(iMax, (height - 1) - centerY); // centralY + i <= height - 1

                for (int i = iMin; i <= iMax; i++)
                {
                    int x = centerX - distance + i;
                    int y = centerY + i;

                    ProcessPoint(driverOnMaps, driversLayer, x, y, distance);
                }
            }

            // 2) Правое верхнее ребро (X++ Y--)
            {
                // Клетки вычисляются по формуле 
                // x = centerX + i;
                // y = centerY + distance - i;

                // x >= 0 и y < height
                int iMin = Math.Max(1, -centerX); // i >= -centerX
                iMin = Math.Max(iMin, (centerY + distance) - (height - 1)); // centerY + distance - i <= height - 1

                // x < width и y >= 0
                int iMax = Math.Min(distance, (width - 1) - centerX); // centerX + i <= width - 1
                iMax = Math.Min(iMax, centerY + distance); // centerY + distance - i >= 0

                for (int i = iMin; i <= iMax; i++)
                {
                    int x = centerX + i;
                    int y = centerY + distance - i;

                    ProcessPoint(driverOnMaps, driversLayer, x, y, distance);
                }
            }

            // 3) Правое нижнее ребро (X-- Y--)
            {
                // Клетки вычисляются по формуле
                // x = centerX + distance - i
                // y = centerY - i
                
                // x < width и y < height
                int iMin = Math.Max(1, (centerX + distance) -  (width -1)); // centerX + distance - i <= width -1
                iMin = Math.Max(iMin, centerY - (height - 1)); //  centerY - i <= height - 1
                
                // x >= 0 и y >= 0
                int iMax = Math.Min(distance, centerX + distance); // centerX + distance - i >= 0
                iMax = Math.Min(iMax, centerY); // centerY - i >=0
                
                for (int i = iMin; i <= iMax; i++)
                {
                    int x = centerX + distance - i;
                    int y = centerY - i;

                    ProcessPoint(driverOnMaps, driversLayer, x, y, distance);
                }
            }
            
            // 4) Левое нижнее ребро (X-- Y++)
            {
                // Клетки вычисляются по формуле
                // x = centerX - i
                // y = centerY - distance + i
                
                // x < width и y >= 0
                int iMin = Math.Max(1, centerX - (width - 1)); // centerX - i <= width - 1
                iMin = Math.Max(iMin, distance - centerY); // centerY - distance + i >= 0
                
                // x >= 0 и y < height
                int iMax = Math.Min(distance - 1, centerX); // centerX - i >= 0 
                iMax = Math.Min(iMax, (height - 1) - (centerY - distance)); // centerY - distance + i <= height - 1
                
                for (int i = iMin; i <= iMax; i++)
                {
                    int x = centerX - i;
                    int y = centerY - distance + i;

                    ProcessPoint(driverOnMaps, driversLayer, x, y, distance);
                }
            }
        }

        private void ProcessPoint(List<DriverDistance> result, DriversLayer driversLayer, int x, int y, int distance)
        {
            var driver = driversLayer.ContentGrid[x, y];
            if (driver != null && driver.IsAvaliable)
            {
                result.Add(new DriverDistance(new DriverOnMap(driver, x, y), distance));
            }
        }
    }
}