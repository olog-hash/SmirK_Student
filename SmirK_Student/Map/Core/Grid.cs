namespace SmirK_Student.Map.Core
{
    /// <summary>
    /// Удобный фасад для работы с сеткой.
    /// Под капотом находиться одномерный массив, для лучшей производительности.
    /// </summary>
    public class Grid<T>
    {
        public int TotalCells => Width * Height;
        public int Width { get; }
        public int Height { get; }

        private readonly T[] _cells;

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            
            _cells = new T[width * height];
        }

        // Обычная перегрузка оператора, для удобства обращения
        public T this[int x, int y]
        {
            get => _cells[y * Width + x];
            set => _cells[y * Width + x] = value;
        }

        /// <summary>
        /// Проверяет, находиться ли указанная координата в границах сетки.
        /// </summary>
        public bool IsValid(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
}