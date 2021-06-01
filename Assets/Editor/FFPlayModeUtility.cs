/* Created by and for usage of FF Studios (2021). */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FFEditor
{
    [InitializeOnLoad]
	public static class FFPlayModeUtility
	{
		public static FFPlayModeUtilitySettings playModeUtilitySettings;
		static FFPlayModeUtility()
        {
            EditorApplication.playModeStateChanged += PlayModeChange;

			//Create Play Mode Settings 
			var path_playModeUtilitySettings = "Assets/Editor/PlayModeUtilitySettings.asset";

			playModeUtilitySettings = AssetDatabase.LoadAssetAtPath( path_playModeUtilitySettings, typeof( FFPlayModeUtilitySettings ) ) as FFPlayModeUtilitySettings;

			if (playModeUtilitySettings == null)
            {
                playModeUtilitySettings = ScriptableObject.CreateInstance< FFPlayModeUtilitySettings >();

                AssetDatabase.CreateAsset( playModeUtilitySettings, path_playModeUtilitySettings);
                Debug.Log("PlayModeUtility Settings Created");
            }

        }

        static void PlayModeChange(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    if( playModeUtilitySettings.useDefaultScene )
					    SceneManager.LoadScene( playModeUtilitySettings.defaultSceneIndex, LoadSceneMode.Single );
					break;
                default:
                    return;
            }
        }


        [MenuItem("FFStudios/Create Play Mode Utility Settings")]
        static void CreatePlayModeUtilitySettings()
        {
			var path_playModeUtilitySettings = "Assets/Editor/PlayModeUtilitySettings.asset";

            playModeUtilitySettings = ScriptableObject.CreateInstance< FFPlayModeUtilitySettings >();

            AssetDatabase.CreateAsset( playModeUtilitySettings, path_playModeUtilitySettings);
            Debug.Log("PlayModeUtility Settings Created");
        }
	}
}