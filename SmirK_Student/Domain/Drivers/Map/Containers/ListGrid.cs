using SmirK_Student.Domain.Drivers.Map.Containers.Core;
using SmirK_Student.Domain.Drivers.Map.Drivers;

namespace SmirK_Student.Domain.Drivers.Map.Containers
{
    /// <summary>
    /// Обыкновенный список (словарь) для хранения водителей и последующего перебора.
    /// Создан специально для алгоритма грубого перебора.
    /// </summary>
    public sealed class ListGrid : DriversContainer
    {
        public override int DriversCount => _driversOnMap.Count;
        public IReadOnlyDictionary<int, DriverOnMap> DriversOnMap => _driversOnMap;
        public IEnumerable<DriverOnMap> GetAllDrivers() => _driversOnMap.Values;
        
        // Словарь водителей по их идентификатору на карте, для быстрого доступа
        private readonly Dictionary<int, DriverOnMap> _driversOnMap;
        
        public ListGrid(int width, int height) : base(width, height)
        {
            _driversOnMap = new ();
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
            // Просто удаляем из словаря
            if (_driversOnMap.Remove(driverID))
            {
                return EDriverLocationUpdateResult.Removed;
            }
            
            return EDriverLocationUpdateResult.InvalidDriver;
        }

        protected override void AddNewDriver(int driverID, int x, int y)
        {
            var driver = new DriverData(driverID);
            var driverOnMap = new DriverOnMap(driver, x, y);

            _driversOnMap.Add(driverID, driverOnMap);
        }

        protected override void MoveDriver(DriverOnMap driverOnMap, int x, int y)
        {
            // Просто обновляем координаты
            driverOnMap.UpdatePosition(x, y);
        }

        protected override bool CanPlaceDriverToPosition(int driverID, int x, int y)
        {
            // Проверяем, есть ли другой водитель, что занимает требуемую позицию
            bool isPositionOccupied = _driversOnMap.Any(d => d.Value.X == x && d.Value.Y == y && d.Value.Driver.ID != driverID);
            return !isPositionOccupied;
        }
    }
}