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

        void Update()
        {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector2 rayPoint = ray.GetPoint(distance);
                transform.position = rayPoint;
        }
    }
}