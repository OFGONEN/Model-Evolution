/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using System.IO;
using System.Collections;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "LevelData", menuName = "FF/Data/LevelData" ) ]
	public class LevelData : ScriptableObject
    {
		[ BoxGroup( "Setup" ), ValueDropdown( "SceneList" ), LabelText( "Scene Index" ) ] public int scene_index;
        [ BoxGroup( "Setup" ), LabelText( "Override As Active Scene" ) ] public bool scene_overrideAsActiveScene;

        [ BoxGroup( "Setup" ) ] public bool cloth_start_special;
        [ BoxGroup( "Setup" ), ShowIf( "cloth_start_special" ) ] public DressData cloth_start_cloth;

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
