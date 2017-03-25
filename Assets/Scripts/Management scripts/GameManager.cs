using UnityEngine;
using System.IO;
using System.Collections;
using System.Xml.Linq;
using System.Linq;
using sys = System;

namespace Completed
{
    using System.Collections.Generic;       
    using UnityEngine.UI;                   

    public class GameManager : MonoBehaviour
    {

		public int timeLeft = 4320; // 30 days * 24 hours * 6 10-minute periods

        public float levelStartDelay = 2f;                      
        public float turnDelay = 0.1f;                          
        public int playerGoldPoints = 100;
		public bool isWerewolf = false;
        public static GameManager instance = null;              
        [HideInInspector]
        public bool playersTurn = true;     
		public bool enemyClicked = false;


        private Text levelText, actionText;                                 
        private GameObject levelImage;                        
        private BoardManager boardScript;   
        public int level = 1;                                  
        private List<Enemy> enemies;
		public Player player;
        private bool enemiesMoving;                             
        private bool doingSetup = true;     
                            


        /// <summary>
        /// Pre-Start initialization
        /// </summary>
        void Awake()
        {
            if (instance == null)

                instance = this;
            else if (instance != this)

                Destroy(gameObject);

            //DontDestroyOnLoad(gameObject);

            enemies = new List<Enemy>();
			player = GameObject.Find("Player").GetComponent<Player>();

            boardScript = GetComponent<BoardManager>();

            InitGame();
        }

        /*void OnLevelWasLoaded(int index)
        {
            level++;
            InitGame();
        }*/

		/// <summary>
		/// Displays day, begins board setup
		/// </summary>
        void InitGame()
        {
            //Retrieve encounters
            string[] files = null;

			files = Directory.GetFiles(Directory.GetCurrentDirectory());
            foreach (string fileName in files)
            {
                Debug.Log(fileName);
				string[] name = fileName.Split('\\');
				if(name[name.Length-1] == "save.xml" && !dataSlave.instance.newGame){

					player.deserialize(dataSlave.instance.playerSave);
				}
            }
			

            doingSetup = true;

            levelImage = GameObject.Find("LevelImage");

            levelText = GameObject.Find("LevelText").GetComponent<Text>();
			actionText = GameObject.Find("ActionText").GetComponent<Text>();


            levelText.text = "Day " + level;

            levelImage.SetActive(true);

            Invoke("HideLevelImage", levelStartDelay);

            enemies.Clear();

			if(dataSlave.instance.newGame)
	            boardScript.SetupScene(level);
			else{
				//If loading, retrieve and split up map
				Debug.Log(dataSlave.instance.areas[boardScript.area.ToString()]);
				List<XElement> areaData = (dataSlave.instance.areas[boardScript.area.ToString()]).Elements().ToList();
				string map = areaData[0].Value;
				string[] splitMap = map.Split(';');
				char[,] charMap = new char[splitMap[0].Split(',').Length+1,splitMap.Length];
				int x = 0, y = 0;
				try{
					foreach(string s in splitMap){
						string[] row = s.Split(',');
						y = 0;
						//Hideous, I know.
						if(x < 120){
							foreach(string c in row){
								charMap[y,x] = c.ToCharArray()[0];
								y++;
								if(y > 120)
									break;
							}
						}
						x++;
					}
				}
				catch{
					print(x+" - "+y);
				}
				//Build map
				Tileset.instance.buildMap(charMap);

				boardScript.boardMap = Tileset.instance.boardMap;
				boardScript.tileMap = Tileset.instance.tileMap;

				//Loops through entire board, creating fog
				for (x = -1; x < boardScript.boardMap.GetLength(0) + 1; x++)
				{
					for (y = -1; y < boardScript.boardMap.GetLength(1) + 1; y++)
					{

						//GameObject f = Instantiate(boardScript.fog, new Vector2(x,y), Quaternion.identity) as GameObject;
						//f.transform.SetParent(this.transform);
					}
				}

				XElement enemyElements = areaData[1];
				foreach(XElement e in enemyElements.Elements()){

					GameObject tileChoice = boardScript.enemyTiles[Random.Range(0, boardScript.enemyTiles.Length)];
					GameObject ob = (GameObject)Instantiate(tileChoice, new Vector3(), Quaternion.identity);

					Enemy eScript = ob.GetComponent<Enemy>();
					eScript.deserialize(e);
				}

				XElement entryPoints = areaData[2];
				foreach(XElement e in entryPoints.Elements()){
					string[] strLoc = e.Value.Split(',');
					int[] loc = new int[2];
					loc[0] = sys.Convert.ToInt32(strLoc[0]);
					loc[1] = sys.Convert.ToInt32(strLoc[1]);
					boardScript.updateEntryPoint(e.Name.ToString(),loc);
				}

				//Create exits
				boardScript.BuildExits();
			}

        }


        void HideLevelImage()
        {
            levelImage.SetActive(false);

            doingSetup = false;
        }

        void Update()
        {
			if(Input.GetKeyUp(KeyCode.S)){
				Save();
			}
            if (playersTurn || enemiesMoving || doingSetup)

                return;
			
			StartCoroutine(MoveEnemies());
        }

        public void AddEnemyToList(Enemy script)
        {
            enemies.Add(script);
        }


        public void GameOver()
        {
            levelText.text = "After " + level + " days, you died.";

            levelImage.SetActive(true);

            enabled = false;
        }

		public void Save(){
			XElement area = boardScript.serialize();

			XElement saveDoc = new XElement("save",
				new XElement("curMap",boardScript.area),
				player.serialize(),
				dataSlave.instance.market,
				dataSlave.instance.slums,
				dataSlave.instance.government,
				dataSlave.instance.entertainment,
				dataSlave.instance.manor,
				dataSlave.instance.university,
				dataSlave.instance.temple);
			 
			System.IO.File.WriteAllText(Directory.GetCurrentDirectory()+"/save.xml", saveDoc.ToString());
		}

		public void RemoveEnemyFromList(Enemy script) 
		{
			enemies.Remove (script);

		}


		/// <summary>
		/// Processes enemy turns
		/// </summary>
        IEnumerator MoveEnemies()
        {
			bool init = true;		//On the first rotation, the enemy adds their speed to their action points
			bool moreMoves = true;	//If there is an enemy who can still take an action, enemy turn does not end
            enemiesMoving = true;

            yield return new WaitForSeconds(turnDelay*2);

            if (enemies.Count == 0)
            {
                yield return new WaitForSeconds(turnDelay);
            }
			//int movesLeft = 0; //for debugging
			//As long as an enemy can move, the enemy turn continues
			while(moreMoves){
				moreMoves = false;
				for (int i = 0; i < enemies.Count; i++)
				{
					bool canMove = enemies[i].takeTurn(init);
					//movesLeft += canMove ? 1 : 0;

					moreMoves |= canMove;	//Logical OR, if any enemy returns true, moreMoves will equal true

					
				}
				//Debug.Log("in while " + movesLeft);
				if (moreMoves) { //(enemies.Count > 0)
					yield return new WaitForSeconds(enemies[0].moveTime+.25f);
				}
				init = false;		//Make sure the enemies don't get speed added each time
			}
			//Debug.Log ("after while " + movesLeft);
			
            playersTurn = true;

            enemiesMoving = false;

        }

		/// <summary>
		/// Gets enemy at position pos
		/// </summary>
		/// <returns>The enemy at position pos.</returns>
		/// <param name="pos">Position.</param>
		public GameObject getEnemy(Vector2 pos){
			foreach(Enemy e in enemies){
				Transform t = e.transform;
				if(new Vector2(t.position.x,t.position.y).Equals(pos)){
					return t.gameObject;
				}
			}
			return null;
		}


		/// <summary>
		/// Returns a list of enemies
		/// </summary>
		/// <returns>The enemies.</returns>
		public List<Enemy> getEnemies(){
			return enemies;
		}

		/// <summary>
		/// Adds "string" to the action log
		/// </summary>
		public void print(string s){
			Debug.Log(s);
			actionText.text += s+"\n";
		}


		public void clearLog(){
			actionText.text = "";
		}
    }
}

