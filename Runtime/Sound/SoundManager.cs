using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;
//using Radkii.Sound;

namespace Radkii.Sound
{
	public class SoundManager : MonoBehaviour {

		public static SoundManager Instance { get; private set; }

		public AudioMixer mainMixer;
		public Sound[] sounds;

		private void Awake()
		{
			//This allows for easy access through any script
			#region Singleton
			if (Instance == null)
			{
				Instance = this;
				//DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
			#endregion

			foreach (Sound sound in sounds)
			{
				if(sound.target == null)
				{
					sound.audioSource = gameObject.AddComponent<AudioSource>();
				}
				else
				{
					sound.audioSource = sound.target.AddComponent<AudioSource>();
				}

				sound.audioSource.clip = sound.clip;
				sound.audioSource.volume = sound.volume;
				sound.audioSource.pitch = sound.pitch;
				sound.audioSource.loop = sound.loop;

				sound.audioSource.outputAudioMixerGroup = mainMixer.FindMatchingGroups($"Master/{sound.soundType}")[0];
			}
		}

		#region Play
		public void PlaySound(string name)
		{
			Sound s = Array.Find(sounds, sound => sound.soundName == name);

			if(s == null)
			{
				Debug.LogError("Could not find " + name + " sound inside sounds array");
				return;
			}

			if(s.loop && s.customLoopTime > s.clip.length + 0.01f)
			{
				s.audioSource.loop = false;
				StartCoroutine(CustomLoopPlay(s));
			}

			s.audioSource.Play();
		}

		public void PlaySound(int index)
		{
			Sound s = sounds[index];

			if (s == null)
			{
				Debug.LogError("Could not find sound with index " + index + " inside sounds array");
				return;
			}

			if (s.loop && s.customLoopTime > s.clip.length + 0.01f)
			{
				s.audioSource.loop = false;
				StartCoroutine(CustomLoopPlay(s));
			}

			s.audioSource.Play();
		}
		#endregion

		#region Play FadeIn
		public void PlaySoundFadeIn(string name, float time, int step)
		{
			StartCoroutine(PlaySoundFadeInEnum(name, time, step));
		}

		public void PlaySoundFadeIn(int index, float time, int step)
		{
			StartCoroutine(PlaySoundFadeInEnum(index, time, step));
		}

		IEnumerator PlaySoundFadeInEnum(string name, float time, int step)
		{
			Sound s = Array.Find(sounds, sound => sound.soundName == name);

			if (s == null)
			{
				Debug.LogError("Could not find " + name + " sound inside sounds array");
				yield break;
			}

			s.audioSource.volume = 0f;
			s.audioSource.Play();

			float posTime = time / step;
			float posVolume = s.volume / step;
			for (int i = 1; i < step + 1; i++)
			{
				yield return new WaitForSeconds(posTime);
				s.audioSource.volume += posVolume;
			}
			s.audioSource.volume = s.volume;
		}

		IEnumerator PlaySoundFadeInEnum(int index, float time, int step)
		{
			Sound s = sounds[index];

			if (s == null)
			{
				Debug.LogError("Could not find sound with index " + index + " inside sounds array");
				yield break;
			}

			s.audioSource.volume = 0f;
			s.audioSource.Play();

			float posTime = time / step;
			float posVolume = s.volume / step;
			for (int i = 1; i < step + 1; i++)
			{
				yield return new WaitForSeconds(posTime);
				s.audioSource.volume += posVolume;
			}
			s.audioSource.volume = s.volume;
		}
		#endregion

		#region Stop
		public void StopSound(string name)
		{
			Sound s = Array.Find(sounds, sound => sound.soundName == name);

			if (s == null)
			{
				Debug.LogError("Could not find " + name + " sound inside sounds array");
				return;
			}

			if (s.loop && s.customLoopTime > s.clip.length + 0.01f)
			{
				s.audioSource.loop = true;
				StopAllCoroutines();
			}

			s.audioSource.Stop();
		}

		public void StopSound(int index)
		{
			Sound s = sounds[index];

			if (s == null)
			{
				Debug.LogError("Could not find sound with index " + index + " inside sounds array");
				return;
			}

			if (s.loop && s.customLoopTime > s.clip.length + 0.01f)
			{
				s.audioSource.loop = true;
				StopAllCoroutines();
			}

			s.audioSource.Stop();
		}
		#endregion

		#region Stop FadeOut
		public void StopSoundFadeOut(string name, float time, int step)
		{
			StartCoroutine(StopSoundFadeOutEnum(name, time, step));
		}

		public void StopSoundFadeOut(int index, float time, int step)
		{
			StartCoroutine(StopSoundFadeOutEnum(index, time, step));
		}
	
		IEnumerator StopSoundFadeOutEnum(string name, float time, int step)
		{
			Sound s = Array.Find(sounds, sound => sound.name == name);

			if (s == null)
			{
				Debug.LogError("Could not find " + name + " sound inside sounds array");
				yield break;
			}

			StopAllCoroutines();

			float posTime = time / step;
			float posVolume = s.audioSource.volume / step;
			for (int i = 1; i < step + 1; i++)
			{
				yield return new WaitForSeconds(posTime);
				s.audioSource.volume -= posVolume;
			}
			s.audioSource.volume = 0f;
			s.audioSource.Stop();
		}

		IEnumerator StopSoundFadeOutEnum(int index, float time, int step)
		{
			Sound s = sounds[index];

			if (s == null)
			{
				Debug.LogError("Could not find sound with index " + index + " inside sounds array");
				yield break;
			}

			StopAllCoroutines();

			float posTime = time / step;
			float posVolume = s.audioSource.volume / step;
			for (int i = 1; i < step + 1; i++)
			{
				yield return new WaitForSeconds(posTime);
				s.audioSource.volume -= posVolume;
			}
			s.audioSource.volume = 0f;
			s.audioSource.Stop();
		}
		#endregion

		#region Pause
		public void PauseSound(string name)
		{
			Sound s = Array.Find(sounds, sound => sound.soundName == name);

			if (s == null)
			{
				Debug.LogError("Could not find " + name + " sound inside sounds array");
				return;
			}

			s.audioSource.Pause();
		}

		public void PauseSound(int index)
		{
			Sound s = sounds[index];

			if (s == null)
			{
				Debug.LogError("Could not find sound with index " + index + " inside sounds array");
				return;
			}

			s.audioSource.Pause();
		}

		public void UnPauseSound(string name)
		{
			Sound s = Array.Find(sounds, sound => sound.soundName == name);

			if (s == null)
			{
				Debug.LogError("Could not find " + name + " sound inside sounds array");
				return;
			}

			s.audioSource.UnPause();
		}

		public void UnPauseSound(int index)
		{
			Sound s = sounds[index];

			if (s == null)
			{
				Debug.LogError("Could not find sound with index " + index + " inside sounds array");
				return;
			}

			s.audioSource.UnPause();
		}
		#endregion

		#region For Each Type
		public void PlayAll(SoundType type)
		{
			foreach(Sound s in sounds)
			{
				if(s.soundType == type) { PlaySound(s.name); }
			}
		}

		public void PlayAllFadeIn(SoundType type, float time, int step)
		{
			foreach (Sound s in sounds)
			{
				if (s.soundType == type) { PlaySoundFadeIn(s.name, time, step); }
			}
		}

		public void StopAll(SoundType type)
		{
			foreach (Sound s in sounds)
			{
				if (s.soundType == type) { StopSound(s.name); }
			}
		}

		public void StopAllFadeOut(SoundType type, float time, int step)
		{
			foreach (Sound s in sounds)
			{
				if (s.soundType == type) { StopSoundFadeOut(s.name, time, step); }
			}
		}
		#endregion

		IEnumerator CustomLoopPlay(Sound s)
		{
			while (true)
			{
				s.audioSource.Play();
				yield return new WaitForSeconds(s.customLoopTime);
			}
		}

		public void SetMixerGroupVolume(SoundType type, float volume)
		{
			mainMixer.SetFloat($"_{type}Volume", volume);
			//return (float volume) => { mainMixer.SetFloat($"_{type}Volume", volume); };
		}
	}
}

