/* Created by and for usage of FF Studios (2021). */

using System;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using FFStudio;

namespace FFEditor
{
	public class FFBuildUtility : IPreprocessBuildWithReport
	{
		public int callbackOrder { get { return 0; } }
		public static readonly string assetPath = "Assets/Scriptable_Object/Shared/build_string.asset";

		/* Creates a STRING for build text. */
		public void OnPreprocessBuild( BuildReport report )
		{
			var buildStringAsset = AssetDatabase.LoadAssetAtPath( assetPath, typeof( SharedString ) );

			if( buildStringAsset == null )
			{
				FFLogger.LogWarning( "FFBuildUtility: Build string can't be updated: Shared build string asset was not found in path \"" + assetPath + "\"." );
				FFLogger.LogWarning( "FFBuildUtility: Please make sure asset path string in FFBuildUtility.cs matches the actual path of the asset." );
				throw new UnityEditor.Build.BuildFailedException( "Build was canceled by the user." );
			}
			else
			{
				var buildString = GetBuildString( report.summary.platform );

				var sharedBuildString = buildStringAsset as SharedString;
				sharedBuildString.sharedValue = buildString;

				EditorUtility.SetDirty( sharedBuildString );
				AssetDatabase.SaveAssets();
			}
		}

		private string GetBuildString( UnityEditor.BuildTarget builtTarget )
		{
			StringBuilder stringBuilder = new StringBuilder( 32 );

			string buildNumber = "";

			if( builtTarget == UnityEditor.BuildTarget.iOS )
			{
				stringBuilder.Append( "IOS_" );
				buildNumber = PlayerSettings.iOS.buildNumber;
			}
			else if( builtTarget == UnityEditor.BuildTarget.Android )
			{
				stringBuilder.Append( "APK_" );
				buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
			}

			var time = DateTime.Now.ToString( "dd/MM/yyyy" ).Replace( '-', '/' );
			var date = time.Substring( 0, time.Length - 2 ).ToCharArray(); // To convert year from 2021 to just 21.
			date[ date.Length - 2 ] = time[ time.Length - 2 ];
			date[ date.Length - 1 ] = time[ time.Length - 1 ];

			stringBuilder.Append( date );
			stringBuilder.Append( "_Build-" );
			stringBuilder.Append( buildNumber );

			return stringBuilder.ToString();
		}
	}
}