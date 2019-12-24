using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class MovementArrowManager : MonoBehaviour
    {
        public float distance;
        public Base_Character_Manager occupier;
        public Panels_Manager hoveredPanel;
        public Panels_Manager originalPanel;

        void OnTriggerStay2D(Collider2D coll)
        {
            hoveredPanel = coll.gameObject.tag == "movementPanels" ? coll.gameObject.GetComponent<Panels_Manager>() : null;
        }

        public void SetPanelandDestroy()
        {
            //occupier.characterManager.characterModel.inThreatZone = false;
            if (hoveredPanel != null)
            {
                if (!hoveredPanel.currentOccupier)
                {
                    originalPanel.currentOccupier = null;
                    originalPanel.animationManager = null;
                    originalPanel.movementManager = null;
                    originalPanel.characterManager = null;
                    hoveredPanel.currentOccupier = occupier.gameObject;
                    /*hoveredPanel.animationManager = occupier.animationManager;
                    hoveredPanel.movementManager = occupier.movementManager;*/
                    occupier.characterManager.characterModel.isMoving = true;
                    /*occupier.movementManager.currentPanel = hoveredPanel.gameObject;
                    occupier.characterManager.characterModel.inVoidZone = hoveredPanel.isVoidZone;
                    occupier.characterManager.characterModel.inVoidCounter = hoveredPanel.isVoidCounter;
                    occupier.characterManager.characterModel.inThreatZone = hoveredPanel.isThreatPanel;*/

                    hoveredPanel.SetStartingPanel(occupier.gameObject);
                }
            }
            //Destroy(this);
        }



        // Use this for initialization
        void Start()
        {
            //originalPanel = occupier.characterManager.currentPanel.GetComponent<Panels_Manager>();
        }

        // Update is called once per frame
        void Update()
        {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector2 rayPoint = ray.GetPoint(distance);
                transform.position = rayPoint;
        }
    }
}