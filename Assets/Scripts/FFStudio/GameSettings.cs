using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class GameSettings : ScriptableObject
    {
        public int maxLevelCount;
        public float uiEntityTweenDuration;
        public float uiFloatingEntityTweenDuration;
        [Tooltip("Percentage of the screen to register a swipe")]
        public int swipeThreshold;
    }
}
