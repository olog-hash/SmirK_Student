using SmirK_Student.Map.Core;

namespace SmirK_Student.Map.Layers
{
    public class DriverData
    {
        public int ID { get; }
        public bool IsAvaliable { get; set; }

        public DriverData(int id)
        {
            ID = id;
            IsAvaliable = true;
        }
    }
    
    public class DriverOnMap
    {
        public DriverData Driver { get; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public DriverOnMap(DriverData driver, int x, int y)
        {
            Driver = driver;
            X = x;
            Y = y;
        }

        public void UpdatePosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    
    public enum EDriverLocationUpdateResult
    {
        InvalidPosition,
        OccupiedPosition,
        Added,
        Updated,
    }
    
    public sealed class DriversLayer : MapLayer<DriverData?>
    {
        public int DriversCount => _driversMap.Count;
        
        // Словарь водителей по их идентификатору на карте, для быстрого доступа
        private readonly Dictionary<int, DriverOnMap> _driversMap = new();
        
        public DriversLayer(int width, int height) : base(width, height)
        {
            
        }

        public EDriverLocationUpdateResult AddOrUpdateDriver(int driverID, int x, int y)
        {
            // Координаты за пределом карты
            if (!_contentGrid.IsValid(x, y))
            {
                // Неверная позиция
                return EDriverLocationUpdateResult.InvalidPosition;
            }
            
            // Если позиция уже занята другим водителем
            if (!CanPlaceDriverToCell(driverID, x, y))
            {
                return EDriverLocationUpdateResult.OccupiedPosition;
            }
            
            // Если водитель уже существует на карте
            if (_driversMap.TryGetValue(driverID, out var driverOnMap))
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
        
        /// <summary>
        /// Добавить нового водителя
        /// </summary>
        private void AddNewDriver(int driverID, int x, int y)
        {
            var driver = new DriverData(driverID);
            var driverOnMap = new DriverOnMap(driver, x, y);

            _driversMap.Add(driverID, driverOnMap);
            _contentGrid[x, y] = driverOnMap.Driver;
        }

        /// <summary>
        /// Переместить существующего водителя в новую позицию
        /// </summary>
        private void MoveDriver(DriverOnMap driverOnMap, int x, int y)
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
        
        /// <summary>
        /// Удалить водителя полностью из всей системы
        /// </summary>
        public void RemoveDriver(int driverID)
        {
            if (_driversMap.TryGetValue(driverID, out var driverOnMap))
            {
                RemoveDriverFromGrid(driverOnMap);
                _driversMap.Remove(driverID);
            }
        }
        
        /// <summary>
        /// Удалить водителя с сетки
        /// </summary>
        private void RemoveDriverFromGrid(DriverOnMap driverOnMap)
        {
            // Удаляем с сетки, если позиция валидна
            if (_contentGrid.IsValid(driverOnMap.X, driverOnMap.Y))
            {
                _contentGrid[driverOnMap.X, driverOnMap.Y] = null;
            }
            
            // Обнуляем координаты
            driverOnMap.UpdatePosition(-1, -1);
        }
        
        /// <summary>
        /// Проверяет заполненность сетки
        /// </summary>
        private bool CanPlaceDriverToCell(int driverID, int x, int y)
        {
            var driverInCell = _contentGrid[x, y];

            if (driverInCell != null)
            {
                // Можем поставить, если водитель тот же (установка собственных координат)
                return driverInCell.ID == driverID;
            }

            return true;
        }
        
        public IEnumerable<DriverOnMap> GetAllDrivers() => _driversMap.Values;
    }
}