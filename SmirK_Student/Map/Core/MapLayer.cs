namespace SmirK_Student.Map.Core
{
    public abstract class MapLayer<TContent>
    {
        public Grid<TContent> ContentGrid => _contentGrid;
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        protected Grid<TContent> _contentGrid;

        public MapLayer(int width, int height)
        {
            Width = width;
            Height = height;
            
            _contentGrid = new Grid<TContent>(width, height);
        }
    }
}