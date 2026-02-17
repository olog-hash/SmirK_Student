using SmirK_Student.Domain.Drivers.Algorithms.Core;
using SmirK_Student.Domain.Drivers.Map.Containers;
using SmirK_Student.Domain.Drivers.Map.Drivers;

namespace SmirK_Student.Domain.Drivers.Algorithms
{
    /// <summary>
    /// Алгоритм поиска ближайших водителей путем грубого перебора всех доступных водителей из списка.
    /// </summary>
    public sealed class BruteForceAlgorithm : BaseDriverSearchStrategy<ListGrid>
    {
        protected override List<DriverDistance> ExecuteAlgorithm(ListGrid classicGrid, int x, int y, int maxDrivers)
        {
            // Вычисляем расстояние от заказа до водителей
            var driversQueue = new PriorityQueue<DriverDistance, int>();
            foreach (var driverOnMap in classicGrid.GetAllDrivers())
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
            
            // Обрезаем список до maxDrivers
            if (result.Count > maxDrivers)
            {
                result.RemoveRange(maxDrivers, result.Count - maxDrivers);
            }
            
            return result;
        }
    }
}