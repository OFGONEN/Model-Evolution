/* Created by and for usage of FF Studios (2021). */
using UnityEngine;

namespace FFStudio
{
	/* This class holds references to ScriptableObject assets. These ScriptableObjects are singletons, so they need to load before a Scene does.
	 * Using this class ensures at least one script from a scene holds a reference to these important ScriptableObjects. */
	public class AssetManager : MonoBehaviour
	{
#region Fields
		public GameSettings gameSettings;
		public CurrentLevelData currentLevelData;

		public Pool_UIPopUpText pool_UIPopUpText;
#endregion

#region UnityAPI
		private void Awake()
		{
			pool_UIPopUpText.InitPool( transform, false );
		}
#endregion
	}
}