using UnityEngine;
using System.Collections;

public class button_clicks : MonoBehaviour {
    
	public GameObject globalScript;
	//public GameObject players;
	//private status playerstatus;
	//public GameObject enemies;
	//private status enemystatus;
	private Task holdTimeTask;
	//private bool holdingSelection;
	private character_select characterSelectScript;
	private character_data characterScript;
	private skill_cd skillCdScript;
	private selectionOverlapControl selectionOverlapScript;
	private Task overlapUITimer;
	private soundController soundContScript;
	public AudioClip audioClip;
    private string swapToChar = "guardian";
    classState guardian; 
    classState walker; 
    classState stalker;    

public string GetClassRole(){
		return characterScript.role;
}
	
bool ConvertRoleToBool( string role, string classSelected ){
		var selectedRole = role + "Selected";
		if( selectedRole == classSelected ){
			return true;
		} else {
			return false;
		}
}

public void DisplaySkillsSecond( ){
		//skillBar.gameObject.SetActive(true);
		//var skillPos = GameObject.FindGameObjectWithTag("PanelSkillHolderSecond");
		var skillDisplayControllers = GameObject.FindGameObjectsWithTag("skillDisplayControl");
		for( int x = 0; x < 2 ; x++ ){
		//for( int x = 0; x < skillDisplayControllers.Length; x++ ){
			skillDisplayControllers[x].GetComponent<skillLabeldisplay>().GetSkillData();
		}
		skillDisplayControllers[2].GetComponent<class_skillLabelDisplay>().BuildClassSkill();
		//skillBar.transform.position = skillPos.transform.position;
}


/*void OnMouseDown(){
	holdTimeTask = new Task( holdtime( 0.1f ) );
}
IEnumerator holdtime( float waitTime ){
	yield return new WaitForSeconds( waitTime );
	selectionOverlapScript.BuildOverlapList();

}*/


//recieves click action
void OnMouseUp(){
		//Time.timeScale = 1f;
//-------------------------------------- Enemy Selection ----------------------------------------------//
		//print ( characterScript.role + " " + GetClassRole() );
		if( skillCdScript.skillActive == true ){
			skill_targetting.instance.currentTarget = characterScript.gameObject;
					//	print( skill_targetting.instance.currentTarget );
		}
//---------------------------------------- Skill swap ------------------------------------------------//
		//swap to class skills
		if( characterScript.role == GetClassRole() && characterScript.isAlive && !skillCdScript.skillActive ){
			// ConvertRoleToBool(GetClassRole());
			if( GetClassRole() != "Boss" ){
				characterSelectScript.tankSelected = ConvertRoleToBool( GetClassRole(), "tankSelected" );
				characterSelectScript.healerSelected = ConvertRoleToBool( GetClassRole(), "healerSelected" );
				characterSelectScript.dpsSelected = ConvertRoleToBool( GetClassRole(), "dpsSelected" );
				print (GetClassRole() + "skills selected");
				DisplaySkillsSecond();
				//DisplaySkills ( characterScript.skillHolderPos.transform.position );
			}
		}
		soundContScript.playSound( audioClip );
}

void OnMouseEnter(){
	if( selectionOverlapScript.overlappedObj != null && !GameObject.Find("CharSelectUI(Clone)") && GetClassRole() != "Boss" ){
		var partnerRowOccupied = GetComponent<movement_script>().IsParnetRowOccupied( selectionOverlapScript.overlappedObj );
		if( partnerRowOccupied ){
			//GameObject.Find("Panel-CharOverlapUI").SetActive(true);
			var distance = Vector2.Distance(selectionOverlapScript.overlappedObj.transform.position, Camera.main.transform.position);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 rayPoint = ray.GetPoint(distance);
			Vector3 spawnPos = gameObject.transform.Find("FXpositions").Find("FXcenter").transform.position;
			var charOverlapUI = (GameObject)Instantiate( Resources.Load("CharSelectUI"), spawnPos, Quaternion.identity );
			selectionOverlapScript.BuildOverlapList( charOverlapUI );
			charOverlapUI.transform.SetParent( GameObject.Find("Canvas - UI").transform );
			charOverlapUI.transform.localScale = new Vector3(1f,1f,1f );
			//GameObject.Find("Panel-CharSelectUI").transform.position = new Vector2( rayPoint.x, rayPoint.y );	
		}
		if( overlapUITimer != null && overlapUITimer.Running ){
			overlapUITimer.Stop();
		}
	}
}

void OnMouseExit(){
	if( GameObject.Find("CharSelectUI(Clone)") && GetClassRole() != "Boss" ){
		overlapUITimer = new Task( overlapTimer( 5f ) );
	}
}

IEnumerator overlapTimer( float waitTime ){
	yield return new WaitForSeconds( waitTime );
	Destroy( GameObject.Find("CharSelectUI(Clone)") );
}

public void SelectChar(){
	if( skillCdScript.skillActive == true ){
		skill_targetting.instance.currentTarget = characterScript.gameObject;
		print( skill_targetting.instance.currentTarget );
	} 
	if( skillCdScript.skillActive == false && characterScript.isAlive ){
		characterSelectScript.tankSelected = ConvertRoleToBool( GetClassRole(), "tankSelected" );
		characterSelectScript.healerSelected = ConvertRoleToBool( GetClassRole(), "healerSelected" );
		characterSelectScript.dpsSelected = ConvertRoleToBool( GetClassRole(), "dpsSelected" );
		DisplaySkillsSecond();
	}
}

bool isAlive( string charObj ){
    var charactersdata = GameObject.Find(charObj).GetComponent<character_data>();
    if( charactersdata.isAlive ){
            return true;
    }   else {
            return false;
    }
}

//class/object for selecting character
    public class classState{
        public string Name; //{ get; set; } 
        public bool Alive; //{ get; set; } 
        public bool Selected;
        public classState( string name, bool alive, bool selected ){
            Name = name;
            Alive = alive;
            Selected = selected;
        } 
    }

//Get a character that is alive and not selected and return it
string GetAlive( classState[] charClass ){
    for( int i=0; i < charClass.Length ; i++ ){
        if( charClass[i].Alive && !charClass[i].Selected ){
            return charClass[i].Name;
        }
    }
    return "bla";
}

//Change Character
void CharSwap(){
    if( characterSelectScript ){
//---------------------------------------- Tab Skill swap ------------------------------------------------//
        guardian.Selected = characterSelectScript.tankSelected;
        stalker.Selected = characterSelectScript.dpsSelected;
        walker.Selected = characterSelectScript.healerSelected;
    
        classState[] charClass = { guardian,stalker,walker };
        
        //string something = GetAlive( charClass );
        string swapTo = GetAlive( charClass );
        
        switch( swapTo ){
            case "guardian":
            characterSelectScript.tankSelected = guardian.Selected = true;
            characterSelectScript.dpsSelected = stalker.Selected = false;
            characterSelectScript.healerSelected = walker.Selected = false;
            DisplaySkillsSecond();
            break;
            case "stalker":
            characterSelectScript.healerSelected = walker.Selected = true;
            characterSelectScript.tankSelected = guardian.Selected = false;
            characterSelectScript.dpsSelected = stalker.Selected = false;
            DisplaySkillsSecond();
            break;
            case "walker":
            characterSelectScript.dpsSelected = stalker.Selected = true;
            characterSelectScript.tankSelected = guardian.Selected = false;
            characterSelectScript.healerSelected = walker.Selected = false;
            DisplaySkillsSecond();
            break;
        }
    	soundContScript.playSound( audioClip );
    }
}
	
	void Start(){
         guardian = new classState( "guardian", characterScript.isAlive, characterSelectScript.tankSelected );
         stalker = new classState( "stalker", characterScript.isAlive, characterSelectScript.dpsSelected );
         walker = new classState( "walker", characterScript.isAlive, characterSelectScript.healerSelected );
		 DisplaySkillsSecond();
	}

	// Use this for initialization
	void Awake () {
        globalScript = GameObject.Find("Main Camera");
		//gameEffectsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<gameEffects>();
		//statusScript = GetComponent<status>();
      	//skillBar.SetActive(false);
		characterSelectScript = globalScript.GetComponent<character_select>();
		characterScript = GetComponent<character_data>();
		skillCdScript = globalScript.GetComponent<skill_cd>();
		selectionOverlapScript = GetComponent<selectionOverlapControl>();
		//playerstatus = players.GetComponents<status>();
		//enemystatus = enemies.GetComponent<status>();
		soundContScript = GetComponent<soundController>();
	}
	
	// Update is called once per frame
	void Update () {
		//recieve Tab press
		if( Input.GetKeyUp( KeyCode.Tab ) && !skillCdScript.skillActive  ){
			CharSwap();
		}

	}
}
