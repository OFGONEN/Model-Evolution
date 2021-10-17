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

		// Creates a STRING for build text
		public void OnPreprocessBuild( BuildReport report )
		{
			StringBuilder stringBuilder = new StringBuilder( 32 );

			string buildNumber = null;

			// Add platform name
			if ( report.summary.platform == UnityEditor.BuildTarget.iOS )
            {
				stringBuilder.Append( "IOS_" );
				buildNumber = PlayerSettings.iOS.buildNumber;
			}
            else if ( report.summary.platform == UnityEditor.BuildTarget.Android )
            {
				stringBuilder.Append( "APK_" );
				buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
			}


			var time = DateTime.Now.ToString( "dd/MM/yyyy" ).Replace( '-', '/' );
			var date = time.Substring( 0, time.Length - 2 ).ToCharArray(); // To convert year from 2021 to just 21
			date[ date.Length - 2 ] = time[ time.Length - 2 ];
			date[ date.Length - 1 ] = time[ time.Length - 1 ];

			stringBuilder.Append( date );
			stringBuilder.Append( "_Build-" );
			stringBuilder.Append( buildNumber );

			var buildString = AssetDatabase.LoadAssetAtPath( "Assets/Scriptable_Objects/Shared/build_string.asset", typeof( SharedString ) ) as SharedString;
			buildString.sharedValue = stringBuilder.ToString();

			EditorUtility.SetDirty( buildString );
			AssetDatabase.SaveAssets();
		}
	}
}

