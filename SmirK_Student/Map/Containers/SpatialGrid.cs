using SmirK_Student.Map.Core;
using SmirK_Student.Map.Core.Data;

namespace SmirK_Student.Map.Containers
{

    // Подсмотрел в интернете способ через IEquatable для структур
    // Можно конечно использовать кортежи... но так просто удобнее
    public struct GridCell : IEquatable<GridCell>
    {
        public int X { get; }
        public int Y { get; }

        public GridCell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(GridCell other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is GridCell other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
    
    /// <summary>
    /// Улучшенная версия двумерной сетки.
    /// Делит карту на ячейки фиксированного размера и размещает водителей в соответствующих ячейках для быстрого поиска.
    /// </summary>
    public sealed class SpatialGrid : DriversContainer
    {
        public override int DriversCount => _driversOnMap.Count;
        public IReadOnlyDictionary<int, DriverOnMap> DriversOnMap => _driversOnMap;
        public IEnumerable<DriverOnMap> GetAllDrivers() => _driversOnMap.Values;
        public IReadOnlyDictionary<GridCell, List<DriverOnMap>> GridCells => _gridCells;
        
        public readonly int CellSize;
        
        // Словарь для быстрого получения доступа по ID водителя
        private readonly Dictionary<int, DriverOnMap> _driversOnMap;

        // Словарь для распределения водителей по секторам/ячейкам
        private readonly Dictionary<GridCell, List<DriverOnMap>> _gridCells;

        public SpatialGrid(int width, int height, int cellSize) : base(width, height)
        {
            CellSize = cellSize;
            
            _driversOnMap = new ();
            _gridCells = new ();
        }

        public GridCell GetCellAddress(int x, int y)
        {
            int cellX = x / CellSize;
            int cellY = y / CellSize;
            return new GridCell(cellX, cellY);
        }

        public override EDriverLocationUpdateResult AddOrUpdateDriver(int driverID, int x, int y)
        {
            // Координаты за пределом карты
            if (!IsValidPosition(x, y))
            {
                // Неверная позиция
                return EDriverLocationUpdateResult.InvalidPosition;
            }
            
            // Если позиция уже занята другим водителем
            if (!CanPlaceDriverToPosition(driverID, x, y))
            {
                return EDriverLocationUpdateResult.OccupiedPosition;
            }
            
            // Если водитель уже существует на карте
            if (_driversOnMap.TryGetValue(driverID, out var driverOnMap))
            {
                // Если водитель перемещается на свои собственные координаты
                if (driverOnMap.X == x && driverOnMap.Y == y)
                {
                    return EDriverLocationUpdateResult.Updated;
                }
                
                // Перемещаем существующего водителя
                MoveDriver(driverOnMap, x, y);
                return EDriverLocationUpdateResult.Updated;
            }
            
            // Создаем нового водителя
            AddNewDriver(driverID, x, y);
            return EDriverLocationUpdateResult.Added;
        }
        
        
        public override EDriverLocationUpdateResult RemoveDriver(int driverID)
        {
            if (_driversOnMap.TryGetValue(driverID, out var driverOnMap))
            {
                RemoveDriverFromGrid(driverOnMap);
                _driversOnMap.Remove(driverID);
                
                return EDriverLocationUpdateResult.Removed;
            }

            return EDriverLocationUpdateResult.InvalidDriver;
        }
        
        protected override void AddNewDriver(int driverID, int x, int y)
        {
            var driver = new DriverData(driverID);
            var driverOnMap = new DriverOnMap(driver, x, y);
            
            var cellAddress = GetCellAddress(x, y);
            if (!_gridCells.ContainsKey(cellAddress))
            {
                // Ленивая инициализация списка для ячеек
                _gridCells[cellAddress] = new();
            }
            
            // Добавляем водителей
            _gridCells[cellAddress].Add(driverOnMap);
            _driversOnMap.Add(driverID, driverOnMap);
        }

        protected override void MoveDriver(DriverOnMap driverOnMap, int x, int y)
        {
            var oldCellAddress = GetCellAddress(driverOnMap.X, driverOnMap.Y);
            var newCellAddress = GetCellAddress(x, y);
            
            // Если водитель переместился в другую ячейку
            if (!oldCellAddress.Equals(newCellAddress))
            {
                // Удаляем из старой ячейки
                if (_gridCells.ContainsKey(oldCellAddress))
                {
                    _gridCells[oldCellAddress].Remove(driverOnMap);
                
                    // Удаляем ячейку, если там больше никого нет
                    if (_gridCells[oldCellAddress].Count == 0)
                    {
                        _gridCells.Remove(oldCellAddress);
                    }
                }
                
                // Добавляем в новую ячейку
                if (!_gridCells.ContainsKey(newCellAddress))
                {
                    _gridCells[newCellAddress] = new List<DriverOnMap>();
                }
                
                _gridCells[newCellAddress].Add(driverOnMap);
            }
            
            // Обновляем позицию в записи
            driverOnMap.UpdatePosition(x,y);
        }

        protected override bool CanPlaceDriverToPosition(int driverID, int x, int y)
        {
            var cellAddress = GetCellAddress(x, y);
            
            // Если такая ячейка существует, смотрим детальнее
            if (_gridCells.ContainsKey(cellAddress))
            {
                // Проверяем, есть ли другой водитель, что занимает требуемую позицию
                bool isPositionOccupied = _gridCells[cellAddress].Any(d => d.X == x && d.Y == y && d.Driver.ID != driverID);
                return !isPositionOccupied;
            }

            return true;
        }

        private void RemoveDriverFromGrid(DriverOnMap driverOnMap)
        {
            // Проверка на валидность координат
            if (!IsValidPosition(driverOnMap.X, driverOnMap.Y))
            {
                return;
            }
            
            // Удаление водителя с сетки
            var cellAddress = GetCellAddress(driverOnMap.X, driverOnMap.Y);
            if (_gridCells.ContainsKey(cellAddress))
            {
                _gridCells[cellAddress].Remove(driverOnMap);
                
                // Удаляем ячейку, если там больше никого нет
                if (_gridCells[cellAddress].Count == 0)
                {
                    _gridCells.Remove(cellAddress);
                }
            }
        }
    }
}