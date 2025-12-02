using SmirK_Student.Map.Core;
using SmirK_Student.Map.Core.Data;

namespace SmirK_Student.Algorithms.Core
{
    /// <summary>
    /// Так как будем использовать семейство алгоритмов, уместно использовать стратегию реализуемую через интерфейс
    /// </summary>
    public interface IDriverSearchStrategy<TContainer> where TContainer : DriversContainer
    {
        List<DriverDistance> FindNearestDrivers(TContainer container, int x, int y, int maxDrivers = 5);
    }
}