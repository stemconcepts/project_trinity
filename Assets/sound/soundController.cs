using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class soundController : MonoBehaviour {
	[SpineAnimation]
	SkeletonAnimation skeletonAnimation;
	animationControl playerAnimationControl;
	private AudioSource audioSourceScript; 
	private AudioClip chosenSound;
	[Header("Swing Sounds:")]
	public List<AudioClip> swingSounds;
	[Header("Step Sounds:")]
	public List<AudioClip> stepSounds;
	[Header("Shout Sounds:")]
	public List<AudioClip> shoutSounds;
	[Header("Crash Sounds:")]
	public List<AudioClip> crashSounds;
	[Header("Custom Sounds:")]
	public List<AudioClip> customSounds;

	public void playSounds( List<AudioClip> sounds ){
		if( sounds.Count >= 0 ){
			var randomNumber = Random.Range (0, (sounds.Count) );
			audioSourceScript.clip = sounds[randomNumber];
			audioSourceScript.Play();
		} else {
			print("no sound to play");
		}
	}

	public void OnEventHit(Spine.TrackEntry state, Spine.Event e ){
		//if( e.Data.name != "movementStart" && e.Data.name != "movementBack" && e.Data.name != "SFX" && e.Data.name != "endEvent" ){
			audioSourceScript.volume = 0.3f;
			audioSourceScript.priority = 130;
			if ( e.Data.name == "swing" && swingSounds.Count > 0 ){
				var randomNumber = Random.Range (0, (swingSounds.Count) );
				chosenSound = swingSounds[randomNumber];
				audioSourceScript.clip = chosenSound;
				audioSourceScript.Play();
			} else if ( e.Data.name == "thud" ){
				var randomNumber = Random.Range (0, (stepSounds.Count) );
				chosenSound = stepSounds[randomNumber];
				audioSourceScript.volume = 1f;
				audioSourceScript.priority = 100;
				audioSourceScript.clip = chosenSound;
				audioSourceScript.Play();
			} else if ( e.Data.name == "shout" && shoutSounds.Count > 0 ){
				var randomNumber = Random.Range (0, (shoutSounds.Count) );
				chosenSound = shoutSounds[randomNumber];
				audioSourceScript.volume = 1f;
				audioSourceScript.priority = 100;
				audioSourceScript.clip = chosenSound;
				audioSourceScript.Play();
			} else if ( e.Data.name == "crash" && crashSounds.Count > 0 ){
				var randomNumber = Random.Range (0, (crashSounds.Count) );
				chosenSound = crashSounds[randomNumber];
				audioSourceScript.volume = 1f;
				audioSourceScript.clip = chosenSound;
				audioSourceScript.Play();
			}  
			
			//audioSourceScript.clip = null;
		//} 
		if( e.Data.name == "endEvent" ){
			//skeletonAnimation.state.Event -= OnEventHit;
		}
	}

	public void playSound( AudioClip audioClip = null ){		
		if( audioClip == null ){
			skeletonAnimation.state.Event += OnEventHit;
		} else {
			audioSourceScript.clip = audioClip;
			audioSourceScript.Play();
		}
	}

	// Use this for initialization
	void Awake () {
		audioSourceScript = GetComponent<AudioSource>();
		if ( this.transform.Find("Animations") ){
			skeletonAnimation = this.transform.Find("Animations").GetComponent<SkeletonAnimation>();
			playerAnimationControl = this.transform.Find("Animations").GetComponent<animationControl>();
		}
	}
	
	void Start () {
		if ( this.transform.Find("Animations") ){
			playSound();
		}
	}
}
