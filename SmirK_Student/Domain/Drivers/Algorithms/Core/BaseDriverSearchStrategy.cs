using SmirK_Student.Domain.Drivers.Map.Containers.Core;
using SmirK_Student.Domain.Drivers.Map.Drivers;
using SmirK_Student.Engine.Constants;

namespace SmirK_Student.Domain.Drivers.Algorithms.Core
{
    /// <summary>
    /// Так как будем использовать семейство алгоритмов, уместно использовать стратегию реализуемую через абстрактный класс
    /// Можно было конечно использовать интерфейс, но инкапсулировать проверки в абстрактном классе всяко удобнее.
    /// </summary>
    public abstract class BaseDriverSearchStrategy<TContainer> where TContainer : DriversContainer
    {
        /// <summary>
        /// Метод поиска ближайших водителей в контейнере по заданным координатам.
        /// Инкапсулирует проверку на корректность поступаемых данных.
        /// </summary>
        public virtual List<DriverDistance> FindNearestDrivers(TContainer container, int x, int y, int maxDrivers = SmirKConstants.DefaultMaxSearchDrivers)
        {
            if (!ValidateInput(container, x, y, maxDrivers))
            {
                // Просто возвращаем пустой список
                return new List<DriverDistance>(); 
            }
            
            return ExecuteAlgorithm(container, x, y, maxDrivers);
        }

        /// <summary>
        /// Проверяет корректность введенных данных и значений
        /// </summary>
        protected virtual bool ValidateInput(TContainer container, int x, int y, int maxDrivers)
        {
            // Для удобства развернул по строчкам.
            if (maxDrivers <= 0) return false;
            if (!container.IsValidPosition(x, y)) return false;
            if (container.DriversCount == 0) return false;
            return true;
        }

        /// <summary>
        /// Абстрактный метод, содержащий сам алгоритм поиска.
        /// </summary>
        protected abstract List<DriverDistance> ExecuteAlgorithm(TContainer container, int x, int y, int maxDrivers);
    }
}