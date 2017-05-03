using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralGenerator : mapGenerator {

	public GameObject floor;
	public GameObject wall;
	public GameObject door;

	public bool debugging = false;


	[Range(0,200)]
	public int gridSize = 51;
	private char[,] gridKey;

	private int[,] boardMap;

	// Use this for initialization
	public override int[,] init () {
	//void Start () {
		boardMap = new int[gridSize, gridSize];
		gridKey = new char[gridSize, gridSize];

		//Create circular region
		for (int h = 0; h < gridSize; h++) {
			for (int w = 0; w < gridSize; w++) {
				if (new Vector2 (w - gridSize / 2, h - gridSize / 2).magnitude - gridSize / 2 >= -1f) {
					gridKey [w, h] = 'w';
				} else {
					gridKey [w, h] = 'g';
				}
			}
		}

		//Create doors to each district
		for (int h = 32; h <= 34; h++) {
			gridKey [2, h] = 'd';
			GameObject currentDoor = Instantiate (door, new Vector3 (2, h, 0f), Quaternion.identity) as GameObject;
			door.GetComponent<ExitPos> ().setTarget ("University");
		}
		for (int h = 16; h <= 18; h++) {
			gridKey [2, h] = 'd';
			GameObject currentDoor = Instantiate (door, new Vector3 (2, h, 0f), Quaternion.identity) as GameObject;
			door.GetComponent<ExitPos> ().setTarget ("Slums");
		}
		for (int h = 32; h <= 34; h++) {
			gridKey [48, h] = 'd';
			GameObject currentDoor = Instantiate (door, new Vector3 (48, h, 0f), Quaternion.identity) as GameObject;
			door.GetComponent<ExitPos> ().setTarget ("Manor");
		}
		for (int h = 16; h <= 18; h++) {
			gridKey [48, h] = 'd';
			GameObject currentDoor = Instantiate (door, new Vector3 (48, h, 0f), Quaternion.identity) as GameObject;
			door.GetComponent<ExitPos> ().setTarget ("Government");
		}
		for (int w = 24; w <= 26; w++) {
			gridKey [w, 49] = 'd';
			GameObject currentDoor = Instantiate (door, new Vector3 (w, 49, 0f), Quaternion.identity) as GameObject;
			door.GetComponent<ExitPos> ().setTarget ("Entertainment");
		}
		for (int w = 24; w <= 26; w++) {
			gridKey [w, 1] = 'd';
			GameObject currentDoor = Instantiate (door, new Vector3 (w, 1, 0f), Quaternion.identity) as GameObject;
			door.GetComponent<ExitPos> ().setTarget ("Market");
		}


		GenerateGridFromKey(gridKey); //generate initial grid (keep at bottom of function)
		return boardMap;
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
					boardMap [w, h] = 0;
					currentTile = floor;
				} else if (gridKey [w, h] == 'w') {
					boardMap [w, h] = 1;
					currentTile = wall;
				} else if (gridKey [w, h] == 'd') {
					boardMap [w, h] = 0;
				}
				if (currentTile != null) {
					Instantiate (currentTile, new Vector3 (w, h, 0f), Quaternion.identity);
				}
			}
		}
	}
}
