using SmirK_Student.Algorithms.Core;
using SmirK_Student.Map.Layers;

namespace SmirK_Student.Algorithms
{
    /// <summary>
    /// Обычный линейный поиск путем методом перебора всех доступных водителей из списка.
    /// </summary>
    public sealed class LinearSearchAlgorithm : IDriverSearchStrategy
    {
        public List<DriverDistance> FindNearestDrivers(DriversLayer driversLayer, int x, int y, int maxDrivers = 5)
        {
            // Неверные координаты или отсутствие водителей
            if (!driversLayer.ContentGrid.IsValid(x, y) || driversLayer.DriversCount == 0)
            {
                return new List<DriverDistance>();;
            }

            // Вычисляем расстояние от заказа до водителей
            var driversQueue = new PriorityQueue<DriverDistance, int>();
            foreach (var driverOnMap in driversLayer.GetAllDrivers())
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