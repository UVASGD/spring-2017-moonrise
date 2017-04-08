using UnityEngine;
using System.IO;
using System.Collections;
using System.Xml.Linq;
using System.Linq;
using sys = System;
using System.Text.RegularExpressions;
using System.Xml;

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
		private InventoryManager inventoryScript;
        public int level = 1;                                  
        private List<Enemy> enemies;
		public Player player;
        private bool enemiesMoving;                             
        private bool doingSetup = true;

        private Queue<string> journalQueue;
        private Text CurrencyText;
        private int prevCurrency;
        public int journalSize = 30;
        private JournalManager journal;



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

            journalQueue = new Queue<string>();
            CurrencyText = GameObject.Find("MenuCanvas").transform.FindChild("CurrencyShow").FindChild("Text").GetComponent<Text>();
            CurrencyText.text = "" + playerGoldPoints;

            boardScript = GetComponent<BoardManager>();
			inventoryScript = GetComponent<InventoryManager> ();

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
			StartGame.load();
            //Retrieve encounters
            string[] files = null;

			files = Directory.GetFiles(Directory.GetCurrentDirectory());
            foreach (string fileName in files)
            {
				string[] name = fileName.Split('\\');
				if(name[name.Length-1] == "save.xml" && !dataSlave.instance.newGame){

					player.deserialize(dataSlave.instance.playerSave);
				}
            }
			

            doingSetup = true;

            levelImage = GameObject.Find("LevelImage");

            levelText = GameObject.Find("LevelText").GetComponent<Text>();
			actionText = GameObject.Find("ActionText").GetComponent<Text>();
            journal = GameObject.Find("JournalContainer").transform.FindChild("Text").GetComponent<JournalManager>();


            levelText.text = "Day " + level;

            levelImage.SetActive(true);

            Invoke("HideLevelImage", levelStartDelay);

            enemies.Clear();

			Debug.Log(dataSlave.instance.newGame);
			Debug.Log(boardScript.area.ToString());
			if(dataSlave.instance.newGame || dataSlave.instance.areas[boardScript.area.ToString()] == null)
	            boardScript.SetupScene(level);
			else{
				//If loading, retrieve and split up map
				List<XElement> areaData = (dataSlave.instance.areas[boardScript.area.ToString()]).Elements().ToList();

				string map = areaData[0].Value;
				string[] splitMap = map.Split(';');
				char[,] charMap = new char[splitMap[0].Split(',').Length,splitMap.Length];
				int x = 0, y = 0;
				try{
					foreach(string s in splitMap){
						string[] row = s.Split(',');
						y = 0;
						//Hideous, I know.
						if(x < splitMap[0].Split(',').Length){
							foreach(string c in row){
								charMap[x,y] = c.ToCharArray()[0];
								y++;
								if(y > splitMap.Length)
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
				Debug.Log("buildin");
				Tileset.instance.buildMap(charMap);
				Debug.Log("done building");
				boardScript.boardMap = Tileset.instance.boardMap;
				boardScript.tileMap = Tileset.instance.tileMap;

				//Loops through entire board, creating fog
				for (x = -1; x < boardScript.boardMap.GetLength(0) + 1; x++)
				{
					for (y = -1; y < boardScript.boardMap.GetLength(1) + 1; y++)
					{

						GameObject f = Instantiate(boardScript.fog, new Vector2(x,y), Quaternion.identity) as GameObject;
						f.transform.SetParent(this.transform);
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

				boardScript.boardHolder = new GameObject("Board").transform;
				boardScript.BuildExits();
				boardScript.LayoutGoodies();

				string logOut = "";
				int[,] fixMap = new int[charMap.GetLength(0), charMap.GetLength(1)];
				for(x = 0; x < charMap.GetLength(1); x++){
					for(y = 0; y < charMap.GetLength(0); y++){
						logOut += charMap[y,x]+" ";
						//		fixMap[x,y] = boardMap[y,x];
					}
					logOut += "\n";
				}
				Debug.Log(logOut);
				//boardMap = fixMap;

				if(dataSlave.instance.curLoc.Value != boardScript.reverseAreaLookup[boardScript.area]){
					string fromArea = dataSlave.instance.curLoc.Value;
					int[] pos  = boardScript.entries[boardScript.areaLookup[fromArea]];
					player.transform.position = new Vector3(pos[0],pos[1],0);
				}
			}

        }


        void HideLevelImage()
        {
            levelImage.SetActive(false);

            doingSetup = false;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                instance.timeLeft -= 100;
            }
            if (Input.GetKeyUp(KeyCode.S)){
				Save();
			}
            if (playersTurn || enemiesMoving || doingSetup)

                return;
			
			StartCoroutine(MoveEnemies());
            CurrencyCheck();
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
			
			string writeString = util.CleanInvalidXmlChars(saveDoc.ToString());
			Debug.Log(writeString[240]);

			//Debug.Log("written string - " + writeString);

			System.IO.File.WriteAllText(Directory.GetCurrentDirectory()+"/save.xml", writeString);
		}


		public void RemoveEnemyFromList(Enemy script) 
		{
			enemies.Remove (script);

		}

		/// <s

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
        /// Gets the current game Journal
        /// </summary>
        /// <returns></returns>
        public Queue<string> getJournal()
        {
            Queue<string> retJ = journalQueue;
            return retJ;
        }

        public void CurrencyCheck()
        {
            if (playerGoldPoints != prevCurrency)
            {
                CurrencyText.text = "" + playerGoldPoints;
                prevCurrency = playerGoldPoints;
            }
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
            actionText.text += s + "\n";

            //Should probably be in its own location, but that would take a lot of effort for little reward.
            if (journalQueue.Count == journalSize)
                journalQueue.Dequeue();
            journalQueue.Enqueue("- " + s);
            journal.displayJournal();
        }


		public void clearLog(){
			actionText.text = "";
		}
    }
}
static public class util{
	public static string CleanInvalidXmlChars(string text){
		string re = @"#x0";
		return Regex.Replace(text, re, "");
	}


	public static string fixXML(string text){
		byte[] toEncodeAsBytes
		= System.Text.ASCIIEncoding.ASCII.GetBytes(text);
		string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
		return returnValue;
	}
}