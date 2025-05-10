using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class MovementArrowManager : MonoBehaviour
    {
        public float distance;
        public BaseCharacterManagerGroup occupier;
        public PanelsManager hoveredPanel;

        void OnTriggerStay2D(Collider2D collider)
        {
            var chosenPanel = collider.gameObject.GetComponent<PanelsManager>();
            var originalPanel = occupier.movementManager.currentPanel.GetComponent<PanelsManager>();    
            if (IsPanelNearyBy(chosenPanel, originalPanel))
            {
                hoveredPanel = chosenPanel;
            } else
            {
                Debug.Log("cleared hovered panel");
                hoveredPanel = null;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Debug.Log("cleared hovered panel");
            hoveredPanel = null;
        }

        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector2 rayPoint = ray.GetPoint(distance);
            transform.position = rayPoint;
        }

        bool IsPanelNearyBy(PanelsManager chosenPanel, PanelsManager originalPanel)
        {
            var validNeigbours = originalPanel.GetNeighbours();
            return validNeigbours.Contains(chosenPanel);
        }
    }
}