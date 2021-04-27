using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedFloatPropertyFallBackTweener", menuName = "FF/Data/Shared/Tweener/FloatPropertyFallBackTweener" )]
	public class SharedFloatPropertyFallBackTweener : SharedFloat
	{
		#region Fields

		public event ChangeEvent changeEvent;

		[Tooltip("Tweening back value after reaching target value")]
		public float defaultValue;

		[Tooltip( "Change of duration for reaching new value" )]
		public float changeDuration;
		[Tooltip( "Amount of value to lost in one second" )]
		public float fallBackSpeed;
		public Ease changeEase;

		private Tween valueChangeTween;
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
				.OnComplete( FallBackToDefault );
			}
		}

		public void CompleteTween()
		{
			if( valueChangeTween != null )
				valueChangeTween.Kill();

		}
		#endregion

		#region Implementation
		void FallBackToDefault()
		{
			valueChangeTween = DOTween.To( () => sharedValue, x => sharedValue = x, defaultValue, sharedValue / fallBackSpeed )
			.SetEase( changeEase )
			.OnUpdate( OnChangeUpdate )
			.OnComplete( () => valueChangeTween = null );
		}

		void OnChangeUpdate()
		{
			changeEvent?.Invoke();
		}
		#endregion

	}
}
