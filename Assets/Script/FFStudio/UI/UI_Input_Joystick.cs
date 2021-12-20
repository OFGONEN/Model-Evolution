/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class UI_Input_Joystick : UIEntity
{
#region Fields
    [ BoxGroup( "Setup" ) ] public RectTransform image_base;
    [ BoxGroup( "Setup" ) ] public RectTransform image_stick;
    [ BoxGroup( "Setup" ) ] public SharedInput_JoyStick input_JoyStick;

#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		input_JoyStick.input_toggle += InputToggle;
	}

    private void OnDisable()
    {
		input_JoyStick.input_toggle -= InputToggle;
    }

	private void Awake()
	{
		image_base.gameObject.SetActive( false );
	}
#endregion

#region API
#endregion

#region Implementation
    private void InputToggle()
    {
		var enabled = input_JoyStick.Input_Enabled;
		image_base.gameObject.SetActive( input_JoyStick.Input_Enabled );

		if( enabled )
		{
			input_JoyStick.changeEvent += InputChange;

			var position             = input_JoyStick.Input_Screen_Position;
			    position.y           = uiTransform.position.y;
			    uiTransform.position = position;
		}
        else
			input_JoyStick.changeEvent -= InputChange;
    }

    private void InputChange()
    {
		image_stick.anchoredPosition = image_base.anchoredPosition + input_JoyStick.SharedValue * GameSettings.Instance.ui_Entity_JoyStick_Gap;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
