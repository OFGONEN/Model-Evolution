/* Created by and for usage of FF Studios (2021). */
using UnityEngine;

namespace FFStudio
{
	/* This class holds references to ScriptableObject assets. These ScriptableObjects are singletons, so they need to load before a Scene does.
	 * Using this class ensures at least one script from a scene holds a reference to these important ScriptableObjects. */
	public class Debug_AssetManager : MonoBehaviour
	{
#region Fields
		public Pool_Debug_UI_Text pool_UI_Debug_Text;

		private void Awake()
		{
			pool_UI_Debug_Text.InitPool( transform, false );
		}
#endregion
	}
}