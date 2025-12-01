namespace SmirK_Student.Algorithms.Core
{

    public static class MapUtilits
    {
        public static int CalculateDistance(int x, int y, int ox, int oy)
        {
            return Math.Abs(x - ox) + Math.Abs(y - oy);
        }
        
        public static bool IsInBounds(int width, int height, int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
    }
}