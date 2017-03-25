using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using UnityEngine.UI;
using System;

namespace Completed
{

	public class Player : Character, SerialOb
	{
		public int wallDamage = 1;
		public int pointsPerGold = 10;
		public float restartLevelDelay = 1f;
		public int baseSneak = 3;
		public float sightRange = 12f;

		// Sprites
		public Sprite werewolfFront;
		public Sprite werewolfBack;
		public Sprite werewolfLeft;
		public Sprite werewolfRight;
		public Sprite humanFront;
		public Sprite humanBack;
		public Sprite humanLeft;
		public Sprite humanRight;
		public Color original;
		public GameObject indicator;

		// Skills
		public int shoot;
		public int sneak;
		public int charm;
		public int dodge;
		public int bite;
		public int rage;
		public int growl;
		public int fortify;

		//Abilities
		public Boolean sneaking = false;

		// Player Stats
		/*
		public double rangedDamage = 1.0; // multipliers
		public double meleeDamage = 1.0;
		public double rangedAccuracy = 0.9; // percentages
		public double meleeAccuracy = 0.9;
		public double rangedBlock = 0.1;
		public double meleeBlock = 0.1;
		*/

		// Displays
		public Text displayText;
		public String timeLeft;
		public String goldText;
		public String hpText;
		public Text actionText;
		public String levelText;

		private BoxCollider2D hitbox;
		//hitbox for the object - used for raycast tests?
		private Animator animator;
		public LayerMask sightBlocks, fogLayer;
		private ArrayList revealed;

		//Healthbar object
		public GameObject hpBar;

		//transformation variables
		private float transformationCounter;
		public Boolean isTransforming;

		// Use this for initialization
		protected override void Start ()
		{

			speed = 1;
			orientation = Orientation.North;
			original = this.gameObject.GetComponent<SpriteRenderer> ().color;
			timeLeft = "Time Left: " + GameManager.instance.timeLeft;
			goldText = "Gold: " + GameManager.instance.playerGoldPoints;
			hpText = "HP: " + this.CurrentHP;
			levelText = "Level: " + GameManager.instance.level;
			UpdateText ();
		
			animator = GetComponent<Animator> ();
			animator.speed = 1;
			animator.enabled = false;
			isTransforming = false;
			transformationCounter = 0;

			hitbox = GetComponent<BoxCollider2D> ();

			this.shoot = 1;
			this.sneak = 1;
			this.charm = 1;
			this.dodge = 1;
			this.bite = 1;
			this.rage = 1;
			this.growl = 1;
			this.fortify = 1;

			//sightBlocks = LayerMask.NameToLayer("BlockingLayer");
			//fogLayer = LayerMask.NameToLayer("Fog");
			revealed = new ArrayList ();

			base.Start ();
			OnFinishMove ();
		}

		private void OnDisable ()
		{
		}

		private void ToggleSneak ()
		{

			if (!sneaking) {
				
				sneaking = true;
				TotalSpeed *= (0.4);
			} else if (sneaking) {
				sneaking = false;
				TotalSpeed /= (0.4);
			}
		}

		public void UpdateText (String message = "")
		{
			levelText = "Level: " + GameManager.instance.level;
			hpText = "HP: " + CurrentHP;
			displayText.text = timeLeft + " | " + goldText + " | " + hpText + " | " + levelText;
			Vector3 scale = hpBar.transform.localScale;
			scale.x = ((float)currentHP / (float)totalHP);
			hpBar.transform.localScale = scale;

			if (message != "") {
				displayText.text += " | " + message;
			}
		}

		protected override void UpdateSprite ()
		{
			Sprite sprite;
			if (GameManager.instance.isWerewolf) {
				if (orientation == Orientation.North)
					sprite = werewolfBack;
				else if (orientation == Orientation.East)
					sprite = werewolfRight;
				else if (orientation == Orientation.South)
					sprite = werewolfFront;
				else
					sprite = werewolfLeft;
			} else {
				if (orientation == Orientation.North)
					sprite = humanBack;
				else if (orientation == Orientation.East)
					sprite = humanRight;
				else if (orientation == Orientation.South)
					sprite = humanFront;
				else
					sprite = humanLeft;
			}
			this.gameObject.GetComponent<SpriteRenderer> ().sprite = sprite;
		}

		// Update is called once per frame
		void Update ()
		{
			if (!GameManager.instance.playersTurn)
				return;
			if (isTransforming) {
				if (Time.time >= transformationCounter) {
					animator.enabled = false;
					isTransforming = false;
					GameObject.FindGameObjectWithTag ("transformbg").GetComponent<Renderer> ().enabled = false;
					GetComponent<SpriteRenderer> ().sortingLayerName = "Units";
					UpdateSprite ();
					GameManager.instance.playersTurn = false;
				}
				return;
			}
			if (Input.GetKeyDown (KeyCode.T)) {
				actionText.text = "";
				switchForm ();
				GameManager.instance.playersTurn = false;
				GameManager.instance.timeLeft--;
				timeLeft = "Time Left: " + GameManager.instance.timeLeft;
				UpdateText ();
				return;
			} else if (Input.GetMouseButtonDown (0)) {
				if (GameManager.instance.enemyClicked) {
					Attack ();
				}
			
			}
				 
				// Upgrade Skills
			else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && (Input.GetKeyDown (KeyCode.Keypad1) || Input.GetKeyDown (KeyCode.Alpha1))) {
				IncreaseSkill (1); //shoot
			} else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && (Input.GetKeyDown (KeyCode.Keypad2) || Input.GetKeyDown (KeyCode.Alpha2))) {
				IncreaseSkill (2); //sneak
			} else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && (Input.GetKeyDown (KeyCode.Keypad3) || Input.GetKeyDown (KeyCode.Alpha3))) {
				IncreaseSkill (3); //charm
			} else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && (Input.GetKeyDown (KeyCode.Keypad4) || Input.GetKeyDown (KeyCode.Alpha4))) {
				IncreaseSkill (4); //dodge	
			} else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && (Input.GetKeyDown (KeyCode.Keypad5) || Input.GetKeyDown (KeyCode.Alpha5))) {
				IncreaseSkill (5); //bite
			} else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && (Input.GetKeyDown (KeyCode.Keypad6) || Input.GetKeyDown (KeyCode.Alpha6))) {
				IncreaseSkill (6); //rage	
			} else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && (Input.GetKeyDown (KeyCode.Keypad7) || Input.GetKeyDown (KeyCode.Alpha7))) {
				IncreaseSkill (7); //growl
			} else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && (Input.GetKeyDown (KeyCode.Keypad8) || Input.GetKeyDown (KeyCode.Alpha8))) {
				IncreaseSkill (8); //fortify
			}

			 // Activate Ability
			else if (Input.GetKeyDown (KeyCode.Keypad1) || Input.GetKeyDown (KeyCode.Alpha1)) {
				//shoot
			} else if (Input.GetKeyDown (KeyCode.Keypad2) || Input.GetKeyDown (KeyCode.Alpha2)) {
				ToggleSneak (); //sneak
				Debug.Log(sneaking ? "sneaky and slow" : "stompy and fast"); 
			} else if (Input.GetKeyDown (KeyCode.Keypad3) || Input.GetKeyDown (KeyCode.Alpha3)) {
				 //charm
			} else if (Input.GetKeyDown (KeyCode.Keypad4) || Input.GetKeyDown (KeyCode.Alpha4)) {
				 //dodge
			} else if (Input.GetKeyDown (KeyCode.Keypad5) || Input.GetKeyDown (KeyCode.Alpha5)) {
				 //bite
			} else if (Input.GetKeyDown (KeyCode.Keypad6) || Input.GetKeyDown (KeyCode.Alpha6)) {
				//rage
			} else if (Input.GetKeyDown (KeyCode.Keypad7) || Input.GetKeyDown (KeyCode.Alpha7)) {
				 //growl
			} else if (Input.GetKeyDown (KeyCode.Keypad8) || Input.GetKeyDown (KeyCode.Alpha8)) {
				//fortify
			} else if (Input.GetKeyDown (KeyCode.H) ) {
				//check speed
				Debug.Log(this.totalSpeed);
			}
				
			int horizontal = 0;
			int vertical = 0;

			horizontal = (int)Input.GetAxisRaw ("Horizontal");
			vertical = (int)Input.GetAxisRaw ("Vertical");
			bool spacebar = Input.GetKeyUp (KeyCode.Space);

			if (horizontal != 0 || vertical != 0 || spacebar) {
				actionText.text = "";
				AttemptMove (horizontal, vertical);
			}
		}

		public void IncreaseSkill (int skill)
		{
			int cost = 100 * (int)Math.Pow (2, GameManager.instance.level - 1);
			if (cost > GameManager.instance.playerGoldPoints) {
				GameManager.instance.print ("You don't have enough gold to level up");
			} else {
				bool upgradedSkill = false;
				switch (skill) {
				case 1:
					if (this.shoot < 5) {
						this.shoot += 1;
						this.rangedDamage *= 1.1;
						this.rangedAccuracy *= 1.05;
						GameManager.instance.print ("Upgraded shoot to level " + this.shoot + "!");
						upgradedSkill = true;
					}
					break;
				case 2:
					if (this.sneak < 5) {
						this.sneak += 1;
						//this.baseSneak += 1;     base sneak stays the same, is the starting value -Bryan
						GameManager.instance.print ("Upgraded sneak to level " + this.sneak + "!");
						upgradedSkill = true;
					}
					break;
				case 3:
					if (this.charm < 5) {
						this.charm += 1;
						// TODO
						GameManager.instance.print ("Upgraded charm to level " + this.charm + "!");
						upgradedSkill = true;
					}
					break;
				case 4:
					if (this.dodge < 5) {
						this.dodge += 1;
						this.rangedBlock += dodge == 4 ? 16.5 : 4.5;
						GameManager.instance.print ("Upgraded dodge to level " + this.dodge + "!");
						upgradedSkill = true;
					}
					break;
				case 5:
					if (this.bite < 5) {
						this.bite += 1;
						this.meleeAccuracy *= 1.1;
						this.meleeDamage *= 1.1;
						GameManager.instance.print ("Upgraded bite to level " + this.bite + "!");
						upgradedSkill = true;
					}
					break;
				case 6:
					if (this.rage < 5) {
						this.rage += 1;
						// TODO
						GameManager.instance.print ("Upgraded rage to level " + this.rage + "!");
						upgradedSkill = true;
					}
					break;
				case 7:
					if (this.growl < 5) {
						this.growl += 1;
						// TODO
						GameManager.instance.print ("Upgraded growl to level " + this.growl + "!");
						upgradedSkill = true;
					}
					break;
				case 8:
					if (this.fortify < 5) {
						this.fortify += 1;
						this.meleeBlock += fortify == 7 ? 16.5 : 4.5;
						GameManager.instance.print ("Upgraded fortify to level " + this.fortify + "!");
						upgradedSkill = true;
					}
					break;
				}
				if (!upgradedSkill) {
					GameManager.instance.print ("You are already at maximum level for that skill.");
				} else {
					GameManager.instance.level += 1;
					int currentHealthLoss = this.TotalHP - this.CurrentHP;

					this.baseHP = (int)(this.baseHP + 20);
					this.TotalHP = (int)(this.TotalHP + 20);

					this.CurrentHP = this.TotalHP - currentHealthLoss;

					AlterGold (-cost);
					EndTurn ();
				}
			}
		}

		protected bool WillHitWall (int xDir, int yDir, out RaycastHit2D hit)
		{
			//Find movement points
			Vector2 start = transform.position;
			Vector2 end = start + new Vector2 (xDir, yDir);

			//BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

			//Check if the move isn't blocked
			//boxCollider.enabled = false;
			hit = Physics2D.Linecast (end, start, blockingLayer);
			//boxCollider.enabled = true;


			if (hit.transform != null) {
				if (hit.transform.tag == "Wall") {
					return true;
				} else {
					return false;
				}
			} else {
				return false;
			}
		}



		protected override void AttemptMove (int xDir, int yDir)
		{
			base.AttemptMove (xDir, yDir);

			RaycastHit2D hit;
			bool canMove = Move (xDir, yDir, out hit);

			bool willHitWall = WillHitWall (xDir, yDir, out hit);

			// Only reset turn if can move
			if (!willHitWall) {
				GameManager.instance.timeLeft--;
				timeLeft = "Time Left: " + GameManager.instance.timeLeft;
				UpdateText ();

				CheckIfGameOver ();

				GameManager.instance.playersTurn = false;
			}

			if (hit.transform != null && !canMove)
				OnCantMove (hit.transform);
		}

		protected void Attack ()
		{
			GameManager.instance.enemyClicked = false;

			EndTurn ();
		}

		protected void EndTurn ()
		{
			GameManager.instance.timeLeft--;
			timeLeft = "Time Left: " + GameManager.instance.timeLeft;
			UpdateText ();

			CheckIfGameOver ();
			GameManager.instance.playersTurn = false;
		}

		//pick up an item. If nothing on square does nothing.
		private void OnTriggerEnter2D (Collider2D other)
		{
			//Debug.Log("#TRIGGERED");
			if (other.tag == "Exit") {
				Invoke ("Restart", restartLevelDelay);
				enabled = false;
			} else if (other.tag == "Item") {
				int chance = UnityEngine.Random.Range (0, 2);
				String message = "";
				switch (other.name) {
				case "Gold1":
					GameManager.instance.print ("Picked up " + pointsPerGold + " gold");
					GameManager.instance.playerGoldPoints += pointsPerGold;
					goldText = "Gold: " + GameManager.instance.playerGoldPoints;
					message = "+" + pointsPerGold + " Gold";
					UpdateText ();
					other.gameObject.SetActive (false);
					break;
				case "HealthPack":
					LoseHp (-20);
					message = "+" + 20 + " Health";
					print (message);
					//UpdateText ();
					other.gameObject.SetActive (false);
					break;
				default:
					GameManager.instance.print ("Picked up " + pointsPerGold + " gold");
					GameManager.instance.playerGoldPoints += pointsPerGold;
					goldText = "Gold: " + GameManager.instance.playerGoldPoints;
					message = "+" + pointsPerGold + " Gold";
					UpdateText ();
					other.gameObject.SetActive (false);
					break;
				}
			}
		}

		private void interact ()
		{

		}

		protected override void OnCantMove (Transform transform)
		{
			Character character = transform.GetComponent<Character> ();
			if (character is Enemy) {
				
			} else if (character is Chest) {
				Chest chest = (Chest)character;
				chest.ObtainItem (this);
			}
		}

		private void Restart ()
		{
			Application.LoadLevel (Application.loadedLevel);
			//SceneManager.LoadScene(SceneManager.GetActiveScene);
		}

		public override void LoseHp (int loss)
		{
			this.CurrentHP -= loss;
			if (this.currentHP < 0) {
				this.currentHP = 0;
			} else if (this.currentHP > 100) {
				this.currentHP = 100;
			}
			/*String message;
			if (loss > 0) {
				message = "-" + loss + " HP";
			} else {
				message = "+" + loss + " HP";
			}*/
			hpText = "HP: " + this.CurrentHP;
			UpdateText ();
			CheckIfGameOver ();
		}

		public void AlterGold (int gain)
		{
			GameManager.instance.playerGoldPoints += gain;
			if (gain < 0) {
				GameManager.instance.print (gain + " gold");
			} else if (gain > 0) {
				GameManager.instance.print ("Picked up " + gain + " gold");
			}
			goldText = "Gold: " + GameManager.instance.playerGoldPoints;
			UpdateText ();
		}

		private void CheckIfGameOver ()
		{
			if (GameManager.instance.timeLeft <= 0 || this.CurrentHP <= 0) {
				GameManager.instance.GameOver ();
			}
		}

		protected override void OnFinishMove ()
		{
			//StartCoroutine(FogCheck());
			FogCheck ();
		}

		//Casts 17 rays, then casts a line along that detected ray to identify which squares are visible.
		public void FogCheck ()
		{
			float angle = 0;
			Vector3 pPos = this.transform.position;
			Vector2 origin = new Vector2 (pPos.x, pPos.y), direction, end;
			RaycastHit2D[] fogHits;
			ArrayList fog = new ArrayList ();
			while (angle <= Math.PI * 2 + 0.01) {//Added small value to account for float point error
				direction = new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle));
				hitbox.enabled = false;
				RaycastHit2D hit;
				hit = Physics2D.Raycast (origin, direction, sightRange, sightBlocks);
				if (hit.distance > 0) {
					end = hit.point;
				} else {
					end = origin + (direction * sightRange);
					//Debug.Log(direction*sightRange);
				}
				//Instantiate(indicator,end,Quaternion.identity);
				fogHits = Physics2D.LinecastAll (origin, end, fogLayer);
				hitbox.enabled = true;
				foreach (RaycastHit2D f in fogHits) {
					f.collider.GetComponent<FogOfWar> ().isVisible (true);
					if (fog.IndexOf (f.collider.GetComponent<FogOfWar> ()) < 0)
						fog.Add (f.collider.GetComponent<FogOfWar> ());
				}
				//Debug.Log(direction);

				angle += (float)(Math.PI / 18f);
				//yield return null;
			}

			//Debug.Log(revealed.Count);
			foreach (FogOfWar f in revealed) {
				int ind = fog.IndexOf (f);
				if (fog.IndexOf (f) < 0) {
					f.isVisible (false);
				}
			}

			revealed = fog;
		}


		//Switchs form (human or werewolf); updates hp and sprite
		private void switchForm ()
		{
			
			GameManager.instance.isWerewolf = !GameManager.instance.isWerewolf;
			if (GameManager.instance.isWerewolf) {
				this.TotalHP *= 2;
				this.CurrentHP *= 2;
				animator.runtimeAnimatorController = Resources.Load ("transform") as RuntimeAnimatorController;
			} else {
				this.TotalHP /= 2;
				this.CurrentHP /= 2;
				animator.runtimeAnimatorController = Resources.Load ("reverse") as RuntimeAnimatorController;
			}
			GetComponent<SpriteRenderer> ().sortingLayerName = "transformation";
			GameObject.FindGameObjectWithTag ("transformbg").GetComponent<Renderer> ().enabled = true;
			isTransforming = true;
			animator.enabled = true;
			transformationCounter = Time.time + 5.25f;
			hpText = "HP: " + this.CurrentHP;
			UpdateText ();
			UpdateSprite ();
		}

		#region serialization

		//Serialization methods
		public override XElement serialize ()
		{
			XElement node = new XElement ("player",
				                new XElement ("locationX", this.transform.localPosition.x),
				                new XElement ("locationY", this.transform.localPosition.y),
				                new XElement ("werewolf", GameManager.instance.isWerewolf),
				                base.serialize ());
			return node;
		}

		public override bool deserialize (XElement s)
		{
			//LocationX, locationY, werewolf status, character object
			List<XElement> info = s.Elements ().ToList<XElement> ();
			Vector3 v = new Vector3 (0, 0, 0);
			v.x = (float)Convert.ToDouble (info [0].Value);
			v.y = (float)Convert.ToDouble (info [1].Value);
			this.transform.localPosition = v;
			GameManager.instance.isWerewolf = Convert.ToBoolean (info [2].Value);
			base.deserialize (new XElement (info [3]));
			return true;
		}

		#endregion
	}

	public enum Orientation
	{
		North,
		East,
		South,
		West
	}
}
