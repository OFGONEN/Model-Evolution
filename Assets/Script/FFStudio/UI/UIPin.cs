/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.UI;
using FFStudio;
using DG.Tweening;

public class UIPin : UIEntity
{
#region Fields
	[ Header( "Event Listeners" ) ]
	public EventListenerDelegateResponse pinMoveListener;
	public EventListenerDelegateResponse pinRevealListener;
	public EventListenerDelegateResponse pinDisapperListener;

	[ Header( "Fired Events" ) ]
	public GameEvent pinnedEvent;

	protected Vector2 startSizeDelta;
	protected Image imageRenderer;
#endregion

#region Unity API
	protected virtual void OnEnable()
	{
		pinMoveListener.OnEnable();
		pinRevealListener.OnEnable();
		pinDisapperListener.OnEnable();
	}

	protected virtual void OnDisable()
	{
		pinMoveListener.OnDisable();
		pinRevealListener.OnDisable();
		pinDisapperListener.OnDisable();
	}

	protected virtual void Awake()
	{
		pinMoveListener.response     = PinMoveResponse;
		pinRevealListener.response   = () => Appear();
		pinDisapperListener.response = () => Disappear();

		imageRenderer = GetComponent< Image >();
	}
	
	public override void Start()
	{
		base.Start();
		startSizeDelta         = uiTransform.sizeDelta;
		uiTransform.localScale = Vector3.zero;
	}
#endregion

#region API
#endregion

#region Implementation
	private Tween GoSizeDelta()
	{
		return uiTransform.DOSizeDelta( destinationTransform.sizeDelta, GameSettings.Instance.ui_Entity_Fade_TweenDuration );
	}

	protected void PinMoveResponse()
	{
		var sequence = DOTween.Sequence();

		sequence.Append( GoToTargetPosition() )
				.Join( GoSizeDelta() )
				.AppendCallback( pinnedEvent.Raise );
	}
#endregion
}
