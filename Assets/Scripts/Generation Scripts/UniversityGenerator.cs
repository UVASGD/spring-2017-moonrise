using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversityGenerator : mapGenerator {

	public GameObject floor;
	public GameObject wall;
	public GameObject[] buildings;
	public GameObject mainBuilding;

	public bool debugging = false;

	private List<Rect> regions = new List<Rect>();

	[Range(0,200)]
	public int gridSize = 40;
	private char[,] gridKey;
	private int radius = 22;

	private int[,] boardMap;

	// Use this for initialization
	public override int[,] init () {
		boardMap = new int[gridSize, gridSize];
		gridKey = new char[gridSize, gridSize];

		//Initialize region list
		for (int i = 0; i < 9; i++) {
			regions.Add (new Rect (0, 0, 0, 0));
		}

		// Initially set ground everywhere except center
		for (int h = 0; h < gridSize; h++) {
			for (int w = 0; w < gridSize; w++) {
				//exclude center circle
				if (w * w + h * h - gridSize * (w + h) + gridSize * gridSize / 2 >= radius * radius) { 
					gridKey [w, h] = 'g';
					boardMap [w, h] = 0;
				} else {
					if (w > 1) {
						gridKey [w - 1, h] = '0';
						boardMap [w-1, h] = 1;
					}
					if (h > 1) {
						gridKey [w, h - 1] = '0';
						boardMap [w, h - 1] = 1;
					}
						gridKey [w, h] = '0';
						boardMap [w, h] = 1;
				}

				if (w == 0 || w == gridSize - 1 || h == 0 || h == gridSize - 1) {
					gridKey [w, h] = 'w';
					boardMap [w, h] = 1;
				}
			}
		}

		for (int i = 0; i < regions.Count; i++) {
			int padding = 5;
			regions [i] = new Rect ((i % 3) * gridSize / 3 + padding, (i / 3) * gridSize / 3 + padding, gridSize / 3 - 2*padding, gridSize / 3 - 2*padding);
		}
			
		regions.RemoveAt (4); //remove region at center

		int numOfBuildings = Random.Range (5, 9);

		for (int i = 0; i < numOfBuildings; i++) {
			int regionIndex = Random.Range (0, regions.Count);
			Rect region = regions [regionIndex];
			regions.RemoveAt (regionIndex);

            if (buildings.Length > 0) {
                GameObject building = buildings[Random.Range(0, buildings.Length)];
                Vector3 position = new Vector3((int)Random.Range(region.xMin, region.xMax - building.GetComponent<BuildingPrefab>().width), (int)Random.Range(region.yMin, region.yMax - building.GetComponent<BuildingPrefab>().height));
                for (int h = 0; h < building.GetComponent<BuildingPrefab>().height - 1; h++)
                {
                    for (int w = 0; w < building.GetComponent<BuildingPrefab>().width - 1; w++)
                    {
                        //print ("[" + (int)(position.x + w) + ", " + (int)(position.y + h) + "]");
                        gridKey[(int)(position.x + w), (int)(position.y + h)] = '0';
						boardMap [(int)(position.x + w), (int)(position.y + h)] = 1;
                    }
                }
                Instantiate(building, position, Quaternion.identity);
            }
		}

		if (mainBuilding != null) {
			Instantiate (mainBuilding, new Vector3 (gridSize / 2, gridSize / 2, 0f), Quaternion.identity);
		}

		GenerateGridFromKey(gridKey); //generate initial grid (keep at bottom of function)
		return boardMap;
	}

	GameObject GetRandomTile(string type) {
		if (type == "floor") {
			return floor;
		} else if (type == "wall") {
			return wall;
		} else if (type == "door") {
			return floor;
		} else if (type == "obstruction") {
			return wall;
		} else {
			return wall;
		}
	}

	void GenerateGridFromKey(char[,] key) {
		//g = ground
		//w = wall
		//c = center

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
				if (gridKey [w, h] == 'g') {
					currentTile = floor;
				} else if (gridKey [w, h] == 'w') {
					currentTile = wall;
				} else if (gridKey [w, h] == '0') {
					currentTile = null;
				}
				if (currentTile != null) {
					Instantiate (currentTile, new Vector3 (w, h, 0f), Quaternion.identity);
				}
			}
		}
	}
}
