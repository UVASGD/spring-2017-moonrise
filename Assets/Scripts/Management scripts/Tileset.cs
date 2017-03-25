using UnityEngine;
using System.Collections;

public class Tileset : MonoBehaviour {

	public GameObject[] northWalls, roofs, walls, floors;
	public char[,] tileMap;
	public int[,] boardMap;

	public static Tileset instance;

	// Use this for initialization
	void Awake () {
		instance = this;
	}

	public void buildMap(char[,] tiles){
		GameObject tileChoice = null;
		tileMap = new char[tiles.GetLength(0),tiles.GetLength(1)-1];
		boardMap = new int[tiles.GetLength(0),tiles.GetLength(1)-1];
		Vector2 offset = new Vector2(0,0);
		Debug.Log(tiles.GetLength(1) + " - " + tiles.GetLength(0));
		for(int y = 0; y < tiles.GetLength(1)-1; y ++){
			for(int x = 0; x < tiles.GetLength(0); x ++){
				offset.x = 0;
				offset.y = 0;
				tileMap[y,x] = tiles[x,y];
				switch(tiles[x,y]){
				case 'n':
					tileChoice = northWalls[Random.Range(0, northWalls.Length)];

					offset.y = 0.5f;
					boardMap[x,y] = 1;
					break;
				case 'r':
					tileChoice = roofs[Random.Range(0, roofs.Length)];

					boardMap[x,y] = 1;
					break;
				case 'w':
					tileChoice = walls[Random.Range(0, walls.Length)];
					if(y > 0 && tiles[x,y-1] == 'n'){
						tileChoice = null;
					}
					boardMap[x,y] = 1;
					break;
				case 'f':
					tileChoice = floors[Random.Range(0, floors.Length)];
					boardMap[x,y] = 0;

					break;
				}

				if(tileChoice != null){
					GameObject tile = (GameObject)Instantiate(tileChoice, new Vector3(x+offset.x,y+offset.y,0),Quaternion.identity);
				}
			}
		}
	}
}
