using UnityEngine;
using System.Collections;

/* Treat this script more as an example as to how Large-Menu scripts should be designed.
 * Keep in mind this script is a component of the panel labelled "Expand"
 * 
 * Note: You MUST put everything in OnEnable. Unity throws unfixable errors when
 * manipulating objects that aren't enabled for some reason. Some sort of 
 * SUCCEEDED(hr) that crashes the script object. 
 */
 
public class ExpandTemplate : MonoBehaviour {

    public Canvas canvas; //No idea how important this is. Probably only important for automated UI Movements
    RectTransform rtf;
	// Use this for initialization. Start or Awake will crash it. 
	void OnEnable () {
        Center(); // While not important to be first, it's best if it is. Centering the window correctly takes little effort and prevents the menu from never appearing due to shenanigans.
    }

    /// <summary>
    ///  Force Centers the UI element. 
    /// </summary>
    private void Center() //Private because nothing outside this script should be using this.
    {
        rtf = gameObject.GetComponent<RectTransform>();

        int width = (int)rtf.rect.width;
        int height = (int)rtf.rect.height;
        float x = (Screen.width) / 2; //Anchor of Panel SHOULD be centered. No need to worry.
        float y = (Screen.height) / 2;
        Vector3 newpos = new Vector3(x, y);
        rtf.position = newpos;
    }

    /* Commented out because superfluous updates eat memory.
    void Update()
    {
        if(gameObject.activeInHierarchy)
        {
            //If the UI element needs to repeatedly update: Do it here.
        }
    }
    */

}
