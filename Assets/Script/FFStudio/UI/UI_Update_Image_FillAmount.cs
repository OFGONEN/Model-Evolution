/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FFStudio;

public class UI_Update_Image_FillAmount : MonoBehaviour
{
#region Fields
	public SharedFloatNotifier sharedDataNotifier;

	private Image ui_Image;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		sharedDataNotifier.Subscribe( OnSharedDataChange );
	}

    private void OnDisable()
    {
		sharedDataNotifier.Unsubscribe( OnSharedDataChange );
    }

    private void Awake()
    {
        ui_Image = GetComponentInChildren< Image >();

		OnSharedDataChange();
	}
#endregion

#region API
#endregion

#region Implementation
    private void OnSharedDataChange()
    {
		ui_Image.fillAmount = sharedDataNotifier.SharedValue;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
