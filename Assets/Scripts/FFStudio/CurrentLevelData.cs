/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
    public class CurrentLevelData : ScriptableObject
    {
#region Fields
		public int currentLevel;
		public int currentConsecutiveLevel;
		public LevelData levelData;

        private static CurrentLevelData instance;

        private delegate CurrentLevelData ReturnCurrentLevel();
        private static ReturnCurrentLevel returnInstance = LoadInstance;

        public static CurrentLevelData Instance
        {
            get
            {
                return returnInstance();
            }
        }
#endregion

#region API
		public void LoadCurrentLevelData()
		{
			if( currentLevel > GameSettings.Instance.maxLevelCount )
				currentLevel = Random.Range( 1, GameSettings.Instance.maxLevelCount );

			levelData = Resources.Load<LevelData>( "level_data_" + currentLevel );
		}
#endregion

#region Implementation
        static CurrentLevelData LoadInstance()
		{
			if( instance == null )
				instance = Resources.Load<CurrentLevelData>( "level_current" );

			returnInstance = ReturnInstance;

            return instance;
        }

        static CurrentLevelData ReturnInstance()
        {
            return instance;
        }
#endregion
    }
}