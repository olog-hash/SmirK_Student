using SmirK_Student.Domain.Drivers.Map.Drivers;
using SmirK_Student.Shared.Primitives;

namespace SmirK_Student.API.Features.DriverOrder.DTOs.Responses
{
    public sealed class FindDriverForOrderResponse
    {
        public int DriverID { get; set; }
        public Vector2Int DriverPosition { get; set; }
        public int Distance { get; set; }
        public Vector2Int[] RouteArray { get; set; }

        public FindDriverForOrderResponse(DriverOnMap driverOnMap, Vector2Int[] routeArray, int distance)
        {
            DriverID = driverOnMap.Driver.ID;
            DriverPosition = routeArray[0];
            RouteArray = routeArray;
            Distance = distance;
        }
    }
}