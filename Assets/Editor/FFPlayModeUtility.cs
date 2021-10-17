/* Created by and for usage of FF Studios (2021). */

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FFEditor
{
    [ InitializeOnLoad ]
	public static class FFPlayModeUtility
	{
		static private FFPlayModeUtilitySettings playModeUtilitySettings;
		static private bool initialized = false;

		static FFPlayModeUtility()
		{
			if( !initialized )
			{
				EditorApplication.playModeStateChanged += PlayModeChange;
				initialized = true;
			}

			//Find PlayModeUtilitySettings file 
			var path_playModeUtilitySettings = "Assets/Editor/PlayModeUtilitySettings.asset";
			    playModeUtilitySettings      = AssetDatabase.LoadAssetAtPath( path_playModeUtilitySettings, 
                                                                     typeof( FFPlayModeUtilitySettings ) ) as FFPlayModeUtilitySettings;

			if( playModeUtilitySettings == null )
			{
				Debug.LogError( "PlayModeUtilitySettings is not found" );
			}
		}

		static void PlayModeChange( PlayModeStateChange change )
		{
			switch( change )
			{
				case PlayModeStateChange.EnteredPlayMode:
					if( playModeUtilitySettings.useDefaultScene )
					{
						var loadedScene = SceneManager.GetActiveScene();

						if( loadedScene.buildIndex != playModeUtilitySettings.defaultSceneIndex )
							SceneManager.LoadScene( playModeUtilitySettings.defaultSceneIndex, LoadSceneMode.Single );
					}
					break;

				default:
					return;
			}
		}
	}
}