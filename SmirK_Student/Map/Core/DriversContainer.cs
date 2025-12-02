using SmirK_Student.Algorithms.Core;
using SmirK_Student.Map.Core.Data;

namespace SmirK_Student.Map.Core
{
    /// <summary>
    /// Базовый класс, что размечает основные параметры и методы для контейнера с водителями.
    /// </summary>
    public abstract class DriversContainer
    {
        public abstract int DriversCount { get; }
        
        public readonly int Width;
        public readonly int Height;

        public DriversContainer(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Проверяет валидность выбранной позиции для размещения/перемещения водителя.
        /// </summary>
        public virtual bool IsValidPosition(int x, int y)
        {
            return MapUtilits.IsInBounds(Width, Height, x, y);
        }
        
        /// <summary>
        /// Добавляет нового водителя по заданным координатам, либо же обновляет позицию уже существующего водителя.
        /// </summary>
        public abstract EDriverLocationUpdateResult AddOrUpdateDriver(int driverID, int x, int y);
        
        /// <summary>
        /// Удаляет водителя из всего контейнера (если он там есть).
        /// </summary>
        public abstract EDriverLocationUpdateResult RemoveDriver(int driverID);

        /// <summary>
        /// Создает нового водителя и помещает его по заданным координатам.
        /// </summary>
        protected abstract void AddNewDriver(int driverID, int x, int y);
        
        /// <summary>
        /// Перемещает существующего водителя на заданные координаты.
        /// </summary>
        protected abstract void MoveDriver(DriverOnMap driverOnMap, int x, int y);
        
        /// <summary>
        /// Проверяет, возможно ли поместить водителя на выбранные координаты.
        /// (Позиция может быть уже занята другим водителем).
        /// </summary>
        protected abstract bool CanPlaceDriverToPosition(int driverID, int x, int y);
    }
}