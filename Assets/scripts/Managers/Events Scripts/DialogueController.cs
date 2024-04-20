using AssemblyCSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.Managers.Events_Scripts
{
    public class DialogueController : MonoBehaviour
    {
        public void ToggleGameHitBoxes(bool enable)
        {
            MainGameManager.instance.DisableEnableLiveBoxColliders(enable);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
