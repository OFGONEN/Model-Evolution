/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class UIPopUpText : MonoBehaviour
{
#region Fields
    private UI_Float ui_float; 
    private UI_Fade_Text ui_text; 

	private Vector3 ui_start_scale;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
        ui_float = GetComponentInChildren< UI_Float >();
        ui_text  = GetComponentInChildren< UI_Fade_Text >();

		ui_start_scale = ui_float.UI_RectTransform.localScale;
	}
#endregion

#region API
	public void Spawn( Vector3 position, string text, float size, Color color )
	{
		gameObject.SetActive( true );
		transform.position = position;

		ui_float.DoFloat( GameSettings.Instance.ui_PopUp_height,
			GameSettings.Instance.ui_PopUp_duration );

		ui_text.DoFade( 0, GameSettings.Instance.ui_PopUp_duration );

		ui_float.UI_RectTransform.localScale = ui_start_scale + Vector3.one * size;

		ui_text.UI_Text.text  = text;
		ui_text.UI_Text.color = color;
	} 
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}