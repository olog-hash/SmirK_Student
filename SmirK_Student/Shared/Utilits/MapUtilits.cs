using SmirK_Student.Shared.Primitives;

namespace SmirK_Student.Shared.Utilits
{
    /// <summary>
    /// Математическая утилита для базовых операций и проверок
    /// </summary>
    public static class MapUtilits
    {
        /// <summary>
        /// Рассчитывает расстояние между двумя точками на основе Манхэттенского расстояния.
        /// </summary>
        public static int CalculateDistance(int x, int y, int ox, int oy)
        {
            return Math.Abs(x - ox) + Math.Abs(y - oy);
        }
        
        /// <summary>
        /// Рассчитывает расстояние между двумя точками на основе Манхэттенского расстояния.
        /// </summary>
        public static int CalculateDistance(Vector2Int point1, Vector2Int point2)
        {
            return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
        }
        
        /// <summary>
        /// Проверяет, находиться ли точка в приделах границы области width x height.
        /// </summary>
        public static bool IsInBounds(int width, int height, int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
                
        /// <summary>
        /// Проверяет, находиться ли точка в приделах границы области width x height.
        /// </summary>
        public static bool IsInBounds(int width, int height, Vector2Int point)
        {
            return point.X >= 0 && point.X < width && point.Y >= 0 && point.Y < height;
        }
    }
}