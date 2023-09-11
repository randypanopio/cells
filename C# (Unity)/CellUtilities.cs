using System;
using System.Collections.Generic;
using System.Linq;

namespace CellGeneration.CellUtilities
{
    public static class CellConstants
    {
        public const int BROKEN_TILE = 69; //something went wrong and the generator for some reason has an unknown tile
        public const int SPAWNER_TILE = -50; // tile read from the tilemap editor, assign an "empty" no sprite tile, is used for the spawner system
        public const int IGNORED_TILE = -40; // Special tiles set in the tilemap editor, generator reads and assigns them as this and is ignored during generation
        public const int SAFE_TILE = -55; // Essentially an empty tile, will be part of any smoothing passes, but cannot be overriden by other tiles
        public const int UNPROCESSED_TERRAIN_TILE = 0; // temporary value assigned from a read tilemap, must be processed further
        public const int EMPTY_TILE = -1; // "dead" value for celular automata, also used in general for final tile processing
        public const int SOLID_TILE = 1; // "alive" value for celular automata, also used in general for final tile processing
        public const int ALMOST_EMPTY_TILE = -2; // "almost dead" value for celular automata
        public const int ALMOST_SOLID_TILE = 2; // "almost alive" value for celular automata

        //additional terrain values
        public const int HALLWAY_TILE = 11;
        //Biome
        public const int UNPROCESSED_BIOME_TILE = -3;
    }


    /// <summary>
    /// Used for Region based logic
    /// </summary>
    public struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    public class CoordSkip
    {
        public int minX = 1;
        public int maxX = 1;
        public int minY = 1;
        public int maxY = 1;

        public CoordSkip(int minX, int minY, int maxX, int maxY)
        {
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }
    }


    // //https://www.youtube.com/watch?v=eVb9kQXvEZM&list=PLFt_AvWsXl0eZgMK_DT5_biRkWXftAOf9&index=6
    // public class Room : IComparable<Room>
    // {
    //     public List<Coord> tiles; // all tiles found on this room
    //     public List<Coord> edgeTiles; // edges of the room
    //     public List<Room> connectedRooms; //see yt @1:30 keep track of each other rooms thats been connected
    //     public int roomSize; //the tile count
    //     public bool isAccessibleFromMainRoom;
    //     public bool isMainRoom;

    //     public Room() { }

    //     public Room(List<Coord> tiles, int[,] map)
    //     {
    //         this.tiles = tiles;
    //         roomSize = tiles.Count;
    //         connectedRooms = new List<Room>();
    //         edgeTiles = new List<Coord>();

    //         // go over the tiles and check their neighbor using the 2d map, if it is adjacent to a wall tile, then that means this tile is the edge
    //         foreach (Coord tile in tiles) //see yt @2:55
    //         {
    //             for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
    //             {
    //                 for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
    //                 {
    //                     if (x == tile.tileX || y == tile.tileY) // check only adjacent and not diagonals
    //                     {
    //                         if (MapGenAlgorithms.IsInMapRange(map, x, y))
    //                         {
    //                             if (MapGenAlgorithms.CheckForValidSolidWallTile(map[x, y])) // check if the neighboring tiles are valid wall tiles
    //                             {
    //                                 edgeTiles.Add(tile); // then that means the tile we were looking at is on the edge
    //                             }
    //                         }
    //                     }
    //                 }
    //             }
    //         }
    //     }

    //     public void SetAccessibleFromMainRoom() //https://www.youtube.com/watch?v=NhMriRLb1fs&list=PLFt_AvWsXl0eZgMK_DT5_biRkWXftAOf9&index=7
    //     {
    //         if (!isAccessibleFromMainRoom)
    //         {
    //             isAccessibleFromMainRoom = true;
    //             foreach (Room connectedRoom in connectedRooms)
    //             {
    //                 connectedRoom.SetAccessibleFromMainRoom();
    //             }
    //         }
    //     }

    //     //see @4:30, this connects regions (rooms) to  each other, but only ones that are immediately accesible to the next room
    //     public static void ConnectRooms(Room roomA, Room roomB)
    //     {
    //         if (roomA.isAccessibleFromMainRoom)
    //         {
    //             roomB.SetAccessibleFromMainRoom();
    //         }
    //         else if (roomB.isAccessibleFromMainRoom)
    //         {
    //             roomA.SetAccessibleFromMainRoom();
    //         }
    //         roomA.connectedRooms.Add(roomB);
    //         roomB.connectedRooms.Add(roomA);
    //     }

    //     public bool IsConnected(Room otherRoom)
    //     {
    //         return connectedRooms.Contains(otherRoom);
    //     }

    //     public int CompareTo(Room otherRoom)
    //     {
    //         return otherRoom.roomSize.CompareTo(roomSize);
    //     }
    // }



    public enum GenerationType
    {
        ReadSmooth = 10, // simply read the map
        ReadSmoothCull = 15, // Read Map and culls any unconnected room to base room
        ReadSmoothCullConnect = 20, //Seb Langues CA bbased dungeon gen
    }

    public enum BiomeGenType
    {
        NoBiomes = 1,
        BSPRoomSimple = 10,
        BSPRoomsSmoothed = 20,
        UseWallGenWithRandomizedSeed = 50,
        FillThenAddRandomPolygons = 100,
    }

    public enum DecoratorGenRules
    {
        SpawnOnAnyValidTile,
        SpawnOnEdgeOnly,
        SpawnOnValidTileAndAvoidEdge
    }
}