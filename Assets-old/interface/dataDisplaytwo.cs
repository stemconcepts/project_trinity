using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class dataDisplaytwo : MonoBehaviour {

	public GameObject characterDataObject;
	private character_data characterDataScript;
	private Text displayText;
	public string healthdata;
    public bool On = true;

	void getData(){
		healthdata = characterDataScript.Health.ToString();
		displayText.text = healthdata;
	}
    
    public void SetDataObjects( int i ){
        characterDataObject = transform.parent.parent.parent.GetChild(2 + i).gameObject;
        characterDataScript = transform.parent.parent.parent.GetChild(2 + i).GetComponent<character_data>();
        displayText = GetComponent<Text>();
    }

	// Use this for initialization
	void Start () {
        if( On ){
            if( characterDataObject ){
    		    characterDataScript = characterDataObject.GetComponent<character_data>();
            } /*else {
                SetDataObjects();
            }*/
            displayText = GetComponent<Text>();
	    }
    }
	
	// Update is called once per frame
	void Update () {
        if( On ){
		    getData();
        }
	}
}
