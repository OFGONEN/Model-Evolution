/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class Test_DressUp : MonoBehaviour
{
#region Fields
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public DressData dressData;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
    public void DressUp()
    {
		gameObject.UpdateSkinnedMeshRenderer( skinnedMeshRenderer, dressData );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}