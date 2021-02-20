using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace FFStudio
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "FF/Event/GameEvent")]
    public class GameEvent : ScriptableObject
    {

        private readonly List<EventListener> eventListeners =
            new List<EventListener>();

        [Button]
        public void Raise()
        {

            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventRaised();
        }

        public void RegisterListener(EventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(EventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }
}