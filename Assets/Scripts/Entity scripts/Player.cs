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

        private int totalTime = 0;

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
		public int lunge;
		public int growl;
		public int fortify;

		// Abilities
		private bool willLunge = false;
		private int lungeCooldown;
		public bool sneaking = false;

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

		// Healthbar object
		public GameObject hpBar;
        //UI element storage
        public GameObject playerUI;
        public GameObject Clock;
        public GameObject skillsContainer;
        public TabManager tabScript;

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
            totalTime = GameManager.instance.timeLeft;
            goldText = "Silver: " + GameManager.instance.playerGoldPoints;
			hpText = "HP: " + this.CurrentHP;
			levelText = "Level: " + GameManager.instance.level;
			UpdateText ();
		
			animator = GetComponent<Animator> ();
			animator.speed = 1;
			animator.enabled = false;
			isTransforming = false;
			transformationCounter = 0;


			//Equip a crossbow
			if(equippedItems.Weapon == null){
				ItemSpace.Weapon w = new ItemSpace.Weapon(ItemSpace.WeaponType.Crossbow,ItemSpace.WeaponWeight.Medium,ItemSpace.WeaponPrefix.None,ItemSpace.WeaponInfix.None,ItemSpace.WeaponSuffix.None);
				equippedItems.Equip(w);
				InventoryManager.instance.RefreshEquippedItems();
			}

			hitbox = GetComponent<BoxCollider2D> ();

//initialize skill levels if they weren't loaded
			if(this.shoot == 0){
        this.shoot = 1;
        this.sneak = 1;
        this.charm = 1;
        this.dodge = 1;
        this.bite = 1;
        this.lunge = 1;
        this.growl = 1;
        this.fortify = 1;

			}
			Debug.Log("setVals");

			this.lungeCooldown = 0;

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
				if (Input.anyKeyDown || Time.time >= transformationCounter) {
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
				EndTurn ();

			} else if (Input.GetMouseButtonDown (0)) {
				if (this.willLunge) {
					Lunge ();
				} else if (GameManager.instance.enemyClicked) {
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
				IncreaseSkill (6); //lunge	
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
				EnableLunge (); //lunge
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

		private void EnableLunge() {
			if (!this.willLunge && this.lungeCooldown < 1) {
				GameManager.instance.print ("Click a tile to lunge to");
				this.willLunge = true;
			} else {
				GameManager.instance.print ("You can't lunge right now");
			}
		}

		private void Lunge() {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			int xDir = (int)Math.Round(ray.origin.x - this.transform.position.x);
			int yDir = (int)Math.Round(ray.origin.y - this.transform.position.y);

			bool canLunge = base.AttemptMove (xDir, yDir);
			if (canLunge) {
				this.willLunge = false;

				bool foundEnemy = false;

				int damage = 0;
				switch (this.lunge) {
				case 1:
					damage = 3;
					this.lungeCooldown = 6;
					break;
				case 2:
					damage = 5;
					this.lungeCooldown = 5;
					break;
				case 3:
					damage = 8;
					this.lungeCooldown = 5;
					break;
				case 4:
					damage = 12;
					this.lungeCooldown = 4;
					break;
				case 5:
					damage = 17;
					this.lungeCooldown = 4;
					break;
				}

				// Attack everyone within one tile
				Collider2D[] enemyColliders = Physics2D.OverlapBoxAll(new Vector2(ray.origin.x, ray.origin.y), new Vector2(2.9f, 2.9f), 0);
				int i = 0;
				while (i < enemyColliders.Length) {
					// If collider found is an enemy, attack for 
					if (enemyColliders[i].tag.Equals("Enemy")) {
						foundEnemy = true;
						enemyColliders [i].gameObject.transform.GetComponent<Character> ().LoseHp (damage);
					}
					i++;
				}

				if (foundEnemy) {
					GameManager.instance.print ("You dealt " + damage + " damage to surrounding enemies");
				}

				EndTurn ();
			} else { // There is something blocking the way
				GameManager.instance.print ("You cannot lunge there.");
			}
		}
			
		private int skillCost (int skill)
		{
			/* 100 base
			 * 1.25x per character level
			 * 2x per skill level
			 * round to nearest 25
			 */
			int skillLevel = 0;
			switch (skill) {
			case 1:
				skillLevel = this.shoot;
				break;
			case 2:
				skillLevel = this.sneak;
				break;
			case 3:
				skillLevel = this.charm;
				break;
			case 4:
				skillLevel = this.dodge;
				break;
			case 5:
				skillLevel = this.bite;
				break;
			case 6:
				skillLevel = this.lunge;
				break;
			case 7:
				skillLevel = this.growl;
				break;
			case 8:
				skillLevel = this.fortify;
				break;
			}
			double rawcost = 100.0 * Math.Pow (1.25, GameManager.instance.level - 1) * Math.Pow (2, skillLevel-1);
			return (int)((Math.Round(rawcost/100 * 4)) / 4 * 100.0);
				//GameManager.instance.print ("You don't have enough silver to level up");
			}
	

		public void IncreaseSkill (int skill)
		{
				bool upgradedSkill = false;
			int cost = skillCost (skill);
            int nextCost = byte.MaxValue; //TODO: Fix this.
			if (cost > GameManager.instance.playerGoldPoints) {
				GameManager.instance.print ("You need " + (skillCost (skill) - GameManager.instance.playerGoldPoints) + " more silver for that! " + GameManager.instance.playerGoldPoints + " "+ cost);
			} else {
				switch (skill) {
				case 1:
					if (this.shoot < 5) {
						this.shoot += 1;
						this.rangedDamage *= 1.1;
						this.rangedAccuracy *= 1.05;
						GameManager.instance.print ("Upgraded shoot to level " + this.shoot + "!");
						upgradedSkill = true;
                        SkillUIUpdate(skill, this.shoot, nextCost);
                        }
					break;
				case 2:
					if (this.sneak < 5) {
						this.sneak += 1;
						//this.baseSneak += 1;     base sneak stays the same, is the starting value -Bryan
						GameManager.instance.print ("Upgraded sneak to level " + this.sneak + "!");
						upgradedSkill = true;
                        SkillUIUpdate(skill, this.sneak, nextCost);
                        }
					break;
				case 3:
					if (this.charm < 5) {
						this.charm += 1;
						// TODO
						GameManager.instance.print ("Upgraded charm to level " + this.charm + "!");
						upgradedSkill = true;
                        SkillUIUpdate(skill, this.charm, nextCost);
                        }
					break;
				case 4:
					if (this.dodge < 5) {
						this.dodge += 1;
						this.rangedBlock += dodge == 4 ? 16.5 : 4.5;
						GameManager.instance.print ("Upgraded dodge to level " + this.dodge + "!");
						upgradedSkill = true;
                        SkillUIUpdate(skill, this.dodge, nextCost);
                        }
					break;
				case 5:
					if (this.bite < 5) {
						this.bite += 1;
						this.meleeAccuracy *= 1.1;
						this.meleeDamage *= 1.1;
						GameManager.instance.print ("Upgraded bite to level " + this.bite + "!");
						upgradedSkill = true;
                        SkillUIUpdate(skill, this.bite, nextCost);
                        }
					break;
				case 6:
					if (this.lunge < 5) {
						this.lunge += 1;
						GameManager.instance.print ("Upgraded lunge to level " + this.lunge + "!");
						upgradedSkill = true;
                        SkillUIUpdate(skill, this.lunge, nextCost);
                        }
					break;
				case 7:
					if (this.growl < 5) {
						this.growl += 1;
						// TODO
						GameManager.instance.print ("Upgraded growl to level " + this.growl + "!");
						upgradedSkill = true;
                        SkillUIUpdate(skill, this.growl, nextCost);
                        }
					break;
				case 8:
					if (this.fortify < 5) {
						this.fortify += 1;
						this.meleeBlock += fortify == 7 ? 16.5 : 4.5;
						GameManager.instance.print ("Upgraded fortify to level " + this.fortify + "!");
						upgradedSkill = true;
                        SkillUIUpdate(skill, this.fortify, nextCost);
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

	
		protected override bool AttemptMove (int xDir, int yDir)
		{
			base.AttemptMove (xDir, yDir);

			RaycastHit2D hit;
			bool canMove = Move (xDir, yDir, out hit);

			bool willHitWall = WillHitWall (xDir, yDir, out hit);

			// Only reset turn if can move
			if (!willHitWall) {
				EndTurn ();
			}

			if (hit.transform != null && !canMove)
				OnCantMove (hit.transform);
			
			return false;
        }

		protected void Attack ()
		{
			GameManager.instance.enemyClicked = false;

			EndTurn ();
		}

		protected void EndTurn ()
		{
			GameManager.instance.timeLeft--;
			this.lungeCooldown--;
			timeLeft = "Time Left: " + GameManager.instance.timeLeft;
			UpdateText ();
            UpdateClock();

			CheckIfGameOver ();
			GameManager.instance.playersTurn = false;
		}

		//pick up an item. If nothing on square does nothing.
		private void OnTriggerEnter2D (Collider2D other)
		{
			//Debug.Log("#TRIGGERED");
            if (other.tag == "Exit")
            {
				Debug.Log("Exit");
				GameManager.instance.Save();
				dataSlave.instance.newGame = false;
				SceneManager.LoadScene(((ExitPos)(other.GetComponent<ExitPos>())).getTarget());
                //Invoke("Restart", restartLevelDelay);
                enabled = false;
            }
            else if (other.tag == "Item")
            {
				int chance = UnityEngine.Random.Range (0, 2);
				String message = "";
				switch (other.name) {
				case "Gold1":
					GameManager.instance.print ("Picked up " + pointsPerGold + " silver");
					GameManager.instance.playerGoldPoints += pointsPerGold;
					goldText = "Silver: " + GameManager.instance.playerGoldPoints;
					message = "+" + pointsPerGold + " Silver";
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
					GameManager.instance.print ("Picked up " + pointsPerGold + " silver");
					GameManager.instance.playerGoldPoints += pointsPerGold;
					goldText = "Silver: " + GameManager.instance.playerGoldPoints;
					message = "+" + pointsPerGold + " Silver";
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
				AddItem (chest.item);
				GameManager.instance.print ("A " + chest.item.Name + " was added to inventory");
				Destroy (chest.gameObject);
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
			} else if (this.currentHP > this.baseHP*(GameManager.instance.isWerewolf?2:1)) {
				this.currentHP = this.baseHP*(GameManager.instance.isWerewolf?2:1);
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
			goldText = "Silver: " + GameManager.instance.playerGoldPoints;
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
            UpdateIndicator();
		}

        /// <summary>
        /// Updates the indicators on the HUD for what state the player is in.
        /// </summary>
        private void UpdateIndicator()
        {
            //This is designed for a text-based interaction. Replacing the FindChild("Text") onward with GetComponent<Image>().sprite and setting the sprite that way
            //TODO Switch this to images
            if (GameManager.instance.isWerewolf)
            {
                skillsContainer.transform.FindChild("FormIndicator").FindChild("Text").GetComponent<Text>().text = "W";

                skillsContainer.transform.FindChild("CharacterSkills").FindChild("Skill1").GetComponent<Image>().sprite = tabScript.skillImages[4];
                skillsContainer.transform.FindChild("CharacterSkills").FindChild("Skill2").GetComponent<Image>().sprite = tabScript.skillImages[5];
                skillsContainer.transform.FindChild("CharacterSkills").FindChild("Skill3").GetComponent<Image>().sprite = tabScript.skillImages[6];
                skillsContainer.transform.FindChild("CharacterSkills").FindChild("Skill4").GetComponent<Image>().sprite = tabScript.skillImages[7];

            }
            else
            {
                skillsContainer.transform.FindChild("FormIndicator").FindChild("Text").GetComponent<Text>().text = "H";

                skillsContainer.transform.FindChild("CharacterSkills").FindChild("Skill1").GetComponent<Image>().sprite = tabScript.skillImages[0];
                skillsContainer.transform.FindChild("CharacterSkills").FindChild("Skill2").GetComponent<Image>().sprite = tabScript.skillImages[1];
                skillsContainer.transform.FindChild("CharacterSkills").FindChild("Skill3").GetComponent<Image>().sprite = tabScript.skillImages[2];
                skillsContainer.transform.FindChild("CharacterSkills").FindChild("Skill4").GetComponent<Image>().sprite = tabScript.skillImages[3];
            }
        }
        /// <summary>
        /// Adjusts the player's skills in the Character tab
        /// </summary>
        /// <param name="skill">The skill as an int. See the levelup method.</param>
        /// <param name="level">What level is the stat going to?</param>
        /// <param name="cost">What is the cost of the stat?</param>
        private void SkillUIUpdate(int skill, int level, int cost)
        {
            GameObject skillBox;
            switch (skill)
            {
                case 1:
                    skillBox = playerUI.transform.FindChild("HumanContainer").FindChild("Skill1").FindChild("Desc").gameObject;
                    skillBox.GetComponent<Text>().text = "Shoot: Level " + level;
                    break;
                case 2:
                    skillBox = playerUI.transform.FindChild("HumanContainer").FindChild("Skill2").FindChild("Desc").gameObject;
                    skillBox.GetComponent<Text>().text = "Sneak: Level " + level;
                    break;
                case 3:
                    skillBox = playerUI.transform.FindChild("HumanContainer").FindChild("Skill3").FindChild("Desc").gameObject;
                    skillBox.GetComponent<Text>().text = "Charm: Level " + level;
                    break;
                case 4:
                    skillBox = playerUI.transform.FindChild("HumanContainer").FindChild("Skill4").FindChild("Desc").gameObject;
                    skillBox.GetComponent<Text>().text = "Dodge: Level " + level;
                    break;
                case 5:
                    skillBox = playerUI.transform.FindChild("WereWolfContainer").FindChild("Skill1").FindChild("Desc").gameObject;
                    skillBox.GetComponent<Text>().text = "Bite: Level " + level;
                    break;
                case 6:
                    skillBox = playerUI.transform.FindChild("WereWolfContainer").FindChild("Skill2").FindChild("Desc").gameObject;
                    skillBox.GetComponent<Text>().text = "Rage: Level " + level;
                    break;
                case 7:
                    skillBox = playerUI.transform.FindChild("WereWolfContainer").FindChild("Skill3").FindChild("Desc").gameObject;
                    skillBox.GetComponent<Text>().text = "Growl: Level " + level;
                    break;
                case 8:
                    skillBox = playerUI.transform.FindChild("WereWolfContainer").FindChild("Skill4").FindChild("Desc").gameObject;
                    skillBox.GetComponent<Text>().text = "Fortify: Level " + level;
                    break;
            }
            GameObject currencyBox = playerUI.transform.FindChild("Currency").gameObject;
            currencyBox.GetComponent<Text>().text = "Cost for Next Level: " + cost;
        }
        /// <summary>
        /// Updates the HUD clock to reflect time left. Works on text at the moment
        /// </summary>
        private void UpdateClock()
        {
            //TODO: Switch this to images
            //Clock if set
            float timeCheck = (float)GameManager.instance.timeLeft / (float)totalTime;
            if (timeCheck > 0.88)
            {
                Clock.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "Full Moon";
            }
            else if (timeCheck <= 0.88 && timeCheck > 0.77)
            {
                Clock.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "Waning Gibbous";
            }
            else if (timeCheck <= 0.77 && timeCheck > 0.66)
            {
                Clock.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "Last Quarter";
            }
            else if (timeCheck <= 0.66 && timeCheck > 0.55)
            {
                Clock.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "Waning Crescent";
            }
            else if (timeCheck <= 0.55 && timeCheck > 0.44)
            {
                Clock.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "New Moon";
            }
            else if (timeCheck <= 0.44 && timeCheck > 0.33)
            {
                Clock.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "Waxing Crescent";
            }
            else if (timeCheck <= 0.33 && timeCheck > 0.22)
            {
                Clock.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "First Quarter";
            }
            else if (timeCheck <= 0.22 && timeCheck > 0.11)
            {
                Clock.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "Waxing Gibbous";
            }
            else if (timeCheck <= 0.11)
            {
                Clock.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "Near Full Moon";
            }
        }

        #region serialization

        //Serialization methods
        public override XElement serialize ()
		{
			XElement node = new XElement ("player",
                new XElement ("locationX", this.transform.localPosition.x),
                new XElement ("locationY", this.transform.localPosition.y),
                new XElement ("werewolf", GameManager.instance.isWerewolf),
                base.serialize (),
				new XElement ("shoot", shoot),
				new XElement ("sneak", sneak),
				new XElement ("charm", charm),
				new XElement ("dodge", dodge),
				new XElement ("bite", bite),
				new XElement ("lunge", lunge),
				new XElement ("growl", growl),
				new XElement ("fortify", fortify),
				new XElement ("level", GameManager.instance.level),
				new XElement ("time", GameManager.instance.timeLeft)


			);

			XElement inventoryNode = new XElement("inventory");
			for(int i = 0; i < this.inventory.Items.Count; i ++){
				XElement item = new XElement("Item");
				if(this.inventory.Items[i].GetType() == typeof(ItemSpace.Weapon))
					item = ((ItemSpace.Weapon)this.inventory.Items[i]).serialize();
				inventoryNode.Add(item);
			}

			node.Add(inventoryNode);

			XElement equipNode = new XElement("equipment");
			if(this.equippedItems.Weapon != null){
				XElement weapon = ((ItemSpace.Weapon)(this.equippedItems.Weapon)).serialize();
				equipNode.Add(weapon);
			}
			if(this.equippedItems.Armor != null){
				XElement armor = new XElement("armor",this.equippedItems.Armor.serialize());
				equipNode.Add(armor);
			}
			node.Add(equipNode);

			node.Add(new XElement("gold", GameManager.instance.playerGoldPoints));
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

			shoot = Convert.ToInt32(info[4].Value);
			sneak = Convert.ToInt32(info[5].Value);
			charm = Convert.ToInt32(info[6].Value);
			dodge = Convert.ToInt32(info[7].Value);
			bite = Convert.ToInt32(info[8].Value);
			lunge = Convert.ToInt32(info[9].Value);
			growl = Convert.ToInt32(info[10].Value);
			fortify = Convert.ToInt32(info[11].Value);
			GameManager.instance.level = Convert.ToInt32(info[12].Value);
			GameManager.instance.timeLeft = Convert.ToInt32(info[13].Value);
			GameManager.instance.playerGoldPoints = Convert.ToInt32(info[16].Value);

			XElement inventoryEle = info[14];
			Debug.Log(info[14].Value);
			foreach(XElement i in inventoryEle.Elements()){
				if(i.Name.ToString().Equals("weapon")){
					ItemSpace.Weapon w = new ItemSpace.Weapon();
					w.deserialize(i);
					inventory.AddItem(w);
				}
			}
			XElement equippedEle = info[15];
			foreach(XElement i in equippedEle.Elements()){
				Debug.Log(i.Name.ToString());
				if(i.Name.ToString().Equals("weapon")){
					ItemSpace.Weapon w = new ItemSpace.Weapon();
					w.deserialize(i);
					Debug.Log(w.Name);
					equippedItems.Equip(w);
					InventoryManager.instance.RefreshEquippedItems();
				}
			}
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
