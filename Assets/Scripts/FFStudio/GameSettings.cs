using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class GameSettings : ScriptableObject
    {
        #region Fields
        public static GameSettings instance;

        public int maxLevelCount;
        [Tooltip("Duration of the movement for ui element")] public float uiEntityMoveTweenDuration;
		[Tooltip("Duration of the scaling for ui element")] public float uiEntityScaleTweenDuration;
		[Tooltip("Duration of the movement for floating ui element")] public float uiFloatingEntityTweenDuration;
        [Tooltip("Percentage of the screen to register a swipe")] public int swipeThreshold;
		

        #endregion

        #region UnityAPI

        private void Awake()
        {
            if(instance == null)
            {
				instance = this;
                FFLogger.Log( "GameSettings instance is set" );
            }
            else if (instance != this)
            {
				Destroy( this );
                FFLogger.Log( "New GameSettings Detected and Destroyed!" );
			}
		}
        #endregion
        

	}
}
