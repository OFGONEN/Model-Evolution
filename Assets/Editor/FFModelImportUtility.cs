/* Created by and for usage of FF Studios (2021). */

using System.Linq;
using UnityEngine;
using UnityEditor;

/* Created by and for usage of FF Studios (2021). */

namespace FFEditor
{
	public class FFModelImportUtility : AssetPostprocessor
	{
		public Material defaultMaterial;

		private void OnPreprocessModel()
		{
			var modelImporter = assetImporter as ModelImporter;
			
			// Info: Only pre-process models for the FIRST time.
			if( modelImporter.importSettingsMissing == false )
				return;

			var modelNameOnly = assetPath.Split( '/' ).Last();
			var modelPrefix   = modelNameOnly.Split( '_' ).First();

			/* Model Tab. */
			modelImporter.importBlendShapes = false;
			modelImporter.importVisibility = false;
			modelImporter.importCameras = false;
			modelImporter.importLights = false;

			/* Rig Tab. */
			if( modelPrefix == "prop" || modelPrefix == "envr" )
				modelImporter.animationType = ModelImporterAnimationType.None;
            else if( modelPrefix == "char" )
				modelImporter.animationType = ModelImporterAnimationType.Human;
            else if( modelPrefix == "gnrc" )
				modelImporter.animationType = ModelImporterAnimationType.Generic;

			/* Animation Tab. */
            modelImporter.importAnimation = modelPrefix == "char";

			AssetDatabase.ImportAsset( assetPath );
		}

		private void OnPostprocessModel( GameObject gameObject )
		{
			var modelImporter = assetImporter as ModelImporter;

			// Info: Only pre-process models for the FIRST time.
			if( modelImporter.importSettingsMissing == false )
				return;

			RemapDefaultMaterial( gameObject.transform );
            
			AssetDatabase.WriteImportSettingsIfDirty( assetPath );
		}

		private void RemapDefaultMaterial( Transform transform )
		{
			Renderer renderer = transform.gameObject.GetComponent< Renderer >();

			if( defaultMaterial == null )
				defaultMaterial = AssetDatabase.LoadAssetAtPath< Material >( "Assets/Material/material_atlas.mat" );

			if( renderer != null )
			{
				Material[] materials = renderer.sharedMaterials;
                
				foreach( var material in materials )
					assetImporter.AddRemap( new AssetImporter.SourceAssetIdentifier( material ), defaultMaterial );
			}

			// Recurse.
			foreach( Transform child in transform )
				RemapDefaultMaterial( child );
		}
	}

}