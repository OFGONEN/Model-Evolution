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
            var path_GameSettings = Path.Combine(Application.dataPath, "Resources", "game_settings.asset");
            if (!File.Exists(path_GameSettings))
            {
                var gameSettings = ScriptableObject.CreateInstance<GameSettings>();

                AssetDatabase.CreateAsset(gameSettings, "Assets/Resources/game_settings.asset");
                Debug.Log("GameSettings Created");
            }

            //Create CurrentLevel
            var path_CurrentLevel = Path.Combine(Application.dataPath, "Resources", "level_current.asset");
            if (!File.Exists(path_CurrentLevel))
            {
                var gameSettings = ScriptableObject.CreateInstance<CurrentLevelData>();

                AssetDatabase.CreateAsset(gameSettings, "Assets/Resources/level_current.asset");
                Debug.Log("CurrentLevel Created");
            }
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
