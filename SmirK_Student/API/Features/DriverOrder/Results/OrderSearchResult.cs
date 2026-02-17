using SmirK_Student.Domain.Drivers.Map.Drivers;
using SmirK_Student.Shared.Primitives;

namespace SmirK_Student.API.Features.DriverOrder.Results
{
    public enum EOrderSearchError
    {
        None,
        InvalidPosition,
        NoDriversAvaliable,
        InternalError,
    }
    
    public sealed class OrderSearchResult
    {
        // Мета данные
        public int OrderID { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        
        // Вычисленные данные
        public DriverOnMap? SelectedDriver { get; set; }
        public Vector2Int[]? RouteArray { get; set; }
        public int Distance { get; set; }

        public static OrderSearchResult ErrorResult(int orderID, EOrderSearchError error)
        {
            return new OrderSearchResult
            {
                OrderID = orderID,
                Success = false,
                ErrorMessage = GetErrorMessage(error),
            };
        }

        public static OrderSearchResult SuccessResult(int orderID, DriverOnMap driver, Vector2Int[] route, int distance)
        {
            return new OrderSearchResult
            {
                OrderID = orderID,
                Success = true,
                ErrorMessage = string.Empty,
                
                SelectedDriver = driver,
                RouteArray = route,
                Distance = distance,
            };
        }

        private static string GetErrorMessage(EOrderSearchError error)
        {
            return error switch
            {
                EOrderSearchError.None => "Ошибок нет",
                EOrderSearchError.InvalidPosition => "Координаты некорректны",
                EOrderSearchError.NoDriversAvaliable => "Свободных водителей нет",
                EOrderSearchError.InternalError => "Внутренняя ошибка логики",
                _ => "Неизвестная ошибка",
            };
        }
    }
}