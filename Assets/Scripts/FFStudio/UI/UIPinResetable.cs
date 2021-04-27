using System.Collections;
using System.Collections.Generic;
using FFStudio;
using UnityEngine;

public class UIPinResetable : UIPin
{
	#region Fields
	[Header( "Event Listeners" )]
	public EventListenerDelegateResponse resetPinListener;
	public bool changeSpriteOnReset;

	#endregion

	#region UnityAPI
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
		resetPinListener.response = PinReset;
	}
	#endregion

	#region Implementation
	void PinReset()
	{
		var resetEvent = resetPinListener.gameEvent;

		if( changeSpriteOnReset )
			imageRenderer.sprite = ( resetEvent as ReferenceGameEvent ).eventValue as Sprite;

		uiTransform.position = startPosition;
		uiTransform.localScale = Vector3.zero;
		uiTransform.sizeDelta = startSizeDelta;

		GoPopOut().onComplete = PinMove;
	}
	#endregion
}
