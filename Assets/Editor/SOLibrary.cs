/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FFEditor
{
	[ CreateAssetMenu( fileName = "TrackedSOLibrary", menuName = "FFEditor/TrackedSOLibrary" ) ]
    public class SOLibrary : ScriptableObject
    {
        public List< ScriptableObject > trackedScriptableObjects = null;

        [ HideInInspector ] public int trackedScriptablesObjectCount = 0;

		private void Awake()
		{
			if( trackedScriptableObjects == null )
				trackedScriptableObjects = new List< ScriptableObject >( 8 );

			trackedScriptablesObjectCount = trackedScriptableObjects.Count;
		}

		public void TrackScriptableObject( ScriptableObject sObject )
		{
			if( !trackedScriptableObjects.Contains( sObject ) )
				trackedScriptableObjects.Add( sObject );

			trackedScriptablesObjectCount = trackedScriptableObjects.Count;
		}

		public void UntrackScriptableObject( ScriptableObject sObject )
		{
			trackedScriptableObjects.Remove( sObject );

			trackedScriptablesObjectCount = trackedScriptableObjects.Count;
		}

		// Removes manually added scriptable objects
		private void OnValidate()
		{
			if( trackedScriptableObjects == null || trackedScriptableObjects.Count == 0 || trackedScriptableObjects.Count == trackedScriptablesObjectCount )
				return;

			Debug.LogError( "Do not add items to SOLibrary manually" );

			trackedScriptableObjects.RemoveRange( trackedScriptablesObjectCount, trackedScriptableObjects.Count - trackedScriptablesObjectCount );
			trackedScriptablesObjectCount = trackedScriptableObjects.Count;

			EditorUtility.SetDirty( this );
			AssetDatabase.SaveAssets();
		}
    }
}