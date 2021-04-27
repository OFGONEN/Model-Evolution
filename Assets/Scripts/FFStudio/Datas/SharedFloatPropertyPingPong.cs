using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedFloatPropertyPingPong", menuName = "FF/Data/Shared/Tweener/FloatPropertyPingPong" )]
	public class SharedFloatPropertyPingPong : SharedFloat
	{

		#region Fields
		public event ChangeEvent changeEvent;

		
		[MinMaxSlider( -1f, 1f )] public Vector2 minMaxValues;
		[Tooltip( "Duration movement of from middle to end" )] public float changeDuration;
		public Ease changeEase;

		private Sequence pingPongSequnce;
		#endregion

		#region API
		public void SetVale(float value)
		{
			if( !Mathf.Approximately( sharedValue, value ) )
			{
				sharedValue = value;

				changeEvent?.Invoke();
			}
		}

		public void StartPingPong()
		{
			if( pingPongSequnce != null )
				pingPongSequnce.Kill();

			sharedValue = 0;

			var _maxTween = DOTween.To( () => sharedValue, x => SetVale(x), minMaxValues.y, changeDuration ).SetEase( changeEase );
			var _middleTween = DOTween.To( () => sharedValue, x => SetVale(x), 0, changeDuration ).SetEase( changeEase );
			var _minTween = DOTween.To( () => sharedValue, x => SetVale(x), minMaxValues.x, changeDuration * 2f ).SetEase( changeEase );

			pingPongSequnce = DOTween.Sequence();
			pingPongSequnce.Join( _maxTween );
			pingPongSequnce.Append( _minTween );
			pingPongSequnce.Append( _middleTween );
			pingPongSequnce.SetLoops( -1 );
		}

		public void EndPingPong()
		{
			if( pingPongSequnce != null )
				pingPongSequnce.Kill();

			sharedValue = 0;
		}
		#endregion

	}
}
