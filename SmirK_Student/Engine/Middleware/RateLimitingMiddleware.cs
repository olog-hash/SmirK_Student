using Microsoft.Extensions.Options;
using SmirK_Student.Engine.Configuration;

namespace SmirK_Student.Engine.Middleware
{
    /// <summary>
    /// Промежуточный слой для ограничения частоты запросов к серверу.
    /// Перехватывает входящие запросы и отклоняет их при достижении допустимого порога обращений.
    /// </summary>
    public sealed class RateLimitingMiddleware
    {
        public static int CurrentRequests => _currentRequests;
        
        private readonly RequestDelegate _next;
        private readonly int _parallelLimit;
        
        private static int _currentRequests = 0;
        private static readonly object _lock = new();
        
        public RateLimitingMiddleware(RequestDelegate next, IOptions<SettingsConfig> settings)
        {
            _next = next;
            _parallelLimit = settings.Value.ParallelLimit;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            int current = Interlocked.Increment(ref _currentRequests);

            // 1. Проверяем, влезаем ли в текущее окно лимита.
            if (current > _parallelLimit)
            {
                // Откатываем свое инкремирование и возвращаем ошибку
                Interlocked.Decrement(ref _currentRequests);
                context.Response.StatusCode = 503;
                await context.Response.WriteAsync("Service Unavailable");
                return;
            }

            // 2. Обрабатываем запрос и после завершения логики освобождаем место
            try
            {
                // Начинаем обработку
                await _next.Invoke(context);
            }
            finally
            {
                Interlocked.Decrement(ref _currentRequests);
            }
        }
    }
}