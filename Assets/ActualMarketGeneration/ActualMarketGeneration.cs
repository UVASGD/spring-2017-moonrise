using UnityEngine;
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
	[HideInInspector]
	public static char[,] tileMap;

	static int gridTileSize = 5;
	static int bigGridTileSize = 15;
	//x, y, xadd, yadd, xlim, ylim
	static int[,] zoneBounds;

	public static List<Market> marketList = new List<Market>();

	static Market cMarket;

	static List<int[]> roadSpaces = new List<int[]>();

	public static int[,] Start (GameObject _n, GameObject _b, GameObject _x, GameObject _i, GameObject _g, GameObject _a, GameObject _r, GameObject _c, GameObject _v, GameObject _h, GameObject _tBuilding1, GameObject _tBuilding2) {
		n = _n; b = _b; x = _x; iA = _i; g = _g; a = _a; rA = _r; cA = _c; v = _v; h = _h;
		GameObject tBuilding1 = _tBuilding1, tBuilding2 = _tBuilding2;

		MonoBehaviour.print (Mathf.Sign (1-1));
		grid = new char[gridSizeX, gridSizeY];
		bigGrid = new char[bigGridSizeX, bigGridSizeY];
		zoneBounds = new int[,]{{2, 2, 0, 10, 2, 30}, {2, 30, 10, 0, 30, 30}, {30, 30, 0, -10, 30, 2}, {30, 2, -10, 0, 2, 2}};

		//array map for pathfinding, later
		int[,] boardMap = new int[gridSizeX, gridSizeY];
		tileMap = new char[gridSizeX, gridSizeY];
	
		for (int r = 0; r < gridSizeX; r ++) {
			for (int c = 0; c < gridSizeY; c++) {
				boardMap[c,r] = 1;
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
		GameObject[,] tiles = new GameObject[gridSizeX,gridSizeY];
		for (int r = 0; r < gridSizeX; r++) {
			for (int c = 0; c < gridSizeY; c++) {
				GameObject currentTile = null;
				Vector2 offset = new Vector2(0,0);
				switch(grid[r, c]) {
				case 'n': //solid tile
					currentTile = n;
					boardMap[c,r] = 1;
					tileMap[r,c] = 'w';
					break;
				case 'b': //market tile
					currentTile = b;
					boardMap[c,r] = 0;
					tileMap[r,c] = 'f';
					break;
				case 'x': //solid tile
					currentTile = x;
					boardMap[c,r] = 1;
					tileMap[r,c] = 'w';
					//fill(255, 0, 0);
					break;
				case 'i': //outer alley tile
					currentTile = iA;
					boardMap[c,r] = 0;
					tileMap[r,c] = 'f';
					//fill(150, 0, 150);
					break;
				case 'g': //gateway tile/market tile
					currentTile = g;
					boardMap[c,r] = 0;
					tileMap[r,c] = 'f';
					break;
				case 'a': //alley/road tile
					currentTile = a;
					boardMap[c,r] = 0;
					tileMap[r,c] = 'f';
					break;
				case 'r': //road tile
					currentTile = rA;
					boardMap[c,r] = 0;
					tileMap[r,c] = 'f';
					break;
				case 'c': //road tile
					currentTile = cA;
					boardMap[c,r] = 0;
					tileMap[r,c] = 'f';
					break;
				case 'v': //Vertical walls
					currentTile = v;
					boardMap[c,r] = 1;
					tileMap[r,c] = 'w';
					//fill(0, 255, 0);
					break;
				case 'h': //Horizontal walls
					if(r > 0 && ("barc").Contains(grid[r-1,c].ToString())){
						float rand = Random.Range(8,10);
						//Occasionally select a building instead, removing extraneous tiles if necessary
						if(rand < 8)
							currentTile = h;
						else{
							if(rand == 8)
								currentTile = tBuilding1;
							else
								currentTile = tBuilding2;
							
							if(r > 0)
								GameObject.Destroy(tiles[r+1,c]);

							offset = new Vector2(0f,0.5f);
							tileMap[r,c] = 'n';
						}
					}else{
						currentTile = h;
						tileMap[r,c] = 'w';
					}
					boardMap[c,r] = 1;
					//fill(0, 0, 255);
					break;
				default:
					currentTile = b;
					boardMap[c,r] = 0;
					tileMap[r,c] = 'f';
					break;
				}
				if (currentTile != null) {
					tiles[r,c] = (GameObject)MonoBehaviour.Instantiate (currentTile, new Vector3 (c+offset.x, r+offset.y, 0f), Quaternion.identity);
				}
			}
		}
		string logOut = "";
		//int[,] fixMap = new int[120, 120];
		for(int i = 0; i < boardMap.GetLength(1); i++){
			for(int y = 0; y < boardMap.GetLength(0); y++){
				logOut += boardMap[y,i]+" ";
		//		fixMap[i,y] = boardMap[y,i];
			}
			logOut += "\n";
		}
		Debug.Log(logOut);
		//boardMap = fixMap;
		return boardMap;
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
		int randomPos;
		for (int i = 1; i < n; i++) {
			//'i', 'x', 'b', 'g'
			int s = Random.Range(1, 5);
			randomPos = Random.Range(5, 30);
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
		randomPos = Random.Range(5, 30);
		RoadBuilder road = new RoadBuilder(new int[, ] {{randomPos, 0}, {randomPos, bigGridSizeX-1}}, new char[] {'i', 'x', 'b', 'g', 'c'});
	}

	static void fillSquares(int x, int y, char t) {
		for (int i = x*3; i < (x*3)+3; i++) {
			for (int j = y*3; j < (y*3)+3; j++) {
				if (grid[i, j] != 'a') grid[i, j] = t;
			}
		}
	}

}
