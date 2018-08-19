using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class equipmentManager: MonoBehaviour {
	public GameObject sceneControllerObject;
	sceneControl sceneControlScript;

	public static equipmentManager _Instance;

	public weapons tankWeaponObject;
	public weapons tankSecondWeaponObject;
    public bauble tankBaubleObject;
	public List<string> tankSkills = new List<string>();
	public classSkills tankClassSkill;

	public weapons healerWeaponObject;
	public weapons healerSecondWeaponObject;
    public bauble healerBaubleObject;
	public List<string> healerSkills = new List<string>();
	public classSkills healerClassSkill;

	public weapons dpsWeaponObject;
	public weapons dpsSecondWeaponObject;
	public bauble dpsBaubleObject;
	public List<string> dpsSkills = new List<string>();
	public classSkills dpsClassSkill;

	public static equipmentManager Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = Object.FindObjectOfType<equipmentManager>();
				
				//Don't destroy on load
				if (_Instance != null){
					DontDestroyOnLoad(_Instance.gameObject);
				}
			}
			
			return _Instance;
		}
	}
	
	void Awake(){
		sceneControlScript = sceneControllerObject.GetComponent<sceneControl>();

		if (_Instance == null)
		{
			//If this is the first instance then make this the singleton
			_Instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			//A singleton already exists and it is not this
			if (this != _Instance)
				Destroy(this.gameObject);
		}
	}

	void Update(){
		if( tankWeaponObject && tankSecondWeaponObject && tankClassSkill != null ){
			sceneControlScript.tankReady = true;
		}
		if( healerWeaponObject && healerSecondWeaponObject && healerClassSkill != null ){
			sceneControlScript.healerReady = true;
		}
		if( dpsWeaponObject && dpsSecondWeaponObject && dpsClassSkill != null ){
			sceneControlScript.dpsReady = true;
		}
	}
	
}
