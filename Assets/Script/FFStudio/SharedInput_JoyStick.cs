/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "shared_input_joystick", menuName = "FF/Data/Shared/Input/JoyStick" ) ]
public class SharedInput_JoyStick : SharedVector2Notifier
{
#region Fields

    // Private \\
    private Vector2 input_received;
    private Vector2 input_screen_position;
    private Vector2 input_direction;
    private bool input_enabled;

    // Events \\
    public event UnityMessage input_toggle; 
#endregion

#region Properties
    public Vector2 Input_Received        => input_received;
    public Vector2 Input_Screen_Position => input_screen_position;
    public Vector2 Input_Direction       => input_direction;
    public bool Input_Enabled            => input_enabled;
#endregion

#region Unity API
#endregion

#region API
    public void Enable( Vector2 screenPosition )
    {
		input_screen_position = screenPosition;
		input_enabled         = true;

		input_toggle?.Invoke();
	}

    public void Disable( Vector2 screenPosition )
    {
		input_screen_position = screenPosition;
		input_enabled         = false;
		SharedValue           = Vector2.zero;

		input_toggle?.Invoke();
    }

    public void ReceiveInput( Vector2 input )
    {
		input_received  = input;
		input_direction = input_received - input_screen_position;
		SharedValue     = input_direction.normalized;
	}

    public void ClearInvokeList()
    {
		input_toggle = null;
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
