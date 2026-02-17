namespace SmirK_Student.API.Features.DriverOrder.DTOs.Requests
{

    public class FindDriverForOrderRequest
    {
        public int OrderID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}