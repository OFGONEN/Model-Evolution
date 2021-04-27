using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
    public class CurrentLevelData : ScriptableObject
    {
		#region Fields
		public static CurrentLevelData instance;

		public int currentLevel;
		public int currentConsecutiveLevel;
		public LevelData levelData;
		#endregion

		#region UnityAPI
		private void Awake()
		{
			if(instance == null)
			{
				instance = this;
				FFLogger.Log( "CurrentLevelData instance is set" );
			}
			else if (instance != this)
			{
				Destroy( this );
                FFLogger.Log( "New CurrentLevelData Detected and Destroyed!" );
			}
		}
			
		#endregion

		#region API
		public void LoadCurrentLevelData()
		{
			if( currentLevel > GameSettings.instance.maxLevelCount )
			{
				currentLevel = Random.Range( 1, GameSettings.instance.maxLevelCount );
			}

			levelData = Resources.Load<LevelData>( "LevelData_" + currentLevel );
		}
		#endregion
    }
}