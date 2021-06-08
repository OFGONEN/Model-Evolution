using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FFEditor
{
	public static class FFShortcutUtility
	{
		static Transform currentTransform;

		[MenuItem("FFShortcut/TakeScreenShot #F12")]
        public static void TakeScreenShot()
        {
			int counter = 0;
			var path = Path.Combine( Application.dataPath, "../" , "ScreenShot_" + counter + ".png" );

			while (File.Exists(path)) // If file is not exits new screen shot will be a new file
            {
				counter++;
				path = Path.Combine( Application.dataPath, "../", "ScreenShot_" + counter + ".png" ); // ScreenShot_1.png
			}

            ScreenCapture.CaptureScreenshot("ScreenShot_" + counter + ".png");
			AssetDatabase.SaveAssets();

			Debug.Log( "ScreenShot Taken: " + "ScreenShot_" + counter + ".png" );
		}

        [MenuItem("FFShortcut/Delete PlayerPrefs _F9")]
        static void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
			Debug.Log("PlayerPrefs Deleted");
        }

        [MenuItem("FFShortcut/Previous Level _F10")]
        static void PreviousLevel()
        {
			var currentLevel = PlayerPrefs.GetInt( "Level" );

			currentLevel = Mathf.Max( currentLevel - 1, 1 );

			PlayerPrefs.SetInt( "Level", currentLevel );
			PlayerPrefs.SetInt( "Consecutive Level", currentLevel );

			Debug.Log( "Level Set:" + currentLevel );
		}

        [MenuItem("FFShortcut/Next Level _F11")]
        static void NextLevel()
        {
			var nextLevel = PlayerPrefs.GetInt( "Level" ) + 1;

			PlayerPrefs.SetInt( "Level", nextLevel );
			PlayerPrefs.SetInt( "Consecutive Level", nextLevel );

			Debug.Log( "Level Set:" + nextLevel );

        }


        [MenuItem("FFStudios/Save All Assets _F12")]
        static void SaveAllAssets()
        {
            AssetDatabase.SaveAssets();
			Debug.Log( "AssetDatabase Saved" );
		}

        [MenuItem("FFStudios/Select Level Data &1")]
		static void SelectLevelData()
		{
			var levelData = Resources.Load( "LevelData_1" );

			Selection.SetActiveObjectWithContext( levelData, levelData );
		}

        [MenuItem("FFStudios/Select Game Settings &2")]
		static void SelectGameSettings()
		{
			var gameSettings = Resources.Load( "game_settings" );

			Selection.SetActiveObjectWithContext( gameSettings, gameSettings );
		}

        [MenuItem("FFStudios/Select App Scene &3")]
		static void SelectAppScene()
		{
			var gameSettings = AssetDatabase.LoadAssetAtPath( "Assets/Scenes/app.unity", typeof(SceneAsset) );

			Selection.SetActiveObjectWithContext( gameSettings, gameSettings );
		}
		
		[MenuItem("FFStudios/Copy Transform &c")]
		static void CopyTransform()
		{
			currentTransform = Selection.activeGameObject.transform;
		}

        [MenuItem("FFStudios/Paste Transform &v")]
		static void PasteTransform()
		{
			var gameObject = Selection.activeGameObject.transform;

			var target             = gameObject.transform;
				target.position    = currentTransform.position;
				target.eulerAngles = currentTransform.eulerAngles;
				target.localScale  = currentTransform.localScale;
		}
	}
}