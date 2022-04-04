using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class MovementArrowManager : MonoBehaviour
    {
        public float distance;
        public BaseCharacterManagerGroup occupier;
        public PanelsManager hoveredPanel;
        public PanelsManager originalPanel;

        void OnTriggerStay2D(Collider2D coll)
        {
            hoveredPanel = coll.gameObject.tag == "movementPanels" ? coll.gameObject.GetComponent<PanelsManager>() : null;
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
                    ((CharacterManager)occupier.characterManager).characterModel.isMoving = true;
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