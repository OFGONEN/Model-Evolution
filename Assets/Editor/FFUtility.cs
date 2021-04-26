using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using System.IO;

namespace FFEditor
{
    [InitializeOnLoad]
    public static class FFUtility
    {
        static FFUtility()
        {
            EditorApplication.playModeStateChanged += PlayModeChange;

			//Create GameSettings 
			var path_GameSettings = Path.Combine( Application.dataPath, "ScriptableObjects", "GameSettings.asset" );
			if(!File.Exists(path_GameSettings))
            {
				var gameSettings = ScriptableObject.CreateInstance< GameSettings >();

				AssetDatabase.CreateAsset( gameSettings, "Assets/ScriptableObjects/GameSettings.asset" );
				Debug.Log( "GameSettings Created" );
			}

			//Create CurrentLevel
			var path_CurrentLevel = Path.Combine( Application.dataPath, "ScriptableObjects", "CurrentLevel.asset" );
			if(!File.Exists(path_CurrentLevel))
            {
				var gameSettings = ScriptableObject.CreateInstance< CurrentLevelData >();

				AssetDatabase.CreateAsset( gameSettings, "Assets/ScriptableObjects/CurrentLevel.asset" );
				Debug.Log( "CurrentLevel Created" );
			}
        }

        static void PlayModeChange(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.ExitingPlayMode:
                    // DOTween.KillAll();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                default:
                    return;
            }
        }
    }
}
