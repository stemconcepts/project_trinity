using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Spine;

public class movementPanelController : MonoBehaviour {
	public bool isOccupied;
    private RectTransform UItransform;
	public bool isVoidZone;
	public bool isVoidCounter;
	public bool isThreatPanel;
	public Color panelColor;
	public GameObject currentOccupier;
	public bool friendlyPanel;
	private bool dragging = false;
	public GameObject positionArrowType;
	private float distance;
	private Vector2 currentPosition;
	private Task holdTimeTask;
	private GameObject positionArrow;
	public Image imageScript;
	public voidZoneType voidZonesTypes;
	public enum voidZoneType{
		HorizontalA,
		HorizontalB,
		HorizontalC,
		VerticalA,
		VerticalB,
		VerticalC
	};
	public bool moved = false;
	//Clear all Panels
//	public void ClearPanels( string role ){
//		var panels = GameObject.FindGameObjectsWithTag("movementPanels");
//		foreach( GameObject panel in panels ){
//			if( panel.GetComponent<movementPanelController>().currentOccupier != null && panel.GetComponent<movementPanelController>().currentOccupier.GetComponent<character_data>().role == role ){
//				panel.GetComponent<movementPanelController>().isOccupied = false;
//			}
//		}
//	}
//
//	//Spawn Move Pointer
//	void OnMouseDown(){
//		if( currentOccupier != null ){
//			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			distance = Vector2.Distance(transform.position, Camera.main.transform.position);
//			holdTimeTask = new Task( holdtime( 0.1f, ray, distance ) );
//		}
//		//currentPosition = this.transform.position;
//		//print (this);
//		//dragging = true;
//		//print ( currentPosition );
//	}
//
//	IEnumerator holdtime( float waitTime, Ray rayVar, float distanceVar ){
//		while( dragging == false ){
//			yield return new WaitForSeconds( waitTime );
//			dragging = true;
//		}
//		positionArrow = (GameObject)Instantiate( positionArrowType, rayVar.GetPoint(distanceVar) , Quaternion.identity );
//		positionArrow.GetComponent<panelArrowBehaviour>().dragging = true;
//		positionArrow.GetComponent<panelArrowBehaviour>().distance = distanceVar;
//		positionArrow.GetComponent<panelArrowBehaviour>().classOccupier = currentOccupier;
//	}
//
//	void OnMouseUp()
//	{
//		dragging = false;
//		if( positionArrow ){
//			var positionArrowData = positionArrow.GetComponent<panelArrowBehaviour>();
//			positionArrowData.classOccupier.transform.FindChild("Animations").GetComponent<MeshRenderer>().sortingOrder = (int)positionArrowData.classOccupier.transform.position.y;
//			holdTimeTask.Stop ();
//			ClearPanels( positionArrow.GetComponent<panelArrowBehaviour>().classOccupier.GetComponent<character_data>().role );
//			positionArrowData.SetPanelandDestroy();
//			Destroy(positionArrow);
//		}
//	}

	public void VoidZoneMark(){
		//if( type == "All" ){
			imageScript.color = new Color(0.9f, 0.1f, 0.1f, 0.8f);
			//print ("Void Zone Mark");
			isVoidZone = true;
			if( isOccupied ){
				currentOccupier.GetComponent<character_data>().inVoidZone = true;
			}
		/*} else if( type == "Random" ) {
			imageScript.color = new Color(0.9f, 0.1f, 0.1f, 0.8f);
			isVoidZone = true;
            if( isOccupied ){
                currentOccupier.GetComponent<character_data>().inVoidZone = true;
            }
		} */
	}

	public void ClearVoidZone(){
		//imageScript.color = new Color(1f,1f,1f,1f);
		imageScript.color = panelColor;
		isVoidZone = false;
		isVoidCounter = false;
		if( isOccupied ){
			currentOccupier.GetComponent<character_data>().inVoidZone = false;
			currentOccupier.GetComponent<character_data>().inVoidCounter = false;
		}
	}

	public void SafePanel(){
		imageScript.color = new Color(0.1f,0.9f,0.1f,0.8f);
		isVoidZone = false;
		isVoidCounter = true;
		if( isOccupied && currentOccupier.GetComponent<character_data>().role == "tank" ){
			currentOccupier.GetComponent<character_data>().inVoidCounter = true;
		}
	}

	public void SetStartingPanel(){
		Vector2 panelPos = UItransform.position;
		panelPos.y = panelPos.y + 6f;
		if( currentOccupier ){
            //Debug.Log( UItransform.position + ":" + gameObject.name );
			currentOccupier.transform.position = new Vector2(panelPos.x, panelPos.y);
			currentOccupier.transform.Find("Animations").GetComponent<MeshRenderer>().sortingOrder = transform.parent.GetComponent<panelProperties>().sortingLayerNumber;
            currentOccupier.GetComponent<attackMovement>().origSortingOrder = transform.parent.GetComponent<panelProperties>().sortingLayerNumber;
			moved = true;
		} else {
			moved = true;
		}
	}

	// Use this for initialization
	public void Start() {
        StartCoroutine( DelayedSetStartingPanel() );
	}

    public IEnumerator DelayedSetStartingPanel( ){
        yield return new WaitForEndOfFrame();
        SetStartingPanel();
    }

	void Awake(){
		imageScript = GetComponent<Image>();
        UItransform = GetComponent<RectTransform>();
	}
}
