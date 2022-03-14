/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class Test_CurrentLevel : MonoBehaviour
{
#region Fields
    public CurrentLevelData currentLevelData;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		currentLevelData.currentLevel_Real = 1;
		currentLevelData.LoadCurrentLevelData();
	}
#endregion

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}