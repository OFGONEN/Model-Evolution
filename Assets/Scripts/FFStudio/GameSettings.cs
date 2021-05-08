using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class GameSettings : ScriptableObject
    {
        #region Fields
        public int maxLevelCount;
        [Tooltip("Duration of the movement for ui element")] public float uiEntityMoveTweenDuration;
		[Tooltip("Duration of the scaling for ui element")] public float uiEntityScaleTweenDuration;
		[Tooltip("Duration of the movement for floating ui element")] public float uiFloatingEntityTweenDuration;
        [Tooltip("Percentage of the screen to register a swipe")] public int swipeThreshold;


        private static GameSettings instance;

        private delegate GameSettings ReturnGameSettings();
        private static ReturnGameSettings returnInstance = LoadInstance;

        public static GameSettings Instance
        {
            get
            {
                return returnInstance();
            }
        }
        #endregion

        #region Implementation
        static GameSettings LoadInstance()
        {
            if (instance == null)
                instance = Resources.Load<GameSettings>("game_settings");

            returnInstance = ReturnInstance;

            return instance;
        }

        static GameSettings ReturnInstance()
        {
            return instance;
        }
        #endregion
    }
}
