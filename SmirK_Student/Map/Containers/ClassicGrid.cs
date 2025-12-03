using SmirK_Student.Map.Core;
using SmirK_Student.Map.Core.Data;

namespace SmirK_Student.Map.Containers
{
    /// <summary>
    /// Классическая двумерная сетка с масштабом 1 к 1.
    /// Хранит в себе позиции водителей в соответствующих клетках по координатам x,y.
    /// </summary>
    public sealed class ClassicGrid : DriversContainer
    {
        public override int DriversCount => _driversOnMap.Count;
        public IReadOnlyDictionary<int, DriverOnMap> DriversOnMap => _driversOnMap;
        public IEnumerable<DriverOnMap> GetAllDrivers() => _driversOnMap.Values;
        public FastGrid<DriverData?> ContentGrid => _contentGrid;
        
        // Словарь водителей по их идентификатору на карте, для быстрого доступа
        private readonly Dictionary<int, DriverOnMap> _driversOnMap;
        
        // Двумерная обычная сетка с местоположением водителей на ней
        private readonly FastGrid<DriverData?> _contentGrid;
        
        public ClassicGrid(int width, int height) : base(width, height)
        {
            _driversOnMap = new ();
            _contentGrid = new (width, height);
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
            return EDriverLocationUpdateResult.Added;;
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

            _driversOnMap.Add(driverID, driverOnMap);
            _contentGrid[x, y] = driverOnMap.Driver;
        }
        
        protected override void MoveDriver(DriverOnMap driverOnMap, int x, int y)
        {
            // Удаляем из старой позиции, если она валидна
            if (_contentGrid.IsValid(driverOnMap.X, driverOnMap.Y))
            {
                // Просто ставим null
                _contentGrid[driverOnMap.X, driverOnMap.Y] = null;
            }

            // Ставим на новую позицию
            _contentGrid[x, y] = driverOnMap.Driver;
            
            // Обновляем координаты
            driverOnMap.UpdatePosition(x, y);
        }
        
        protected override bool CanPlaceDriverToPosition(int driverID, int x, int y)
        {
            var driverInCell = _contentGrid[x, y];

            if (driverInCell != null)
            {
                // Можем поставить, если водитель тот же (установка собственных координат)
                return driverInCell.ID == driverID;
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

            // Удаляем с сетки
            _contentGrid[driverOnMap.X, driverOnMap.Y] = null;

            // Обнуляем координаты
            driverOnMap.UpdatePosition(-1, -1);
        }
    }
}