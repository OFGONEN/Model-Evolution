/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using DG.Tweening;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedFloatPropertyTweener", menuName = "FF/Data/Shared/Tweener/FloatPropertyTweener" ) ]
	public class SharedFloatPropertyTweener : SharedFloat
	{
#region Fields
		[ Header( "Event Listeners" ) ]
		public EventListenerDelegateResponse cleanUpListener;

		public event ChangeEvent changeEvent;

		[ Tooltip("Duration for reaching a new value") ]
		public float changeDuration;
		
		public Ease changeEase;

		private Tween valueChangeTween;
#endregion

#region UnityAPI
		private void OnEnable()
		{
			cleanUpListener.OnEnable();
			cleanUpListener.response = () => sharedValue = 0;
		}

		private void OnDisable()
		{
			cleanUpListener.OnDisable();
		}
#endregion

#region API
		public void SetValue( float value )
		{
			if( !Mathf.Approximately( sharedValue, value ) )
			{
				if( valueChangeTween != null )
					valueChangeTween.Kill();

				valueChangeTween = DOTween.To( () => sharedValue, x => sharedValue = x, value, changeDuration )
					.SetEase( changeEase )
					.OnUpdate( OnChangeUpdate )
					.OnComplete( () => valueChangeTween = null );
			}
		}

		public void KillTween()
		{
			if( valueChangeTween != null )
				valueChangeTween.Kill();
		}
#endregion

#region Implementation
		void OnChangeUpdate()
		{
			changeEvent?.Invoke();
		}
#endregion
	}
}
