using System;
using ItemSpace;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Completed
{
	public class Character : MovingObject
	{
		//leveled up with magic character points
		protected int baseHP;
		protected double rangedBlock;
		protected double meleeBlock;
		protected double rangedDamage;
		protected double meleeDamage;
		protected double rangedAccuracy;
		protected double meleeAccuracy;

		//affected by items
		protected int totalHP;
		protected int range;
		protected int currentHP;

		protected double baseSpeed;
		protected double totalSpeed;

		protected EquippedItemSet equippedItems;
		protected Inventory inventory;

		public Character () : this(100, 10, 10, 1, 1, 90, 90, 5, 1.0)
		{
		}
			
		public Character (int hp, double dodge, double block, int rangedDamage, int meleeDamage, double rangedAccuracy, double meleeAccuracy, int range, double speed)
		{

			this.rangedBlock = dodge;
			this.meleeBlock = block;

			this.rangedDamage = rangedDamage;
			this.meleeDamage = meleeDamage;

			this.rangedAccuracy = rangedAccuracy;
			this.meleeAccuracy = meleeAccuracy;

			this.range = range;

			baseHP = hp;
			currentHP = baseHP;
			totalHP = baseHP;

			baseSpeed = speed;
			totalSpeed = speed;

			equippedItems = new EquippedItemSet ();
			inventory = new Inventory ();
		}

		/// <summary>
		/// Ranged attack function
		/// </summary>
		/// <returns>The attack.</returns>
		/// <param name="target">Target.</param>
		public int RangedAttack(Character target) {
			float distance = Mathf.Sqrt (Mathf.Pow (target.transform.position.x - this.transform.position.x, 2) + Mathf.Pow (target.transform.position.y - this.transform.position.y, 2));

			//Weapon weap = (Weapon)(equippedItems.Get (ItemClass.Weapon));

			// Placeholder weapon values
			int weaponMin = 3;
			int weaponMax = 5;

			// If distance is 1, use melee values instead of ranged values
			double accuracyValue = distance <= 1 ? (this.RangedAccuracy / 2 + this.MeleeAccuracy) / 1.5 : (this.RangedAccuracy + this.MeleeAccuracy/2) / 1.5;
			double blockValue = distance <= 1 ? (target.RangedBlock / 2 + target.MeleeBlock) / 1.5 : (target.RangedBlock + this.MeleeBlock/2) / 1.5;

			if (accuracyValue - blockValue > UnityEngine.Random.Range (0.0f, 100.0f)) {
				int damage = (int)(this.RangedDamage + this.MeleeDamage/2 / 1.5) * (UnityEngine.Random.Range (weaponMin, weaponMax+1));
				target.LoseHp(damage);
				return damage;
			}
			return 0;
		}

		/// <summary>
		/// Melee attack function
		/// </summary>
		/// <returns>The attack.</returns>
		/// <param name="target">Target.</param>
		public int MeleeAttack(Character target) {
			//Weapon weap = (Weapon)(equippedItems.Get (ItemClass.Weapon));
		
			// Placeholder weapon values
			int weaponMin = 3;
			int weaponMax = 5;

			// If distance is 1, use melee values instead of ranged values
			double accuracyValue = this.RangedAccuracy / 2 + this.MeleeAccuracy;
			double blockValue = target.RangedBlock / 2 + target.MeleeBlock;

			if (accuracyValue - blockValue > UnityEngine.Random.Range (0.0f, 100.0f)) {
				int damage = (int)(this.RangedDamage/2 + this.MeleeDamage / 1.5) * (UnityEngine.Random.Range (weaponMin, weaponMax+1));
				((Player)target).LoseHp(damage);
				return damage;
			}
			return 0;
		}

		/// <summary>
		/// Add the item to the inventory.
		/// </summary>
		/// <param name="item">Item.</param>
		public void AddItem(Item item)
		{
			inventory.AddItem (item);
		}

		/// <summary>
		/// Remove the item from the inventory.
		/// </summary>
		/// <returns><c>true</c>, if item was removed, <c>false</c> otherwise.</returns>
		/// <param name="item">Item.</param>
		public bool RemoveItem(Item item)
		{
			return inventory.RemoveItem (item);
		}

		/// <summary>
		/// Equip the selected item from the inventory, 
		/// and remove the item from the inventory.
		/// If an item of the same type is already equipped, unequip it.
		/// </summary>
		/// <param name="item">Item.</param>
		public void EquipItem(Item item)
		{
			EquipItem equippable;
			if (item is EquipItem) {
				equippable = (EquipItem)item;
				if (RemoveItem (equippable)) {
					Item unequipped = equippedItems.Unequip (equippable.ItemClass);
					if (equippable != null)
						inventory.AddItem (unequipped);
					equippedItems.Equip (equippable);
				}
			}
			// TODO: update stats based on changed items
		}

		/// <summary>
		/// Drops hp by loss
		/// </summary>
		/// <param name="loss">Loss.</param>
		public virtual void LoseHp(int loss)
		{
			Debug.Log ("losing " + loss + " from " + this);
			currentHP -= loss;
			if (currentHP <= 0) {
				KillObject ();
			}
			return;
		}

		protected virtual void KillObject()
		{
		}
		/// <summary>
		/// Unequip the item and add it to the inventory.
		/// </summary>
		/// <param name="ic">Item class.</param>
		public void UnequipItem(ItemClass ic)
		{
			Item unequipped = equippedItems.Unequip (ic);
			AddItem (unequipped);
		}

		#region properties	
		public int TotalHP {
			get {
				return this.totalHP;
			}
			set {
				totalHP = value;
			}
		}

		public double RangedBlock {
			get {
				return this.rangedBlock;
			}
			set {
				rangedBlock = value;
			}
		}

		public double RangedDamage {
			get {
				return this.rangedDamage;
			}
			set {
				rangedDamage = value;
			}
		}

		public double MeleeDamage {
			get {
				return this.meleeDamage;
			}
			set {
				meleeDamage = value;
			}
		}

		public double MeleeBlock {
			get {
				return this.meleeBlock;
			}
			set {
				meleeBlock = value;
			}
		}

		public double RangedAccuracy {
			get {
				return this.rangedAccuracy;
			}
			set {
				rangedAccuracy = value;
			}
		}

		public double MeleeAccuracy {
			get {
				return this.meleeAccuracy;
			}
			set {
				meleeAccuracy = value;
			}
		}

		public int Range {
			get {
				return this.range;
			}
			set {
				range = value;
			}
		}

		public double TotalSpeed {
			get {
				return this.totalSpeed;
			}
			set {
				totalSpeed = value;
			}
		}

		public int CurrentHP {
			get {
				return this.currentHP;
			}
			set {
				currentHP = value;
			}
		}

		public EquippedItemSet EquippedItems {
			get {
				return this.equippedItems;
			}
			set {
				equippedItems = value;
			}
		}

		public Inventory Inventory {
			get {
				return this.inventory;
			}
			set {
				inventory = value;
			}
		}
		#endregion

		protected override void OnCantMove(Transform transform)
		{

		}

		protected override void OnFinishMove(){

		}

		protected override void UpdateSprite(){
		}
	}
}

