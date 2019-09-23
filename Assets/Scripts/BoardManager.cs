﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{

    [Serializable]
    public class Count
    {
        [SerializeField] int minimum;
        [SerializeField] int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }

        public int GetMinimum() { return minimum; }
        public int GetMaximum() { return maximum; }
    }

    [SerializeField] int columns = 8;
    [SerializeField] int rows = 8;
    [SerializeField] Count wallCount = new Count(5, 9);
    [SerializeField] Count foodCount = new Count(5, 9);

    [SerializeField] GameObject exit = null;
    [SerializeField] GameObject[] floorTiles = null;
    [SerializeField] GameObject[] wallTiles = null;
    [SerializeField] GameObject[] foodTiles = null;
    [SerializeField] GameObject[] enemyTiles = null;
    [SerializeField] GameObject[] outerWallTiles = null;

    Transform boardHolder;
    List<Vector3> gridPositions = new List<Vector3>();

    void InitialiseList()
    {
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.GetMinimum(), wallCount.GetMaximum());
        LayoutObjectAtRandom(foodTiles, foodCount.GetMinimum(), foodCount.GetMaximum());
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }

}