using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerator2 : mapGenerator {
	//NOTE: I highly recommend putting all of the prefabs into another class that is referenced by every generation class (ex. this and BoardManager)
	public GameObject floor; //floor prefab
	public GameObject wall; //wall prefab
	public GameObject water; //water prefab
	public GameObject dock; //dock prefab
	public GameObject boat; //boat prefab
	public GameObject exitDoor; //exit door

	public int radius = 21; //a third of the total width of the square
	[Range(0.0001f, 1f)]
	public float speed; //how quickly each step is performed (when in a coroutine for demonstration purposes)
	public Tile[,] grid; //a 2D array of tiles
	public int[,] boardMap; //int representation of map
	private int[] start = {1, 1}; //start position of maze generator (doesn't matter as long as both numbers are odd and less than radius)
	private Tile[,] largeGrid; //the same 2d array of tiles, but inflated by 3 (each tile becomes a group of 3x3 tiles)
	private Room[] rooms; //array of all of the rooms to be generated after the paths
	[Header("Room Properties")]
	public int minRoomCount = 5; //minimum number of rooms spawned
	public int maxRoomCount = 10; //maximum number of rooms spawned
	public int minRoomSize = 3; //minimum room dimension
	public int maxRoomSize = 10; //maximum room dimension

	// Use this for initialization
	public override int[,] init () {
		boardMap = new int[radius*3, radius*3];
		tileMap = new char[radius*3, radius*3];
		largeGrid = new Tile[radius * 3, radius * 3]; //ensures that the large grid is a 3x inflation of the original grid
		rooms = new Room[Random.Range (minRoomCount, maxRoomCount)]; //creates a randomly sized list of rooms
		for (int i = 0; i < rooms.Length; i++) {
			rooms [i] = new Room (minRoomSize, maxRoomSize); //initializes each room
		}
		Debug.Log("mazegen initialized");
		GeneratePath ();
		//StartCoroutine ("GeneratePath");
		string logOut = "";
		//int[,] fixMap = new int[radius*3, radius*3];
		for(int x = 0; x < boardMap.GetLength(1); x++){
			for(int y = 0; y < boardMap.GetLength(0); y++){
				logOut += boardMap[y,x]+" ";
		//		fixMap[x,y] = boardMap[y,x];
			}
			logOut += "\n";
		}
		Debug.Log(logOut);
		//boardMap = fixMap;
		return boardMap;
	}

	//The entire algorithm. Recommend eventually moving this into several functions
	public int[,] GeneratePath () {
		if (radius % 2 == 0) {
			radius++; //if the user is dumb and makes the radius an even number, makes it odd to ensure the path generator doesn't look weird
		}
		grid = new Tile[radius, radius]; //initializes grid
		for (int h = 0; h < radius; h++) {
			for (int w = 0; w < radius; w++) {
				grid [w, h] = new Tile (); //initialized a tile at every grid space
				grid [w, h].visited = false; //the generator will not try to create a path at a tile if that tile has already been visited

				//Generates a floor in a swiss cheese style (part of the algorithm), each floor tile is surrounded by 8 wall tiles
				if ((h == 0 || h == radius - 1 || w == 0 || w == radius - 1) || !(h % 2 == 1 && w % 2 == 1)) {
					grid [w, h].obj = wall;
					grid [w, h].obj.transform.position = new Vector3 (w, h, 0);
					grid [w, h].passable = 1;
					//Instantiate (wall,new Vector3 (w, h, 0), Quaternion.identity) as GameObject;
				} else {
					grid [w, h].obj = floor;
					grid [w, h].obj.transform.position = new Vector3(w, h, 0);
					//Instantiate (floor,new Vector3 (w, h, 0), Quaternion.identity) as GameObject;
					grid[w,h].passable = 0; //Mark as passable terrain
				}
			}
		}

		List<int[]> stack = new List<int[]> (0); //when a tile has been used, it is put in the stack. this way the algorithm knows how to backtrack when there's nowhere to go
		bool finished = false; //only true when the algorithm is complete

		//setting up start position
		int startX = start [0];
		int startY = start [1];
		int[] position = new int[2];
		position [0] = startX;
		position [1] = startY;

		int index = 0; //how far a branch is from the start point
		stack.Add (position); //adds start position to the stack

		while (!finished) {
			position = new int[2] {position[0], position[1]};
			stack.Insert(index, position); //stack is only used for one branch. when backtracking occurs, any info in the previous tree is irellevant
			List<int> directions = new List<int> (0); //initialized all the directions
			grid [position[0], position[1]].visited = true; //sets tile at current position in grid to visited (prevents path from looping around

			//determines which directions the path can go from the current position
			if (position[0] > 1) {
				if (grid[position[0] - 2, position[1]].visited == false) {
					directions.Add (0);
					//print ("Can go left");
				}
			}
			if (position[0] < grid.GetLength (0) - 2) {
				if (grid[position[0] + 2, position[1]].visited == false) {
					directions.Add (2);
					//print ("Can go right");
				}
			}
			if (position[1] > 1) {
				if (grid[position[0], position[1] - 2].visited == false) {
					directions.Add (3);
					//print ("Can go down");
				}
			}
			if (position[1] < grid.GetLength (1) - 2) {
				if (grid[position[0], position[1] + 2].visited == false) {
					directions.Add (1);
					//print ("Can go up");
				}
			}

			//if the path can continue, then choose a random direction and generate a path between the points, updating the current position. otherwise backtrack until it can move
			if (directions.Count > 0) {
				int direction = directions [Random.Range (0, directions.Count)];
				//print ("Going direction " + direction);
				if (direction == 0) {
					//Destroy (grid [position [0] - 1, position [1]].obj);
					grid [position [0] - 1, position [1]].obj = floor;
					grid [position [0] - 1, position [1]].obj.transform.position = new Vector3(position[0] - 1, position[1], 0f);
					//Instantiate(floor, new Vector3(position[0] - 1, position[1], 0f), Quaternion.identity) as GameObject;
					grid [position[0] - 1, position[1]].passable = 0;
					position[0] -= 2;
				} else if (direction == 1) {
					//Destroy (grid [position [0], position [1] + 1].obj);
					grid [position [0], position [1] + 1].obj = floor;
					grid [position [0], position [1] + 1].obj.transform.position = new Vector3(position[0], position[1] + 1, 0f);
					//grid [position[0], position[1] + 1].obj = Instantiate(floor, new Vector3(position[0], position[1] + 1, 0f), Quaternion.identity) as GameObject;
					grid [position[0], position[1] + 1].passable = 0;
					position[1] += 2;
				} else if (direction == 2) {
					//Destroy (grid [position [0] + 1, position [1]].obj);
					grid [position [0] + 1, position [1]].obj = floor;
					grid [position [0] + 1, position [1]].obj.transform.position = new Vector3(position[0] + 1, position[1], 0f);
					//grid [position[0] + 1, position[1]].obj = Instantiate(floor, new Vector3(position[0] + 1, position[1], 0f), Quaternion.identity) as GameObject;
					grid [position[0] + 1, position[1]].passable = 0;
					position[0] += 2;
				} else if (direction == 3) {
					//Destroy (grid [position [0], position [1] - 1].obj);
					grid [position [0], position [1] - 1].obj = floor;
					grid [position [0], position [1] - 1].obj.transform.position = new Vector3(position[0], position[1] - 1, 0f);
					//grid [position[0], position[1] - 1].obj = Instantiate(floor, new Vector3(position[0], position[1] - 1, 0f), Quaternion.identity) as GameObject;
					grid [position[0], position[1] - 1].passable = 0;
					position[1] -= 2;
				}
				index++;
			} else {
				//if it backtracks back to the beginning without finding a place to go, then the maze is completely generated
				if (index == 0) {
					finished = true;
				} else {
					//update position
					index--;
					position [0] = stack [index] [0];
					position [1] = stack [index] [1];

				}
			}

			//yield return new WaitForSeconds (speed);
		}

		//for each tile of the maze, expand it to a 3x3 set of tiles in the larger grid
		for (int w = 0; w < radius; w++) {
			for (int h = 0; h < radius; h++) {
				largeGrid[w*3,h*3] = new Tile();
				largeGrid[w*3+1,h*3] = new Tile();
				largeGrid[w*3+2,h*3] = new Tile();
				largeGrid[w*3,h*3+1] = new Tile();
				largeGrid[w*3,h*3+2] = new Tile();
				largeGrid[w*3+1,h*3+1] = new Tile();
				largeGrid[w*3+2,h*3+1] = new Tile();
				largeGrid[w*3+2,h*3+2] = new Tile();
				largeGrid[w*3+1,h*3+2] = new Tile();
				if (grid [w, h].obj != null) {
					largeGrid [w * 3, h * 3].obj = Instantiate (grid [w, h].obj, new Vector3 (w * 3, h * 3, 0), Quaternion.identity) as GameObject;
					largeGrid [w * 3, h * 3+1].obj = Instantiate (grid [w, h].obj, new Vector3 (w * 3, h * 3+1, 0), Quaternion.identity) as GameObject;
					largeGrid [w * 3, h * 3+2].obj = Instantiate (grid [w, h].obj, new Vector3 (w * 3, h * 3+2, 0), Quaternion.identity) as GameObject;
					largeGrid [w * 3+1, h * 3].obj = Instantiate (grid [w, h].obj, new Vector3 (w * 3+1, h * 3, 0), Quaternion.identity) as GameObject;
					largeGrid [w * 3+1, h * 3+1].obj = Instantiate (grid [w, h].obj, new Vector3 (w * 3+1, h * 3+1, 0), Quaternion.identity) as GameObject;
					largeGrid [w * 3+1, h * 3+2].obj = Instantiate (grid [w, h].obj, new Vector3 (w * 3+1, h * 3+2, 0), Quaternion.identity) as GameObject;
					largeGrid [w * 3+2, h * 3].obj = Instantiate (grid [w, h].obj, new Vector3 (w * 3+2, h * 3, 0), Quaternion.identity) as GameObject;
					largeGrid [w * 3+2, h * 3+1].obj = Instantiate (grid [w, h].obj, new Vector3 (w * 3+2, h * 3+1, 0), Quaternion.identity) as GameObject;
					largeGrid [w * 3+2, h * 3+2].obj = Instantiate (grid [w, h].obj, new Vector3 (w * 3+2, h * 3+2, 0), Quaternion.identity) as GameObject;
					for(int x = 0; x < 3; x++){
						for(int y = 0; y < 3; y++){
							boardMap[w*3+x,h*3+y] = grid[w,h].passable;
							if(grid[w,h].passable == 0){
								tileMap[w*3+x,h*3+y] = 'f';
							}
							else
								tileMap[w*3+x,h*3+y] = 'w';
						}
					}
					//yield return new WaitForSeconds (speed);
				}
				//destroy the original grid instantiations
				//print("Destroying " + grid[w,h].obj.name);
				//DestroyImmediate (grid [w, h].obj);
			}

		}
		//create a parent object to link all room tiles together and set up origin point
		GameObject[] roomParents = new GameObject[rooms.Length];
		for (int i = 0; i < roomParents.Length; i++) {
			roomParents [i] = new GameObject ("Room");
		}

		//give each room a random position. if that position is intersecting an existing room, try another position. if it can't find one, don't spawn
		//NOTE: This is not the best system. It is inefficient and makes it so it very often will not spawn the max number of rooms. If you have any ideas how to change it go for it
		int iter = 0;
		foreach (Room room in rooms) {
			//print ("Checking room " + iter);
			bool isValid = true;
			bool spawnRoom = true;
			int tries = 0;
			do {
				roomParents[iter].transform.position = new Vector3 (Random.Range(0, radius * 3 - room.size.GetLength(0)), Random.Range(0, radius * 3 - room.size.GetLength(1)));
				for (int i = 0; i < iter; i++) {
					if (roomParents[i].transform.position.x < roomParents[iter].transform.position.x + (float)rooms[iter].size.GetLength(0) &&
						roomParents[i].transform.position.x + (float)rooms[i].size.GetLength(0) > roomParents[iter].transform.position.x &&
						roomParents[i].transform.position.y < roomParents[iter].transform.position.y + (float)rooms[iter].size.GetLength(1) &&
						roomParents[i].transform.position.y + (float)rooms[i].size.GetLength(1) > roomParents[iter].transform.position.y) {
						isValid = false;
					}
					if (roomParents[i].transform.position.y + (float)rooms[i].size.GetLength(1) >= radius * 3 - 15) {
						isValid = false;
					}
				}
				tries++;
				if (tries > 1000) {
					isValid = true;
					spawnRoom = false;
					//print("Could not find a valid location");
				}
			} while (!isValid);
			if (spawnRoom) {
				for (int h = 1; h < room.tiles.GetLength (1); h++) {
					for (int w = 1; w < room.tiles.GetLength (0); w++) {
						room.tiles [w, h] = new Tile ();
						Destroy (largeGrid [(int)roomParents [iter].transform.position.x + w, (int)roomParents [iter].transform.position.y + h].obj);
						/*if (h == 0 || w == 0 || h == room.tiles.GetLength (1) - 1 || w == room.tiles.GetLength (0) - 1) {
							room.tiles [w, h].obj = Instantiate (wall) as GameObject;
						} else {
							room.tiles [w, h].obj = Instantiate (floor) as GameObject;
						}*/
						if (h < radius * 3 - 15) {
							boardMap[w+(int)roomParents[iter].transform.position.x,h+(int)roomParents[iter].transform.position.y] = 0;
							tileMap[w+(int)roomParents[iter].transform.position.x,h+(int)roomParents[iter].transform.position.y] = 'f';
							room.tiles [w, h].obj = Instantiate (floor) as GameObject;
							//yield return new WaitForSeconds (speed);
							room.tiles [w, h].obj.transform.parent = roomParents [iter].transform;
							room.tiles [w, h].obj.transform.localPosition = new Vector3 (w, h, 0);
						} else {
							Destroy (room.tiles [w, h].obj);
						}
					}
				}
			}
			iter++;
		}

		//DOCK GENERATION
		//---------------------------------------------------------------------------------------------------------
		for (int h = radius * 3 - 3; h < radius * 3; h++) {
			for (int w = 0; w < radius * 3; w++) {
				Destroy (largeGrid [w, h].obj);
				Instantiate (floor, new Vector3 (w, h, 0), Quaternion.identity);
				boardMap[w,h] = 0;
				tileMap[h,w] = 'f';
			}
		}
		/*for (int h = radius * 3; h < radius * 3 + 12; h++) {
			for (int w = 0; w < radius * 3; w += 8) {
				Instantiate (dock, new Vector3 (w, h, 0), Quaternion.identity);
				Instantiate (dock, new Vector3 (w + 1, h, 0), Quaternion.identity);
				Instantiate (dock, new Vector3 (w + 2, h, 0), Quaternion.identity);
				if (h == 68) {
					int rand = Random.Range (0, 2);
					print ("Random: " + rand);
					if (rand == 0) {
						Instantiate(boat, new Vector3 (w + 5, h, 0), Quaternion.identity);
					}
				}
				if (w + 3 < radius * 3)
					Instantiate (water, new Vector3 (w + 3, h, 0), Quaternion.identity);
				if (w + 4 < radius * 3)
					Instantiate (water, new Vector3 (w + 4, h, 0), Quaternion.identity);
				if (w + 5 < radius * 3)
					Instantiate (water, new Vector3 (w + 5, h, 0), Quaternion.identity);
				if (w + 6 < radius * 3)
					Instantiate (water, new Vector3 (w + 6, h, 0), Quaternion.identity);
				if (w + 7 < radius * 3)
					Instantiate (water, new Vector3 (w + 7, h, 0), Quaternion.identity);
			}
		}*/

		//Add Doors
		for (int x = 2; x < radius * 3; x += (radius-1) * 3) {
			for (int y = 58; y <= 61; y++) {
				Destroy (grid [x, y].obj);
				grid[x,y].obj = Instantiate (exitDoor, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				if (x == 0) {
					grid[x,y].obj.GetComponent<ExitPos> ().setTarget ("Market");
				} else {
					grid[x,y].obj.GetComponent<ExitPos> ().setTarget ("University");
				}

			}
		}
		for (int x = 58; x <= 61; x++) {
			tileMap[x,2] = 'e';
			grid[x,2].obj = Instantiate (exitDoor, new Vector3 (x, 0f, 0f), Quaternion.identity) as GameObject;
			grid[x,2].obj.GetComponent<ExitPos> ().setTarget ("Central");
		}


		return boardMap;
	}
}

public class Tile {
	public GameObject obj;
	public int[] position;
	public bool visited;
	public int passable;

	public Tile () {
		passable = 1;
		position = new int[2];
		visited = false;
		passable = 0;
		obj = new GameObject ();
	}

	public Tile (GameObject pf, int[] pos, int pass) {
		obj = pf;
		position = pos;
		passable = pass;
	}

	public Tile (GameObject pf, int[] pos) {
		obj = pf;
		position = pos;
		passable = 1;
	}
}

public class Room {
	public int[,] size;
	public Tile[,] tiles;

	public Room (int min, int max) {
		size = new int[Random.Range(min, max),Random.Range(min, max)];
		tiles = new Tile[size.GetLength(0), size.GetLength(1)];
	}
}

