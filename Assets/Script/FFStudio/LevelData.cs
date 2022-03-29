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
        [ BoxGroup( "Setup" ), LabelText( "Clean All Persistent Cloth Data" ) ] public bool cloth_start_clean;

        [ BoxGroup( "Cloth" ) ] public string cloth_pose;
        [ BoxGroup( "Cloth" ) ] public bool cloth_start_special;
        [ BoxGroup( "Cloth" ), ShowIf( "cloth_start_special" ) ] public EvolveData cloth_start_cloth;

        [ BoxGroup( "Cloth" ) ] public int cloth_time_min;
        [ BoxGroup( "Cloth" ) ] public int cloth_time_max;
		[ BoxGroup( "Cloth" ) ] public float cloth_rotate_clamp = 40f;
        [ BoxGroup( "Cloth" ) ] public EvolveData[] cloth_evolve_datas;


#if UNITY_EDITOR
		private static IEnumerable SceneList()
        {
			var list = new ValueDropdownList< int >();

			var scene_count = SceneManager.sceneCountInBuildSettings;

			for( var i = 0; i < scene_count; i++ )
				list.Add( Path.GetFileNameWithoutExtension( SceneUtility.GetScenePathByBuildIndex( i ) ) + $" ({i})", i );

			return list;
		}

		private void OnValidate()
		{
			for( var i = 0; i < cloth_evolve_datas.Length; i++ )
			{
				cloth_evolve_datas[ i ].evolve_dress_color = cloth_evolve_datas[ i ].evolve_dress_color.SetAlpha( 1 );
			}

			cloth_start_cloth.evolve_dress_color = cloth_start_cloth.evolve_dress_color.SetAlpha( 1 );
		}
#endif
    }
}
