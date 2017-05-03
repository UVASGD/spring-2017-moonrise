using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Xml.Linq;
using sys = System;

namespace Completed
{
	/// <summary>
	/// Generic enemy class, contains a melee attack and line-of-sight based aggression.
	/// </summary>
	public class Enemy : Character
    {

        public int playerDamage;
		public GameObject indicator;
		public float sightRange;

        private Animator animator;
        private Transform target;
		private Player player;
		private BoardManager board;
		private Vector2 targetLoc;
        private bool skipMove, visible;
		private List<Vector2> path;

		public Sprite looterFront;
		public Sprite looterBack;
		public Sprite looterLeft;
		public Sprite looterRight;


		//Contains enemy's hitbox
		private BoxCollider2D hitbox;

        protected override void Start()
        {
            GameManager.instance.AddEnemyToList(this);

            animator = GetComponent<Animator>();
			hitbox = GetComponent<BoxCollider2D>();
			orientation = Orientation.South;

            target = GameObject.FindGameObjectWithTag("Player").transform;
			player = target.GetComponent<Player>();
			board = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardManager>();

			targetLoc = new Vector2(-1	,-1);
            base.Start();
			path = new List<Vector2>();
			UpdateSprite ();

			currentHP = 20;
        }

		protected override void UpdateSprite()
		{
			Sprite sprite;
			if (orientation == Orientation.North)
				sprite = looterBack;
			else if (orientation == Orientation.East)
				sprite = looterRight;
			else if (orientation == Orientation.South)
				sprite = looterFront;
			else
				sprite = looterLeft;
			this.gameObject.GetComponent<SpriteRenderer> ().sprite = sprite;
		}

		/// <summary>
		/// Reduces enemy's HP when clicked and in range
		/// </summary>
		void OnMouseDown() {
			if (GameManager.instance.playersTurn && GameManager.instance.rangeHighlighted) {
				Debug.Log ("Range Highlighted: " + GameManager.instance.rangeHighlighted); 
				GameManager.instance.clearLog();
				float distance = Mathf.Sqrt (Mathf.Pow (target.position.x - this.transform.position.x, 2) + Mathf.Pow (target.position.y - this.transform.position.y, 2));
				if(!GameManager.instance.isWerewolf){
					// Ranged attack (hoo-man)
					if (distance <= player.Range) {
						GameManager.instance.enemyClicked = true;
							int damage = player.RangedAttack (this);
							if (damage > 0) {
								GameManager.instance.print ("Ranged damage: " + damage + ". HP remaining: " + currentHP);
							} else
								GameManager.instance.print ("You miss!");
					} else {
							GameManager.instance.print("Enemy out of range");
					}
				} else {
					// Melee attack (werewolf who is both were and a wolf)
					if (distance <= 1) {
						GameManager.instance.enemyClicked = true;
						int damage = player.MeleeAttack (this);
						//LoseHp (damage);
						if(damage > 0	){
							GameManager.instance.print ("Melee damage: " + damage);
						}
						else
							GameManager.instance.print ("You miss!");
					} else {
						GameManager.instance.print ("Enemy out of range");
					}
				}
			}
		}

		protected void Update(){
			//checkVisible();
			Color c = this.GetComponent<SpriteRenderer>().color;
			if(c.a > 0 && !visible)
				c.a -= 0.05f;
			else if(c.a < 1 && visible)
				c.a += 0.05f;
			this.GetComponent<SpriteRenderer>().color = c;
		}

		/// <summary>
		///	This is where the enemy logic goes, for the base class should be a basic line of sight = attack processing.
		/// </summary>
        public virtual bool takeTurn(bool init)
        {
			if (player.isTransforming) {
				return false;
			}
			RaycastHit2D hit;
			hitbox.enabled = false;
			hit = Physics2D.Linecast(new Vector2(transform.position.x,transform.position.y),new Vector2(target.position.x,target.position.y), blockingLayer);
			hitbox.enabled = true;
			range = sightRange-1-((player.baseSneak+(player.sneak))/1.00f*(player.sneaking ? 1 : 0)); //David: baseSneak=3, player.sneak is 1 at lvl 1 and 5 at lvl 4. Right now enemy range decreases by 1 per level
			if(hit.transform == target && hit.distance <= range){
				targetLoc = new Vector2(target.position.x,target.position.y);
				if(path.Count == 0)
					GameManager.instance.print("You hear a shout!");
				path = board.findPath(new Vector2((int)transform.position.x,(int)transform.position.y),targetLoc);
				/*for(int i = 0; i < path.Count; i++){
					Vector2 targ = path[i];
					Instantiate(indicator, new Vector2(targ.x,targ.y), Quaternion.identity);
				}*/
				//Debug.Log(targetLoc);
			}

			if(init)
				AP += speed;
				
			if(AP < player.TotalSpeed)
				return false;
				
			
			int xDir = 0;
			int yDir = 0;

			//Attempt to attack target
			float distance = Mathf.Sqrt (Mathf.Pow (target.position.x - this.transform.position.x, 2) + Mathf.Pow (target.position.y - this.transform.position.y, 2));
			if (distance <= this.Range && hit.transform == target) {
				int damage;
				if (distance <= 1) {
					damage = this.MeleeAttack (player);
					if (damage > 0) {
						GameManager.instance.print ("The enemy strikes you for " + damage + " damage!");
					} else {
						GameManager.instance.print ("The enemy tries to attack but misses!");
					}
				} else {
					damage = this.RangedAttack (player);
					if (damage > 0) {
						GameManager.instance.print ("The enemy shoots you for " + damage + " damage!");
					} else {
						GameManager.instance.print ("The enemy tries to attack but misses!");
					}
				}
				//player.LoseHp(playerDamage);
			} else{

				//If cannot attack target, attempt to pursue target
				if(path.Count > 0){
					yDir = 0;
					xDir = 0;
					//Debug.Log(path[0]+" "+path[path.Count-1]);
					while(Mathf.Abs(yDir)+Mathf.Abs(xDir) < float.Epsilon){
						yDir = (int)(path[path.Count-1].y-transform.position.y);
						xDir = (int)(path[path.Count-1].x-transform.position.x);
						
						if(Mathf.Abs(yDir)+Mathf.Abs(xDir) < float.Epsilon){
							path.RemoveAt(path.Count-1);

							if(path.Count == 0){
								Debug.Log("\"Target Lost.\" - "+this.name.ToString());
								break;
							}
						}
					}
					/*if (Mathf.Abs(targetLoc.y - transform.position.y) > float.Epsilon)
						yDir = targetLoc.y > transform.position.y ? 1 : -1;

					else if (Mathf.Abs(targetLoc.x - transform.position.x) > float.Epsilon)
						xDir = targetLoc.x > transform.position.x ? 1 : -1;
					else
						targetLoc = new Vector2();*/ 	
					
				}
				//If no target is known, move randomly
				else{
					int moveType = Mathf.FloorToInt(Random.Range(0,5));
					switch(moveType){
					case 0:
						break;
					case 1:
						xDir = 1;
						break;
					case 2:
						xDir = -1;
						break;
					case 3:
						yDir = 1;
						break;
					case 4:
						yDir = -1;
						break;
					}
				}

				AttemptMove(xDir, yDir);
			}
			AP -= (float)player.TotalSpeed;
			
			//Return true if the enemy can move again
			//Debug.Log("Ap: "+AP+" Player speed: "+ player.TotalSpeed);
			return (AP >= player.TotalSpeed);
        }

		protected override void OnFinishMove ()
		{
			

		}

		private void checkVisible(){
			if(DistToTarget() > 6)
				return;
			RaycastHit2D hit;
			hitbox.enabled = false;
			hit = Physics2D.Linecast(new Vector2(transform.position.x,transform.position.y),new Vector2(target.position.x,target.position.y), blockingLayer);
			hitbox.enabled = true;

			if(hit != null){
				if(hit.distance > 0 && hit.collider.gameObject.tag == "Player"){
					//c.a = 1;
					//this.GetComponent<SpriteRenderer>().color = c;
					visible = true;
				}
				else{
					visible = true;
					//c.a = 0;
					//this.GetComponent<SpriteRenderer>().color = c;
				}
			}
		}


		private float DistToTarget(){
			return Mathf.Pow(Mathf.Pow(transform.position.x-target.position.x,2f)+Mathf.Pow(transform.position.y-target.position.y,2f),0.5f);
		}

		public void isVisible(bool v){
			visible = v;
		}



		protected override void KillObject()
		{
			GameManager.instance.RemoveEnemyFromList (this);
			GameManager.instance.playerGoldPoints += Random.Range(10,30);
			Destroy (gameObject);
		}

		#region serialize
		public XElement serialize(){
			XElement enemyNode = new XElement("enemy",
				new XElement("locationX",this.transform.localPosition.x),
				new XElement("locationY",this.transform.localPosition.y),
				base.serialize());

			return enemyNode;
		}


		public override bool deserialize(XElement s){
			//LocationX, locationY, werewolf status, character object
			List<XElement> info = s.Descendants().ToList<XElement>();
			Vector3 v = new Vector3(0,0,0);
			v.x = (float)sys.Convert.ToDouble(info[0].Value);
			v.y = (float)sys.Convert.ToDouble(info[1].Value);
			this.transform.localPosition = v;
			base.deserialize(new XElement(info[2]));
			return true;
		}
		#endregion

		public new float Range {
			get {
				return this.range;
			}
			set {
				range = value;
			}
		}
		
    }
}
