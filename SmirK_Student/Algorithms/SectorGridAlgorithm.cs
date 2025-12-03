using SmirK_Student.Algorithms.Core;
using SmirK_Student.Map.Containers;
using SmirK_Student.Map.Core.Data;

namespace SmirK_Student.Algorithms
{

    /// <summary>
    /// Алгоритм поиска ближайших водителей на сетке, что разбита на сектора (группировки).
    /// </summary>
    public sealed class SectorGridAlgorithm : BaseDriverSearchStrategy<SpatialGrid>
    {
        protected override List<DriverDistance> ExecuteAlgorithm(SpatialGrid spatialGrid, int x, int y, int maxDrivers)
        {
            // Неверные координаты или отсутствие водителей
            if (!spatialGrid.IsValidPosition(x, y) || spatialGrid.DriversCount == 0)
            {
                return new List<DriverDistance>();
            }
            
            // Определяем предельные коодинаты секторов
            var centerCell = spatialGrid.GetCellAddress(x, y);
            var endCell = spatialGrid.GetCellAddress(spatialGrid.Width - 1, spatialGrid.Height - 1);
            
            // Определяем расстояние до границ (максимально возможный радиус)
            int leftDistance = centerCell.X;
            int rightDistance = endCell.X - centerCell.X;;
            int topDistance = centerCell.Y;
            int bottomDistance = endCell.Y - centerCell.Y;
            int maxRadius = Math.Max(Math.Max(leftDistance, rightDistance), Math.Max(topDistance, bottomDistance));
            
            // Используем приоритетную очередь для автоматической сортировки
            // Инвертируем приоритет (-distance), чтобы удалять самых дальних
            var driversQueue = new PriorityQueue<DriverDistance, int>();
            
            // Перебираем слои с потенциальными водителями
            for (int layer = 0; layer <= maxRadius; layer++)
            {
                var cellsInLayer = GetCellsInLayer(centerCell, layer);

                foreach (var cell in cellsInLayer)
                {
                    // Пропускаем ячейки, что находятся за пределами созданной сетки
                    // Тут проверок будет сильно меньше, чем по клеткам, поэтому можно обойтись таким способом.
                    if (cell.X < 0 || cell.X > endCell.X || cell.Y < 0 || cell.Y > endCell.Y)
                    {
                        continue;
                    }

                    // Перебираем водителей в ячейке, если они там есть
                    if (spatialGrid.GridCells.TryGetValue(cell, out var driversInCell))
                    {
                        foreach (var driverOnMap in driversInCell)
                        {
                            // Только доступных водителей
                            if (!driverOnMap.Driver.IsAvaliable)
                                continue;
                            
                            int distance = MapUtilits.CalculateDistance(x, y, driverOnMap.X, driverOnMap.Y);
                            
                            // Добавляем в приоритетную очередь с инвертированным приоритетом
                            driversQueue.Enqueue(new DriverDistance(driverOnMap, distance), -distance);
                            
                            // Если водителей больше - удаляем самого дальнего
                            if (driversQueue.Count > maxDrivers)
                            {
                                driversQueue.Dequeue();
                            }
                        }
                    }
                }
                
                // Однако, есть шанс, что водитель X (из второго слоя) находиться ближе, чем водитель Y, которого мы нашли (из первого слоя).
                // Такое возможно, если водитель Y находиться на одной прямой с точкой заказа, либо же если меньше искривлен, чем до водителя X.
                // Поэтому, чтобы получить гарантию близости всех найденных водителей, нужно быть уверенным, что в следующих слоях они будут точно дальше.
                if (driversQueue.Count >= maxDrivers && layer < maxRadius)
                {
                    // Получаем максимальное расстояние в очереди среди найденных водителей
                    // Peek возвращает элемент с самым высоким приоритетом (-distance значит самый дальний)
                    int maxDistanceInQueue = driversQueue.Peek().Distance;

                    // Вычисляем минимально возможное расстояние до следующего слоя, от точки заказа
                    int minDistanceToNextLayer = CalculateMinDistanceToLayer(x, y, centerCell, layer + 1, spatialGrid.CellSize);

                    // Если есть шанс, что в следующем слое есть водители ближе - продолжаем искать дальше
                    if (minDistanceToNextLayer > maxDistanceInQueue)
                    {
                        break;
                    }
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
            
            return result;
        }

        private List<GridCell> GetCellsInLayer(GridCell center, int radius)
        {
            var cells = new List<GridCell>();

            // Проверяем только центральную ячейку
            if (radius == 0)
            {
                cells.Add(center);
                return cells;
            }
            
            // Для слоев > 0 будем брать ребра двумерного квадрата/прямоугольника
            // Учтем то, что ребра имеют общие точки пересечения (точки вершин)
            
            // Верхнее ребро (слева направо) - включаем оба угла
            for (int x = center.X - radius; x <= center.X + radius; x++)
            {
                cells.Add(new GridCell(x, center.Y + radius));
            }
            
            // Правое ребро (сверху вниз) - без верхнего угла но с нижним углом
            for (int y = center.Y + radius - 1; y >= center.Y - radius; y--)
            {
                cells.Add(new GridCell(center.X + radius, y));
            }
            
            // Нижнее ребро (справа налево) - без правого угла но с левым углом
            for (int x = center.X + radius - 1; x >= center.X - radius; x--)
            {
                cells.Add(new GridCell(x, center.Y - radius));
            }

            // Левое ребро (снизу вверх) - без верхного и без нижнего угла
            for (int y = center.Y - radius + 1; y <= center.Y + radius - 1; y++)
            {
                cells.Add(new GridCell(center.X - radius, y));
            }

            return cells;
        }

        private int CalculateMinDistanceToLayer(int x, int y, GridCell centerCell, int layer, int cellSize)
        {
            if (layer == 0)
            {
                return 0;
            }
            
            // Вычисляем границы в клеточных координатах учитывая уровень слоя
            int borderLeft = (centerCell.X - layer + 1) * cellSize;
            int borderRight = (centerCell.X + layer) * cellSize - 1;
            int borderTop = (centerCell.Y + layer) * cellSize - 1;
            int borderBottom = (centerCell.Y - layer + 1) * cellSize;

            // Точка должна быть внутри этого квадрата
            if (x >= borderLeft && x <= borderRight && y >= borderBottom && y <= borderTop)
            {
                // Расстояние до границы точки (x,y)
                int distantToLeft = x - borderLeft;
                int distantToRight = borderRight - x;
                int distantToTop = borderTop - y;
                int distantToBottom = y - borderBottom;

                // Ближайшее расстояние до следующего слоя
                return Math.Min(Math.Min(distantToLeft, distantToRight), Math.Min(distantToTop, distantToBottom)) + 1;
            }
            
            // В ином случаи, она уже за пределами указанного слоя
            return 0;
        }
    }
}