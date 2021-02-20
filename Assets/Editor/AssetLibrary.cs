using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FFEditor
{
    public class AssetLibrary : ScriptableObject
    {
        public List<GameObject> trackedAssets = null;

        public int trackedAssetCount = 0;

        private void Awake()
        {
            if (trackedAssets == null)
                trackedAssets = new List<GameObject>(8);

            trackedAssetCount = trackedAssets.Count;
        }

        public void TrackAsset(GameObject gameObject)
        {
            if (trackedAssets.Contains(gameObject))
            {
                Debug.LogWarning("PreFab is already tracked");
                return;
            }

            trackedAssets.Add(gameObject);
            trackedAssetCount = trackedAssets.Count;
        }

        public void UnTrackAsset(GameObject gameObject)
        {
            if (!trackedAssets.Contains(gameObject))
            {
                Debug.LogWarning("PreFab is not in the Library");
                return;
            }

            trackedAssets.Remove(gameObject);
            trackedAssetCount = trackedAssets.Count;

            Debug.LogWarning("PreFab: " + gameObject + " is untracked");
        }

        private void OnValidate()
        {
            if (trackedAssets == null || trackedAssets.Count == 0 || trackedAssets.Count == trackedAssetCount) return;

            Debug.LogError("Do not add items to SOLibrary manually");

            trackedAssets.RemoveRange(trackedAssetCount, trackedAssets.Count - trackedAssetCount);
            trackedAssetCount = trackedAssets.Count;

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }

}