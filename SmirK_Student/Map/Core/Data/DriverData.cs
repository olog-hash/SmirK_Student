namespace SmirK_Student.Map.Core.Data
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