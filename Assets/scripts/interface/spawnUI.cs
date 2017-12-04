using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class spawnUI : MonoBehaviour {
	private combatDisplay combatDisplayScript;
	//variables for status labels
	public GameObject status_symbol;
	//public statussinglelabel statuslabelScript;
	private status statusscript;
	public Transform status_position;
	//public Canvas canvas;
	public GameObject buffParent;
	public GameObject debuffParent;
	//public List<GameObject> objectlist = new List<GameObject>();
	public GameObject live_object;
	private int positionidbuff;
	private int positionid;

	//variables for damage data
	public GameObject dataObject;
	public Transform combatDataPosition;
	//public List<GameObject> datalist = new List<GameObject>();
	public GameObject live_dataObject;
    
    public void AddStacks(){

    }

	//controls damage/healing/absorb numbers
	public void ShowDamageNumber( float dmgValue, string dmgValueSource ){
			live_dataObject = (GameObject)Instantiate( dataObject, new Vector2 ( combatDataPosition.position.x , combatDataPosition.position.y + 6f ) , combatDataPosition.rotation );
			var damageData = live_dataObject.GetComponent<damagDataBehaviour>();
			damageData.damageData = (int)dmgValue;
			//damageData.damageData = (int)combatDisplayScript.displayDamageData;
			damageData.isDmg = true;
		damageData.skillLabel = dmgValueSource;
	}

	public void ShowHealNumber( float healValue ){
			live_dataObject = (GameObject)Instantiate( dataObject, new Vector2 ( combatDataPosition.position.x + 1f , combatDataPosition.position.y + 6f ) , combatDataPosition.rotation );
			var damageData = live_dataObject.GetComponent<damagDataBehaviour>();
			damageData.healData = (int)healValue;
			//damageData.healData = (int)combatDisplayScript.displayHealData;
			damageData.isDmg = false;
	}

	public void ShowAbsorbNumber( float absorbValue, string absorbValueSource ){
		live_dataObject = (GameObject)Instantiate( dataObject, new Vector2 ( combatDataPosition.position.x + 1f , combatDataPosition.position.y + 6f ) , combatDataPosition.rotation );
		var damageData = live_dataObject.GetComponent<damagDataBehaviour>();
		damageData.absorbData = (int)absorbValue;
		//damageData.healData = (int)combatDisplayScript.displayHealData;
		damageData.isAbsorb = true;
		damageData.skillLabel = absorbValueSource;
	}

	public void ShowImmune( string absorbValueSource ){
		live_dataObject = (GameObject)Instantiate( dataObject, new Vector2 ( combatDataPosition.position.x + 1f , combatDataPosition.position.y + 6f ) , combatDataPosition.rotation );
		var damageData = live_dataObject.GetComponent<damagDataBehaviour>();
		//damageData.absorbData = (int)absorbValue;
		//damageData.healData = (int)combatDisplayScript.displayHealData;
		damageData.isImmune = true;
		damageData.skillLabel = absorbValueSource;
	}

	//Controls status label spawning
	public void ShowLabel( singleStatus status ){
		
		//statuslabelScript.statusname = status.name;
		//statuslabelScript.statusIcon = status.labelIcon;
		
		//print("spawn prefab attempt" + statuslabelScript.statusname + statusname);
		if( status.buff ){
			live_object = (GameObject)Instantiate( status_symbol, new Vector3 ( status_position.position.x, 0.5f + status_position.position.y, status_position.position.z ) , status_position.rotation );
			//positionidbuff = positionidbuff + 1;
		} else {
			live_object = (GameObject)Instantiate( status_symbol, new Vector3 ( status_position.position.x, status_position.position.y, status_position.position.z  ) , status_position.rotation );
			//positionid = positionid + 1;
		}
		Image iconImageScript = live_object.GetComponent<Image>();
		iconImageScript.sprite = status.labelIcon;

		if( buffParent && status.buff ){
			live_object.transform.SetParent( buffParent.transform, true );
		} else
		if( debuffParent && !status.buff  ){
			live_object.transform.SetParent( debuffParent.transform, true );
		}
		var getprefab = live_object.GetComponent<statussinglelabel>();
		//objectlist.Insert ( statusscript.statuscount, live_object );
		//getprefab.labelid = statusscript.statuscount;
		getprefab.buff = status.buff;
		getprefab.singleStatus = status;
		getprefab.statusname = status.name;
		getprefab.dispellable = status.debuffable;
//		if( isbuff == true ){
//			getprefab.labelid = positionidbuff;
//		} else {
//			getprefab.labelid = positionid;
//		}
		//statusscript.buffposition = statusscript.buffposition + 1;
		//statusscript.statuscount += 1;
	}

	public void RemoveLabel( string statuslabel, bool isbuff ){
		//print ("label is " + statuslabel );
		if ( statusscript.DoesStatusExist(statuslabel) ){
			var chosenStatus = statusscript.GetStatusIfExist( statuslabel );
			//Destroy( chosenStatus.gameObject );
			chosenStatus.DestroyMe();
			//statusscript.statuscount -= 1;
		}
		/*foreach( GameObject labelobject in objectlist ) {
			if( labelobject ){
			//	print (labelobject);
				var currentlabelscript = labelobject.GetComponent<statussinglelabel>();
				var labelname = currentlabelscript.statusname;
				var labelnumber = currentlabelscript.labelid;
				//print (labelname + " " + statuslabel + " " + labelnumber);
				if( labelname == statuslabel && statusscript.statuscount > 0 ){
					Destroy(objectlist[labelnumber]);
					statusscript.statuscount = statusscript.statuscount - 1;
					//objectlist.RemoveAt(labelnumber);
//					if( isbuff == true && positionidbuff > 0 ){
//						//statusscript.buffposition = statusscript.buffposition - 1;
//						positionidbuff = positionidbuff - 1;
//					}else
//					if( isbuff == false && positionid > 0 ){
//						positionid = positionid - 1;
//					}
				} 
			}
		}*/
	}

//	public void RemoveLabel( string statuslabel, bool statusname ){
//		print ("label is " + statuslabel + " name is " + statusname);
//		int labelnumber = this.statuslabelScript.labelid;
//		var currentobj = objectlist[labelnumber].GetComponent<statussinglelabel>();
//		print ( currentobj.statusname );
//		if( currentobj.statusname == statuslabel ){
//			print ("removal attempt");
//			string labelname = this.statuslabelScript.statusname;
//			print ( labelnumber + labelname );
//			Destroy( objectlist[labelnumber] );
//			//Destroy( objectlist[0] );
//			print ( objectlist[labelnumber] + "removed" );
//
//			objectlist.RemoveAt(labelnumber);
//
//			//if ( statusscript.statuscount > 0 ){
//				statusscript.statuscount = statusscript.statuscount - 1 ;
//			//}
//		}
//	}

    void Start(){
        if( !status_position ){
            var statusBox = transform.parent.GetChild(1).GetChild(0).gameObject;
            status_position = statusBox.transform;
            buffParent = statusBox.transform.GetChild(0).gameObject;
            debuffParent = statusBox.transform.GetChild(1).gameObject;
            combatDataPosition = gameObject.transform;
        }
    }

	// Use this for initialization
	void Awake () {
		//statuslabelScript = status_symbol.GetComponent<statussinglelabel>();
		statusscript = GetComponent<status>();
		combatDisplayScript = GetComponent<combatDisplay>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
