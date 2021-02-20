using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FFEditor
{
    [InitializeOnLoad, System.Serializable]
    public static class SOUtility
    {
        public static SOLibrary soLibrary = null;

        static SOUtility()
        {
            CreateFolders();

            EditorApplication.playModeStateChanged += PlayModeChange;

            soLibrary = AssetDatabase.LoadAssetAtPath("Assets/Editor/TrackedSOLib.asset", typeof(SOLibrary)) as SOLibrary;

            if (soLibrary == null)
            {
                soLibrary = ScriptableObject.CreateInstance<SOLibrary>();
                AssetDatabase.CreateAsset(soLibrary, "Assets/Editor/TrackedSOLib.asset");

                Debug.Log("Scriptable Object Library is Created!");
            }
        }

        static void PlayModeChange(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.ExitingPlayMode:
                    LoadAllDefaultSO();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    CreateAllDefaultSO();
                    break;
                default:
                    return;
            }
        }

        static void CreateFolders()
        {
            var _isExits = AssetDatabase.IsValidFolder("Assets/Editor/DefaultScriptableObjects");

            if (_isExits) return;

            AssetDatabase.CreateFolder("Assets/Editor", "DefaultScriptableObjects");
        }

        [MenuItem("FFStudios/SO/Track SO")]
        public static void TrackSO()
        {
            var _objects = Selection.objects;
            var _guids = Selection.assetGUIDs;

            if (_guids.Length != _objects.Length)
            {
                Debug.LogError("DO NOT SELECT FROM SCENE");
                EditorApplication.Beep();
                return;
            }

            for (int i = 0; i < _objects.Length; i++)
            {
                var _object = _objects[i];
                var _SO = _object as ScriptableObject;

                if (_SO == null) continue;

                var _path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]);

                var _assetName = "Default_" + Path.GetFileNameWithoutExtension(_path) + ".asset";
                var _newPath = Path.Combine("Assets/Editor/DefaultScriptableObjects", _assetName);

                AssetDatabase.CopyAsset(_path, _newPath);

                soLibrary.TrackSO(_SO);
            }

            EditorUtility.SetDirty(soLibrary);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("FFStudios/SO/UnTrack SO")]
        public static void UnTrackSO()
        {
            var _objects = Selection.objects;
            var _guids = Selection.assetGUIDs;

            if (_guids.Length != _objects.Length)
            {
                Debug.LogError("DO NOT SELECT FROM SCENE");
                EditorApplication.Beep();
                return;
            }

            for (int i = 0; i < _objects.Length; i++)
            {
                var _object = _objects[i];
                var _SO = _object as ScriptableObject;

                if (_SO == null) continue;

                var _path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]);

                var _assetName = "Default_" + Path.GetFileNameWithoutExtension(_path) + ".asset";
                var _newPath = Path.Combine("Assets/Editor/DefaultScriptableObjects", _assetName);

                AssetDatabase.DeleteAsset(_newPath);

                soLibrary.UnTrackSO(_SO);
            }

            EditorUtility.SetDirty(soLibrary);
            AssetDatabase.SaveAssets();
        }


        [MenuItem("FFStudios/SO/Load All Default SO")]
        public static void LoadAllDefaultSO()
        {
            var _trackedSO = soLibrary.trackedSO;

            for (int i = 0; i < _trackedSO.Count; i++)
            {
                LoadDefaultSO(_trackedSO[i]);
            }
        }

        [MenuItem("FFStudios/SO/Create All Default SO")]
        public static void CreateAllDefaultSO()
        {
            var _trackedSO = soLibrary.trackedSO;

            for (int i = 0; i < _trackedSO.Count; i++)
            {
                CreateDefaultSO(_trackedSO[i]);
            }
        }

        static void CreateDefaultSO(ScriptableObject sObject)
        {
            var _assetName = "Default_" + sObject.name + ".asset";
            var _newPath = Path.Combine("Assets/Editor/DefaultScriptableObjects", _assetName);

            string _sObjectGUID;
            long _sObjectLocalID;
            string _sObjectAssetPath;

            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sObject, out _sObjectGUID, out _sObjectLocalID);
            _sObjectAssetPath = AssetDatabase.GUIDToAssetPath(_sObjectGUID);
            AssetDatabase.CopyAsset(_sObjectAssetPath, _newPath);
        }
        static void LoadDefaultSO(ScriptableObject sObject)
        {
            ClearLog();


            var _assetName = "Default_" + sObject.name + ".asset";
            var _newPath = Path.Combine("Assets/Editor/DefaultScriptableObjects", _assetName);

            var _defaultSO = AssetDatabase.LoadAssetAtPath<ScriptableObject>(_newPath);

            if (_defaultSO == null)
            {
                Debug.LogError(sObject.name + " does not have default SO");
                EditorApplication.Beep();
                return;
            }

            var _defaultSOFields = _defaultSO.GetType().GetFields();
            var _SOFields = sObject.GetType().GetFields();

            for (int x = 0; x < _defaultSOFields.Length; x++)
            {
                _SOFields[x].SetValue(sObject, _defaultSOFields[x].GetValue(_defaultSO));
            }

            EditorUtility.SetDirty(sObject);

            AssetDatabase.SaveAssets();

        }
        private static void ClearLog()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

    }

}