/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using NaughtyAttributes;

namespace FFEditor
{
	public class FFPlayModeUtilitySettings : ScriptableObject
	{
#region Fields
		[ Tooltip( "Scene to load whenever entering the play mode" ) ]
		public bool useDefaultScene;

		[ ShowIf( "useDefaultScene" ), Scene() ]
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