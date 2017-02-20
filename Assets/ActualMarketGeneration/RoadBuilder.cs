using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadBuilder {
	int x;
	int y;
	int[,] bounds;
	public int[] mov = new int[2];
	bool vert = false;
	bool hor = false;
	List<char> avoidList = new List<char>();

	public RoadBuilder(int[,] b, char[] aList) {
		bounds = b;
		x = bounds[0, 0];
		y = bounds[0, 1];
		mov[0] = (bounds [1, 0] - bounds [0, 0] == 0) ? 0 : (int)Mathf.Sign((bounds[1, 0] - bounds[0, 0]));
		mov[1] = (bounds [1, 1] - bounds [0, 1] == 0) ? 0 : (int)Mathf.Sign((bounds[1, 1] - bounds[0, 1]));
		if (Mathf.Abs(mov[0]) == 1) {
			vert = true;
			avoidList.Add('v');
		}
		else if (Mathf.Abs(mov[1]) == 1) {
			hor = true;
			avoidList.Add('h');
		}
		foreach (char c in aList) {
			avoidList.Add(c);
		}

		if (canMove(x, y)) {
			ActualMarketGeneration.bigGrid[x, y] = 'r';
			x += mov[0];
			y += mov[1];
		}
		else { return; }

		while (x != bounds[1,0] || y != bounds[1,1]) {
			if (canMove(x, y)) {
				ActualMarketGeneration.bigGrid[x, y] = 'r';
				placeBlocks();
				x += mov[0];
				y += mov[1];
			}
			else { 
				//if (canMove (x, y)) {
					if (ActualMarketGeneration.bigGrid [x, y] == 'i' || ActualMarketGeneration.bigGrid [x, y] == 'c') {
						ActualMarketGeneration.bigGrid [x, y] = 'c';
					}
				//}
				return; 
			}  
		}

		if (canMove(x, y)) {
			ActualMarketGeneration.bigGrid[x, y] = 'r';
			x += mov[0];
			y += mov[1];
		}  
	}

	public void placeBlocks() {
		if (vert) {
			if (y > 0) {
				if (ActualMarketGeneration.bigGrid [x, y - 1] == 'n' || ActualMarketGeneration.bigGrid [x, y - 1] == 'i') {
					ActualMarketGeneration.bigGrid [x, y - 1] = 'v';
				} else if (ActualMarketGeneration.bigGrid [x, y - 1] == 'h') {
					ActualMarketGeneration.bigGrid [x, y - 1] = 'x';
				}
			}
			if (y < ActualMarketGeneration.bigGridSizeY - 1) {
				if (ActualMarketGeneration.bigGrid [x, y + 1] == 'n' || ActualMarketGeneration.bigGrid [x, y + 1] == 'i') {
					ActualMarketGeneration.bigGrid [x, y + 1] = 'v';
				} else if (ActualMarketGeneration.bigGrid [x, y + 1] == 'h') {
					ActualMarketGeneration.bigGrid [x, y + 1] = 'x';
				}
			}
		}
		if (hor) {
			if (x > 0) {
				if (ActualMarketGeneration.bigGrid [x - 1, y] == 'n' || ActualMarketGeneration.bigGrid [x - 1, y] == 'i') {
					ActualMarketGeneration.bigGrid [x - 1, y] = 'h';
				} else if (ActualMarketGeneration.bigGrid [x - 1, y] == 'v') {
					ActualMarketGeneration.bigGrid [x - 1, y] = 'x';
				}
			}
			if (x < ActualMarketGeneration.bigGridSizeX - 1) {
				if (ActualMarketGeneration.bigGrid [x + 1, y] == 'n' || ActualMarketGeneration.bigGrid [x + 1, y] == 'i') {
					ActualMarketGeneration.bigGrid [x + 1, y] = 'h';
				} else if (ActualMarketGeneration.bigGrid [x + 1, y] == 'v') {
					ActualMarketGeneration.bigGrid [x + 1, y] = 'x';
				}
			}
		}
	}

	public bool canMove(int X, int Y) {
		if (!(X >= 0 && X < ActualMarketGeneration.bigGridSizeX && Y >= 0 && Y < ActualMarketGeneration.bigGridSizeY)) {
			return false;
		}
		else if (avoidList.IndexOf (ActualMarketGeneration.bigGrid [X, Y]) != -1) {
			return false;
		}
		return true;
	}

}