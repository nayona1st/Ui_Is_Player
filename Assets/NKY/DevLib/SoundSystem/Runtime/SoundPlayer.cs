using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace DevLib.SoundSystem.Runtime
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup sfxGroup;
        [SerializeField] private AudioMixerGroup musicGroup;
        
        private AudioSource _audioSource;

        public event Action<SoundPlayer> OnSoundFinished;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound(SoundClipSO clipData)
        {
            if (clipData.audioType == AudioType.Sfx)
                _audioSource.outputAudioMixerGroup = sfxGroup;
            else if (clipData.audioType == AudioType.Music)
                _audioSource.outputAudioMixerGroup = musicGroup;
            
            _audioSource.volume = clipData.volume;
            _audioSource.pitch = clipData.pitch;
            
            if (clipData.randomizePitch)
                _audioSource.pitch += Random.Range(-clipData.randomPitchModifier ,  clipData.randomPitchModifier);
            
            _audioSource.clip = clipData.clip;
            _audioSource.loop = clipData.isLoop;
            
            float startTime = clipData.startTime;
            float endTime = clipData.endTime;
            
            _audioSource.timeSamples = Mathf.RoundToInt(startTime * clipData.clip.frequency); // frequency = 주파수
            _audioSource.Play();

            if (!clipData.isLoop)
            {
                float duration = (endTime - startTime) / Mathf.Abs(_audioSource.pitch);
                _ = DisableSoundTimer(duration + 0.2f); // 완전 안전하고 문제가 안생겨서 얘는 쓰고 버릴거임
            }
        }

        private async Task DisableSoundTimer(float time)
        {
            await Awaitable.WaitForSecondsAsync(time);
            _audioSource.Stop();
            OnSoundFinished?.Invoke(this);
        }

        public void ForceStopSound()
        {
            _audioSource.Stop();
            OnSoundFinished?.Invoke(this);
        }
    }
}