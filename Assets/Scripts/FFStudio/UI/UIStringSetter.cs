/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class UIStringSetter : UIText
{
#region Fields
    public SharedString sharedString;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		textRenderer.text = sharedString.sharedValue;
	}
#endregion

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if( textRenderer != null && sharedString != null )
		    textRenderer.text = sharedString.sharedValue;
    }
#endif
#endregion
}
