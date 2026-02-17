using BenchmarkDotNet.Attributes;
using SmirK_Student.Domain.Drivers.Algorithms;
using SmirK_Student.Domain.Drivers.Map.Containers;

namespace SmirK_Student.Algorithms.Benchmarks
{
    public enum EDriverPlacement
    {
        Random,
        WorstCase
    }

    public enum ESearchPosition
    {
        Center,
        Corner
    }

    [MemoryDiagnoser]
    [HtmlExporter]
    [CsvExporter]
    [MarkdownExporterAttribute.GitHub]
    public class DriverSearchBenchmarks
    {
        [Params(100, 500, 1000)] public int MapSize { get; set; }

        [Params(0.01, 0.10)] public double DriverDensity { get; set; }

        [Params(EDriverPlacement.Random, EDriverPlacement.WorstCase)]
        public EDriverPlacement DriverPlacement { get; set; }

        [Params(10, 100)] public int MaxDrivers { get; set; }

        [Params(ESearchPosition.Center, ESearchPosition.Corner)]
        public ESearchPosition SearchPosition { get; set; }

        // Контейнеры для разных алгоритмов
        private ListGrid _listGrid;
        private ClassicGrid _classicGrid;
        private SpatialGrid _spatialGrid;

        // Алгоритмы
        private BruteForceAlgorithm _bruteForceAlgorithm;
        private BFSAlgorithm _bfsAlgorithm;
        private ManhattanRadialSearch _manhattanRadialSearch;
        private SectorGridAlgorithm _sectorGridAlgorithm;

        // Координаты точки поиска
        private int _searchX;
        private int _searchY;

        // Утилиты
        private Random _random;

        [GlobalSetup]
        public void Setup()
        {
            // Фиксированный сид
            _random = new Random(17);

            // Вычисляем количество водителей
            int totalCells = MapSize * MapSize;
            int driverCount = (int)(totalCells * DriverDensity);

            // Вычисляем размер сектора для SpatialGrid
            int cellSize = Math.Max(1, (int)(Math.Sqrt(MapSize) / 4));

            // Инициализируем контейнеры
            _listGrid = new ListGrid(MapSize, MapSize);
            _classicGrid = new ClassicGrid(MapSize, MapSize);
            _spatialGrid = new SpatialGrid(MapSize, MapSize, cellSize);

            // Инициализируем алгоритмы
            _bruteForceAlgorithm = new BruteForceAlgorithm();
            _bfsAlgorithm = new BFSAlgorithm();
            _manhattanRadialSearch = new ManhattanRadialSearch();
            _sectorGridAlgorithm = new SectorGridAlgorithm();

            // Определяем точку заказа
            (_searchX, _searchY) = GetSearchPosition();

            // Размещаем водителей
            PlaceDrivers(driverCount);
        }

        private (int x, int y) GetSearchPosition()
        {
            return SearchPosition switch
            {
                ESearchPosition.Center => (MapSize / 2, MapSize / 2),
                ESearchPosition.Corner => (0, 0),
            };
        }

        private void PlaceDrivers(int driverCount)
        {
            switch (DriverPlacement)
            {
                case EDriverPlacement.Random:
                    PlaceDriversRandomly(driverCount);
                    break;

                case EDriverPlacement.WorstCase:
                    PlaceDriversWorstCase(driverCount);
                    break;

                default:
                    break;
            }
        }

        private void PlaceDriversRandomly(int driverCount)
        {
            var occupiedPositions = new HashSet<(int, int)>();

            for (int i = 0; i < driverCount; i++)
            {
                int x, y;

                // Ищем свободную позицию
                do
                {
                    x = _random.Next(0, MapSize);
                    y = _random.Next(0, MapSize);
                } while (occupiedPositions.Contains((x, y)));

                occupiedPositions.Add((x, y));

                // Добавляем во все контейнеры
                _listGrid.AddOrUpdateDriver(i, x, y);
                _classicGrid.AddOrUpdateDriver(i, x, y);
                _spatialGrid.AddOrUpdateDriver(i, x, y);
            }
        }

        private void PlaceDriversWorstCase(int driverCount)
        {
            // Худший случай -  все водители максимально далеко от точки поиска
            // Размещаем их в противоположных углах и по краям карты

            var positions = new List<(int x, int y)>();

            // Генерируем позиции в дальних углах и по краям
            // Определяем дальний угол от точки поиска
            int farX = _searchX < MapSize / 2 ? MapSize - 1 : 0;
            int farY = _searchY < MapSize / 2 ? MapSize - 1 : 0;

            // Заполняем спирально
            int radius = 0;
            while (positions.Count < driverCount && radius < MapSize)
            {
                for (int dx = -radius; dx <= radius && positions.Count < driverCount; dx++)
                {
                    for (int dy = -radius; dy <= radius && positions.Count < driverCount; dy++)
                    {
                        if (Math.Abs(dx) == radius || Math.Abs(dy) == radius)
                        {
                            int x = farX + dx;
                            int y = farY + dy;

                            if (x >= 0 && x < MapSize && y >= 0 && y < MapSize)
                            {
                                positions.Add((x, y));
                            }
                        }
                    }
                }

                radius++;
            }

            // Добавляем водителей на эти позиции
            for (int i = 0; i < Math.Min(driverCount, positions.Count); i++)
            {
                var (x, y) = positions[i];

                _listGrid.AddOrUpdateDriver(i, x, y);
                _classicGrid.AddOrUpdateDriver(i, x, y);
                _spatialGrid.AddOrUpdateDriver(i, x, y);
            }
        }

        [Benchmark]
        public void BruteForce()
        {
            _bruteForceAlgorithm.FindNearestDrivers(_listGrid, _searchX, _searchY, MaxDrivers);
        }

        [Benchmark]
        public void BFS()
        {
            _bfsAlgorithm.FindNearestDrivers(_classicGrid, _searchX, _searchY, MaxDrivers);
        }

        [Benchmark]
        public void DistanceSearch()
        {
            _manhattanRadialSearch.FindNearestDrivers(_classicGrid, _searchX, _searchY, MaxDrivers);
        }

        [Benchmark]
        public void SectorGrid()
        {
            _sectorGridAlgorithm.FindNearestDrivers(_spatialGrid, _searchX, _searchY, MaxDrivers);
        }
    }
}