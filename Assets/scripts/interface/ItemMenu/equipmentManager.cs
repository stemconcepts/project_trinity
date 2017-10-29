using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class equipmentManager: MonoBehaviour {
	public GameObject sceneControllerObject;
	sceneControl sceneControlScript;

	public static equipmentManager _Instance;

	public string tankWeapon;
	public weapons tankWeaponObject;
	public weapons tankSecondWeaponObject;
	public classSkills tankSkillsObject;
	public List<string> tankSkills = new List<string>();
	public string tankClassSkill;

	public string healerWeapon;
	public weapons healerWeaponObject;
	public weapons healerSecondWeaponObject;
	public List<string> healerSkills = new List<string>();
	public string healerClassSkill;

	public string dpsWeapon;
	public weapons dpsWeaponObject;
	public weapons dpsSecondWeaponObject;
	//public string dpsPassives;
	public List<string> dpsSkills = new List<string>();
	public string dpsClassSkill;

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
		if( tankWeapon != "" && tankClassSkill != "" ){
			sceneControlScript.tankReady = true;
		}
		if( healerWeapon != "" && healerClassSkill != "" ){
			sceneControlScript.healerReady = true;
		}
		if( dpsWeapon != "" && dpsClassSkill != "" ){
			sceneControlScript.dpsReady = true;
		}
	}
	
}
