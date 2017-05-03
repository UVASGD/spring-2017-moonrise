using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GovernmentGenerator : mapGenerator {


	//4 22-wide buildings with 3 8-wide roads and 2 4-wide roads on the ends
	//6 14-high buildings with 5 6-wide roads and 2 3-wide roads on the ends

	//Declare every relevant tile
	public GameObject road;
	public GameObject grass;
	public GameObject sidewalk;
	public GameObject wall;
	public GameObject buildingWall;
	public GameObject buildingFloor;
	public GameObject buildingDoor;
	public GameObject exitDoor;

	//If true, prints grid key characters in console
	public bool debugging = false;

	//Regions is a list of rectangles (max and min x and y coords) wherein building resides
	private List<Rect> regions = new List<Rect>();

	//Keeping range so it looks pretty
	[Range(0,200)]
	public int gridSize = 122; //DO NOT CHANGE THIS, THE CODE IS not SCALABLE AND THE GRID SIZE MUST BE 122
	private char[,] gridKey; //A 2D array of characters that tells the script what tile to print where

	//####################################
	//Board Map not yet implemented
	private int[,] boardMap;

	// Use this for initialization. For testing, use Start (), for implementation, use init () and uncomment return
	public override int[,] init () {
	//public void Start() {
		boardMap = new int[gridSize, gridSize];
		gridKey = new char[gridSize, gridSize];

		//Initialize region list, 
		for (int i = 0; i < 24; i++) {
			regions.Add (new Rect (0, 0, 0, 0));
		}
			
		//initialize outer barrier and floor
		for (int h = 0; h < gridSize; h++) {
			for (int w = 0; w < gridSize; w++) {
				if (w == 0 || w == gridSize - 1 || h == 0 || h == gridSize - 1) {
					gridKey [w, h] = 'w';
				} else {
					gridKey [w, h] = 'r';
				}
			}
		}

		List<Vector2> corners = new List<Vector2> (0);

		//Create building regions
		for (int i = 0; i < regions.Count; i++) {
			int roadWidth = 4;
			int roadHeight = 3;
			regions [i] = new Rect ((i % 4) * (22 + 2 * roadWidth) + roadWidth + 1, (int)(i / 4) * (14 + 2 * roadHeight) + roadHeight + 1, 22, 14);

			print ("[" + (int)(regions[i].x + (regions [i].xMax - regions [i].x) / 2) + ", " + (int)(regions[i].y + (regions [i].yMax - regions [i].y) / 2 - 1) + "]");

			//Add sidewalks around each building plot
			for (int x = 0; x < gridSize; x++) {
				for (int y = 0; y < gridSize; y++) {
					if ((x == (int)regions [i].xMin - 1 && y <= (int)regions [i].yMax && y >= (int)regions [i].yMin - 1) ||
						(x == (int)regions [i].xMax && y <= (int)regions [i].yMax && y >= (int)regions [i].yMin - 1) ||
						(y == (int)regions [i].yMin - 1 && x <= (int)regions [i].xMax && x >= (int)regions [i].xMin - 1) ||
						(y == (int)regions [i].yMax && x <= (int)regions [i].xMax && x >= (int)regions [i].xMin - 1)) {
						gridKey [x, y] = 's';
					}
				}
			}


			//Take generic rectangular region and cut corners to make distinct
			Rect[] paint = PaintBuilding (regions [i]);
			for (int h = (int)regions [i].yMin; h < (int)regions [i].yMax; h++) {
				for (int w = (int)regions [i].xMin; w < (int)regions [i].xMax; w++) {
					bool build = true;
					for (int b = 0; b < paint.Length; b++) {
						if ((w >= paint [b].x && w <= paint [b].xMax && h >= paint [b].y && h <= paint [b].yMax) && paint [b].width != 0) {
							build = false;
						} else {
							gridKey [w, h] = 'g';
						}
					}
					if (build) {
						gridKey [w, h] = 'b';
					}
				}	
			}
			for (int d = 0; d < 4; d++) {
				switch (Random.Range(0,4 + d)) {
				case 0:
					gridKey [(int)(regions[i].x + (regions [i].xMax - regions [i].x) / 2), (int)regions [i].y] = 'd';
					break;
				case 1:
					gridKey [(int)(regions[i].x + (regions [i].xMax - regions [i].x) / 2), (int)regions [i].yMax-1] = 'd';
					break;
				case 2:
					gridKey [(int)regions [i].x, (int)(regions[i].y + (regions [i].yMax - regions [i].y) / 2)] = 'd';
					break;
				case 3:
					gridKey [(int)regions [i].xMax-1, (int)(regions[i].y + (regions [i].yMax - regions [i].y) / 2)] = 'd';
					break;
				default:
					break;
				}
			}
		}

		//List building interior grid spaces
		List<Vector2> buildingInteriors = new List<Vector2>(0);
		for (int h = 1; h < gridKey.GetLength(1)-1; h++) {
			for (int w = 1; w < gridKey.GetLength(0)-1; w++) {
				//It is only interior if it is fully encased in walls and doors
				if ((gridKey [w, h] == 'b' || gridKey [w, h] == 'd') && 
					(gridKey [w - 1, h] == 'b' || gridKey [w - 1, h] == 'd') && 
					(gridKey [w, h - 1] == 'b' || gridKey [w, h - 1] == 'd') && 
					(gridKey [w + 1, h] == 'b' || gridKey [w + 1, h] == 'd') && 
					(gridKey [w, h + 1] == 'b' || gridKey [w, h + 1] == 'd') && 
					(gridKey [w + 1, h + 1] == 'b' || gridKey [w + 1, h + 1] == 'd') && 
					(gridKey [w - 1, h + 1] == 'b' || gridKey [w - 1, h + 1] == 'd') && 
					(gridKey [w - 1, h - 1] == 'b' || gridKey [w - 1, h - 1] == 'd') && 
					(gridKey [w + 1, h - 1] == 'b' || gridKey [w + 1, h - 1] == 'd')) {
					buildingInteriors.Add (new Vector2(w,h));
				}
			}
		}

		//Hollow out building
		for (int i = 0; i < buildingInteriors.Count; i++) {
			gridKey [(int)buildingInteriors[i].x, (int)buildingInteriors[i].y] = 'f';
		}

		//Add Doors
		for (int x = 0; x < gridSize; x += gridSize-1) {
			for (int y = 59; y <= 62; y++) {
				gridKey [x, y] = 'e';
				GameObject currentDoor = Instantiate (exitDoor, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				if (x == 0) {
					currentDoor.GetComponent<ExitPos> ().setTarget ("Manor");
				} else {
					currentDoor.GetComponent<ExitPos> ().setTarget ("Market");
				}

			}
		}
		for (int x = 59; x <= 62; x++) {
			gridKey [x, 0] = 'e';
			GameObject currentDoor = Instantiate (exitDoor, new Vector3 (x, 0f, 0f), Quaternion.identity) as GameObject;
			currentDoor.GetComponent<ExitPos> ().setTarget ("Central");
		}



		GenerateGridFromKey(gridKey); //generate initial grid (keep at bottom of function)
		return boardMap;
	}

	//Painting is the term used to describe creating anywhere from 0 to 4 distinct regions of varying sizes and cutting out parts of the building
	Rect[] PaintBuilding (Rect bounds) {
		Rect[] regions = { Rect.zero, Rect.zero, Rect.zero, Rect.zero };

		for (int i = 0; i < regions.Length; i++) {
			if (Random.Range (0, 2) == 0) {
				regions [i] = new Rect ((i % 2) * 13 + (int)bounds.x + ((i % 2 == 0) ? -1 : 1) * Random.Range (2, 8), (int)(i / 2) * 7 + (int)bounds.y + (((int)(i / 2) == 0) ? -1 : 1) * Random.Range (2, 5), (int)(bounds.width / 2), (int)(bounds.height / 2));
			}
		}

		return regions;
	}

	//NOT YET IMPLEMENTED
	GameObject GetRandomTile(string type) {
		if (type == "road") {
			return road;
		} else if (type == "wall") {
			return wall;
		} else if (type == "door") {
			return road;
		} else if (type == "obstruction") {
			return wall;
		} else {
			return wall;
		}
	}

	void GenerateGridFromKey(char[,] key) {
		//r = road
		//g = grass
		//s = sidewalk
		//w = wall
		//b = building wall
		//f = building floor
		//d = building door
		//e = exit door

		//Debugging
		if (debugging) {
			for (int h = 0; h < gridSize; h++) {
				string ans = "";
				for (int w = 0; w < gridSize; w++) {
					ans += gridKey [w, h] + " ";
				}
				print (ans);
			}
		}

		//Tile Instantiation from Key
		for (int h = 0; h < gridSize; h++) {
			for (int w = 0; w < gridSize; w++) {
				GameObject currentTile = null;
				if (gridKey [w, h] == 'r') {
					boardMap [w, h] = 0;
					currentTile = road;
				} else if (gridKey [w, h] == 'g') {
					boardMap [w, h] = 0;
					currentTile = grass;
				} else if (gridKey [w, h] == 's') {
					boardMap [w, h] = 0;
					currentTile = sidewalk;
				} else if (gridKey [w, h] == 'w') {
					boardMap [w, h] = 1;
					currentTile = wall;
				} else if (gridKey [w, h] == 'b') {
					boardMap [w, h] = 1;
					currentTile = buildingWall;
				} else if (gridKey [w, h] == 'f') {
					boardMap [w, h] = 1;
					currentTile = buildingFloor;
				} else if (gridKey [w, h] == 'd') {
					boardMap [w, h] = 0;
					currentTile = buildingDoor;
				}
				if (currentTile != null) {
					Instantiate (currentTile, new Vector3 (w, h, 0f), Quaternion.identity);
				}
			}
		}
	}
}
