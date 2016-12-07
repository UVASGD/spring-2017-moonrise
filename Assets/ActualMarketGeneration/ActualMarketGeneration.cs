﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ActualMarketGeneration {

	public static int gridSizeX = 120;
	public static int gridSizeY = 120;
	public static int bigGridSizeX = 40;
	public static int bigGridSizeY = 40;

	private static GameObject n, b, x, iA, g, a, rA, cA, v, h;

	[HideInInspector]
	public static char[,] grid;
	[HideInInspector]
	public static char[,] bigGrid;

	static int gridTileSize = 5;
	static int bigGridTileSize = 15;
	//x, y, xadd, yadd, xlim, ylim
	static int[,] zoneBounds;

	public static List<Market> marketList = new List<Market>();

	static Market cMarket;

	static List<int[]> roadSpaces = new List<int[]>();

	public static void Start (GameObject _n, GameObject _b, GameObject _x, GameObject _i, GameObject _g, GameObject _a, GameObject _r, GameObject _c, GameObject _v, GameObject _h) {
		n = _n; b = _b; x = _x; iA = _i; g = _g; a = _a; rA = _r; cA = _c; v = _v; h = _h;
		MonoBehaviour.print (Mathf.Sign (1-1));
		grid = new char[gridSizeX, gridSizeY];
		bigGrid = new char[bigGridSizeX, bigGridSizeY];
		zoneBounds = new int[,]{{2, 2, 0, 10, 2, 30}, {2, 30, 10, 0, 30, 30}, {30, 30, 0, -10, 30, 2}, {30, 2, -10, 0, 2, 2}};
	
		for (int r = 0; r < gridSizeX; r ++) {
			for (int c = 0; c < gridSizeY; c++) {
				grid[r,c] = 'n';
				if (r % 3 == 0)
					bigGrid[r/3,c/3] = 'n';
			}
		}

		cMarket = new CenterMarket();

		for (int i = 0; i < zoneBounds.GetLength(0); i++) {
			makeMarkets(2, i);
		}

		foreach (Market m in marketList) {
			m.buildRoads();
		}  

		//randomRoads(10);

		foreach (Market m in marketList) {
			m.checkCrosses();
		}

		for (int i = 0; i < bigGridSizeX; i++) {
			for (int j = 0; j < bigGridSizeY; j++) {
				if (bigGrid[i, j] == 'r') {
					roadSpaces.Add(new int[] {i, j});
				}
				/*for (int[] r : roadSpaces) {
        int c = Random.Range(1, 50));
        if (c == 1) {
          
        }
      }*/
				fillSquares(i, j, bigGrid[i, j]);
			}
		}

		for (int r = 0; r < gridSizeX; r++) {
			for (int c = 0; c < gridSizeY; c++) {
				GameObject currentTile = null;
				switch(grid[r, c]) {
				case 'n': //solid tile
					currentTile = n;
					break;
				case 'b': //market tile
					currentTile = b;
					break;
				case 'x': //solid tile
					currentTile = x;
					//fill(255, 0, 0);
					break;
				case 'i': //solid tile
					currentTile = iA;
					//fill(150, 0, 150);
					break;
				case 'g': //gateway tile/market tile
					currentTile = g;
					break;
				case 'a': //alley/road tile
					currentTile = a;
					break;
				case 'r': //road tile
					currentTile = rA;
					break;
				case 'c': //road tile
					currentTile = cA;
					break;
				case 'v': //solid tile
					currentTile = v;
					//fill(0, 255, 0);
					break;
				case 'h': //solid tile
					currentTile = h;
					//fill(0, 0, 255);
					break;
				default:
					currentTile = b;
					break;
				}
				if (currentTile != null) {
					MonoBehaviour.Instantiate (currentTile, new Vector3 (c, r, 0f), Quaternion.identity);
				}
			}
		}
	}

	static void makeMarkets(int n, int j) {
		//if (j < 6) {
			for (int i = 0; i < n; i++) {
				Market sMarket = new SideMarket (zoneBounds [j, 0] + Random.Range (1, 5), zoneBounds [j, 1] + Random.Range (1, 5), Random.Range (2, 5), Random.Range (2, 5));
				zoneBounds [j, 0] += zoneBounds [j, 2];
				zoneBounds [j, 1] += zoneBounds [j, 3];
			}
		//}
	}

	static void randomRoads(int n) {
		for (int i = 0; i < n; i++) {
			//'i', 'x', 'b', 'g'
			int s = Random.Range(1, 5);
			int randomPos = Random.Range(5, 30);;
			if (s == 1) {
				RoadBuilder r = new RoadBuilder(new int[, ] {{bigGridSizeX-1, randomPos}, {0, randomPos}}, new char[] {'i', 'x', 'b', 'g', 'c'});
			}
			else if (s == 2) {
				RoadBuilder r = new RoadBuilder(new int[, ] {{randomPos, 0}, {randomPos, bigGridSizeY-1}}, new char[] {'i', 'x', 'b', 'g', 'c'});
			}
			else if (s == 3) {
				RoadBuilder r = new RoadBuilder(new int[, ] {{0, randomPos}, {bigGridSizeX-1, randomPos}}, new char[] {'i', 'x', 'b', 'g', 'c'});
			}
			else if (s == 4) {
				RoadBuilder r = new RoadBuilder(new int[, ] {{randomPos, bigGridSizeY-1}, {randomPos, 0}}, new char[] {'i', 'x', 'b', 'g', 'c'});
			}
		}
	}

	static void fillSquares(int x, int y, char t) {
		for (int i = x*3; i < (x*3)+3; i++) {
			for (int j = y*3; j < (y*3)+3; j++) {
				if (grid[i, j] != 'a') grid[i, j] = t;
			}
		}
	}
}