using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* Since this is the main script driving the entire mini-menu design, some documentation here would probably be best on editing in Unity.
 * The mini-menu is designed to show small menu layouts in the "Contents" panels while allowing the user to "pop out" the menus for their larger variants. 
 * The overall design is mostly scalable. 
 * 
 * ADDING A NEW TAB:
 * - Resize the TabManager Panel to hold the new tab
 * - Duplicate one of the tabs. Saves effort over building a new one.
 * - Rename tab, position it accordingly. 
 * - Update the tabs array in Unity and add the tab.
 * - NOTE THAT THE TABS ARRAY IS IN LEFT TO RIGHT ORDER. THIS IS IMPORTANT.
 * - Go to KeyPoll() and write in a new if statement for the new tab's key.
 * */

public class TabManager : MonoBehaviour {

    private int uiTab;
    public Canvas otherCanvas;
    public GameObject expander;
    public GameObject[] tabs = new GameObject[5]; //This is best modified in the Manager Panel instead of here. Easier and simpler.
    private GameObject expandedTab;
    private int opacity; //0 = 0%, 1 = 25%, 2 = 50%, 3 = 75%
    private CanvasGroup cGroup;
    private Text opacityText;
    public Sprite[] skillImages = new Sprite[8]; // Used to store changing sprites for Skills.
    public Sprite[] moon;


    private bool currentExpansion = false;

	void Start () {  //For init
        cGroup = GameObject.Find("MenuCanvas").GetComponent<CanvasGroup>();
        opacityText = gameObject.transform.FindChild("Settings").FindChild("Contents").FindChild("Opacity").FindChild("OText").GetComponent<Text>();

        uiTab = 0;
        opacity = 0;
        switchTabs(0);
	}

    /// <summary>
    ///     Switches to the designated tab, disabling the panels except the designated tab.
    ///     This is expected to be used in the onClick events in the buttons and via Update KeyUp events.
    ///     The button must have a panel named "Contents" as a child. 
    /// </summary>
    /// <param name="tabNumber">
    ///     The INDEXED tab that should be switched to. Note that 
    ///     the index of the tabs are by the array's stored GameObjects.
    /// </param>
    public void switchTabs(int tabNumber)
    {
        uiTab = tabNumber;
        Debug.Log("Attempting to set indexed tab " + tabNumber + " to active tab");
        for (int i = 0; i < tabs.Length; i++)
        {
            var tabContents = tabs[i].transform.Find("Contents").gameObject;
            if (tabContents != null)
            {
                if (i == tabNumber)
                {
                    tabContents.SetActive(true);
                }
                else tabContents.SetActive(false);
            }
        }
        Debug.Log("Tab switch success!");
    }


    /// <summary>
    /// Gets the tab the manager is currently on.
    /// </summary>
    /// <returns></returns>
    public int getCurrentTab()
    {
        return uiTab;
    }
    /// <summary>
    /// Opens the mini-menu's large menu. 
    /// </summary>
    public void expandTab()
    {
        if(!currentExpansion) // If no menu is up
        {
            Debug.Log("Opening Tab " + uiTab);
            var expandContents = tabs[uiTab].transform.Find("Expand").gameObject;
            expandContents.SetActive(true);
            expandedTab = expandContents;
            expander.transform.Find("Text").gameObject.GetComponent<Text>().text = ">";
            currentExpansion = true;
        }
        else if(currentExpansion) // If a menu is up
        {
            for (int i = 0; i < tabs.Length; i++)
            {
                var tabContents = tabs[i].transform.Find("Expand").gameObject;
                if (tabContents != null)
                {
                    if (i == uiTab)
                    {
                        if (tabContents.Equals(expandedTab)) // If the menu from the same tab
                        {
                            Debug.Log("Closing Tab " + i);
                            tabContents.SetActive(false);
                            expandedTab = null;
                            expander.transform.Find("Text").gameObject.GetComponent<Text>().text = "<";
                            currentExpansion = false;
                        }
                        else // Otherwise
                        {
                            Debug.Log("Opening Tab " + i);
                            expandedTab = tabContents;
                            tabContents.SetActive(true);
                        }
                    }
                    else tabContents.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    ///  Polls the keyboard for input based on menu keys. 
    /// </summary>
    private void KeyPoll()
    {
        //TODO: Find a way to recode keys directly to the tabs, so that hard-coded keys and tab numbers aren't required.
        //TODO: Maybe find a way to simplify this? There has to be a more efficient way.
        //TODO: Learn Unity keyconfig stuff to allow keyswitching in pre-menu.
        if (Input.GetKeyUp(KeyCode.M))
        {
            if (uiTab != 0)
                switchTabs(0);
            else
                expandTab();
        }
        else if (Input.GetKeyUp(KeyCode.J))
        {
            if (uiTab != 1)
                switchTabs(1);
            else
                expandTab();
        }
        else if (Input.GetKeyUp(KeyCode.K))
        {
            if (uiTab != 2)
                switchTabs(2);
            else
                expandTab();
        }
        else if (Input.GetKeyUp(KeyCode.L))
        {
            if (uiTab != 3)
                switchTabs(3);
            else
                expandTab();
        }

    }

    //Needs to poll keyboard input, since the game is intended for either keyboard or mouse.
    public void Update()
    {
        KeyPoll();
    }
    /// <summary>
    /// Pretty hacky, but allows for exiting via UI buttons.
    /// </summary>
    public void performExit()
    {
        Debug.Log("Game Exit");
        Application.Quit();
    }

    public void opacityChange()
    {
        switch(opacity)
        {
            case 0:
                opacity = 1;
                cGroup.alpha = 0.75f;
                otherCanvas.GetComponent<CanvasGroup>().alpha = 0.75f;
                opacityText.text = "Opacity: 75%";
                break;
            case 1:
                opacity = 2;
                cGroup.alpha = 0.50f;
                otherCanvas.GetComponent<CanvasGroup>().alpha = 0.50f;
                opacityText.text = "Opacity: 50%";
                break;
            case 2:
                opacity = 3;
                cGroup.alpha = 0.25f;
                otherCanvas.GetComponent<CanvasGroup>().alpha = 0.25f;
                opacityText.text = "Opacity: 25%";
                break;
            case 3:
                opacity = 0;
                cGroup.alpha = 1.0f;
                otherCanvas.GetComponent<CanvasGroup>().alpha = 1.0f;
                opacityText.text = "Opacity: 100%";
                break;


        }
    }

    public void beep()
    {
        Debug.Log("Beep");
    }
}