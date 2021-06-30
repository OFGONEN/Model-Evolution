/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FFEditor
{
    [ CreateAssetMenu ]
    public class SOLibrary : ScriptableObject
    {
        public List< ScriptableObject > trackedScriptableObject = null;

        public int trackedScriptableObjectCount = 0;

		private void Awake()
		{
			if( trackedScriptableObject == null )
				trackedScriptableObject = new List< ScriptableObject >( 8 );

			trackedScriptableObjectCount = trackedScriptableObject.Count;
		}

		public void TrackScriptableObject( ScriptableObject sObject )
		{
			if( !trackedScriptableObject.Contains( sObject ) )
				trackedScriptableObject.Add( sObject );

			trackedScriptableObjectCount = trackedScriptableObject.Count;
		}

		public void UntrackScriptableObject( ScriptableObject sObject )
		{
			trackedScriptableObject.Remove( sObject );

			trackedScriptableObjectCount = trackedScriptableObject.Count;
		}

		private void OnValidate()
		{
			if( trackedScriptableObject == null || trackedScriptableObject.Count == 0 || trackedScriptableObject.Count == trackedScriptableObjectCount )
				return;

			Debug.LogError( "Do not add items to SOLibrary manually" );

			trackedScriptableObject.RemoveRange( trackedScriptableObjectCount, trackedScriptableObject.Count - trackedScriptableObjectCount );
			trackedScriptableObjectCount = trackedScriptableObject.Count;

			EditorUtility.SetDirty( this );
			AssetDatabase.SaveAssets();
		}
    }
}