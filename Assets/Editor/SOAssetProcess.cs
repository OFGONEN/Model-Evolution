/* Created by and for usage of FF Studios (2021). */

using UnityEditor;
using System.IO;

namespace FFEditor
{
    public class SOAssetProcess : UnityEditor.AssetModificationProcessor
    {
		// Removes a tracked scriptable object from track list if it is being deleted 
        static AssetDeleteResult OnWillDeleteAsset( string path, RemoveAssetOptions options )
        {
			// Deleted file name
            var fileName = Path.GetFileNameWithoutExtension( path );

            var soLibrary                = SOUtility.SOLibrary;
            var trackedScriptableObjects = soLibrary.trackedScriptableObjects;

			for( int i = 0; i < trackedScriptableObjects.Count; i++ )
			{
				var scriptableObject = trackedScriptableObjects[ i ];

				// If deleted scriptable object is found
				if( fileName == scriptableObject.name )
				{
					var assetName  = "Default_" + scriptableObject.name + ".asset";
					var deletePath = Path.Combine( "Assets/Editor/Tracked_Scriptable_Objects", assetName );

					// Remove from track list
					trackedScriptableObjects.RemoveAt( i );
					soLibrary.trackedScriptablesObjectCount = trackedScriptableObjects.Count; // Update tracked scriptable objects count

					EditorUtility.SetDirty( soLibrary );

					AssetDatabase.DeleteAsset( deletePath );
					AssetDatabase.SaveAssets();

					break;
				}
			}

			return AssetDeleteResult.DidNotDelete;
        }
    }
}