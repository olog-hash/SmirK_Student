using SmirK_Student.Domain.Drivers.Map.Drivers;
using SmirK_Student.Shared.Utilits;

namespace SmirK_Student.Domain.Drivers.Map.Containers.Core
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
            // Проверка на минимальное значение карты
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Ширина должна быть больше нуля!");
            }
            
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Высота должна быть больше нуля!");
            }
            
            // Проверка на максимальное значение карты
            if (width >= int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(width), $"Ширина превышает допустимый лимит {int.MaxValue}!");
            }
            
            if (height >= int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(height), $"Высота превышает допустимый лимит {int.MaxValue}!");
            }
            
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