// See https://aka.ms/new-console-template for more information
namespace CellGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            int width = 8, height = 26;
            int fillAmount = 50;
            float seed = 6123129;//CellAlgorithms.GenerateSeed();
            Console.WriteLine("seed: " + seed);

            // STEP 1 TODO build out base biome map generation like good oll times
            int[,] map = new int[width, height];
            map = CellAlgorithms.RandomFill(map, seed, fillAmount);
            // map = CellAlgorithms.SmoothMap(map, 1);
            string arrayString = "";
            for (int i = 0; i <  map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {

                    arrayString += string.Format("{0}, ", map[i, j] > 0 ? "X" : "O");
                }
                arrayString += Environment.NewLine + Environment.NewLine;
            }
            Console.WriteLine(string.Format("generated map:\n{0}", arrayString));
            //
        }


    }
}