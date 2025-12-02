using SmirK_Student.Algorithms.Core;
using SmirK_Student.Map.Containers;
using SmirK_Student.Map.Core.Data;

namespace SmirK_Student.Algorithms
{
    /// <summary>
    /// Обычный поиск путем грубого перебора всех доступных водителей из списка.
    /// </summary>
    public sealed class BruteForceAlgorithm : IDriverSearchStrategy<SimpleGrid>
    {
        public List<DriverDistance> FindNearestDrivers(SimpleGrid simpleGrid, int x, int y, int maxDrivers = 5)
        {
            // Неверные координаты или отсутствие водителей
            if (!simpleGrid.IsValidPosition(x, y) || simpleGrid.DriversCount == 0)
            {
                return new List<DriverDistance>();;
            }

            // Вычисляем расстояние от заказа до водителей
            var driversQueue = new PriorityQueue<DriverDistance, int>();
            foreach (var driverOnMap in simpleGrid.GetAllDrivers())
            {
                // Если водитель недоступен - пропускаем
                if (!driverOnMap.Driver.IsAvaliable)
                    continue;

                // Так как движение предусматривает поремещение только ↑ ↓ ← →, то будем использовать манхэттенское расстояние
                int distance = Math.Abs(driverOnMap.X - x) + Math.Abs(driverOnMap.Y - y);

                // Приоритетная очередь сама проводит сортировку, но чтобы удалять самые дальные (с конца) - инвертируем дистанцию.
                driversQueue.Enqueue(new DriverDistance(driverOnMap, distance), -distance);

                // Если водителей больше - удаляем самого дальнего
                if (driversQueue.Count > maxDrivers)
                {
                    driversQueue.Dequeue();
                }
            }

            // Извлекаем найденных водителей
            var result = new List<DriverDistance>(driversQueue.Count);
            while (driversQueue.Count > 0)
            {
                result.Add(driversQueue.Dequeue());
            }

            // Переворачиваем, чтобы получить список по возрастанию
            result.Reverse();
            
            return result;
        }
    }
}