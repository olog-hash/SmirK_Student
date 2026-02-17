namespace SmirK_Student.API.Features.DriverOrder.DTOs.Requests
{
    public sealed class DriverLocationRequest
    {
        public int DriverID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}