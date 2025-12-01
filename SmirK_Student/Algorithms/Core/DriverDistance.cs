using SmirK_Student.Map.Layers;

namespace SmirK_Student.Algorithms.Core
{
    public struct DriverDistance
    {
        public DriverOnMap DriverOnMap;
        public int Distance;

        public DriverDistance(DriverOnMap driver, int distance)
        {
            DriverOnMap = driver;
            Distance = distance;
        }
    }
}