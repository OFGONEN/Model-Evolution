/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;

namespace FFStudio
{
    public class CurrentLevelData : ScriptableObject
    {
#region Fields
		[ BoxGroup( "Shared" ) ] public DressDataList list_dressData;

		[ ReadOnly ] public int currentLevel_Real;
		[ ReadOnly ] public int currentLevel_Shown;
		[ ReadOnly ] public LevelData levelData;

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
			if( currentLevel_Real > GameSettings.Instance.maxLevelCount )
				currentLevel_Real = Random.Range( 1, GameSettings.Instance.maxLevelCount );

			levelData = Resources.Load< LevelData >( "level_data_" + currentLevel_Real );

            if( levelData.cloth_start_clean )
				list_dressData.ClearSet();
		}
#endregion

#region Implementation
        static CurrentLevelData LoadInstance()
		{
			if( instance == null )
				instance = Resources.Load< CurrentLevelData >( "level_current" );

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