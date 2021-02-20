using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FFEditor
{
    [CreateAssetMenu]
    public class SOLibrary : ScriptableObject
    {
        public List<ScriptableObject> trackedSO = null;

        public int trackedSOCount = 0;


        private void Awake()
        {
            if (trackedSO == null)
                trackedSO = new List<ScriptableObject>(8);

            trackedSOCount = trackedSO.Count;
        }

        public void TrackSO(ScriptableObject sObject)
        {
            if (!trackedSO.Contains(sObject))
                trackedSO.Add(sObject);

            trackedSOCount = trackedSO.Count;
        }

        public void UnTrackSO(ScriptableObject sObject)
        {
            var _removed = trackedSO.Remove(sObject);

            trackedSOCount = trackedSO.Count;
        }

        private void OnValidate()
        {
            if (trackedSO == null || trackedSO.Count == 0 || trackedSO.Count == trackedSOCount) return;

            Debug.LogError("Do not add items to SOLibrary manually");

            trackedSO.RemoveRange(trackedSOCount, trackedSO.Count - trackedSOCount);
            trackedSOCount = trackedSO.Count;

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }

}