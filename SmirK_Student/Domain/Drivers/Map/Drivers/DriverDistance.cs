namespace SmirK_Student.Domain.Drivers.Map.Drivers
{
    public struct DriverDistance
    {
        public DriverOnMap DriverOnMap { get; private set; }
        public int Distance { get; private set; }

        public DriverDistance(DriverOnMap driver, int distance)
        {
            DriverOnMap = driver;
            Distance = distance;
        }
    }
}