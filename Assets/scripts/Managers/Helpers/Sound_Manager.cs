using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UIElements;
//using UnityEditorInternal;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine.Rendering;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class Sound_Manager : MonoBehaviour
    {
        public AudioSource mainMusicTrack;
        public AudioSource inputAudioSourceScript;
        public AudioSource musicAudioSourceScript;
        public AudioSource sfxAudioSourceScript;
        public List<AudioSource> backUpSourceScripts;
        private AudioClip chosenSound;
        public AudioClip gearSwapSound;
        public AudioClip gearSwapReady;
        [Header("UI Sounds:")]
        public List<AudioClip> uiSounds;
        [Header("Character Select Sound:")]
        public AudioClip charSelectSound;
        [Header("Dodge/Miss Sounds:")]
        public List<AudioClip> dodgeMissSounds;
        [Header("Generic Magic Charge Sounds:")]
        public List<AudioClip> magicChargeSounds;
        [Header("Generic Magic Cast Sounds:")]
        public List<AudioClip> magicCastSounds;
        [Header("Generic Buff Sounds:")]
        public AudioClip positiveEffectSound;
        [Header("Generic Debuff Sounds:")]
        public AudioClip negativeEffectSound;
        [Header("Hit Sounds:")]
        public List<AudioClip> hitSounds;
        [Header("Block Sounds:")]
        public List<AudioClip> blockSounds;
        [Header("Step Sounds:")]
        public List<AudioClip> stepSounds;
        [Header("Shout Sounds:")]
        public List<AudioClip> shoutSounds;
        [Header("Miss Sounds:")]
        public List<AudioClip> missSounds;
        [Header("Crash Sounds:")]
        public List<AudioClip> crashSounds;
        [Header("Custom Sounds:")]
        public List<AudioClip> customSounds;

        //[Header("Delegates/Events")]
        delegate void PlayDefaultClickSound();
        PlayDefaultClickSound mouseUpAction;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                TriggerSoundFromMouseEvent(true);
            }
            if (Input.GetMouseButtonUp(0))
            {
                TriggerSoundFromMouseEvent(false);
            }
        }

        void Start()
        {

        }

        AudioSource GetFreeAudioSource()
        {
            if (inputAudioSourceScript.isPlaying)
            {
                AudioSource freeSource = inputAudioSourceScript;
                foreach (AudioSource backUpSource in backUpSourceScripts)
                {
                    if (!backUpSource.isPlaying)
                    {
                        freeSource = backUpSource;
                        break;
                    }
                };
                return freeSource;
            }
            return inputAudioSourceScript;
        }

        public void playSound(List<AudioClip> sounds, float? volume = null)
        {
            var audioSource = GetFreeAudioSource();
            if (sounds.Count > 0)
            {
                var randomNumber = Random.Range(0, sounds.Count);
                audioSource.clip = sounds[randomNumber];
                audioSource.volume = volume == null ? 0.3f : (float)volume;
                audioSource.Play();
            }
            else
            {
                print("no sound to play");
            }
        }

        public void playAllSounds(List<AudioClip> sounds, float? volume = null)
        {
            if (sounds.Count >= 0)
            {
                foreach (AudioClip soundClip in sounds)
                {
                    var audioSource = GetFreeAudioSource();
                    audioSource.volume = volume == null ? 0.3f : (float)volume;
                    audioSource.PlayOneShot(soundClip);
                };
            }
        }

        /// <summary>
        /// Play sounds in order
        /// </summary>
        /// <param name="sounds"></param>
        public void playSoundsInOrder(List<AudioClip> sounds, bool playAtRandom, float? volume = null)
        {
            if (sounds.Count >= 0)
            {
                var audioSource = GetFreeAudioSource();
                int clipNumber = 0;
                float clipLength = 0.0f;
                System.Random rnd = new System.Random();
                sounds = playAtRandom ? sounds.OrderBy(o => rnd.Next()).ToList() : sounds;
                audioSource.volume = volume == null ? 0.3f : (float)volume;
                foreach (var sound in sounds)
                {
                    if (clipNumber != 0)
                    {
                        MainGameManager.instance.taskManager.CallTask(clipLength, () =>
                        {
                            //audioSource.clip = sound;
                            audioSource.PlayOneShot(sound);
                        });
                    }
                    else
                    {
                        audioSource.PlayOneShot(sound);
                    }
                    clipLength = sound.length;
                    clipNumber++;
                }
            }
            else
            {
                print("no sound to play");
            }
        }

        public void PlayCharacterSelectSound()
        {
            if (missSounds.Count > 0)
            {
                var audioSource = GetFreeAudioSource();
                audioSource.volume = 0.5f;
                audioSource.clip = charSelectSound;
                audioSource.Play();
            }
        }

        public void PlayMissSound()
        {
            if (missSounds.Count > 0)
            {
                var audioSource = GetFreeAudioSource();
                var randomNumber = Random.Range(0, (missSounds.Count));
                chosenSound = missSounds[randomNumber];
                audioSource.volume = 0.3f;
                audioSource.clip = chosenSound;
                audioSource.Play();
            }
        }

        public void PlayAAHitSound()
        {
            if (hitSounds.Count > 0)
            {
                var audioSource = GetFreeAudioSource();
                var randomNumber = Random.Range(0, (hitSounds.Count));
                chosenSound = hitSounds[randomNumber];
                audioSource.volume = 0.3f;
                audioSource.clip = chosenSound;
                audioSource.Play();
            }
        }

        public void PlayPositiveEffectSound()
        {
            if (positiveEffectSound != null)
            {
                var audioSource = GetFreeAudioSource();
                audioSource.volume = 0.3f;
                audioSource.clip = positiveEffectSound;
                audioSource.Play();
            }
        }

        public void PlayNegativeEffectSound()
        {
            if (negativeEffectSound != null)
            {
                var audioSource = GetFreeAudioSource();
                audioSource.volume = 0.3f;
                audioSource.clip = negativeEffectSound;
                audioSource.Play();
            }
        }

        public void OnEventHit(Spine.TrackEntry state, Spine.Event e)
        {
            var audioSource = GetFreeAudioSource();
            if (e.Data.Name == "thud")
            {
                var randomNumber = Random.Range(0, (stepSounds.Count));
                chosenSound = stepSounds[randomNumber];
                audioSource.volume = 0.3f;
                audioSource.clip = chosenSound;
                audioSource.Play();
            }
            /*else if (e.Data.Name == "shout" && shoutSounds.Count > 0)
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
            }*/
            if (e.Data.Name == "endEvent")
            {
                //skeletonAnimation.state.Event -= OnEventHit;
            }
        }

        public void playSound(AudioClip audioClip = null, SkeletonAnimation skeletonAnimation = null)
        {
            var audioSource = GetFreeAudioSource();
            if (audioClip == null && skeletonAnimation != null)
            {
                //skeletonAnimation.state.Event += OnEventHit; 
            }
            else
            {
                audioSource.clip = audioClip;
                audioSource.volume = 0.3f;
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
                case "block":
                    var randomNumber3 = Random.Range(0, (blockSounds.Count));
                    return blockSounds[randomNumber3];
                case "gearSwapSound":
                    return gearSwapSound;
                case "gearSwapReady":
                    return gearSwapReady;
                case "crafting":
                    return uiSounds[3];
                case "gearDrop":
                    return uiSounds[3];
                case "gearEquip":
                    return uiSounds[4];
                case "gearSelect":
                    return uiSounds[5];
                case "skill":
                    return stepSounds[1];
                default:
                    return uiSounds[2];
            }
        }

        public void playSoundUsingAudioSource(string sound, AudioSource source)
        {
            var audioSource = source;
            audioSource.pitch = 1;
            audioSource.clip = GetAudioFromString(sound);
            audioSource.Play();
        }

        public void playSound(string sound, float? volume = null)
        {
            var audioSource = inputAudioSourceScript;
            audioSource.clip = GetAudioFromString(sound);
            audioSource.volume = volume == null ? 1.0f : (float)volume;
            audioSource.Play();
        }

        public void ChangeMainMusicTrack(AudioClip music)
        {
            mainMusicTrack.Stop();
            mainMusicTrack.clip = music;
            mainMusicTrack.loop = true;
            mainMusicTrack.Play();
        }

        void TriggerSoundFromMouseEvent(bool mouseDown)
        {
            Vector3 mousePos = MainGameManager.instance.currentCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.transform && hit.transform.gameObject)
            {
                if (hit.transform.gameObject.GetComponent<ExplorerItemsController>())
                {
                    playSound("crafting");
                }
                else if (hit.transform.gameObject.GetComponent<equipControl>() && hit.transform.GetComponentInChildren<itemBehaviour>())
                {
                    if (!mouseDown)
                    {
                        playSound("gearEquip");
                    }
                }
                else if (hit.transform.gameObject.GetComponent<itemBehaviour>() || hit.transform.GetComponentInChildren<itemBehaviour>())
                {
                    if (mouseDown) 
                    { 
                        playSound("gearSelect");
                    }
                }
                else
                {
                    if (!mouseDown && !inputAudioSourceScript.isPlaying)
                    {
                        playSound("defaultClick", 0.3f);
                    }
                }
            } else
            {
                if (!mouseDown && !inputAudioSourceScript.isPlaying)
                {
                    playSound("defaultClick", 0.3f);
                }
            }
        }
    }
}

