using UnityEditor;
using UnityEngine;
using System.IO;
namespace FFEditor
{
    public class SOAssetProcess : UnityEditor.AssetModificationProcessor
    {
        static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
        {
            var _fileName = Path.GetFileNameWithoutExtension(path);

            var _soLibrary = SOUtility.soLibrary;
            var _trackedSO = _soLibrary.trackedSO;

            for (int i = 0; i < _trackedSO.Count; i++)
            {
                var _sObject = _trackedSO[i];

                if (_fileName == _sObject.name)
                {
                    var _assetName = "Default_" + _sObject.name + ".asset";
                    var _deletePath = Path.Combine("Assets/Editor/DefaultScriptableObjects", _assetName);


                    _trackedSO.RemoveAt(i);
                    _soLibrary.trackedSOCount = _trackedSO.Count;

                    EditorUtility.SetDirty(_soLibrary);

                    AssetDatabase.DeleteAsset(_deletePath);
                    AssetDatabase.SaveAssets();

                    break;
                }
            }

            return AssetDeleteResult.DidNotDelete;
        }

    }

}