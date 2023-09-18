using System;
using System.Collections.Generic;
using System.Linq;
using CellGeneration.CellUtilities;

namespace CellGeneration
{

	// TODO rename
	public class CellValue
	{
		public int Value;
		public float Weight; // 0%-100%
	}


	public class Cell
	{
		private readonly bool cacheValue, cacheBiome;

		public Cell()
		{
			Value = CellConstants.UNSET;
			BiomeValue = CellConstants.UNSET;
			cacheValue = true;
			cacheBiome = true;
			Overridable = true;
		}

		public Cell(int value, int biomeValue, bool Overridable, bool cacheValue = true, bool cacheBiome = true)
		{
			this.Overridable = Overridable;
			if (Overridable)
			{
				// TODO i dont need to set defaults, B/C when doing passes, i would just check if custom cell
				// NVM i do need to still set values so they can be distinguished during passes, but instead CANNOT be overriden
				Value = CellConstants.IGNORED;
				BiomeValue = CellConstants.IGNORED;
				Overridable = false;
			}
			else
			{
				Value = value;
				Overridable = true;
			}
			BiomeValue = biomeValue;
			this.cacheBiome = cacheBiome;
			this.cacheValue = cacheValue;
		}


		// SET when used by custom cells, or whenever a certain cell is no longer needed in the processing passes
		public bool Overridable { get; set; }

		private int _value;
		public int Value
		{
			get { return _value; }
			set
			{
				if (Overridable) return; // Not sure if i need this
				if (cacheValue)
				{
					PreviousValue = _value;
				}
				_value = value;
			}
		}

		public int PreviousValue { get; private set; }

		private int _biomeValue;
		public int BiomeValue
		{
			get { return _biomeValue; }
			set
			{
				if (Overridable) return;
				if (cacheBiome)
				{
					PreviousBiomeValue = _biomeValue;
				}
				_biomeValue = value;
			}
		}

		public int PreviousBiomeValue { get; private set; }
	}


	/// <summary>
	/// Base Functions to Tile a map for future processing
	/// </summary>
	public static class BaseCellMaps
	{
		// MAYBE TODO, add generic helper functions that use int maps to convert into Cell[,] maps, if that makes sense/useful

		public static Cell[,] GenerateEmptyMap(int width, int height){
			Cell[,] map = new Cell[width, height];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					map[x,y] = new Cell();
				}
			}
			return map;
		}

		public static Cell[,] RandomFill(Cell[,] map, float seed, List<KeyValuePair<int, float>> cellValues){
			if (cellValues.Count > 0) {
				Random pseudoRandom = new(seed.GetHashCode());
				float[] cellValueWeights = cellValues.Select(kv => kv.Value).ToArray();
				for (int x = 0; x < map.GetLength(0); x++)
				{
					for (int y = 0; y < map.GetLength(1); y++)
					{
						if (map[x, y].Overridable)
						{
							// generate a random value from seed
							// pick a value from cellValues based on spreaded weight
							map[x,y].Value = cellValues[Randomize.GetRandomWeightedIndex(cellValueWeights)].Key;
						}
					}
				}
				return map;
			}
			else
			{
				return map;
			}
		}

		public static Cell[,] RandomFill (Cell[,] map, float seed, float fillPercent)
		{
			Random pseudoRandom = new(seed.GetHashCode());
			for (int x = 0; x < map.GetLength(0); x++)
			{
				for (int y = 0; y < map.GetLength(1); y++)
				{
					if (map[x, y].Overridable)
					{
						map[x, y].Value = (pseudoRandom.Next(0, 100) < fillPercent) ? CellConstants.ALMOST_SOLID_TILE : CellConstants.ALMOST_EMPTY_TILE;

						//
					}
				}
			}
			return map;
		}
	}

	public static class CellAlgorithms
	{
		public static float GenerateSeed()
		{
			return DateTime.Now.Millisecond; ;
		}

		/// <summary>
		/// Reads an existing 2D array map then randomly fills its remaining unprocessed tiles
		/// </summary>
		/// <param name="map">The 2d array to be processed </param>
		/// <param name="seed">Used to randomize filling the map</param>
		/// <param name="randomFillPercent">Chance to Fill with A solid tile</param>
		/// <returns></returns>
		public static int[,] RandomFill(int[,] map, float seed, float randomFillPercent, int tilesToRandomFill = CellConstants.UNPROCESSED_TERRAIN_TILE)
		{
			Random pseudoRandom = new(seed.GetHashCode());
			for (int x = 0; x < map.GetLength(0); x++)
			{
				for (int y = 0; y < map.GetLength(1); y++)
				{
					if (map[x, y] == tilesToRandomFill)
					{
						map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? CellConstants.ALMOST_SOLID_TILE : CellConstants.ALMOST_EMPTY_TILE;
					}
				}
			}
			return map;
		}

		public static int[,] SmoothMap(int[,] map, int smoothPasses)
		{
			int[,] processedMap = map;
			for (int i = 0; i < smoothPasses; i++)
			{
				processedMap = CellularAutomataSmooth(processedMap);
			}
			return processedMap;
		}

		static int[,] CellularAutomataSmooth(int[,] map)
		{
			int[,] processedMap = map;
			const int countToBeWall = 4;

			for (int x = 0; x < processedMap.GetLength(0); x++)
			{
				for (int y = 0; y < processedMap.GetLength(1); y++)
				{
					if (CheckForValidProcessableTile(processedMap[x, y]))
					{
						int neighbourWallTiles = GetSurroundingWallCount(processedMap, x, y);

						if (neighbourWallTiles > countToBeWall)
						{
							processedMap[x, y] = CellConstants.ALMOST_SOLID_TILE;
						}
						else if (neighbourWallTiles < countToBeWall)
						{
							processedMap[x, y] = CellConstants.ALMOST_EMPTY_TILE;
						}
					}
				}
			}
			return processedMap;
		}

		public static int[] ValidSolidTiles()
		{
			return new int[] { CellConstants.SOLID_TILE, CellConstants.ALMOST_SOLID_TILE };
		}

		public static int[] ValidEmptyTiles(bool includeSafeTile = false)
		{
			if (includeSafeTile)
				return new int[] { CellConstants.EMPTY_TILE, CellConstants.ALMOST_EMPTY_TILE, CellConstants.HALLWAY_TILE, CellConstants.SAFE_TILE };
			else
				return new int[] { CellConstants.EMPTY_TILE, CellConstants.ALMOST_EMPTY_TILE, CellConstants.HALLWAY_TILE };
		}

		public static bool CheckForValidSolidWallTile(int value)
		{
			foreach (int v in ValidSolidTiles())
			{
				if (v == value)
				{
					return true;
				}
				else
					continue;
			}
			return false;
		}

		public static bool CheckForValidEmptyWallTile(int value, bool includeSafeTile = false)
		{
			foreach (int v in ValidEmptyTiles())
			{
				if (v == value)
					return true;
			}

			if (includeSafeTile && value == CellConstants.SAFE_TILE)
			{
				return true;
			}
			return false;
		}

		public static bool CheckForValidSpawnerTile(int value)
		{
			return value == CellConstants.SPAWNER_TILE;
		}

		static bool CheckForValidProcessableTile(int value)
		{
			return value == CellConstants.ALMOST_EMPTY_TILE || value == CellConstants.ALMOST_SOLID_TILE;
		}

		public static bool IsInMapRange(int width, int height, int x, int y)
		{
			return x > 0 && x < width && y > 0 && y < height;
		}

		public static bool IsInMapRange(int[,] map, int x, int y)
		{
			return x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1);
		}

		static int GetSurroundingWallCount(int[,] map, int gridX, int gridY)
		{
			int wallCount = 0;
			for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
			{
				for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
				{
					if (IsInMapRange(map, neighbourX, neighbourY))
					{
						if (neighbourX != gridX || neighbourY != gridY)
						{
							if (CheckForValidSolidWallTile(map[neighbourX, neighbourY]))
							{
								wallCount += 1;
							}
						}
					}
					else
					{
						wallCount++;
					}
				}
			}
			return wallCount;
		}

		static int GetSurroundingEmptyWallCount(int[,] map, int gridX, int gridY)
		{
			int wallCount = 0;
			for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
			{
				for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
				{
					if (IsInMapRange(map, neighbourX, neighbourY))
					{
						if (neighbourX != gridX || neighbourY != gridY)
						{
							if (CheckForValidEmptyWallTile(map[neighbourX, neighbourY], true))
							{
								wallCount += 1;
							}
						}
					}
					else
					{
						wallCount++;
					}
				}
			}
			return wallCount;
		}


		#region

		#endregion
	}
}