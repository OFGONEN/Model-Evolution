using UnityEngine;

namespace FFStudio
{
    [CreateAssetMenu(fileName = "StringGameEvent", menuName = "FF/Event/StringGameEvent")]
    public class StringGameEvent : GameEvent
    {
        public string eventValue;
    }
}