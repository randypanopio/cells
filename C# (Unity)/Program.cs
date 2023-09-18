using CellGeneration.CellUtilities;

namespace CellGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            int width = 64, height = 64;
            float seed = CellAlgorithms.GenerateSeed();
            Console.WriteLine("seed: " + seed);
            Console.WriteLine("Cell Count: " + width * height);

            // STEP 1 TODO build out base biome map generation like good oll times
            // int[,] imap = new int[width, height];
            // imap = CellAlgorithms.RandomFill(imap, seed, 50);
            // map = CellAlgorithms.SmoothMap(map, 12);

            List<KeyValuePair<int, float>> cellValues = new()
            {
                new(CellConstants.ALMOST_EMPTY_TILE, CellConstants.ALMOST_EMPTY_TILE_PROBABILITY),
                new(CellConstants.ALMOST_SOLID_TILE, CellConstants.ALMOST_SOLID_TILE_PROBABILITY)
            };

            Cell[,] map = BaseCellMaps.GenerateEmptyMap(width, height);
            map = BaseCellMaps.RandomFill(map, seed, cellValues);


            // for (int i = 0; i <  map.GetLength(0); i++)
            // {
            //     for (int j = 0; j < map.GetLength(1); j++)
            //     {

            //         arrayString += string.Format("{0}, ", map[i, j] > 0 ? "X" : "O");
            //     }
            //     arrayString += Environment.NewLine + Environment.NewLine;
            // }
            // /Console.WriteLine(string.Format("generated map:\n{0}", arrayString));
            //
            watch.Stop();
            Console.WriteLine(string.Format("Time ellapsed (in ms): {0}", watch.ElapsedMilliseconds));
        }


    }
}