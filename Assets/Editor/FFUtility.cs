using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using System.IO;
using UnityEditor.Build;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Reporting;

namespace FFEditor
{
    [InitializeOnLoad]
    public static class FFUtility
    {
		static GameSettings gameSettings;
		static bool inited;

		static FFUtility()
        {
            if (!inited)
            {
                EditorApplication.playModeStateChanged += PlayModeChange;
				inited = true;
			}

            //Create GameSettings 
            var path_GameSettings = "Assets/Resources/game_settings.asset";
			gameSettings = AssetDatabase.LoadAssetAtPath( path_GameSettings, typeof( GameSettings ) ) as GameSettings;

			if ( gameSettings == null )
            {
                gameSettings = ScriptableObject.CreateInstance<GameSettings>();

                AssetDatabase.CreateAsset(gameSettings, "Assets/Resources/game_settings.asset");
                Debug.Log("GameSettings Created");
            }

			//Create CurrentLevel
            var path_CurrentLevel = "Assets/Resources/level_current.asset";
			var currentLevel = AssetDatabase.LoadAssetAtPath( path_CurrentLevel, typeof( CurrentLevelData ) ) as CurrentLevelData;

			if ( currentLevel == null )
            {
                currentLevel = ScriptableObject.CreateInstance< CurrentLevelData >();

                AssetDatabase.CreateAsset(currentLevel, "Assets/Resources/level_current.asset");
                Debug.Log("CurrentLevel Created");
            }
        }

		[MenuItem( "FFStudios/Update LevelDatas" )]
		public static void LevelDatas()
		{
			var maxLevelCount = GameSettings.Instance.maxLevelCount;

			for( var i = 1; i <= maxLevelCount; i++ )
			{
				var levelData = Resources.Load<LevelData>( "LevelData_" + i );
				levelData.sceneIndex = i + 1;
				EditorUtility.SetDirty( levelData );
			}

			AssetDatabase.SaveAssets();
		}

        [MenuItem("FFStudios/Sort Folder _F7")]
        static void SortFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                AssetDatabase.CreateFolder("Assets", "Materials");

            if (!AssetDatabase.IsValidFolder("Assets/Textures"))
                AssetDatabase.CreateFolder("Assets", "Textures");

            if (!AssetDatabase.IsValidFolder("Assets/Shaders"))
                AssetDatabase.CreateFolder("Assets", "Shaders");

            if (!AssetDatabase.IsValidFolder("Assets/Models"))
                AssetDatabase.CreateFolder("Assets", "Models");

            var undefinedTypes = new HashSet<string>();

            var context = Selection.assetGUIDs;
            var path = AssetDatabase.GUIDToAssetPath(context[0]);
            var parentFolderName = Path.GetFileName(path);

            Debug.Log("Folder Path:" + path);

            string[] guids = AssetDatabase.FindAssets("t:Object", new[] { path });

            foreach (string guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var fileName = Path.GetFileName(assetPath);

                if (fileName.ToLower().Contains(".prefab"))
                {
                    var newPath = $"Assets/Prefabs/{parentFolderName}/{fileName}";

                    if (!AssetDatabase.IsValidFolder("Assets/Prefabs/" + parentFolderName))
                    {
                        AssetDatabase.CreateFolder("Assets/Prefabs", parentFolderName);
                        AssetDatabase.Refresh();
                    }

                    if (AssetDatabase.ValidateMoveAsset(assetPath, newPath) == string.Empty)
                        AssetDatabase.MoveAsset(assetPath, newPath);
                    else
                        TryToMove(assetPath, "Prefabs/" + parentFolderName, 1);
                }
                else if (fileName.ToLower().Contains(".fbx"))
                {
                    var newPath = $"Assets/Models/{parentFolderName}/{fileName}";

                    if (!AssetDatabase.IsValidFolder("Assets/Models/" + parentFolderName))
                    {
                        AssetDatabase.CreateFolder("Assets/Models", parentFolderName);
                        AssetDatabase.Refresh();
                    }

                    if (AssetDatabase.ValidateMoveAsset(assetPath, newPath) == string.Empty)
                        AssetDatabase.MoveAsset(assetPath, newPath);
                    else
                        TryToMove(assetPath, "Models/" + parentFolderName, 1);
                }
                else if (fileName.ToLower().Contains(".obj"))
                {
                    var newPath = $"Assets/Models/{parentFolderName}/{fileName}";

                    if (!AssetDatabase.IsValidFolder("Assets/Models/" + parentFolderName))
                    {
                        AssetDatabase.CreateFolder("Assets/Models", parentFolderName);
                        AssetDatabase.Refresh();
                    }

                    if (AssetDatabase.ValidateMoveAsset(assetPath, newPath) == string.Empty)
                        AssetDatabase.MoveAsset(assetPath, newPath);
                    else
                        TryToMove(assetPath, "Models/" + parentFolderName, 1);
                }
                else if (fileName.ToLower().Contains(".mat"))
                {
                    var newPath = $"Assets/Materials/{parentFolderName}/{fileName}";

                    if (!AssetDatabase.IsValidFolder("Assets/Materials/" + parentFolderName))
                    {
                        AssetDatabase.CreateFolder("Assets/Materials", parentFolderName);
                        AssetDatabase.Refresh();
                    }

                    if (AssetDatabase.ValidateMoveAsset(assetPath, newPath) == string.Empty)
                        AssetDatabase.MoveAsset(assetPath, newPath);
                    else
                        TryToMove(assetPath, "Materials/" + parentFolderName, 1);
                }
                else if (fileName.ToLower().Contains(".png"))
                {
                    var newPath = $"Assets/Textures/{parentFolderName}/{fileName}";

                    if (!AssetDatabase.IsValidFolder("Assets/Textures/" + parentFolderName))
                    {
                        AssetDatabase.CreateFolder("Assets/Textures", parentFolderName);
                        AssetDatabase.Refresh();
                    }

                    if (AssetDatabase.ValidateMoveAsset(assetPath, newPath) == string.Empty)
                        AssetDatabase.MoveAsset(assetPath, newPath);
                    else
                        TryToMove(assetPath, "Textures/" + parentFolderName, 1);
                }
                else if (fileName.ToLower().Contains(".jpg"))
                {
                    var newPath = $"Assets/Textures/{parentFolderName}/{fileName}";

                    if (!AssetDatabase.IsValidFolder("Assets/Textures/" + parentFolderName))
                    {
                        AssetDatabase.CreateFolder("Assets/Textures", parentFolderName);
                        AssetDatabase.Refresh();
                    }

                    if (AssetDatabase.ValidateMoveAsset(assetPath, newPath) == string.Empty)
                        AssetDatabase.MoveAsset(assetPath, newPath);
                    else
                        TryToMove(assetPath, "Textures/" + parentFolderName, 1);
                }
                else if (fileName.ToLower().Contains(".tga"))
                {
                    var newPath = $"Assets/Textures/{parentFolderName}/{fileName}";

                    if (!AssetDatabase.IsValidFolder("Assets/Textures/" + parentFolderName))
                    {
                        AssetDatabase.CreateFolder("Assets/Textures", parentFolderName);
                        AssetDatabase.Refresh();
                    }

                    if (AssetDatabase.ValidateMoveAsset(assetPath, newPath) == string.Empty)
                        AssetDatabase.MoveAsset(assetPath, newPath);
                    else
                        TryToMove(assetPath, "Textures/" + parentFolderName, 1);
                }
                else if (fileName.ToLower().Contains(".shader"))
                {
                    var newPath = $"Assets/Shaders/{parentFolderName}/{fileName}";

                    if (!AssetDatabase.IsValidFolder("Assets/Shaders/" + parentFolderName))
                    {
                        AssetDatabase.CreateFolder("Assets/Shaders", parentFolderName);
                        AssetDatabase.Refresh();
                    }

                    if (AssetDatabase.ValidateMoveAsset(assetPath, newPath) == string.Empty)
                        AssetDatabase.MoveAsset(assetPath, newPath);
                    else
                        TryToMove(assetPath, "Shaders/" + parentFolderName, 1);
                }
                else if (fileName.ToLower().Contains(".cginc"))
                {
                    var newPath = $"Assets/Shaders/{parentFolderName}/{fileName}";

                    if (!AssetDatabase.IsValidFolder("Assets/Shaders/" + parentFolderName))
                    {
                        AssetDatabase.CreateFolder("Assets/Shaders", parentFolderName);
                        AssetDatabase.Refresh();
                    }

                    if (AssetDatabase.ValidateMoveAsset(assetPath, newPath) == string.Empty)
                        AssetDatabase.MoveAsset(assetPath, newPath);
                    else
                        TryToMove(assetPath, "Shaders/" + parentFolderName, 1);
                }
                else if (fileName.ToLower().Contains(".controller"))
                {
                    var newPath = $"Assets/Animation/{parentFolderName}/{fileName}";

                    if (!AssetDatabase.IsValidFolder("Assets/Animation/" + parentFolderName))
                    {
                        AssetDatabase.CreateFolder("Assets/Animation", parentFolderName);
                        AssetDatabase.Refresh();
                    }

                    if (AssetDatabase.ValidateMoveAsset(assetPath, newPath) == string.Empty)
                        AssetDatabase.MoveAsset(assetPath, newPath);
                    else
                        TryToMove(assetPath, "Animation/" + parentFolderName, 1);
                }
                else if (fileName.ToLower().Contains(".mask"))
                {
                    var newPath = $"Assets/Animation/{parentFolderName}/{fileName}";

                    if (!AssetDatabase.IsValidFolder("Assets/Animation/" + parentFolderName))
                    {
                        AssetDatabase.CreateFolder("Assets/Animation", parentFolderName);
                        AssetDatabase.Refresh();
                    }

                    if (AssetDatabase.ValidateMoveAsset(assetPath, newPath) == string.Empty)
                        AssetDatabase.MoveAsset(assetPath, newPath);
                    else
                        TryToMove(assetPath, "Animation/" + parentFolderName, 1);
                }
                else if (fileName.ToLower().Contains(".unity"))
                {
                    var newPath = $"Assets/Scenes/{parentFolderName}/{fileName}";

                    if (AssetDatabase.ValidateMoveAsset(assetPath, newPath) == string.Empty)
                        AssetDatabase.MoveAsset(assetPath, newPath);
                    else
                        TryToMove(assetPath, "Scenes/" + parentFolderName, 1);
                }
                else if (fileName.ToLower().Contains(".lighting"))
                {
                    var newPath = $"Assets/Scenes/{parentFolderName}/{fileName}";

                    if (!AssetDatabase.IsValidFolder("Assets/Scenes/" + parentFolderName))
                    {
                        AssetDatabase.CreateFolder("Assets/Scenes", parentFolderName);
                        AssetDatabase.Refresh();
                    }

                    if (AssetDatabase.ValidateMoveAsset(assetPath, newPath) == string.Empty)
                        AssetDatabase.MoveAsset(assetPath, newPath);
                    else
                        TryToMove(assetPath, "Scenes/" + parentFolderName, 1);
                }
                else
                {
                    var extension = Path.GetExtension(fileName);
                    undefinedTypes.Add(extension);
                }
            }

            foreach (var extension in undefinedTypes)
            {
                Debug.Log("Undefined Type: " + extension); // empty string is the folder type
            }

            AssetDatabase.Refresh();
        }

        static void TryToMove(string path, string folderName, int index)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var fileExtension = Path.GetExtension(path);

            fileName = fileName + "_" + index + fileExtension;

            var newPath = $"Assets/{folderName}/{fileName}";

            if (AssetDatabase.ValidateMoveAsset(path, newPath) == string.Empty)
            {
                AssetDatabase.MoveAsset(path, newPath);
            }
            else
                TryToMove(path, folderName, index + 1);
        }
        static void PlayModeChange(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredPlayMode:
					SetMaxLevelForGameSettings();
					break;
                default:
                    return;
            }
        }

        [MenuItem("FFStudios/Set Max Level Count for Game Settings")]
        public static void SetMaxLevelForGameSettings()
        {
			string[] guids = AssetDatabase.FindAssets( "LevelData_ t:levelData", new[] { "Assets/Resources" } );

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