using UnityEngine;
using FFStudio;

/* This class holds referance to ScriptableObject assets. These ScriptableObjects are singleton so they need to load before a 'Scene' does.
*  Using this class unsures at least one script from a scene hold referance to these important ScriptableObjects.
*/
public class AppAssetHolder : MonoBehaviour
{

	#region Fields
	public GameSettings gameSettings;
	public CurrentLevelData currentLevelData;
	#endregion
}
