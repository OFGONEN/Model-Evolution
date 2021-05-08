using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FFStudio;
using DG.Tweening;

public class UIPin : UIEntity
{
	#region Fields
	[Header( "Event Listeners" )]
	public EventListenerDelegateResponse pinMoveListener;
	public EventListenerDelegateResponse pinRevealListener;
	public EventListenerDelegateResponse pinDisapperListener;

	[Header( "Fired Events" )]
	public GameEvent pinnedEvent;


	protected Vector2 startSizeDelta;
	protected Image imageRenderer;
	#endregion

	#region UnityAPI
	protected virtual void OnEnable()
	{
		pinMoveListener    .OnEnable();
		pinRevealListener  .OnEnable();
		pinDisapperListener.OnEnable();
	}

	protected virtual void OnDisable()
	{
		pinMoveListener    .OnDisable();
		pinRevealListener  .OnDisable();
		pinDisapperListener.OnDisable();
	}

	protected virtual void Awake()
	{
		pinMoveListener.response     = PinMove;
		pinRevealListener.response   = () => GoPopOut();
		pinDisapperListener.response = () => GoPopIn();

		imageRenderer = GetComponent<Image>();
	}
	public override void Start()
	{
		base.Start();
		startSizeDelta = uiTransform.sizeDelta;
		uiTransform.localScale = Vector3.zero;
	}
	#endregion

	#region API
	#endregion

	#region Implementation
	Tween GoSizeDelta()
	{
		return uiTransform.DOSizeDelta( destinationTransform.sizeDelta, GameSettings.Instance.ui_Entity_Fade_TweenDuration );
	}

	protected void PinMove()
	{
		var sequence = DOTween.Sequence();

		sequence.Append( GoTargetPosition() );
		sequence.Join( GoSizeDelta() );
		sequence.AppendCallback( pinnedEvent.Raise );
	}
	#endregion
}
