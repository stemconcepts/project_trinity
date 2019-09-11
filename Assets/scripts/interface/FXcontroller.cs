using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FXcontroller : MonoBehaviour {
	private soundController soundContScript;
    private ParticleSystem particleSystem = new ParticleSystem();
	[Header("Custom Sounds:")]
	public List<AudioClip> sounds;
    public GameObject destroyMeTrigger;
    private ParticleSystem childTrigger;

	void DestroyMe(){
		Destroy(gameObject);
	}

	// Use this for initialization
	void Start () {
		if( soundContScript != null ){
			soundContScript.playSounds( sounds );
		}
        childTrigger = destroyMeTrigger ? destroyMeTrigger.GetComponent<ParticleSystem>() : null;
	}

	void Awake(){
        particleSystem = GetComponent<ParticleSystem>();
		soundContScript = GetComponent<soundController>();
	}

	// Update is called once per frame
	void Update () {
        if( particleSystem && !particleSystem.IsAlive() ){
            DestroyMe();
        } else if ( childTrigger && !childTrigger.IsAlive() ){
            DestroyMe();
        }
	}
} 
