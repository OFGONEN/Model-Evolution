using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
    public class SoundManager : MonoBehaviour
    {
        public SoundEvent[] soundEvents;

        List<EventListenerDelegateResponse> soundEventListeners;
        Dictionary<int, AudioSource> audioSources;
        private void Awake()
        {
            soundEventListeners = new List<EventListenerDelegateResponse>(soundEvents.Length);
            audioSources = new Dictionary<int, AudioSource>(soundEvents.Length);

            for (int i = 0; i < soundEvents.Length; i++)
            {
                var _audioComponent = gameObject.AddComponent<AudioSource>();
                _audioComponent.playOnAwake = false;
                _audioComponent.loop = false;

                var _audioEvent = soundEvents[i];

                _audioComponent.clip = _audioEvent.audioClip;
                audioSources.Add(_audioEvent.GetInstanceID(), _audioComponent);

                var _listener = new EventListenerDelegateResponse();
                _listener.gameEvent = _audioEvent;
                soundEventListeners.Add(_listener);

            }
        }
        private void OnEnable()
        {
            for (int i = 0; i < soundEvents.Length; i++)
            {
                soundEventListeners[i].OnEnable();
            }
        }
        private void OnDisable()
        {
            for (int i = 0; i < soundEvents.Length; i++)
            {
                soundEventListeners[i].OnDisable();
            }
        }
        private void Start()
        {
            foreach (var soundEventListener in soundEventListeners)
            {
                soundEventListener.response = (() => PlaySound(soundEventListener.gameEvent.GetInstanceID()));
            }
        }
        void PlaySound(int instanceId)
        {
            AudioSource _source;
            audioSources.TryGetValue(instanceId, out _source);

            if (_source != null && !_source.isPlaying)
            {
                _source.Play();
            }
        }
    }
}