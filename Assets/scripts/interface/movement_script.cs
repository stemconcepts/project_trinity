using UnityEngine;
using System.Collections;

public class movement_script : MonoBehaviour {
	private character_data characterScript;
	private status statusScript;
	public bool dragging = false;
	private float distance;
	private Vector2 currentPosition;
	private Vector2 newPosition;
	private Vector2 difference;
	private Task holdTimeTask;
	BoxCollider2D colliderScript;
	private GameObject skillBar;
	private gameEffects gameEffectsScript;
	//public bool isOccupied;
	//public GameObject currentOccupier;
	//public bool friendlyPanel;
	public GameObject positionArrowType;
	public GameObject positionArrow;
	private GameObject floorPanels;

	//Clear all Panels
	public void ClearPanels( string role ){
		var panels = GameObject.FindGameObjectsWithTag("movementPanels");
		foreach( GameObject panel in panels ){
			if( panel.GetComponent<movementPanelController>().currentOccupier != null && panel.GetComponent<movementPanelController>().currentOccupier.GetComponent<character_data>().role == role ){
				panel.GetComponent<movementPanelController>().isOccupied = false;
				panel.GetComponent<movementPanelController>().currentOccupier = null;
			}
		}
	}

	//Character Hitbox Controller
	public void HitBoxControl( string role, bool hitBoxSwitch , bool allCharacters ){
		var allPlayers = GameObject.FindGameObjectsWithTag("Player");
		if( allCharacters == true ){
			foreach( GameObject character in allPlayers ){
				character.GetComponent<BoxCollider2D>().enabled = hitBoxSwitch;
			}
		} else {
			foreach( GameObject character in allPlayers ){
				if( character.GetComponent<character_data>().role == role ){
					character.GetComponent<BoxCollider2D>().enabled = hitBoxSwitch;
				}
			}
		}
	}

	//Spawn Move Pointer
	public void OnMouseDown(){
		if( !statusScript.DoesStatusExist("stun") ){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			distance = Vector2.Distance(transform.position, Camera.main.transform.position);
			holdTimeTask = new Task( holdtime( 0.1f, ray, distance ) );
			HitBoxControl( "", false, true );
		}
	}
	IEnumerator holdtime( float waitTime, Ray rayVar, float distanceVar ){
		while( dragging == false ){
			yield return new WaitForSeconds( waitTime );
			//if( !floorPanels ){
			//	floorPanels.gameObject.SetActive(true);
			//}
			dragging = true;
		}
		//Time.timeScale = 0.1f;
		if( dragging ){
			positionArrow = (GameObject)Instantiate( positionArrowType, rayVar.GetPoint(distanceVar) , Quaternion.identity );
			positionArrow.GetComponent<panelArrowBehaviour>().dragging = true;
			positionArrow.GetComponent<panelArrowBehaviour>().distance = distanceVar;
			positionArrow.GetComponent<panelArrowBehaviour>().classOccupier = this.gameObject;
		}
	}
	
	public void OnMouseUp()
	{
        print( gameObject.name + ":Mouse Up active");
		//Time.timeScale = 1.0f;
		if( !dragging && holdTimeTask != null && holdTimeTask.Running ){
			holdTimeTask.Stop();
		}
		dragging = false;
		if( positionArrow ){
			print("Position Arrow true");
			var positionArrowData = positionArrow.GetComponent<panelArrowBehaviour>();
			//positionArrowData.classOccupier.transform.FindChild("Animations").GetComponent<MeshRenderer>().sortingOrder = (int)positionArrowData.classOccupier.transform.position.y;
			//holdTimeTask.Stop ();
			ClearPanels( positionArrow.GetComponent<panelArrowBehaviour>().classOccupier.GetComponent<character_data>().role );
			positionArrowData.SetPanelandDestroy();
			positionArrowData.classOccupier.transform.Find("Animations").GetComponent<MeshRenderer>().sortingOrder = characterScript.currentPanel.transform.parent.GetComponent<panelProperties>().sortingLayerNumber;
			positionArrowData.classOccupier.GetComponent<attackMovement>().origSortingOrder = characterScript.currentPanel.transform.parent.GetComponent<panelProperties>().sortingLayerNumber;
			//print ( characterScript.currentPanel.transform.parent.GetComponent<panelProperties>().sortingLayerNumber );
			if( positionArrowData.hoveredPanel ){
				positionArrowData.classOccupier.GetComponent<character_data>().rowNumber = positionArrowData.hoveredPanel.transform.parent.GetComponent<panelProperties>().sortingLayerNumber;
			}
			Destroy(positionArrow);
			if( GameObject.Find("CharSelectUI(Clone)") ){
				Destroy( GameObject.Find("CharSelectUI(Clone)") );
			}
		}
		HitBoxControl( "", true, true );
		
		//if( floorPanels ){
		//	floorPanels.gameObject.SetActive(false);
		//}
	}

//	//Move Character
//	void OnMouseDown(){
//		holdTimeTask = new Task( holdtime( 0.1f ) );
//		distance = Vector2.Distance(transform.position, Camera.main.transform.position);
//		currentPosition = this.transform.position;
//		print (this);
//		//dragging = true;
//		//print ( currentPosition );
//	}

//	IEnumerator holdtime( float waitTime ){
//		while( dragging == false ){
//			yield return new WaitForSeconds( waitTime );
//			dragging = true;
//		} 
//		characterScript.actionPoints = characterScript.actionPoints - 1;
//	}

//	void OnMouseUp()
//	{
//		dragging = false;
//		holdTimeTask.Stop ();
//		newPosition = this.transform.position;
//		difference = currentPosition - newPosition;
//		//print (currentPosition + " and " + newPosition );
//		//print ("different between " + difference);
//	}

	//check if partner row is occupied
	public bool IsParnetRowOccupied( GameObject overlappedObj ){
		if( overlappedObj != null ){
			var overlappedObjRowNum = overlappedObj.GetComponent<character_data>().rowNumber;
			var rowDiff = Mathf.Abs( characterScript.rowNumber - overlappedObjRowNum );
			print( rowDiff );
			if( rowDiff == 1 ){
				return true;
			}
		}
		return false;
	}

	// Use this for initialization
	void Awake () {
		floorPanels = GameObject.Find("Panel-floor");
		characterScript = GetComponent<character_data>();
		statusScript = GetComponent<status>();
		colliderScript = GetComponent<BoxCollider2D>();
		gameEffectsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<gameEffects>();
	}

	void Start(){
		skillBar = GameObject.FindGameObjectWithTag("PanelSkillHolder");
	}

	// Update is called once per frame
	void Update () {

	}
}
