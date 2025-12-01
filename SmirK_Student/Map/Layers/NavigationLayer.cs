using SmirK_Student.Map.Core;

namespace SmirK_Student.Map.Layers
{

    public enum EBlockType
    {
        None, // Пустое (пешеходное) место - невозможно проехать
        Road  // Дорога - единственное место, где можно проехать 
    }
    
    public sealed class NavigationLayer : MapLayer<EBlockType>
    {
        public NavigationLayer(int width, int height) : base(width, height)
        {
            FillMapByRoad();
        }

        /// <summary>
        /// Заполнить всю навигационную карту дорогой (дать возможность перемещаться везде)
        /// </summary>
        private void FillMapByRoad()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _contentGrid[x, y] = EBlockType.Road;
                }
            }
        }
    }
}