using Microsoft.AspNetCore.Mvc;
using SmirK_Student.API.Features.DriverOrder.DTOs.Requests;
using SmirK_Student.API.Features.DriverOrder.DTOs.Responses;
using SmirK_Student.API.Features.DriverOrder.Services;
using SmirK_Student.Domain.Drivers.Algorithms;
using SmirK_Student.Domain.Drivers.Map.Containers;
using SmirK_Student.Domain.Drivers.Map.Containers.Core;

namespace SmirK_Student.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriverOrderController : ControllerBase 
    {
        private readonly DriverLocationAdapter<ClassicGrid, ManhattanRadialSearch> _driverLocationAdapter;

        public DriverOrderController(DriverLocationAdapter<ClassicGrid, ManhattanRadialSearch> driverLocationAdapter)
        {
            _driverLocationAdapter = driverLocationAdapter;
        }
        
        [HttpPost("AddOrUpdateDriver")]
        public ActionResult<EDriverLocationUpdateResult> AddOrUpdateDriver([FromBody] DriverLocationRequest request)
        {
            // Получаем результат операции через адаптер.
            var result = _driverLocationAdapter.AddOrUpdateDriver(request.DriverID, request.X, request.Y);

            // Конвертируем в читабельный вид.
            return result switch
            {
                EDriverLocationUpdateResult.InvalidPosition => BadRequest("Координаты некорректны"),
                EDriverLocationUpdateResult.OccupiedPosition => BadRequest("Здесь уже находится другой водитель"),
                EDriverLocationUpdateResult.Added => Ok("Координаты успешно добавлены"),
                EDriverLocationUpdateResult.Updated => Ok("Координаты успешно изменены"),
                _ => StatusCode(500)
            };
        }
    
        [HttpPost("FindDriverForOrder")]
        public async Task<ActionResult<FindDriverForOrderResponse>> FindDriverForOrder([FromBody] FindDriverForOrderRequest request)
        {
            // Получаем результат операции через адаптер.
            var result = await _driverLocationAdapter.FindDriver(request.OrderID, request.X, request.Y);

            // Смотрим по успешности
            if (result.Success)
            {
                return Ok(new FindDriverForOrderResponse(result.SelectedDriver, result.RouteArray, result.Distance));
            }
            
            // В ином случаи выводим ошибку
            return BadRequest(result.ErrorMessage);
        }
        
        [HttpGet("slow-test")]
        public async Task<ActionResult> SlowEndpoint()
        {
            // Искусственная задержка в 5 секунд для проверки лимита по числу обращений на сервер.
            await Task.Delay(5000);
            return Ok("Done");
        }
    }
}