using SmirK_Student.Algorithms.Core;
using SmirK_Student.Map.Containers;
using SmirK_Student.Map.Core.Data;

namespace SmirK_Student.Algorithms
{

    /// <summary>
    /// Алгоритм поиска ближайших водителей на сетке, что разбита на сектора (группировки).
    /// </summary>
    public sealed class SectorGridAlgorithm : IDriverSearchStrategy<SpatialGrid>
    {
        public List<DriverDistance> FindNearestDrivers(SpatialGrid spatialGrid, int x, int y, int maxDrivers = 5)
        {
            // Неверные координаты или отсутствие водителей
            if (!spatialGrid.IsValidPosition(x, y) || spatialGrid.DriversCount == 0)
            {
                return new List<DriverDistance>();
            }
            
            // Определяем предельные коодинаты секторов
            var centerCell = spatialGrid.GetCellAddress(x, y);
            var endCell = spatialGrid.GetCellAddress(spatialGrid.Width - 1, spatialGrid.Height - 1);
            
            // Определяем расстояние до границ
            int leftDistance = centerCell.X;
            int rightDistance = endCell.X - centerCell.X;;
            int topDistance = centerCell.Y;
            int bottomDistance = endCell.Y - centerCell.Y;
            
            int maxRadius = Math.Max(Math.Max(leftDistance, rightDistance), Math.Max(topDistance, bottomDistance));
            
            // Ищем водителей в этих секторах
            var foundDrivers = new List<DriverDistance>();
            for (int layer = 0; layer <= maxRadius && foundDrivers.Count < maxDrivers; layer++)
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
                            int distance = MapUtilits.CalculateDistance(x, y, driverOnMap.X, driverOnMap.Y);
                            
                            // Приоритетная очередь сама проводит сортировку, но чтобы удалять самые дальные (с конца) - инвертируем дистанцию.
                            driversQueue.Enqueue(new DriverDistance(driverOnMap, distance), -distance);
                            
                            // Если водителей больше - удаляем самого дальнего
                            if (driversQueue.Count > maxDrivers)
                            {
                                driversQueue.Dequeue();
                            }
                        }
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
            
            // Обрезаем список до maxDrivers
            if (result.Count > maxDrivers)
            {
                result.RemoveRange(maxDrivers, result.Count - maxDrivers);
            }
            
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
    }
}