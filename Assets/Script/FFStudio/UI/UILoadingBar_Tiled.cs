/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using NaughtyAttributes;
using UnityEngine.UI;

public class UILoadingBar_Tiled : UIEntity
{
#region Fields
    [ Header( "Shared Variables" ) ]
    public SharedFloatNotifier progressNotifier;

	[ HorizontalLine ]
	[ Header( "UI Elements" ) ]
	public Image fillingImage;
    
    [ Range( 0.0f, 1.0f ) ]
    public float stepSize;
#endregion

#region Unity API
    private void OnEnable()
    {
		progressNotifier.Subscribe( OnValueChange_Inverse );
	}

    private void OnDisable()
    {
		progressNotifier.Unsubscribe( OnValueChange_Inverse );
    }

	private void Awake()
	{
		OnValueChange_Inverse(); // Set filling amount to value at the start 
	}
#endregion

#region API
#endregion

#region Implementation
    private void OnValueChange_Inverse()
    {
		fillingImage.fillAmount = Mathf.FloorToInt( ( 1.0f - progressNotifier.SharedValue ) / stepSize ) * stepSize;
	}
#endregion
}
