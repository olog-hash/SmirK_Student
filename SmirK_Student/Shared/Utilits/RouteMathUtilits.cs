using SmirK_Student.Shared.Primitives;

namespace SmirK_Student.Shared.Utilits
{
    /// <summary>
    /// Математическая утилита для рассчетов связанных с маршрутом.
    /// </summary>
    public static class RouteMathUtilits
    {
        /// <summary>
        /// Строит маршрут от водителя до точки заказа.
        /// Возвращает массив координатных точек, по которым нужно проехать.
        /// </summary>
        public static Vector2Int[] BuildRoute(Vector2Int driverPosition, Vector2Int orderPosition)
        {
            // Заранее рассчитываем размер массива для координат маршрута (ведь это не трудно).
            int distance = MapUtilits.CalculateDistance(driverPosition, orderPosition);
            var routeArray = new Vector2Int[distance + 1];

            // Вычисляем направление куда нужно двигаться
            Vector2Int deltaPosition = orderPosition - driverPosition;
            int stepX = Math.Sign(deltaPosition.X);
            int stepY = Math.Sign(deltaPosition.Y);

            // Подготавливаем основу
            int index = 0;
            routeArray[index++] = driverPosition;
            Vector2Int currentPosition = driverPosition;

            // Двигаемся по X
            for (int i = 0; i < Math.Abs(deltaPosition.X); i++)
            {
                currentPosition.X += stepX;
                routeArray[index++] = currentPosition;
            }
            
            // Двигаемся по Y
            for (int i = 0; i < Math.Abs(deltaPosition.Y); i++)
            {
                currentPosition.Y += stepY;
                routeArray[index++] = currentPosition;
            }
            
            return routeArray;
        }
    }
}