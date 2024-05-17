using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine.TerrainUtils;
using Unity.VisualScripting;

public class NewBehaviourScript : MonoBehaviour
{
    public bool useCellularAutomata;
    [Range(0, 100)] public int initChance;
    [Range(1, 8)] public int birthLimit;
    [Range(1, 8)] public int deathLimit;
    [Range(0, 10)] public int numGrowthIteration;

    [Range(0, 100)] public int numMaxHeight;
    [Range(0, 200)] public float scale;

    [Range(0, 500)] public int mapRadius;

    private enum tileType
    {
        None = 0,
        Matched = 1,

        DeepOcean,
        Ocean,
        DeepSea,
        Sea,
        Coast,
        LowLand,
        Land,
        Hill,
        LowMountain,
        Mountain
    };

    public int seed;

    private tileType[,] terrarianMap;
    public Vector2Int tmapsize;

    public Tilemap topMap;
    public Tilemap botMap;

    public Tile Tile;

    private int width;
    private int height;

    tileType getTileTypeFromHeight(int height)
    {
        if (valueInRange(height, 0, 10))
            return tileType.DeepOcean;

        if (valueInRange(height, 10, 20))
            return tileType.Ocean;

        if (valueInRange(height, 20, 30))
            return tileType.DeepSea;

        if (valueInRange(height, 30, 40))
            return tileType.Sea;

        if (valueInRange(height, 40, 50))
            return tileType.Coast;

        if (valueInRange(height, 50, 60))
            return tileType.LowLand;

        if (valueInRange(height, 60, 70))
            return tileType.Land;

        if (valueInRange(height, 70, 80))
            return tileType.Hill;

        if (valueInRange(height, 80, 90))
            return tileType.LowMountain;

        if (valueInRange(height, 90, 101))
            return tileType.Mountain;

        return tileType.None;
    }

    Color getColorFromTileType(tileType status)
    {
        switch(status)
        {
            case (tileType.DeepOcean):
                return new Color(0.12f, 0.29f, 0.4f);

            case (tileType.Ocean):
                return new Color(0.18f, 0.39f, 0.55f);

            case (tileType.DeepSea):
                return new Color(0.21f, 0.48f, 0.71f);

            case (tileType.Sea):
                return new Color(0.21f, 0.6f, 0.71f);

            case (tileType.Coast):
                return new Color(0.21f, 0.76f, 0.8f);

            case (tileType.LowLand):
                return new Color(0.32f, 0.67f, 0.27f);

            case (tileType.Land):
                return new Color(0.57f, 0.78f, 0.24f);

            case (tileType.Hill):
                return new Color(0.82f, 0.79f, 0.15f);

            case (tileType.LowMountain):
                return new Color(0.64f, 0.65f, 0.66f);

            case (tileType.Mountain):
                return new Color(1f, 1f, 1f);

            case (tileType.None):
                return new Color(0, 0, 0);
        }
        return new Color(0, 0, 0);
    }

    void groundGenType()
    {
        if (terrarianMap == null)
            terrarianMap = new tileType[width, height];

        tileType[,] newMap = new tileType[width, height];
        newMap = terrarianMap;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int height = getIdHeightingPerlin(x, y, seed);
                newMap[x, y] = getTileTypeFromHeight(height);
            }
        }
    }

    int getIdHeightingPerlin(int x, int y, int seed)
    {
        float raw_perlin = Mathf.PerlinNoise((x+seed) / scale, (y+seed) / scale);
 
        int scaled_perlin = Mathf.CeilToInt(raw_perlin * numMaxHeight);
        return scaled_perlin;
    }

    bool valueInRange(int value, int min, int max)
    {
        if (value >= min && value <= max)
            return true;
        return false;
    }

    bool CheckPointInCircle(int x, int y, int centerX, int centerY, int radius)
    {
        double distance = System.Math.Sqrt(System.Math.Pow(x - centerX, 2) + System.Math.Pow(y - centerY, 2));

        return distance <= radius;
    }

    void drawMap()
    {
        clearMap(false);

        if (terrarianMap == null)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!CheckPointInCircle(x, y, height/2, width/2, mapRadius))
                        continue;

                Color color = getColorFromTileType(terrarianMap[x, y]);

                if (terrarianMap[x, y] > tileType.Coast)
                {
                    Tile.color = color;
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2), Tile);
                }
                else
                {
                    Tile.color = color;
                    botMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2), Tile);
                }
            }
        }
    }

    void clearMap(bool complete)
    {
        topMap.ClearAllTiles();
        botMap.ClearAllTiles();

        if (complete)
            terrarianMap = null;
    }

    // Update is called once per frame
    void Update()
    {
        width = tmapsize.x;
        height = tmapsize.y;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            clearMap(true);
            //groundGenInitPoints();
            drawMap();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            //groundGenWidth();
            drawMap();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            groundGenType();
            drawMap();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            clearMap(true);
        }
    }
}
