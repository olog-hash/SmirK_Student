namespace SmirK_Student.Domain.Drivers.Map.Drivers
{

    public class DriverOnMap
    {
        public DriverData Driver { get; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public DriverOnMap(DriverData driver, int x, int y)
        {
            Driver = driver;
            X = x;
            Y = y;
        }

        public void UpdatePosition(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public DriverOnMap Clone()
        {
            return new DriverOnMap(Driver, X, Y);
        }
    }
}