using UnityEngine;
using System.IO;
using System.Collections;

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

        public int journalSize = 30;


        private Text levelText, actionText;                                 
        private GameObject levelImage;                        
        private BoardManager boardScript;                       
        public int level = 1;                                  
        private List<Enemy> enemies;                          
        private bool enemiesMoving;                             
        private bool doingSetup = true;

        private Queue<string> journalQueue;   
                            


        /// <summary>
        /// Pre-Start initialization
        /// </summary>
        void Awake()
        {
            if (instance == null)

                instance = this;
            else if (instance != this)

                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            enemies = new List<Enemy>();

            journalQueue = new Queue<string>();

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
            }

            doingSetup = true;

            levelImage = GameObject.Find("LevelImage");

            levelText = GameObject.Find("LevelText").GetComponent<Text>();
			actionText = GameObject.Find("ActionText").GetComponent<Text>();


            levelText.text = "Day " + level;

            levelImage.SetActive(true);

            Invoke("HideLevelImage", levelStartDelay);

            enemies.Clear();

            boardScript.SetupScene(level);

        }


        void HideLevelImage()
        {
            levelImage.SetActive(false);

            doingSetup = false;
        }

        void Update()
        {
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

			//As long as an enemy can move, the enemy turn continues
			while(moreMoves){
				moreMoves = false;
				for (int i = 0; i < enemies.Count; i++)
				{
					bool canMove = enemies[i].takeTurn(init);
					
					moreMoves |= canMove;	//Logical OR, if any enemy returns true, moreMoves will equal true
					
					
				}
				if (enemies.Count > 0) {
					yield return new WaitForSeconds(enemies[0].moveTime+0.05f);
				}
				init = false;		//Make sure the enemies don't get speed added each time
			}
			
            playersTurn = true;

            enemiesMoving = false;
        }

		/// <summary>
		/// Gets enemy at position pos
		/// </summary>
		/// <returns>The enemy.</returns>
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

		/// <summary>
		/// Adds "string" to the action log
		/// </summary>
		public void print(string s){
			Debug.Log(s);
			actionText.text += s+"\n";

            //Should probably be in its own location, but that would take a lot of effort for little reward.
            if (journalQueue.Count == journalSize)
                journalQueue.Dequeue();
            journalQueue.Enqueue("- " + s);
        }


		public void clearLog(){
			actionText.text = "";
		}
    }
}

