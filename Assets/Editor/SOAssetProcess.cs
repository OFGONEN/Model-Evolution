/* Created by and for usage of FF Studios (2021). */

using UnityEditor;
using System.IO;

namespace FFEditor
{
    public class SOAssetProcess : UnityEditor.AssetModificationProcessor
    {
        static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);

            var soLibrary = SOUtility.soLibrary;
            var trackedScriptableObject = soLibrary.trackedScriptableObject;

			for( int i = 0; i < trackedScriptableObject.Count; i++ )
			{
				var scriptableObject = trackedScriptableObject[ i ];

				if( fileName == scriptableObject.name )
				{
					var assetName = "Default_" + scriptableObject.name + ".asset";
					var deletePath = Path.Combine( "Assets/Editor/DefaultScriptableObjects", assetName );

					trackedScriptableObject.RemoveAt( i );
					soLibrary.trackedScriptableObjectCount = trackedScriptableObject.Count;

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