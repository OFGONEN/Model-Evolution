/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using NaughtyAttributes;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "LevelData", menuName = "FF/Data/LevelData" ) ]
	public class LevelData : ScriptableObject
    {
		[ BoxGroup( "Setup" ), Scene() ] public int sceneIndex;

        [ BoxGroup( "Setup" ) ] public bool overrideAsActiveScene;

    }
}
