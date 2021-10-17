/* Created by and for usage of FF Studios (2021). */

using UnityEditor;
using UnityEngine;

namespace FFEditor
{
    [ InitializeOnLoad ]
    public static class AssetLibraryUtility
    {
        private static AssetLibrary assetLibrary;

        private static AssetLibrary AssetLibrary
		{
			get
			{
				if( assetLibrary == null )
					assetLibrary = AssetDatabase.LoadAssetAtPath( "Assets/Editor/AssetLibrary.asset", typeof( AssetLibrary ) ) as AssetLibrary;

				return assetLibrary;
			}
		}

        static AssetLibraryUtility()
		{
			assetLibrary = AssetDatabase.LoadAssetAtPath( "Assets/Editor/AssetLibrary.asset", typeof( AssetLibrary ) ) as AssetLibrary;

			if( assetLibrary == null )
			{
				Debug.LogError( "AssetLibrary is not FOUND!" );
			}
		}
        
		[ MenuItem( "FFStudios/Asset/Track Asset" ) ]
		public static void TrackAsset()
		{
			var activeGameObjects = Selection.gameObjects;
			var guids   		  = Selection.assetGUIDs;

			if( activeGameObjects == null || guids.Length != activeGameObjects.Length )
			{
				Debug.LogError( "Select a PreFab to track\nDO NOT SELECT FROM SCENE" );
				EditorApplication.Beep();
				return;
			}

			for( int i = 0; i < activeGameObjects.Length; i++ )
			{
				Debug.Log( "Path: " + AssetDatabase.GUIDToAssetPath( Selection.assetGUIDs[ i ] ) );
				AssetLibrary.TrackAsset( activeGameObjects[ i ] );
			}

			EditorUtility.SetDirty( AssetLibrary );
			AssetDatabase.SaveAssets();
		}

		[ MenuItem( "FFStudios/Asset/UnTrack Asset" ) ]
		public static void UnTrackAsset()
		{
			var activeGameObjects = Selection.gameObjects;
			var guids   		  = Selection.assetGUIDs;

			if( activeGameObjects == null || guids.Length != activeGameObjects.Length )
			{
				Debug.LogError( "Select a PreFab to Untrack\nDO NOT SELECT FROM SCENE" );
				EditorApplication.Beep();
				return;
			}

			for( int i = 0; i < activeGameObjects.Length; i++ )
			{
				AssetLibrary.UnTrackAsset( activeGameObjects[ i ] );
			}

			EditorUtility.SetDirty( AssetLibrary );
			AssetDatabase.SaveAssets();
		}
	}
}