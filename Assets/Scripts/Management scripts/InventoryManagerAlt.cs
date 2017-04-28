using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Completed;
using ItemSpace;

namespace Completed
{

    public class InventoryManagerAlt : MonoBehaviour
    {
        private GameObject InventoryUI;
        private GameObject InventoryContainer;
        private GameObject inventoryExpand;
        private GameObject inventoryMini;

        public GameObject itemPrefab;

        public static InventoryManagerAlt instance;

        private GameObject inventoryBox;

        private List<GameObject> itemGrid;

        private bool active;

        private Player player;
        private EquippedItemSet equipped;
        private Inventory inventory;

        private static string noWeapon = "no weapon", noArmor = "no armor";

        void Awake()
        {
            Debug.Log("BEEP INVENTORY");
            if (InventoryManagerAlt.instance == null)
                InventoryManagerAlt.instance = this;
            else
            {
                Destroy(this);
            }
            player = (Player)GameObject.Find("Player").transform.GetComponent<Player>();
            equipped = player.EquippedItems;
            inventory = player.Inventory;
            InventoryUI = GameObject.Find("Inventory");
            inventoryExpand = InventoryUI.transform.FindChild("Expand").gameObject;
            inventoryMini = InventoryUI.transform.FindChild("Contents").gameObject;
            InventoryContainer = inventoryExpand.transform.FindChild("InventoryStore").gameObject;

            inventoryMini.transform.FindChild("WeaponText").GetChild(0).GetComponent<Text>().text = noWeapon;
            inventoryMini.transform.FindChild("ArmorText").GetChild(0).GetComponent<Text>().text = noArmor;
            itemGrid = new List<GameObject>();

            active = false;
        }

        public void reload()
        {
            player = (Player)GameObject.Find("Player").transform.GetComponent<Player>();
            equipped = player.EquippedItems;
            inventory = player.Inventory;
            InventoryUI = GameObject.Find("Inventory");
            inventoryExpand = InventoryUI.transform.FindChild("Expand").gameObject;
            inventoryMini = InventoryUI.transform.FindChild("Contents").gameObject;
            InventoryContainer = inventoryExpand.transform.FindChild("InventoryStore").gameObject;

            inventoryMini.transform.FindChild("WeaponText").GetChild(0).GetComponent<Text>().text = noWeapon;
            inventoryMini.transform.FindChild("ArmorText").GetChild(0).GetComponent<Text>().text = noArmor;

            InventoryManagerAlt.instance.RefreshEquippedItems();
            active = false;
        }
        
        // Update is called once per frame
        void Update()
        {
            //Optimizing Update is kind of annoying.
            if (inventoryExpand.activeInHierarchy && !active)
            {
                active = true;
                Refresh();
            }
            else if (!inventoryExpand.activeInHierarchy && active)
            {
                active = false;
            }
        }
    
        /// <summary>
        /// Equips a weapon.
        /// </summary>
        /// <param name="index">The item index from inventory.</param>
        public void EquipWeapon(int index)
        {
            player.UnequipItem(ItemClass.Weapon);
            player.EquipItem(inventory.Items[index]);
            Refresh();
        }
        /// <summary>
        /// This script is injected into buttons that are created. Allows for selling and equipping.
        /// </summary>
        /// <param name="index">The item index from inventory.</param>
        public void WeaponButton(int index)
        {
            if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                int value = 100; //TODO: Get a means of calculating item value.
                GameManager.instance.print("Offered " + inventory.Items[index].Name + " for " + value + " silver.");
                inventory.RemoveItem(inventory.Items[index]);
                GameManager.instance.playerGoldPoints += value;
                GameManager.instance.CurrencyCheck();
                Refresh();
            }
            else if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            {
                EquipWeapon(index);
            }
        }

        public void EquipArmor(int index)
        {
            player.UnequipItem(ItemClass.Armor);
            player.EquipItem(inventory.Items[index]);
            Refresh();
        }

        /// <summary>
        /// This script is injected into buttons that are created. Allows for selling and equipping.
        /// </summary>
        /// <param name="index">The item index from inventory.</param>
        public void ArmorButton(int index)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                int value = 100; //TODO: Get a means of calculating item value.
                GameManager.instance.print("Offered " + inventory.Items[index].Name + " for " + value + " silver.");
                inventory.RemoveItem(inventory.Items[index]);
                GameManager.instance.playerGoldPoints += value;
                GameManager.instance.CurrencyCheck();
                Refresh();
            }
            else if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            {
                EquipArmor(index);
            }
        }

        /// <summary>
        /// Refreshed the inventory layout.
        /// </summary>
        public void Refresh()
        {
            // reinitialize item grid members
            Debug.Log("Starting Refresh");
            foreach (GameObject child in itemGrid)
            {
                if (child != null)
                    Destroy(child);
            }
            itemGrid.Clear(); //Ensure that the button list is empty.
            foreach (Item item in inventory.Items)
            {
                GameObject itemObj = Instantiate(itemPrefab);
                itemGrid.Add(itemObj);
                itemObj.transform.GetChild(0).GetComponent<Text>().text = item.Name;
                switch (item.ItemClass) {
                    case ItemClass.Weapon:
                        itemObj.transform.SetParent(InventoryContainer.transform.FindChild("Weapons").FindChild("Content"));
                        itemObj.transform.GetChild(0).GetComponent<Text>().text = item.Name;
                        itemObj.GetComponent<Button>().onClick.AddListener(delegate { WeaponButton(inventory.Items.IndexOf(item)); });
                        break;
                    case ItemClass.Armor:
                        itemObj.transform.SetParent(InventoryContainer.transform.FindChild("Armor").FindChild("Content"));
                        itemObj.transform.GetChild(0).GetComponent<Text>().text = item.Name;
                        itemObj.GetComponent<Button>().onClick.AddListener(delegate { ArmorButton(inventory.Items.IndexOf(item)); });
                        break;
                    default:
                        Debug.Log("What the shit?"); //TODO: Figure out what all is in the game.
                        break;
                }
            }

            RefreshEquippedItems();
        }
        /// <summary>
        /// Refreshes the currently equipped items.
        /// </summary>
        public void RefreshEquippedItems()
        {
            // reinitialize equipped items
            Weapon weapon = equipped.Weapon;
            Armor armor = equipped.Armor;
            if (inventoryMini != null)
            {
                inventoryMini.transform.FindChild("WeaponText").GetChild(0).GetComponent<Text>().text = weapon != null ? weapon.Name : noWeapon;
                inventoryMini.transform.FindChild("ArmorText").GetChild(0).GetComponent<Text>().text = armor != null ? armor.Name : noArmor;
            }
        }
    }

}