using System.Text.Json;

namespace SmirK_Student.Shared.Services
{
    /// <summary>
    /// Сервис генерации случайных чисел в заданном диапазоне.
    /// Выполняет запрос к внешнему API "randomnumberapi.com" и использует полученное значение.
    /// При возникновении ошибок соединения или недоступности сервиса - использует встроенные средства рандомизации C#.
    /// </summary>
    public sealed class RandomizeService
    {
        private readonly HttpClient _httpClient = new();
        private readonly Random _random = new();
        
        /// <summary>
        /// Возвращает случайное число в диапазоне [0:max)
        /// </summary>
        /// <param name="max">Максимальное число (исключенное)</param>
        public async Task<int> Next(int max)
        {
            try
            {
                // Пытаемся установить соединение с сервисом "randomnumberapi.com
                string apiUrl = $"http://www.randomnumberapi.com/api/v1.0/random?min=0&max={max}&count=1";
                var response = await _httpClient.GetAsync(apiUrl);

                // Все прошло успешно - отправляет число
                if (response.IsSuccessStatusCode)
                {
                    // По API он может возвращать несколько чисел, это стоит учитывать.
                    string result = await response.Content.ReadAsStringAsync();
                    var numbers = JsonSerializer.Deserialize<int[]>(result);
                    return numbers[0];
                }
            }
            catch (HttpRequestException e)
            {
                //Console.WriteLine($"Ошибк: {e.StatusCode}");
            }
            
            // Если будут проблемы с соединением - он вернет число из простого рандомайзера.
            return _random.Next(max);
        }
    }
}