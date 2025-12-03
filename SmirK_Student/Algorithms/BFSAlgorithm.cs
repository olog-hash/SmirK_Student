using SmirK_Student.Algorithms.Core;
using SmirK_Student.Map.Containers;
using SmirK_Student.Map.Core;
using SmirK_Student.Map.Core.Data;

namespace SmirK_Student.Algorithms
{

    public sealed class BFSAlgorithm : BaseDriverSearchStrategy<ClassicGrid>
    {
        private readonly int[] _xOffsets = new[] { -1, 0, 1, 0 };
        private readonly int[] _yOffsets = new[] { 0, 1, 0, -1 };
        
        protected override List<DriverDistance> ExecuteAlgorithm(ClassicGrid classicGrid, int x, int y, int maxDrivers)
        {
            int width = classicGrid.Width;
            int height = classicGrid.Height;

            var foundDrivers = new List<DriverDistance>();

            // Очередь для посетителей - можно взять обычный кортеж
            var queue = new Queue<(int, int, int)>();

            // Запись посещенных клеток
            var visited = new FastGrid<bool>(width, height);

            // Точка поиска заказа будет являться стартовой
            queue.Enqueue((x, y, 0));
            visited[x, y] = true;

            while (queue.Count > 0)
            {
                // Получаем начальную точку
                var (centerX, centerY, distance) = queue.Dequeue();

                // Проверяем текущую точку на наличие водителя
                var driver = classicGrid.ContentGrid[centerX, centerY];
                if (driver != null && driver.IsAvaliable)
                {
                    foundDrivers.Add(new DriverDistance(new DriverOnMap(driver, centerX, centerY), distance));
                    
                    // Если кол-во найденных водителей достаточно
                    if (foundDrivers.Count >= maxDrivers)
                        break;
                }
                
                // Добавляем всех соседей ↑ ↓ ← →
                for (int i = 0; i < 4; i++)
                {
                    int xPos = centerX + _xOffsets[i];
                    int yPos = centerY + _yOffsets[i];

                    // Проверяем границы точки и ее посещение
                    if (MapUtilits.IsInBounds(width, height, xPos, yPos) && !visited[xPos, yPos])
                    {
                        visited[xPos, yPos] = true;
                        queue.Enqueue((xPos, yPos, distance + 1));
                    }
                }
            }
            
            return foundDrivers;
        }
    }
}