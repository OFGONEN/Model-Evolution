﻿/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FFStudio;
using System.IO;
using UnityEditor.Build;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Reporting;

namespace FFEditor
{
    [ InitializeOnLoad ]
    public static class FFUtility
    {
		private static GameSettings gameSettings;
		private static bool initialized;

		static FFUtility()
        {
			if( !initialized )
			{
				EditorApplication.playModeStateChanged += PlayModeChange;
				initialized = true;
			}

			var path_GameSettings = "Assets/Resources/game_settings.asset";
			gameSettings = AssetDatabase.LoadAssetAtPath( path_GameSettings, typeof( GameSettings ) ) as GameSettings;
		}

		[ MenuItem( "FFStudios/Set LevelDatas" ) ]
		public static void SetLevelDatas()
		{
			var maxLevelCount = GameSettings.Instance.maxLevelCount;

			for( var i = 1; i <= maxLevelCount; i++ )
			{
				var levelData = Resources.Load< LevelData >( "level_data_" + i );
				levelData.sceneIndex = i;
				EditorUtility.SetDirty( levelData );
			}

			AssetDatabase.SaveAssets();
		}

		[ MenuItem( "FFStudios/Update LevelDatas" ) ]
		public static void LevelDatas()
		{
			var maxLevelCount = GameSettings.Instance.maxLevelCount;

			for( var i = 1; i <= maxLevelCount; i++ )
			{
				var levelData = Resources.Load< LevelData >( "level_data_" + i );
				levelData.sceneIndex = i + 1;
				EditorUtility.SetDirty( levelData );
			}

			AssetDatabase.SaveAssets();
		}

		[ MenuItem( "FFStudios/Sort Folder _F7" ) ]
		private static void SortFolder()
		{
			var undefinedTypes = new HashSet< string >();

			var context = Selection.assetGUIDs;
			var path = AssetDatabase.GUIDToAssetPath( context[ 0 ] );
			var parentFolderName = Path.GetFileName( path );

			string[] guids = AssetDatabase.FindAssets( "t:Object", new[] { path } );

			foreach( string guid in guids )
			{
				var assetPath = AssetDatabase.GUIDToAssetPath( guid );
				var fileName = Path.GetFileName( assetPath );

				if( fileName.ToLower().Contains( ".prefab" ) )
				{
					var newPath = $"Assets/Prefab/{parentFolderName}/{fileName}";

					if( !AssetDatabase.IsValidFolder( "Assets/Prefab/" + parentFolderName ) )
					{
						AssetDatabase.CreateFolder( "Assets/Prefab", parentFolderName );
						AssetDatabase.Refresh();
					}

					if( AssetDatabase.ValidateMoveAsset( assetPath, newPath ) == string.Empty )
						AssetDatabase.MoveAsset( assetPath, newPath );
					else
						TryToMove( assetPath, "Prefab/" + parentFolderName, 1 );
				}
				else if( fileName.ToLower().Contains( ".fbx" ) )
				{
					var newPath = $"Assets/Model/{parentFolderName}/{fileName}";

					if( !AssetDatabase.IsValidFolder( "Assets/Model/" + parentFolderName ) )
					{
						AssetDatabase.CreateFolder( "Assets/Model", parentFolderName );
						AssetDatabase.Refresh();
					}

					if( AssetDatabase.ValidateMoveAsset( assetPath, newPath ) == string.Empty )
						AssetDatabase.MoveAsset( assetPath, newPath );
					else
						TryToMove( assetPath, "Model/" + parentFolderName, 1 );
				}
				else if( fileName.ToLower().Contains( ".obj" ) )
				{
					var newPath = $"Assets/Model/{parentFolderName}/{fileName}";

					if( !AssetDatabase.IsValidFolder( "Assets/Model/" + parentFolderName ) )
					{
						AssetDatabase.CreateFolder( "Assets/Model", parentFolderName );
						AssetDatabase.Refresh();
					}

					if( AssetDatabase.ValidateMoveAsset( assetPath, newPath ) == string.Empty )
						AssetDatabase.MoveAsset( assetPath, newPath );
					else
						TryToMove( assetPath, "Model/" + parentFolderName, 1 );
				}
				else if( fileName.ToLower().Contains( ".mat" ) )
				{
					var newPath = $"Assets/Material/{parentFolderName}/{fileName}";

					if( !AssetDatabase.IsValidFolder( "Assets/Material/" + parentFolderName ) )
					{
						AssetDatabase.CreateFolder( "Assets/Material", parentFolderName );
						AssetDatabase.Refresh();
					}

					if( AssetDatabase.ValidateMoveAsset( assetPath, newPath ) == string.Empty )
						AssetDatabase.MoveAsset( assetPath, newPath );
					else
						TryToMove( assetPath, "Material/" + parentFolderName, 1 );
				}
				else if( fileName.ToLower().Contains( ".png" ) )
				{
					var newPath = $"Assets/Texture/{parentFolderName}/{fileName}";

					if( !AssetDatabase.IsValidFolder( "Assets/Texture/" + parentFolderName ) )
					{
						AssetDatabase.CreateFolder( "Assets/Texture", parentFolderName );
						AssetDatabase.Refresh();
					}

					if( AssetDatabase.ValidateMoveAsset( assetPath, newPath ) == string.Empty )
						AssetDatabase.MoveAsset( assetPath, newPath );
					else
						TryToMove( assetPath, "Texture/" + parentFolderName, 1 );
				}
				else if( fileName.ToLower().Contains( ".jpg" ) )
				{
					var newPath = $"Assets/Texture/{parentFolderName}/{fileName}";

					if( !AssetDatabase.IsValidFolder( "Assets/Texture/" + parentFolderName ) )
					{
						AssetDatabase.CreateFolder( "Assets/Texture", parentFolderName );
						AssetDatabase.Refresh();
					}

					if( AssetDatabase.ValidateMoveAsset( assetPath, newPath ) == string.Empty )
						AssetDatabase.MoveAsset( assetPath, newPath );
					else
						TryToMove( assetPath, "Texture/" + parentFolderName, 1 );
				}
				else if( fileName.ToLower().Contains( ".tga" ) )
				{
					var newPath = $"Assets/Texture/{parentFolderName}/{fileName}";

					if( !AssetDatabase.IsValidFolder( "Assets/Texture/" + parentFolderName ) )
					{
						AssetDatabase.CreateFolder( "Assets/Texture", parentFolderName );
						AssetDatabase.Refresh();
					}

					if( AssetDatabase.ValidateMoveAsset( assetPath, newPath ) == string.Empty )
						AssetDatabase.MoveAsset( assetPath, newPath );
					else
						TryToMove( assetPath, "Texture/" + parentFolderName, 1 );
				}
				else if( fileName.ToLower().Contains( ".shader" ) )
				{
					var newPath = $"Assets/Shader/{parentFolderName}/{fileName}";

					if( !AssetDatabase.IsValidFolder( "Assets/Shader/" + parentFolderName ) )
					{
						AssetDatabase.CreateFolder( "Assets/Shader", parentFolderName );
						AssetDatabase.Refresh();
					}

					if( AssetDatabase.ValidateMoveAsset( assetPath, newPath ) == string.Empty )
						AssetDatabase.MoveAsset( assetPath, newPath );
					else
						TryToMove( assetPath, "Shader/" + parentFolderName, 1 );
				}
				else if( fileName.ToLower().Contains( ".cginc" ) )
				{
					var newPath = $"Assets/Shader/{parentFolderName}/{fileName}";

					if( !AssetDatabase.IsValidFolder( "Assets/Shader/" + parentFolderName ) )
					{
						AssetDatabase.CreateFolder( "Assets/Shader", parentFolderName );
						AssetDatabase.Refresh();
					}

					if( AssetDatabase.ValidateMoveAsset( assetPath, newPath ) == string.Empty )
						AssetDatabase.MoveAsset( assetPath, newPath );
					else
						TryToMove( assetPath, "Shader/" + parentFolderName, 1 );
				}
				else if( fileName.ToLower().Contains( ".controller" ) )
				{
					var newPath = $"Assets/Animation/{parentFolderName}/{fileName}";

					if( !AssetDatabase.IsValidFolder( "Assets/Animation/" + parentFolderName ) )
					{
						AssetDatabase.CreateFolder( "Assets/Animation", parentFolderName );
						AssetDatabase.Refresh();
					}

					if( AssetDatabase.ValidateMoveAsset( assetPath, newPath ) == string.Empty )
						AssetDatabase.MoveAsset( assetPath, newPath );
					else
						TryToMove( assetPath, "Animation/" + parentFolderName, 1 );
				}
				else if( fileName.ToLower().Contains( ".mask" ) )
				{
					var newPath = $"Assets/Animation/{parentFolderName}/{fileName}";

					if( !AssetDatabase.IsValidFolder( "Assets/Animation/" + parentFolderName ) )
					{
						AssetDatabase.CreateFolder( "Assets/Animation", parentFolderName );
						AssetDatabase.Refresh();
					}

					if( AssetDatabase.ValidateMoveAsset( assetPath, newPath ) == string.Empty )
						AssetDatabase.MoveAsset( assetPath, newPath );
					else
						TryToMove( assetPath, "Animation/" + parentFolderName, 1 );
				}
				else if( fileName.ToLower().Contains( ".unity" ) )
				{
					var newPath = $"Assets/Scenes/{parentFolderName}/{fileName}";

					if( AssetDatabase.ValidateMoveAsset( assetPath, newPath ) == string.Empty )
						AssetDatabase.MoveAsset( assetPath, newPath );
					else
						TryToMove( assetPath, "Scenes/" + parentFolderName, 1 );
				}
				else if( fileName.ToLower().Contains( ".lighting" ) )
				{
					var newPath = $"Assets/Scenes/{parentFolderName}/{fileName}";

					if( !AssetDatabase.IsValidFolder( "Assets/Scenes/" + parentFolderName ) )
					{
						AssetDatabase.CreateFolder( "Assets/Scenes", parentFolderName );
						AssetDatabase.Refresh();
					}

					if( AssetDatabase.ValidateMoveAsset( assetPath, newPath ) == string.Empty )
						AssetDatabase.MoveAsset( assetPath, newPath );
					else
						TryToMove( assetPath, "Scenes/" + parentFolderName, 1 );
				}
				else
				{
					var extension = Path.GetExtension( fileName );
					undefinedTypes.Add( extension );
				}
			}

			foreach( var extension in undefinedTypes )
				Debug.Log( "Undefined Type: " + extension ); // empty string is the folder type

			AssetDatabase.Refresh();
		}

		private static void TryToMove( string path, string folderName, int index )
		{
			var fileName = Path.GetFileNameWithoutExtension( path );
			var fileExtension = Path.GetExtension( path );

			fileName = fileName + "_" + index + fileExtension;

			var newPath = $"Assets/{folderName}/{fileName}";

			if( AssetDatabase.ValidateMoveAsset( path, newPath ) == string.Empty )
				AssetDatabase.MoveAsset( path, newPath );
			else
				TryToMove( path, folderName, index + 1 );
		}
		
		private static void PlayModeChange( PlayModeStateChange change )
		{
			switch( change )
			{
				case PlayModeStateChange.EnteredPlayMode:
					SetMaxLevelForGameSettings();
					break;
				default:
					return;
			}
		}

		[ MenuItem( "FFStudios/Set Max Level Count for Game Settings" ) ]
		public static void SetMaxLevelForGameSettings()
		{
			string[] guids = AssetDatabase.FindAssets( "level_data_ t:levelData", new[] { "Assets/Resources" } );

			if( gameSettings == null )
			{
				var path_GameSettings = "Assets/Resources/game_settings.asset";
				gameSettings = AssetDatabase.LoadAssetAtPath( path_GameSettings, typeof( GameSettings ) ) as GameSettings;
			}

			gameSettings.maxLevelCount = guids.Length;

			EditorUtility.SetDirty( gameSettings );
			AssetDatabase.SaveAssets();

			Debug.Log( "Game Settings max level count: " + gameSettings.maxLevelCount );
		}
	}
}

namespace FFEditor
{
	public class FFUtilityBuildProcessor : IProcessSceneWithReport
	{
		public int callbackOrder { get { return 0; } }

		public void OnProcessScene( Scene scene, BuildReport report )
		{
			FFUtility.SetMaxLevelForGameSettings();
		}
	}
}