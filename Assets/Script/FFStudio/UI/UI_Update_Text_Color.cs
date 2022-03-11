/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class UI_Update_Text_Color : UI_Update_Text< SharedColorNotifier, Color >
{
	protected override void OnSharedDataChange()
    {
		ui_Text.color = sharedDataNotifier.SharedValue;
    }
}