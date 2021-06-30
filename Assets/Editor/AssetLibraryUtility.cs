/* Created by and for usage of FF Studios (2021). */

using UnityEditor;
using UnityEngine;

namespace FFEditor
{
    [ InitializeOnLoad ]
    public static class AssetLibraryUtility
    {
        public static AssetLibrary assetLibrary;

        static AssetLibraryUtility()
		{
			assetLibrary = AssetDatabase.LoadAssetAtPath( "Assets/Editor/EditorAssetLibrary.asset", typeof( AssetLibrary ) ) as AssetLibrary;

			if( assetLibrary == null )
			{
				assetLibrary = ScriptableObject.CreateInstance< AssetLibrary >();
				AssetDatabase.CreateAsset( assetLibrary, "Assets/Editor/EditorAssetLibrary.asset" );
			}
		}
        
		[ MenuItem( "FFStudios/Asset/Track Asset" ) ]
		public static void TrackAsset()
		{
			var activeGameObject = Selection.activeGameObject;

			if( activeGameObject == null )
			{
				Debug.LogError( "Select a PreFab to track" );
				EditorApplication.Beep();
				return;
			}

			if( !activeGameObject.name.Contains( "_" ) )
			{
				Debug.LogError( "PreFab name convention should be PreFab_1 to track" );
				EditorApplication.Beep();
				return;
			}
			else
			{
				var index = activeGameObject.name.IndexOf( '_' );

				if( activeGameObject.name.Length == index + 1 || activeGameObject.name[ index + 1 ] != '1' )
				{
					Debug.LogError( "Always track the first variant of a Prefab." +
					"After '_' character there should be '1'" );
					EditorApplication.Beep();

					return;
				}
			}

			assetLibrary.TrackAsset( activeGameObject );

			EditorUtility.SetDirty( assetLibrary );
			AssetDatabase.SaveAssets();
		}

		[ MenuItem( "FFStudios/Asset/UnTrack Asset" ) ]
		public static void UnTrackAsset()
		{
			var activeGameObject = Selection.activeGameObject;

			if( activeGameObject == null )
			{
				Debug.LogError( "Select a PreFab to track" );
				EditorApplication.Beep();
				return;
			}

			assetLibrary.UnTrackAsset( activeGameObject );

			EditorUtility.SetDirty( assetLibrary );
			AssetDatabase.SaveAssets();
		}
	}
}