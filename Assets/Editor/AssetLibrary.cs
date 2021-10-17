/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FFEditor
{
	[ CreateAssetMenu( fileName = "AssetLibrary", menuName = "FFEditor/AssetLibrary" ) ]
    public class AssetLibrary : ScriptableObject
    {
        public List< GameObject > trackedAssets = null;
        [ HideInInspector ] public int trackedAssetsCount = 0;

		private void Awake()
		{
			if( trackedAssets == null )
				trackedAssets = new List< GameObject >( 8 );

			trackedAssetsCount = trackedAssets.Count;
		}

		public void TrackAsset( GameObject gameObject )
		{
			if( trackedAssets.Contains( gameObject ) )
			{
				Debug.LogWarning( "PreFab is already tracked" );
				return;
			}

			trackedAssets.Add( gameObject );
			trackedAssetsCount = trackedAssets.Count;
		}

		public void UnTrackAsset( GameObject gameObject )
		{
			if( !trackedAssets.Contains( gameObject ) )
			{
				Debug.LogWarning( "PreFab is not in the Library" );
				return;
			}

			trackedAssets.Remove( gameObject );
			trackedAssetsCount = trackedAssets.Count;

			Debug.LogWarning( "PreFab: " + gameObject + " is untracked" );
		}

		private void OnValidate()
		{
			if( trackedAssets == null || trackedAssets.Count == 0 || trackedAssets.Count == trackedAssetsCount )
                return;

			Debug.LogError( "Do not add items to SOLibrary manually" );

			trackedAssets.RemoveRange( trackedAssetsCount, trackedAssets.Count - trackedAssetsCount );
			trackedAssetsCount = trackedAssets.Count;

			EditorUtility.SetDirty( this );
			AssetDatabase.SaveAssets();
		}
    }

}