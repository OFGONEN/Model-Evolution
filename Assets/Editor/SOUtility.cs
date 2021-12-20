/* Created by and for usage of FF Studios (2021). */

using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FFEditor
{
    [ InitializeOnLoad, System.Serializable ]
    public static class SOUtility
    {
        private static SOLibrary soLibrary = null;
		private static bool initialized = false;

		public static SOLibrary SOLibrary
		{
			get
			{
				if( soLibrary == null )
					soLibrary = AssetDatabase.LoadAssetAtPath( "Assets/Editor/TrackedSOLib.asset", typeof( SOLibrary ) ) as SOLibrary;

				return soLibrary;
			}
		}

        static SOUtility()
        {
			if( !initialized )
			{
				EditorApplication.playModeStateChanged += PlayModeChange;
				initialized = true;
			}
		}

		private static void PlayModeChange( PlayModeStateChange change )
		{
			switch( change )
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

		[ MenuItem( "FFStudios/SO/Track SO" ) ]
		public static void TrackSO()
		{
			var objects = Selection.objects;
			var guids   = Selection.assetGUIDs;

			if( guids.Length != objects.Length )
			{
				Debug.LogError( "DO NOT SELECT FROM SCENE" );
				EditorApplication.Beep();
				return;
			}

			for( int i = 0; i < objects.Length; i++ )
			{
				var _object          = objects[ i ];
				var scriptableObject = _object as ScriptableObject;

				// If selected object is not a ScriptableObject then continue
				if( scriptableObject == null )
                    continue;

				var path      = AssetDatabase.GUIDToAssetPath( Selection.assetGUIDs[ i ] );
				var assetName = "Default_" + Path.GetFileNameWithoutExtension( path ) + ".asset";
				var newPath   = Path.Combine( "Assets/Editor/Tracked_Scriptable_Objects", assetName );

				AssetDatabase.CopyAsset( path, newPath );

				SOLibrary.TrackScriptableObject( scriptableObject );
			}

			EditorUtility.SetDirty( SOLibrary );
			AssetDatabase.SaveAssets();
		}

		[ MenuItem( "FFStudios/SO/UnTrack SO" ) ]
		public static void UnTrackSO()
		{
			var objects = Selection.objects;
			var guids   = Selection.assetGUIDs;

			if( guids.Length != objects.Length )
			{
				Debug.LogError( "DO NOT SELECT FROM SCENE" );
				EditorApplication.Beep();
				return;
			}

			for( int i = 0; i < objects.Length; i++ )
			{
				var _object          = objects[ i ];
				var scriptableObject = _object as ScriptableObject;

				// If selected object is not a ScriptableObject then continue
				if( scriptableObject == null )
					continue;

				var path      = AssetDatabase.GUIDToAssetPath( Selection.assetGUIDs[ i ] );
				var assetName = "Default_" + Path.GetFileNameWithoutExtension( path ) + ".asset";
				var newPath   = Path.Combine( "Assets/Editor/Tracked_Scriptable_Objects", assetName );

				AssetDatabase.DeleteAsset( newPath );

				SOLibrary.UntrackScriptableObject( scriptableObject );
			}

			EditorUtility.SetDirty( SOLibrary );
			AssetDatabase.SaveAssets();
		}

		[ MenuItem( "FFStudios/SO/Load All Default SO" ) ]
		public static void LoadAllDefaultSO()
		{
			var trackedScriptableObjects = SOLibrary.trackedScriptableObjects;

			for( int i = 0; i < trackedScriptableObjects.Count; i++ )
				LoadDefaultSO( trackedScriptableObjects[ i ] );
		}

		[ MenuItem( "FFStudios/SO/Create All Default SO" ) ]
		public static void CreateAllDefaultSO()
		{
			var trackedScriptableObjects = SOLibrary.trackedScriptableObjects;

			for( int i = 0; i < trackedScriptableObjects.Count; i++ )
				CreateDefaultSO( trackedScriptableObjects[ i ] );
		}

		private static void CreateDefaultSO( ScriptableObject sObject )
		{
			var _assetName = "Default_" + sObject.name + ".asset";
			var _newPath   = Path.Combine( "Assets/Editor/Tracked_Scriptable_Objects", _assetName );

			string _sObjectGUID;
			long _sObjectLocalID;
			string _sObjectAssetPath;

			AssetDatabase.TryGetGUIDAndLocalFileIdentifier( sObject, out _sObjectGUID, out _sObjectLocalID );
			
			_sObjectAssetPath = AssetDatabase.GUIDToAssetPath( _sObjectGUID );
			AssetDatabase.CopyAsset( _sObjectAssetPath, _newPath );
		}
        
		private static void LoadDefaultSO( ScriptableObject sObject )
		{
			var assetName = "Default_" + sObject.name + ".asset";
			var newPath   = Path.Combine( "Assets/Editor/Tracked_Scriptable_Objects", assetName );

			var defaultScriptableObject = AssetDatabase.LoadAssetAtPath< ScriptableObject >( newPath );

			if( defaultScriptableObject == null )
			{
				Debug.LogError( sObject.name + " does not have default SO" );
				EditorApplication.Beep();
				return;
			}

			var defaultScriptableObjectFields = defaultScriptableObject.GetType().GetFields();
			var scriptableObjectFields        = sObject.GetType().GetFields();

			for( int x = 0; x < defaultScriptableObjectFields.Length; x++ )
				scriptableObjectFields[ x ].SetValue( sObject, defaultScriptableObjectFields[ x ].GetValue( defaultScriptableObject ) );

			EditorUtility.SetDirty( sObject );
			AssetDatabase.SaveAssets();
		}
	}
}