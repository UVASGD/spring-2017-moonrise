using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Completed;
using ItemSpace;

namespace Completed
{

	public class InventoryManager : MonoBehaviour
	{
		public GameObject itemPrefab;
		public GameObject inventoryBoxPrefab;

		public static InventoryManager instance;

		private GameObject inventoryBox;
		private Text inventoryText, bagText;
		private GameObject weaponBox, armorBox, talisBox, bag, itemGrid;

		private bool active;

		private Player player;
		private EquippedItemSet equipped;
		private Inventory inventory;

		private static string noWeapon = "no weapon", noArmor = "no armor", noTalisman = "no talisman";

		void Awake(){

			InventoryManager.instance = this;

			player = (Player) GameObject.Find ("Player").transform.GetComponent<Player> ();
			equipped = player.EquippedItems;
			inventory = player.Inventory;

			inventoryBox = Instantiate (inventoryBoxPrefab);
			GameObject canvas  = GameObject.Find ("Canvas");
			RectTransform canvasRect = canvas.GetComponent<RectTransform>();
			inventoryBox.transform.SetParent (canvas.transform);
			float[] size = {inventoryBox.GetComponent<RectTransform>().rect.width,inventoryBox.GetComponent<RectTransform>().rect.height};
			float posX = (-canvasRect.rect.width/2)+(size[0]/2);
			float posY = (-canvasRect.rect.height/2)+(size[1]/2);

			Transform inventoryTransform = inventoryBox.transform;
			inventoryText = inventoryTransform.GetChild (0).gameObject.GetComponent<Text>();
			weaponBox = inventoryTransform.GetChild (1).gameObject;
			weaponBox.GetComponent<Text> ().text = noWeapon;
			armorBox = inventoryTransform.GetChild (2).gameObject;
			armorBox.GetComponent<Text> ().text = noArmor;
			bag = inventoryTransform.GetChild (3).gameObject;

			Transform bagTransform = bag.transform;
			bagText = bagTransform.GetChild (0).gameObject.GetComponent<Text> ();
			itemGrid = bagTransform.GetChild (1).gameObject;

			active = false;
			bag.SetActive (active);
		}

		// Use this for initialization
		void Start ()
		{
			




//			// test of how to initialize item prefabs, getting the index of a child
//			for(int i = 0; i < 10; i++) {
//				GameObject testItem = Instantiate(itemPrefab);
//				testItem.GetComponent<Text> ().text = "Unmarked Test Item";
//				testItem.transform.SetParent (itemGrid.transform);
//			}
//
//			foreach(Transform child in itemGrid.transform) {
//				child.gameObject.GetComponent<Text> ().text = "Test Item " + child.GetSiblingIndex ();
//			}
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(Input.GetKeyDown(KeyCode.I)) {
				ToggleInventory ();
			}

			// filler weapon equipping capability
			if(Input.GetKeyDown(KeyCode.E)) {
				if (inventory.Items.Count > 0) {
					UnequipItem (1);
					EquipItem (0);
				}
			}

			// not-working mouse functionality
//			// from unity answers
//			if( Input.GetMouseButtonDown(0) )
//			{
//				Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
//				RaycastHit hit;
//
//				if( Physics.Raycast( ray, out hit, 100 ) )
//				{
//					Transform target = hit.transform;
//					Debug.Log( target.gameObject.name );
//					if(target.parent == itemGrid.transform)
//						Debug.Log( target.gameObject.GetComponent<Text>().text );
//				}
//			}
		}

		void EquipItem(int index)
		{
			Item item = inventory.Items [index];
			player.EquipItem (item);
			Refresh ();
		}

		void UnequipItem(int index)
		{
			if (index == 1)
				player.UnequipItem (ItemClass.Weapon);
			else if (index == 2)
				player.UnequipItem (ItemClass.Armor);
			Refresh ();
		}

		void ToggleInventory()
		{
			active = !active;
			bag.SetActive (active);
			if(active) {
				Refresh ();
			}
		}

		public void Refresh()
		{
			// reinitialize item grid members
			foreach(Transform child in itemGrid.transform) {
				if(child.gameObject != null)
					Destroy (child.gameObject);
			}
			foreach(Item item in inventory.Items) {
				GameObject itemObj = Instantiate(itemPrefab);
				itemObj.GetComponent<Text> ().text = item.Name;
				itemObj.transform.SetParent (itemGrid.transform);
			}

			RefreshEquippedItems();
		}

		public void RefreshEquippedItems(){
			// reinitialize equipped items
			Weapon weapon = equipped.Weapon;
			Armor armor = equipped.Armor;
			weaponBox.GetComponent<Text> ().text = weapon != null ? weapon.Name : noWeapon;
			armorBox.GetComponent<Text> ().text = armor != null ? armor.Name : noArmor;
		}
	}

}