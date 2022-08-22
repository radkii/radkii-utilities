using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Radkii.Sound
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Sound")]
    public class Sound : ScriptableObject {  
        public AudioClip clip;

        [HideInInspector]
        public AudioSource audioSource;

        public GameObject target;

        public string soundName;

        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 4f)]
        public float pitch = 1f;
        public SoundType soundType;

        public bool loop;
        public float customLoopTime;
    }

    public enum SoundType {Music, FX}
}