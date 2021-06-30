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
        public static SOLibrary soLibrary = null;

        static SOUtility()
        {
            CreateFolders();

            EditorApplication.playModeStateChanged += PlayModeChange;

			soLibrary = AssetDatabase.LoadAssetAtPath( "Assets/Editor/TrackedSOLib.asset", typeof( SOLibrary ) ) as SOLibrary;

			if( soLibrary == null )
			{
				soLibrary = ScriptableObject.CreateInstance< SOLibrary >();
				AssetDatabase.CreateAsset( soLibrary, "Assets/Editor/TrackedSOLib.asset" );

				Debug.Log( "Scriptable Object Library is Created!" );
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

		private static void CreateFolders()
		{
			if( AssetDatabase.IsValidFolder( "Assets/Editor/DefaultScriptableObjects" ) )
                return;

			AssetDatabase.CreateFolder( "Assets/Editor", "DefaultScriptableObjects" );
		}

		[ MenuItem( "FFStudios/SO/Track SO" ) ]
		public static void TrackSO()
		{
			var objects = Selection.objects;
			var guids = Selection.assetGUIDs;

			if( guids.Length != objects.Length )
			{
				Debug.LogError( "DO NOT SELECT FROM SCENE" );
				EditorApplication.Beep();
				return;
			}

			for( int i = 0; i < objects.Length; i++ )
			{
				var _object = objects[ i ];
				var scriptableObject = _object as ScriptableObject;

				if( scriptableObject == null )
                    continue;

				var path = AssetDatabase.GUIDToAssetPath( Selection.assetGUIDs[ i ] );

				var assetName = "Default_" + Path.GetFileNameWithoutExtension( path ) + ".asset";
				var newPath = Path.Combine( "Assets/Editor/DefaultScriptableObjects", assetName );

				AssetDatabase.CopyAsset( path, newPath );

				soLibrary.TrackScriptableObject( scriptableObject );
			}

			EditorUtility.SetDirty( soLibrary );
			AssetDatabase.SaveAssets();
		}

		[ MenuItem( "FFStudios/SO/UnTrack SO" ) ]
		public static void UnTrackSO()
		{
			var objects = Selection.objects;
			var guids = Selection.assetGUIDs;

			if( guids.Length != objects.Length )
			{
				Debug.LogError( "DO NOT SELECT FROM SCENE" );
				EditorApplication.Beep();
				return;
			}

			for( int i = 0; i < objects.Length; i++ )
			{
				var _object = objects[ i ];
				var scriptableObject = _object as ScriptableObject;

				if( scriptableObject == null )
					continue;

				var path = AssetDatabase.GUIDToAssetPath( Selection.assetGUIDs[ i ] );

				var assetName = "Default_" + Path.GetFileNameWithoutExtension( path ) + ".asset";
				var newPath = Path.Combine( "Assets/Editor/DefaultScriptableObjects", assetName );

				AssetDatabase.DeleteAsset( newPath );

				soLibrary.UntrackScriptableObject( scriptableObject );
			}

			EditorUtility.SetDirty( soLibrary );
			AssetDatabase.SaveAssets();
		}

		[ MenuItem( "FFStudios/SO/Load All Default SO" ) ]
		public static void LoadAllDefaultSO()
		{
			var trackedScriptableObject = soLibrary.trackedScriptableObject;

			for( int i = 0; i < trackedScriptableObject.Count; i++ )
				LoadDefaultSO( trackedScriptableObject[ i ] );
		}

		[ MenuItem( "FFStudios/SO/Create All Default SO" ) ]
		public static void CreateAllDefaultSO()
		{
			var trackedScriptableObject = soLibrary.trackedScriptableObject;

			for( int i = 0; i < trackedScriptableObject.Count; i++ )
				CreateDefaultSO( trackedScriptableObject[ i ] );
		}

		private static void CreateDefaultSO( ScriptableObject sObject )
		{
			var _assetName = "Default_" + sObject.name + ".asset";
			var _newPath = Path.Combine( "Assets/Editor/DefaultScriptableObjects", _assetName );

			string _sObjectGUID;
			long _sObjectLocalID;
			string _sObjectAssetPath;

			AssetDatabase.TryGetGUIDAndLocalFileIdentifier( sObject, out _sObjectGUID, out _sObjectLocalID );
			_sObjectAssetPath = AssetDatabase.GUIDToAssetPath( _sObjectGUID );
			AssetDatabase.CopyAsset( _sObjectAssetPath, _newPath );
		}
        
		private static void LoadDefaultSO( ScriptableObject sObject )
		{
			ClearLog();

			var assetName = "Default_" + sObject.name + ".asset";
			var newPath = Path.Combine( "Assets/Editor/DefaultScriptableObjects", assetName );

			var defaultScriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>( newPath );

			if( defaultScriptableObject == null )
			{
				Debug.LogError( sObject.name + " does not have default SO" );
				EditorApplication.Beep();
				return;
			}

			var defaultScriptableObjectFields = defaultScriptableObject.GetType().GetFields();
			var scriptableObjectFields = sObject.GetType().GetFields();

			for( int x = 0; x < defaultScriptableObjectFields.Length; x++ )
				scriptableObjectFields[ x ].SetValue( sObject, defaultScriptableObjectFields[ x ].GetValue( defaultScriptableObject ) );

			EditorUtility.SetDirty( sObject );

			AssetDatabase.SaveAssets();
		}

		private static void ClearLog()
		{
			var assembly = Assembly.GetAssembly( typeof( UnityEditor.Editor ) );
			var type     = assembly.GetType( "UnityEditor.LogEntries" );
			var method   = type.GetMethod( "Clear" );
			method.Invoke( new object(), null );
		}
	}
}