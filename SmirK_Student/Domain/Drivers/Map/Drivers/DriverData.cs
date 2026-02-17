namespace SmirK_Student.Domain.Drivers.Map.Drivers
{
    public class DriverData
    {
        public int ID { get; }
        public bool IsAvaliable { get; set; }

        public DriverData(int id)
        {
            ID = id;
            IsAvaliable = true;
        }
    }
}