using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElephantEvent
{
    LevelStarted,
    LevelCompleted,
    LevelFailed
}

namespace FFStudio
{
    [CreateAssetMenu(fileName = "ElephantEvent", menuName = "FF/Event/ElephantEvent")]
    public class ElephantLevelEvent : GameEvent
    {
        public ElephantEvent elephantEventType;
        public int level;
    }
}