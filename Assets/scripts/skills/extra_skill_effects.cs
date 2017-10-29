using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Data{
	public float modifier = 1f;
	public List<GameObject> target;
	public enemySkill enemySkill;
	public classSkills classSkill;
}

public class extra_skill_effects : MonoBehaviour {
	//private status statusScript;
//	private spawnUI spawnUIscript;
	private skill_effects skillEffectScripts;

	// Use this for initialization
	void Start () {
		//statusScript = GetComponent<status>();
		//spawnUIscript = GetComponent<spawnUI>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
