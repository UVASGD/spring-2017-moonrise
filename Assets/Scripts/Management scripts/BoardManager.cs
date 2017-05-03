using UnityEngine;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Completed;

public class BoardManager : MonoBehaviour, SerialOb {
    [Serializable]

    //Class that acts as a range for a number of uses
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }
	//Enum for maps
	public enum areas{
		Market,
		Slums,
		Entertainment,
		Government,
		Temple_NI,
		University,
		Manor
	}

	public Dictionary<string,areas> areaLookup = new Dictionary<string, areas>{
		{"Market", areas.Market},
		{"Slums", areas.Slums},
		{"Government", areas.Government},
		{"Manor", areas.Manor}
	};

	public Dictionary<areas,string> reverseAreaLookup = new Dictionary<areas, string>{
		{areas.Market, "Market"},
		{areas.Slums, "Slums"},
		{areas.Government, "Government"},
		{areas.Manor, "Manor"}
	};

	//References to the map generators
	private MazeGenerator2 slumGen;
	private GenerateMarket marketGen;
	private UniversityGenerator universityGen;
	private GovernmentGenerator governmentGen;
	private ManorGenerator manorGen;
	private Dictionary<areas,mapGenerator> generators = new Dictionary<areas,mapGenerator>();
	public areas area = areas.Market;
	private Dictionary<areas,int[]> startLocs = new Dictionary<areas,int[]>{
		{areas.Market, new int[2]{60,60}},
		{areas.Slums, new int[2]{4,4}},
		{areas.Government, new int[2]{1,50}},
		{areas.Manor, new int[2]{1,50}}
	};

	public Dictionary<areas,int[]> entries = new Dictionary<areas,int[]>{
	};


	[SerializeField]
	private LayerMask blockingLayer;

    public int columns = 100;                       //Columns on the board
    public int rows = 100;                          //Rows on the board
    public Count wallCount = new Count(20, 40 );    //Number of wall tiles
    public Count goldCount = new Count(10, 30);     //Number of gold tiles

    //Prefab slots, to fill in the listed prefab
    //Filled in the editor
    public GameObject door1;                        
    public GameObject player;
	public GameObject fog;

    //Prefab arrays, so the system can randomly select tiles by type for flavor.
    //Filled in the editor
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] goldTiles;
    public GameObject[] enemyTiles;
	public GameObject[] chestTiles;
    public GameObject[] outerWallTiles;

    public GameObject[,] fogTiles;

    
    /// <summary>
    /// 2D array of the board, to be pulled for pathfinding, etc.
    /// Current tiles - 0 = floor, 1 = wall
    /// </summary>
    public int[,] boardMap;
	public char[,] tileMap;

    public Transform boardHolder;                              //Holds the parent transform of the board   
    /// <summary>
    /// Creates a list of grid coordinates, [0,0] to [columns,rows]
    /// </summary>

    /// <summary>
    /// Initializes board with floor tiles and outer wall tiles
    /// </summary>
    void BoardSetup()
    {
        Debug.Log("Starting");
        boardHolder = new GameObject("Board").transform;
		
		slumGen = GetComponent<MazeGenerator2>();
		marketGen = GetComponent<GenerateMarket>();
		universityGen = GetComponent<UniversityGenerator> ();
		governmentGen = GetComponent<GovernmentGenerator> ();
		manorGen = GetComponent<ManorGenerator> ();

		generators.Add(areas.Market, marketGen);
		generators.Add(areas.Slums, slumGen);
		generators.Add (areas.University, universityGen);
		generators.Add (areas.Government, governmentGen);
		generators.Add (areas.Manor, manorGen);

		mapGenerator generator = generators[area];

		boardMap = generator.init();
		tileMap = generator.tileMap;
		if (tileMap == null) {
			tileMap = generator.getTileMap ();
		}

		rows = boardMap.GetLength(0);
		columns = boardMap.GetLength(1);

        fogTiles = new GameObject[columns + 2, rows + 2];

        Vector3 loc = new Vector3(startLocs[area][0],startLocs[area][1],0);
		Debug.Log(loc);
		player.transform.localPosition = loc;


        //Loops through entire board, creating fog
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                
				GameObject f = Instantiate(fog, new Vector2(x,y), Quaternion.identity) as GameObject;
				f.transform.SetParent(this.transform);
                fogTiles[x + 1, y + 1] = f;
            }
        }
    }

    /// <summary>
    /// Grabs a copy of the boardMap for mapping purposes
    /// </summary>
    /// <returns></returns>
    public int[,] getBoard()
    {
        int[,] returnBoard = (int[,])boardMap.Clone(); //To ensure nothing is changed in the array by accident.
        return returnBoard;
    }
    /// <summary>
    /// Grabs all fog tiles for mapping purposes
    /// </summary>
    /// <returns></returns>
    public GameObject[,] getFogTiles()
    {
        GameObject[,] returnBoard = (GameObject[,])fogTiles.Clone();
        return returnBoard;
    }
    /// <summary>
    /// Depending on the area, parses the map and creates exits to other zones
    /// </summary>
    public void BuildExits(){
		int entry = 0;
		switch(area){
		case areas.Market:
			for (int y = 0; y < boardMap.GetLength (1); y++) {
				int[] start = new int[2];
				start [0] = 1;
				if (boardMap [0, y] == 0) {
					entry++;
					if (entry == 1) {
						start [1] = y;
						entries [areas.Slums] = start;
					}
					GameObject ob = (GameObject)Instantiate (door1, new Vector2 (0, y), Quaternion.identity);//Create the floor exit
					((ExitPos)ob.GetComponent<ExitPos> ()).setTarget ("Slums");
				}
			}
			entry = 0;
			for(int y = 0; y < boardMap.GetLength(1); y++){
				int[] start = new int[2];
				start[0] = boardMap.GetLength(0)-2;
				if(boardMap[boardMap.GetLength(0)-1,y] == 0){
					entry++;
					if(entry == 1){
						start[1] = y;
						Debug.Log("made Government entrance");
						entries[areas.Government] = start;
					}
					GameObject ob = (GameObject)Instantiate(door1, new Vector2(boardMap.GetLength(1)-1, y), Quaternion.identity);//Create the floor exit
					((ExitPos)ob.GetComponent<ExitPos>()).setTarget("Government");
				}
			}
			break;
		case areas.Slums:
			for (int y = 0; y < boardMap.GetLength (1); y++) {
				int[] start = new int[2];
				start [0] = 1;
				if (boardMap [0, y] == 0) {
					entry++;
					if (entry == 1) {
						start [1] = y;
						entries [areas.Manor] = start;
					}
					GameObject ob = (GameObject)Instantiate (door1, new Vector2 (0, y), Quaternion.identity);//Create the floor exit
					((ExitPos)ob.GetComponent<ExitPos> ()).setTarget ("Manor");
				}
			}
			entry = 0;
			for(int y = 0; y < boardMap.GetLength(1); y++){
				int[] start = new int[2];
				start [0] = boardMap.GetLength(0)-2;
				if(boardMap[boardMap.GetLength(0)-1,y] == 0){
					entry++;
					if(entry == 1){
						start[1] = y;
						Debug.Log("made Market entrance");
						entries[areas.Market] = start;
					}
					GameObject ob = (GameObject)Instantiate(door1, new Vector2(boardMap.GetLength(1)-1, y), Quaternion.identity);//Create the floor exit
					((ExitPos)ob.GetComponent<ExitPos>()).setTarget("Market");
				}
			}
			break;
		case areas.University:
			break;
		case areas.Government:
			for (int y = 0; y < boardMap.GetLength (1); y++) {
				int[] start = new int[2];
				start [0] = 1;
				if (boardMap [0, y] == 0) {
					entry++;
					if (entry == 1) {
						start [1] = y;
						entries [areas.Market] = start;
					}
					GameObject ob = (GameObject)Instantiate (door1, new Vector2 (0, y), Quaternion.identity);//Create the floor exit
					((ExitPos)ob.GetComponent<ExitPos> ()).setTarget ("Market");
				}
			}
			entry = 0;
			for(int y = 0; y < boardMap.GetLength(1); y++){
				int[] start = new int[2];
				start[0] = boardMap.GetLength(0)-2;
				if(boardMap[boardMap.GetLength(0)-1,y] == 0){
					entry++;
					if(entry == 1){
						start[1] = y;
						entries[areas.Manor] = start;
					}
					GameObject ob = (GameObject)Instantiate(door1, new Vector2(boardMap.GetLength(1)-1, y), Quaternion.identity);//Create the floor exit
					((ExitPos)ob.GetComponent<ExitPos>()).setTarget("Manor");
				}
			}
			break;
		case areas.Manor:
			for (int y = 0; y < boardMap.GetLength (1); y++) {
				int[] start = new int[2];
				start [0] = 1;
				if (boardMap [0, y] == 0) {
					entry++;
					if (entry == 1) {
						start [1] = y;
						entries [areas.Government] = start;
					}
					GameObject ob = (GameObject)Instantiate (door1, new Vector2 (0, y), Quaternion.identity);//Create the floor exit
					((ExitPos)ob.GetComponent<ExitPos> ()).setTarget ("Government");
				}
			}
			entry = 0;
			for(int y = 0; y < boardMap.GetLength(1); y++){
				int[] start = new int[2];
				start[0] = boardMap.GetLength(0)-2;
				if(boardMap[boardMap.GetLength(0)-1,y] == 0){
					entry++;
					if(entry == 1){
						start[1] = y;
						entries[areas.Slums] = start;
					}
					GameObject ob = (GameObject)Instantiate(door1, new Vector2(boardMap.GetLength(1)-1, y), Quaternion.identity);//Create the floor exit
					((ExitPos)ob.GetComponent<ExitPos>()).setTarget("Slums");
				}
			}
			break;
			break;
		}
	}

    /// <summary>
    /// Creates a random position in the grid
    /// </summary>
    /// <returns>A Vector2 representing a location on the board</returns>
    Vector2 RandomPosition()
	{
		bool repeat = true;
		Vector2 randomPosition = Vector2.zero;
		while (repeat) {
			randomPosition = new Vector2 (Random.Range (0, rows-1), Random.Range (0, columns-1));
			//RaycastHit2D hit = Physics2D.Raycast (randomPosition, Vector2.up, 0.1f, blockingLayer);
			Debug.Log(randomPosition);
			if (boardMap[(int)randomPosition.x,(int)randomPosition.y] == 0) {
				repeat = false;
			} else {
				
			}
		
		}
		return randomPosition;
    }


    /// <summary>
    /// Takes an array of tiles, and places a random number of them between minimum and maximum on the board in random locations.
    /// </summary>
    /// <param name="tileArray">Array of tiles to select from</param>
    /// <param name="minimum">Minimum number of tiles to place</param>
    /// <param name="maximum">Maximum number of tiles to place</param>
	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum,int tiletype, bool blocking = false)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector2 randomPosition = RandomPosition();
			while(boardMap[(int)randomPosition.x,(int)randomPosition.y] > 0)
				randomPosition = RandomPosition();
            if (blocking)
                boardMap[(int)randomPosition.x, (int)randomPosition.y] = 1;
            else
                boardMap[(int)randomPosition.x, (int)randomPosition.y] = tiletype;
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
			GameObject ob = (GameObject)Instantiate(tileChoice, randomPosition, Quaternion.identity);
			ob.transform.SetParent(boardHolder.transform);
			ob.name = tileChoice.name;
        }
    }

	/// <summary>
	/// Lays out the encounters.
	/// </summary>
	/// <param name="locations">Possible encounter locations, 2D array [X,2]</param>
	/// <param name="min">Min number of encounters</param>
	/// <param name="max">Max number of encounters</param>
	void layoutEncounters(int[,] locations, int min, int max){
		int encounterNum = Random.Range(min,max);
		List<int> usedLocs = new List<int>();
		for(int i = 0; i < encounterNum; i++){
			int loc = Random.Range(0,locations.GetLength(0));
			while(usedLocs.Contains(loc))
				loc = Random.Range(0,locations.GetLength(0));
			usedLocs.Add(loc);
			GameObject.Find("dataSlave").GetComponent<EncounterManager>().makeEncounter(new Vector2(locations[loc,0],locations[loc,1]));
		}
	}


   public void SetupScene (int level)
    {
        BoardSetup();           //Initialize board with floor/outer wall tiles
        //LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);      //Place wall tiles
        LayoutObjectAtRandom(goldTiles, goldCount.minimum, goldCount.maximum,10);      //Place gold tiles
		int chestCount = 70;
		LayoutObjectAtRandom (chestTiles, chestCount, chestCount,15);
        int enemyCount = 18;//(int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount,0);                   //Place enemies

		BuildExits();

		if(area == areas.Market){
			int head = 0;
			int[,] positions = new int[20,2];

			while(head < 20){
				Vector2 rLoc = RandomPosition();
				positions[head,0] = (int)Math.Floor(rLoc.x);
				positions[head,1] = (int)Math.Floor(rLoc.y);
				head++;
			}

			layoutEncounters(positions,10,20);
		}



		if(!dataSlave.instance.newGame){
			if(dataSlave.instance.curLoc.Value != reverseAreaLookup[area]){
				string fromArea = dataSlave.instance.curLoc.Value;
				int[] pos  = entries[areaLookup[fromArea]];
				player.transform.position = new Vector3(pos[0],pos[1],0);
			}
		}
    }

	public void LayoutGoodies(){
		LayoutObjectAtRandom(goldTiles, goldCount.minimum, goldCount.maximum, 10);      //Place gold tiles
		int chestCount = 7;
		LayoutObjectAtRandom (chestTiles, chestCount, chestCount, 15);
	}


	/// <summary>
	/// Finds a path between startLoc and target locations, using A*. Path is returned in the path list
	/// </summary>
	/// <param name="startLoc">Start location.</param>
	/// <param name="target">Target Location</param>
	public List<Vector2> findPath(Vector2 startLoc, Vector2 target){
		
		List<ASquare> open = new List<ASquare>();
		List<Vector2> closed = new List<Vector2>();
		List<Vector2> path = new List<Vector2>();

		ASquare current = new ASquare((startLoc-target).magnitude,0,startLoc);
		int maxSteps = 0;
		while(!current.loc.Equals(target) && maxSteps < 100){
			maxSteps++;
			Vector2[] moves = getAdjacentMoves(current.loc);
			for(int i = 0; i < 4; i++){
				Vector2 move = moves[i];
				if(!isClosed(move,closed)){
					ASquare newMove = new ASquare((move-target).magnitude,current.pathLength+1,move);
					newMove.setPrevious(current);
					open.Add(newMove);
				}
			}

			open.Sort();
			if (open.Count == 0)	// return empty list if there is no possible path
				return new List<Vector2> ();
			current = open[0];
			open.RemoveAt(0);
			closed.Add(current.loc);
		}


		while(current.getPrevious() != null){
			path.Add(current.loc);
			current = current.getPrevious();
		}


		return path;
	}

	/// <summary>
	/// Gets valid adjacent squares
	/// </summary>
	/// <returns>adjacent squares, in order [N,E,S,W]. Entry is null if not a valid move</returns>
	/// <param name="loc">Starting location</param>
	private Vector2[] getAdjacentMoves(Vector2 loc){
		//diretions
		Vector2[] moves = {new Vector2(-1,-1),new Vector2(-1,-1),new Vector2(-1,-1),new Vector2(-1,-1)};
		int x = (int)loc.x;
		int y = (int)loc.y;
		//Debug.Log(x+","+y);
		//North
		if(y != 0){
			if(boardMap[x,y-1] == 0)
				moves[0] = new Vector2(x,y-1);
		}
		//East
		if(x != columns-1){
			if(boardMap[x+1,y] == 0)
				moves[1] = new Vector2(x+1,y);
		}
		//South
		if(y != rows-1){
			if(boardMap[x,y+1] == 0)
				moves[2] = new Vector2(x,y+1);
		}
		//West
		if(x != 0){
			if(boardMap[x-1,y] == 0)
				moves[3] = new Vector2(x-1,y);
		}

		return moves;
	}

	private bool isClosed(Vector2 loc, List<Vector2> closed){
		if(loc.x == -1)
			return true;
		for(int i = 0; i < closed.Count; i++){
			if(loc.Equals(closed[i]))
				return true;
		}
		return false;
	}

	#region serialization
	public XElement serialize(){
		XElement enemies = new XElement("enemies");
		foreach(Enemy enemy in GameManager.instance.getEnemies()){
			XElement e = enemy.serialize();
			enemies.Add(e);
		}

		XElement entryPoints = new XElement("entries");
		Debug.Log ("keys -" + entries.Keys.Count.ToString());
		foreach(areas areaCode in entries.Keys){
			XElement startLoc = new XElement(areaCode.ToString(), entries[areaCode][0]+","+entries[areaCode][1]);
			entryPoints.Add(startLoc);
		}

		string stringMap = "";
		Debug.Log(tileMap.GetLength(0)+" - "+ tileMap.GetLength(1));
		for(int y = 0; y < tileMap.GetLength(0); y++){
			for(int x = 0; x < tileMap.GetLength(1); x++){
				if(tileMap[x,y] != '\x0'){
					stringMap += tileMap[x,y];
					if(x+1 < tileMap.GetLength(1))
						stringMap += ",";
					else
						stringMap += ";";
				}
			}
		}
		Debug.Log(stringMap);

		XElement tiles = new XElement("tiles", stringMap);

		XElement node = new XElement(area.ToString(),tiles,enemies,entryPoints);

		dataSlave.instance.areas[area.ToString()] = node;
		dataSlave.instance.updateEles();

		return node;
	}


	public bool deserialize(XElement s){
			return true;
	}
	#endregion

	public void updateEntryPoint(string area, int[] point){
		entries[areaLookup[area]] = point;
	}
}


public class ASquare : IComparable<ASquare>{
	public float cost;
	public int pathLength;
	public Vector2 loc;
	private ASquare previous;

	public ASquare(float d, int len, Vector2 l){
		cost = len+d;
		pathLength = len;
		loc = l;
	}

	public void setPrevious(ASquare a){
		previous = a;
	}

	public ASquare getPrevious(){
		return previous;
	}

	public void setCost(int c){
		cost = c;
	}

	public int CompareTo(ASquare compareASquare){
		return this.cost.CompareTo(compareASquare.cost);
	}


}