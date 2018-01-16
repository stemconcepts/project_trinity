using UnityEngine;
using System.Collections;

public class panelArrowBehaviour : MonoBehaviour {
	public bool dragging;
	public float distance;
	public GameObject classOccupier;
	public GameObject hoveredPanel;
	public GameObject originalPanel;

	void OnTriggerStay2D(Collider2D coll) {
		hoveredPanel = coll.gameObject.tag == "movementPanels" ? coll.gameObject : null;
	}

	public void SetPanelandDestroy(){
		dragging = false;
		classOccupier.GetComponent<character_data>().inThreatZone = false;
		if(hoveredPanel != null && hoveredPanel.tag == "movementPanels" ){
			if( !hoveredPanel.GetComponent<movementPanelController>().isOccupied ){
				hoveredPanel.GetComponent<movementPanelController>().currentOccupier = classOccupier;
				hoveredPanel.GetComponent<movementPanelController>().isOccupied = true;
				classOccupier.GetComponent<character_data>().isMoving = true;
				//classOccupier.GetComponent<character_data>().currentPanel = hoveredPanel;
				classOccupier.GetComponent<characterMovementController>().MoveToPanel(hoveredPanel);
				if( hoveredPanel.GetComponent<movementPanelController>().isVoidZone ){
					classOccupier.GetComponent<character_data>().inVoidZone = true;
				} else if ( hoveredPanel.GetComponent<movementPanelController>().isVoidCounter ){
					classOccupier.GetComponent<character_data>().inVoidCounter = true;
				}
				if( hoveredPanel.GetComponent<movementPanelController>().isThreatPanel ){
					classOccupier.GetComponent<character_data>().inThreatZone = true;
				}
			}else {
				print("panel is taken");
			}
		}
		Destroy(this);
	}



	// Use this for initialization
	void Start () {
		originalPanel = hoveredPanel;
	}
	
	// Update is called once per frame
	void Update () {
		if (dragging)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector2 rayPoint = ray.GetPoint(distance);
			transform.position = rayPoint;
		}
	}
}
