/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;

namespace FFEditor
{
	[ CreateAssetMenu( fileName = "PlayModeUtilitySettings", menuName = "FFEditor/PlayModeUtilitySettings" ) ]
	public class FFPlayModeUtilitySettings : ScriptableObject
	{
#region Fields
		[ Tooltip( "Scene to load whenever entering the play mode" ) ]
		public bool useDefaultScene;

		[ ShowIf( "useDefaultScene" ), NaughtyAttributes.Scene() ]
		public int defaultSceneIndex = 0;
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
#endregion
	}
}