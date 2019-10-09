using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class equipedSkill : MonoBehaviour {
    private character_data characterScript;
    public classSkills skill;

	// Use this for initialization
	void Awake () {
		characterScript = GetComponent<character_data>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
