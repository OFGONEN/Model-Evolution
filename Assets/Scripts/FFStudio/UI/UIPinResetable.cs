/* Created by and for usage of FF Studios (2021). */

using FFStudio;
using UnityEngine;

public class UIPinResetable : UIPin
{
#region Fields
	[ Header( "Event Listeners" ) ]
	public EventListenerDelegateResponse resetPinListener;
	
	public bool changeSpriteOnReset;
#endregion

#region Unity API
	protected override void OnEnable()
	{
		base.OnEnable();
		resetPinListener.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		resetPinListener.OnDisable();
	}

	protected override void Awake()
	{
		base.Awake();
		resetPinListener.response = PinResetResponse;
	}
#endregion

#region Implementation
	private void PinResetResponse()
	{
		var resetEvent = resetPinListener.gameEvent;

		if( changeSpriteOnReset )
			imageRenderer.sprite = ( resetEvent as ReferenceGameEvent ).eventValue as Sprite;

		uiTransform.position   = startPosition;
		uiTransform.localScale = Vector3.zero;
		uiTransform.sizeDelta  = startSizeDelta;

		Appear().onComplete = PinMoveResponse;
	}
#endregion
}
