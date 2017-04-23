using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntertainmentGenerator : mapGenerator
{
    public GameObject floor;
    public GameObject wall;
    public GameObject[] buildings;
    public GameObject mainBuilding;

    public bool debugging = false;

    [Range(0, 200)]
    public int gridSize = 120;
    private int cent = 60;
    private char[,] gridKey;
    private int[,] boardMap;

    // Use this for initialization
    public override int[,] init()
    {
        boardMap = new int[gridSize, gridSize];
        gridKey = new char[gridSize, gridSize];

        // Initially set WALL
        for (int h = 0; h < gridSize; h++)
        {
            for (int w = 0; w < gridSize; w++)
            {
                gridKey[h, w] = 'w';
                boardMap[h, w] = 1;
            }
        }

        Builder(new int[,] { {0, cent-3}, {cent, cent+3} }, 'g', 0); //Central road, six tiles wide
        EntertainmentBlockRoads();
        Asylum();
        Casino();
                
        GenerateGridFromKey(gridKey); //generate initial grid (keep at bottom of function)
        return boardMap;
    }

    void Builder(int[,] area, char c, int b) {//Function to insert a homogeneous block of characters
    
        for (int h = area[0, 0]; h < area[1, 0]; h++)
        {
            for (int w = area[0, 1]; w < area[1, 1]; w++)
            {
                gridKey[h, w] = c;
                boardMap[h, w] = b;

            }
        }
    }

    void Insert(char[,] charArr, int[,] boardArr, int[] loc) { //Function to insert commingled block of characters
        for (int h = 0; h < charArr.GetLength(0); h++) {
            for (int w = 0; w < charArr.GetLength(1)-1; w++) {
                char c = charArr[h, w];
                int b = boardArr[h, w];
                gridKey[loc[0] + h, loc[1] + w] = c;
                boardMap[loc[0] + h, loc[1] + w] = b;
            }
        }
    }

    //GENERATION OF DIFFERENT PARTS OF THE MAP

    //GENERATE CRISS CROSS ROADS ON THE SIDE
    void EntertainmentBlockRoads() { 
        int[] loc = new int[] { 9, cent - 3 }; //Starting point is 15 tiles up, 3 tiles to the left of center
        int longo = Random.Range(0, 6);
        int p = 0;
        int l = 50;

        while (loc[0] < 63) //Only generate offshoot roads until 60 tiles up
        {
            if (p == longo)
            {
                l = 57;
            }
            else { l = 50; }

            Builder(new int[,] { { loc[0], loc[1] - l }, { loc[0] + 3, loc[1] } }, 'g', 0); //Build 3 tile wide offshoot from main road

            for (int i = loc[1]-l; i < loc[1]-5; i += Random.Range(3, 8)) //Generate 2 tile wide road going up/down from 3 tile wide offshoot from main road
            {
                Builder(new int[,] { { loc[0], i }, 
                    { loc[0] + (9 * (int)Mathf.Sign(Random.Range(-1, 2))), i + 2 } }, 'g', 0);
            }

            loc[0] += 9; //Offshoots are evenly spaced 9 tiles apart
            p++;
        }
    }

    //INSERT ASYLUM
    void Asylum() {
        char[,] asylumCharArr = new char[,] { };
        int[,] asylumBoardArr = new int[,] { };

        Insert(asylumCharArr, asylumBoardArr, new int[] {cent, cent-30});
    }

    //INSERT CASINO
    void Casino() {
        int pick = Random.Range(0, 2);
        char[,] casinoCharArr = new char[,] { };
        int[,] casinoBoardArr = new int[,] { };
        switch (pick) {
            case 0:
                casinoCharArr = new char[,] { };
                casinoBoardArr = new int[,] { };
                break;
            case 1:
                casinoCharArr = new char[,] { };
                casinoBoardArr = new int[,] { };
                break;
        }


        Insert(casinoCharArr, casinoBoardArr, new int[] { 0, cent });
    }

    //THIS IS ALL THE BULLSHIT THAT CONVERTS A CHARACTER ARRAY TO AN ARRAY OF TILE OBJECTS

    GameObject GetRandomTile(string type)
    {
        if (type == "floor")
        {
            return floor;
        }
        else if (type == "wall")
        {
            return wall;
        }
        else if (type == "door")
        {
            return floor;
        }
        else if (type == "obstruction")
        {
            return wall;
        }
        else
        {
            return wall;
        }
    }

    void GenerateGridFromKey(char[,] key)
    {
        //g = ground
        //w = wall
        //c = center

        //Debugging
        if (debugging)
        {
            for (int h = 0; h < gridSize; h++)
            {
                string ans = "";
                for (int w = 0; w < gridSize; w++)
                {
                    ans += gridKey[w, h] + " ";
                }
                print(ans);
            }
        }

        //Tile Instantiation from Key
        for (int h = 0; h < gridSize; h++)
        {
            for (int w = 0; w < gridSize; w++)
            {
                GameObject currentTile = null;
                if (gridKey[h, w] == 'g')
                {
                    currentTile = floor;
                }
                else if (gridKey[h, w] == 'w')
                {
                    currentTile = wall;
                }
                else if (gridKey[h, w] == '0')
                {
                    currentTile = null;
                }
                if (currentTile != null)
                {
                    Instantiate(currentTile, new Vector3(w, h, 0f), Quaternion.identity);
                }
            }
        }
    }
}
