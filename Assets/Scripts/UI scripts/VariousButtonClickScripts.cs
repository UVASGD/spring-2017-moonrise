using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariousButtonClickScripts : MonoBehaviour {
    /*This script is used to store any UI button scripts.
     *  
     */
    public GameObject toggler;

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SwitchCredits()
    {
        
        if (toggler.activeInHierarchy)
        {
            toggler.SetActive(false);

        }
            
        else
        {
            toggler.SetActive(true);
            toggler.transform.GetChild(2).GetComponent<Scrollbar>().value = 1;
        }
            

    }
    
}
