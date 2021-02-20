using UnityEngine;

namespace FFStudio
{
    [CreateAssetMenu(fileName = "BoolGameEvent", menuName = "FF/Event/BoolGameEvent")]
    public class BoolGameEvent : GameEvent
    {
        public bool eventValue;
    }
}