using DG.Tweening;

namespace FFStudio
{
	public class RecycledTween
	{
		private UnityMessage onComplete;
		private Tween tween;

		public void Recycle( Tween tween_unsafe, UnityMessage onComplete )
		{
			tween = tween.KillProper();
			tween = tween_unsafe;

			this.onComplete = onComplete;

			tween.OnComplete( OnComplete_Safe );
		}

		public void Recycle( Tween tween_unsafe )
		{
			tween = tween.KillProper();
			tween = tween_unsafe;

			tween.OnComplete( OnComplete_Safe );
		}

		private void OnComplete_Safe()
		{
			onComplete?.Invoke();

			tween = null;
		}
	}
}