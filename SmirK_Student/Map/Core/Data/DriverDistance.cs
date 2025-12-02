namespace SmirK_Student.Map.Core.Data
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