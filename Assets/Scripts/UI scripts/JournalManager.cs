using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Completed {
    using System.Collections.Generic;
    using UnityEngine.UI;

    public class JournalManager : MonoBehaviour {

        public GameObject gameManager;
        public Scrollbar bar;

        private Queue<string> journal;

        void Start()
        {
            collectJournal();
            displayJournal();
        }

        /// <summary>
        /// Grabs the journal from GameManager
        /// </summary>
        private void collectJournal()
        {
            var manager = (GameManager)gameManager.GetComponent(typeof(GameManager));
            journal = manager.getJournal();

        }

        /// <summary>
        /// Places text from the Journal into the text box
        /// </summary>
        public void displayJournal()
        {
            var textBox = gameObject.GetComponent<Text>();
            textBox.text = "";
            foreach(string s in journal)
            {
                textBox.text += s + "\n";
            }

        }

        void Update()
        {
            if (gameObject.activeInHierarchy)
            {
                //If the UI element needs to repeatedly update: Do it here.
                collectJournal();
                //displayJournal();
                //Keyboard controls
                /*
                if(Input.GetKey(KeyCode.Z))
                {
                    bar.value += 0.1f;
                }
                else if(Input.GetKey(KeyCode.X))
                {
                    bar.value -= 0.1f;
                }
                */
            }
        }

    }
}
