/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace FFStudio
{
	public class GameSettings : ScriptableObject
    {
#region Singleton Related
        private static GameSettings instance;

        private delegate GameSettings ReturnGameSettings();
        private static ReturnGameSettings returnInstance = LoadInstance;

		public static GameSettings Instance => returnInstance();
#endregion
        
#region Fields
        [ BoxGroup( "Remote Config" ) ] public bool useRemoteConfig_GameSettings;
        [ BoxGroup( "Remote Config" ) ] public bool useRemoteConfig_Components;

        public int maxLevelCount;
        [ BoxGroup( "UI Setting" ), Tooltip( "Duration of the movement for ui element"          ) ] public float ui_Entity_Move_TweenDuration;
        [ BoxGroup( "UI Setting" ), Tooltip( "Duration of the fading for ui element"            ) ] public float ui_Entity_Fade_TweenDuration;
		[ BoxGroup( "UI Setting" ), Tooltip( "Duration of the scaling for ui element"           ) ] public float ui_Entity_Scale_TweenDuration;
		[ BoxGroup( "UI Setting" ), Tooltip( "Duration of the movement for floating ui element" ) ] public float ui_Entity_FloatingMove_TweenDuration;
		[ BoxGroup( "UI Setting" ), Tooltip( "Joy Stick"                                        ) ] public float ui_Entity_JoyStick_Gap;
        [ BoxGroup( "UI Setting" ), Tooltip( "Percentage of the screen to register a swipe"     ) ] public int swipeThreshold;

		[ BoxGroup( "UI PopUp Setting" ) ] public float ui_PopUp_height;
		[ BoxGroup( "UI PopUp Setting" ) ] public float ui_PopUp_duration;

		[ BoxGroup( "Movement" ) ] public Ease movement_path_ease;
		[ BoxGroup( "Movement" ) ] public float movement_clampDistance;
		[ BoxGroup( "Movement" ) ] public float movement_speed_forward;
		[ BoxGroup( "Movement" ) ] public float movement_speed_lateral;

        [ BoxGroup( "Debug" ) ] public float debug_ui_text_float_height;
        [ BoxGroup( "Debug" ) ] public float debug_ui_text_float_duration;
#endregion

#region Implementation
        private static GameSettings LoadInstance()
		{
			if( instance == null )
				instance = Resources.Load< GameSettings >( "game_settings" );

			returnInstance = ReturnInstance;

			return instance;
		}

		private static GameSettings ReturnInstance()
        {
            return instance;
        }
#endregion
    }
}
