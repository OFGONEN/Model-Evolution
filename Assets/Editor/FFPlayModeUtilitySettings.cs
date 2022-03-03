/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

namespace FFEditor
{
	[ CreateAssetMenu( fileName = "PlayModeUtilitySettings", menuName = "FFEditor/PlayModeUtilitySettings" ) ]
	public class FFPlayModeUtilitySettings : ScriptableObject
	{
#region Fields
		[ Tooltip( "Scene to load whenever entering the play mode" ) ]
		public bool useDefaultScene;

		[ ShowIf( "useDefaultScene" ), ValueDropdown( "SceneList" ) ]
		public int defaultSceneIndex = 0;
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
#endregion

#if UNITY_EDITOR
		private static IEnumerable SceneList()
        {
			var list = new ValueDropdownList< int >();

			var scene_count = SceneManager.sceneCountInBuildSettings;

			for( var i = 0; i < scene_count; i++ )
				list.Add( Path.GetFileNameWithoutExtension( SceneUtility.GetScenePathByBuildIndex( i ) ) + $" ({i})", i );

			return list;
		}
#endif
	}
}