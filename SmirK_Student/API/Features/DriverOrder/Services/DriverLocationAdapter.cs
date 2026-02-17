using SmirK_Student.API.Features.DriverOrder.Results;
using SmirK_Student.Domain.Drivers.Algorithms.Core;
using SmirK_Student.Domain.Drivers.Map.Containers.Core;
using SmirK_Student.Shared.Primitives;
using SmirK_Student.Shared.Services;
using SmirK_Student.Shared.Utilits;

namespace SmirK_Student.API.Features.DriverOrder.Services
{
    /// <summary>
    /// Адаптер для работы с картой поверх DriversContainer.
    /// Позволяет изменять логику добавления/обновления координат водителя, а также специфичную логики подбора водителя.
    /// Не нужно изменять сам DriversContainer (принцип OCP).
    /// </summary>
    public sealed class DriverLocationAdapter<TContainer, TAlgorithm> 
        where TContainer : DriversContainer 
        where TAlgorithm : BaseDriverSearchStrategy<TContainer>, new()
    {
        private readonly TContainer _driversContainer;
        private readonly TAlgorithm _algorithm;
        
        // Сервисы
        private readonly RandomizeService _randomService;

        public DriverLocationAdapter(TContainer driversContainer)
        {
            _driversContainer = driversContainer;
            _algorithm = new TAlgorithm();
            
            _randomService = new RandomizeService();
        }

        public EDriverLocationUpdateResult AddOrUpdateDriver(int driverID, int x, int y)
        {
            var result = _driversContainer.AddOrUpdateDriver(driverID, x, y);
            
            // Если позиция невалида - вызываем удаления водителя на карте.
            if (result == EDriverLocationUpdateResult.InvalidPosition)
            {
                // В задании не было четко прописано что делать с водителем, просто "его предыдущие координаты должны быть удалены".
                // Трактовать это можно по разному - либо полное удаление с карты, либо маркировка о недоступности.
                // Так как метод обновления координат тесно связан с добавление нового водители, посчитал, что лучше удаление.
                // Если потом потребуется переместить водителя с этим ID на правильную позицию - он просто создасться заново.
                
                // Наличие водителя не критично, удаление инкапсулирует проверку.
                _driversContainer.RemoveDriver(driverID);
            }
            
            return result;
        }

        public async Task<OrderSearchResult> FindDriver(int orderID, int x, int y, int maxDrivers = 5)
        {
            // 1. Валидация координат заказа
            if (!_driversContainer.IsValidPosition(x, y))
            {
                return OrderSearchResult.ErrorResult(orderID, EOrderSearchError.InvalidPosition);
            }

            // 2. Проверка численности водителей
            if (_driversContainer.DriversCount == 0)
            {
                return OrderSearchResult.ErrorResult(orderID, EOrderSearchError.NoDriversAvaliable);
            }
            
            // 3. Получаем список водителей к точке заказа
            var nearestDrivers = _algorithm.FindNearestDrivers(_driversContainer, x, y, maxDrivers);
            
            // Если список пустой - значит может быть ошибка в работе алгоритма.
            if (nearestDrivers.Count == 0)
            {
                return OrderSearchResult.ErrorResult(orderID, EOrderSearchError.InternalError);
            }
            
            // 4. Выбираем случайного водителя (если их несколько)
            var selectedDriver = nearestDrivers[0];
            if (nearestDrivers.Count > 1)
            {
                int selectedIndex = await _randomService.Next(nearestDrivers.Count);
                selectedDriver = nearestDrivers[selectedIndex];
            }
            
            // 5. Строим маршрут к точке заказа
            var orderPosition = new Vector2Int(x, y);
            var driverPosition = new Vector2Int(selectedDriver.DriverOnMap.X, selectedDriver.DriverOnMap.Y);
            
            Vector2Int[] routeArray = RouteMathUtilits.BuildRoute(driverPosition, orderPosition);
            
            // 6. Отправляем обработанный результат
            return OrderSearchResult.SuccessResult(orderID, selectedDriver.DriverOnMap, routeArray, selectedDriver.Distance);
        }
    }
}