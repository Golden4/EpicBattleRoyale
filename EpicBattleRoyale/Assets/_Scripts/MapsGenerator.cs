using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapsGenerator
{
    public static void GenerateMaps(int mapSize, ref MapsController.MapInfo[,] maps)
    {
        // Initializing
        maps = new MapsController.MapInfo[mapSize, mapSize];

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                maps[i, j] = new MapsController.MapInfo();
            }
        }

        //Set coords
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                maps[i, j].coord = new Vector2Int(i, j);
            }
        }

        GenerateRoads(mapSize, ref maps);

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                //is city
                if (Random.Range(0, 2) == 0)
                {
                    int houseCount = Random.Range(1, 4);

                    for (int k = -houseCount; k < houseCount; k++)
                    {
                        if (k == 0 && maps[i, j].centerRoad != Direction.None)
                            continue;

                        maps[i, j].houses.Add(new MapsController.HouseInfo(k * 10, (MapsController.HouseType)Random.Range(0, System.Enum.GetNames(typeof(MapsController.HouseType)).Length)));
                    }
                }
            }
        }
    }

    static void GenerateRoads(int mapSize, ref MapsController.MapInfo[,] maps)
    {
        //Go to random directions
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                GoRoadToDirection(new Vector2Int(i, j), (Direction)Random.Range(0, 4), mapSize, ref maps);
            }
        }

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                int roadsCount = Random.Range(2, 4);

                if (maps[i, j].GetRoadsCount() < roadsCount)
                {
                    int randNum = Random.Range(0, 4);

                    for (int k = 0; k < 4; k++)
                    {
                        if (maps[i, j].roads.Contains((Direction)((k + randNum) % 4)))
                            continue;
                        if (GoRoadToDirection(new Vector2Int(i, j), (Direction)((k + randNum) % 4), mapSize, ref maps))

                            if (maps[i, j].GetRoadsCount() >= roadsCount)
                                break;
                    }

                }
            }
        }

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                maps[i, j].ValidateRoads();
            }
        }
    }

    static bool GoRoadToDirection(Vector2Int mapCoords, Direction direction, int mapSize, ref MapsController.MapInfo[,] maps)
    {
        if (mapCoords.x >= 0 && mapCoords.x < mapSize && mapCoords.y >= 0 && mapCoords.y < mapSize)
        {
            Vector2Int coords = mapCoords + MapsController.directions[(int)direction];
            if (coords.x >= 0 && coords.x < mapSize && coords.y >= 0 && coords.y < mapSize)
            {


                Direction[] dir = new Direction[] { Direction.Top, Direction.Bottom, Direction.Left, Direction.Right };
                Direction[] oppositeDir = new Direction[] { Direction.Bottom, Direction.Top, Direction.Right, Direction.Left };

                if (!maps[mapCoords.x, mapCoords.y].roads.Contains(direction) && !maps[coords.x, coords.y].roads.Contains(oppositeDir[(int)direction]) && maps[coords.x, coords.y].GetRoadsCount() < 3)
                {
                    maps[mapCoords.x, mapCoords.y].roads.Add(dir[(int)direction]);
                    maps[coords.x, coords.y].roads.Add(oppositeDir[(int)direction]);
                    return true;
                }
            }
        }

        return false;
    }

    void ShowDirectionsMapsInConsole(int mapSize, ref MapsController.MapInfo[,] maps)
    {
        for (int j = 0; j < mapSize; j++)
        {
            string roads = "";

            for (int i = 0; i < mapSize; i++)
            {
                roads += "   " + string.Format("[{0},{1}]", i, j);

                for (int k = 0; k < maps[i, j].roads.Count; k++)
                {
                    roads += " " + maps[i, j].roads[k].ToString();
                }
            }

            Debug.Log(roads + j);
        }
    }
}
