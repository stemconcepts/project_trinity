using UnityEngine;
using System.Collections;

public class character_select : MonoBehaviour {
	public bool tankSelected = true;
	public bool healerSelected = false;
	public bool dpsSelected = false;      

	//returns what character is selected as a ?
	public string GetClassRole(){
		if( tankSelected ){
			return "Guardian";
		} else if( healerSelected){
			return "Walker";
		} else if( dpsSelected ){
			return "Stalker";
		}
		return null;
	}

	public string GetClassRoleCaps(){
		if( tankSelected ){
			return "Tank";
		} else if( healerSelected){
			return "Healer";
		} else if( dpsSelected ){
			return "Dps";
		}
		return null;
	}

    public GameObject GetClassObject(){
        if( tankSelected ){
            return GameObject.Find("Guardian");
        } else if( healerSelected){
            return GameObject.Find("Walker");
        } else if( dpsSelected ){
            return GameObject.Find("Stalker");
        }
        return null;
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
