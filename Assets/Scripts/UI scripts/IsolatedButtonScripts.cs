using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsolatedButtonScripts : MonoBehaviour {
/* Literally a set of random mini-scripts needed for various buttons. 
 * For example: Exit Game needs a tiny script since buttons don't innately have exits. It's dumb.
 * Things will be added here as necessary. This script also should never have any MonoBehavior calls
 *    No Start(),Awake(),Update(),FixedUpdate(), etc.
 */

    public void ExitGame()
    {
        Application.Quit();
    }
}
