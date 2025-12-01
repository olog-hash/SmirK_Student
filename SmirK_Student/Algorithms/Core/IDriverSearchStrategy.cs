using SmirK_Student.Map.Layers;

namespace SmirK_Student.Algorithms.Core
{
    /// <summary>
    /// Так как будем использовать семейство алгоритмов, уместно использовать стратегию реализуемую через интерфейс
    /// </summary>
    public interface IDriverSearchStrategy
    {
        List<DriverDistance> FindNearestDrivers(DriversLayer driversLayer, int x, int y, int maxDrivers = 5);
    }
}