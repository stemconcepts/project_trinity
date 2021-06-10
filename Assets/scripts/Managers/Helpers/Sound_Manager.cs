using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class Sound_Manager : MonoBehaviour
    {
        //SkeletonAnimation skeletonAnimation;
        //Animation_Manager playerAnimationManager;
        public AudioSource audioSourceScript;
        public List<AudioSource> audioSourceScripts;
        private AudioClip chosenSound;
        public AudioClip charSwapSound;
        public AudioClip gearSwapSound;
        public AudioClip gearSwapReady;
        [Header("UI Sounds:")]
        public List<AudioClip> uiSounds;
        [Header("Dodge/Miss Sounds:")]
        public List<AudioClip> dodgeMissSounds;
        [Header("Generic Magic Charge Sounds:")]
        public List<AudioClip> magicChargeSounds;
        [Header("Generic Magic Cast Sounds:")]
        public List<AudioClip> magicCastSounds;
        [Header("Generic Buff Sounds:")]
        public List<AudioClip> buffSounds;
        [Header("Generic Debuff Sounds:")]
        public List<AudioClip> debuffSounds;
        [Header("Swing Sounds:")]
        public List<AudioClip> swingSounds;
        [Header("Block Sounds:")]
        public List<AudioClip> blockSounds;
        [Header("Step Sounds:")]
        public List<AudioClip> stepSounds;
        [Header("Shout Sounds:")]
        public List<AudioClip> shoutSounds;
        [Header("Crash Sounds:")]
        public List<AudioClip> crashSounds;
        [Header("Custom Sounds:")]
        public List<AudioClip> customSounds;

        void Start()
        {
            audioSourceScript.volume = 0.3f;
            audioSourceScript.priority = 130;
            audioSourceScripts.ForEach(o =>
            {
                o.volume = 0.3f;
                o.priority = 130;
            });
        }

        public void playSounds( List<AudioClip> sounds ){
            var audioSource = audioSourceScript;
            if ( sounds.Count >= 0 ){
                var randomNumber = Random.Range (0, (sounds.Count) );
                if (audioSource.isPlaying)
                {
                    audioSourceScripts.ForEach(o =>
                    {
                        if (!o.isPlaying)
                        {
                            audioSource = o;
                        }
                    });
                }
                audioSource.clip = sounds[randomNumber];
                audioSource.Play();
            } else {
                print("no sound to play");
            }
        }
    
        public void OnEventHit(Spine.TrackEntry state, Spine.Event e ){
            var audioSource = audioSourceScript;
            if (audioSource.isPlaying)
            {
                audioSourceScripts.ForEach(o =>
                {
                    if (!o.isPlaying)
                    {
                        audioSource = o;
                    }
                });
            }
            if ((e.Data.Name == "hit") && swingSounds.Count > 0)
            {
                var randomNumber = Random.Range(0, (swingSounds.Count));
                chosenSound = swingSounds[randomNumber];
                audioSource.clip = chosenSound;
                audioSource.Play();
            } 
            else if (e.Data.Name == "thud" || e.Data.Name.Contains("movement"))
            {
                var randomNumber = Random.Range(0, (stepSounds.Count));
                chosenSound = stepSounds[randomNumber];
                audioSource.volume = 1f;
                audioSource.priority = 100;
                audioSource.clip = chosenSound;
                audioSource.Play();
            }
            else if (e.Data.Name == "shout" && shoutSounds.Count > 0)
            {
                var randomNumber = Random.Range(0, (shoutSounds.Count));
                chosenSound = shoutSounds[randomNumber];
                audioSource.volume = 0.5f;
                audioSource.priority = 100;
                audioSource.clip = chosenSound;
                audioSource.Play();
            }
            else if (e.Data.Name == "crash" && crashSounds.Count > 0)
            {
                var randomNumber = Random.Range(0, (crashSounds.Count));
                chosenSound = crashSounds[randomNumber];
                audioSource.volume = 0.5f;
                audioSource.clip = chosenSound;
                audioSource.Play();
            }  
            if( e.Data.Name == "endEvent" ){
                //skeletonAnimation.state.Event -= OnEventHit;
            }
        }
    
        public void playSound( AudioClip audioClip = null, SkeletonAnimation skeletonAnimation = null ){
            var audioSource = audioSourceScript;
            if (audioSource.isPlaying)
            {
                audioSourceScripts.ForEach(o =>
                {
                    if (!o.isPlaying)
                    {
                        audioSource = o;
                    }
                });
            }
            if ( audioClip == null && skeletonAnimation != null ){
                //skeletonAnimation.state.Event += OnEventHit; 
            } else {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }

        public void playSoundUsingAudioSource(AudioClip audioClip, AudioSource source)
        {
            var audioSource = source;
            audioSource.pitch = 1;
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        private AudioClip GetAudioFromString(string sound)
        {
            switch (sound)
            {
                case "buff":
                    var randomNumber0 = Random.Range(0, (buffSounds.Count));
                    return buffSounds[randomNumber0];
                case "debuff":
                    var randomNumber1 = Random.Range(0, (debuffSounds.Count));
                    return debuffSounds[randomNumber1];
                case "miss":
                    var randomNumber2 = Random.Range(0, (crashSounds.Count));
                    return dodgeMissSounds[randomNumber2];
                case "block":
                    var randomNumber3 = Random.Range(0, (blockSounds.Count));
                    return blockSounds[randomNumber3];
                case "gearSwapSound":
                    return gearSwapSound;
                case "gearSwapReady":
                    return gearSwapReady;
                default:
                    return null;
            }
        }

        public void playSoundUsingAudioSource(string sound, AudioSource source)
        {
            var audioSource = source;
            audioSource.pitch = 1;
            audioSource.clip = GetAudioFromString(sound);
            audioSource.Play();
        }

        public void playSound( string sound ){
            var audioSource = audioSourceScript;
            if (audioSource.isPlaying)
            {
                audioSourceScripts.ForEach(o =>
                {
                    if (!o.isPlaying)
                    {
                        audioSource = o;
                    }
                });
            }
            audioSource.clip = GetAudioFromString(sound);
            audioSource.Play();
        }
    }
}

