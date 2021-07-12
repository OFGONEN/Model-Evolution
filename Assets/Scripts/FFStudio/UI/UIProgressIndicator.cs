/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public abstract class UIProgressIndicator : UIEntity
	{
#region Fields
		public SharedFloatProperty indicatorProgress;

		public float offsetPercentage;

		// Private Fields
		private RectTransform indicatingParent;

		protected Vector3[] indicatingParentWorldPos = new Vector3[ 4 ];
		protected Vector3 indicator_BasePosition;
		protected Vector3 indicator_EndPosition;
#endregion

#region Unity API
        private void OnEnable()
        {
			indicatorProgress.changeEvent += OnProgressChange;
		}

        private void OnDisable()
        {
			indicatorProgress.changeEvent -= OnProgressChange;
        }

        public override void Start()
        {
			base.Start();

			indicatingParent = uiTransform.parent.GetComponent< RectTransform >();
			indicatingParent.GetWorldCorners( indicatingParentWorldPos );

			offsetPercentage = offsetPercentage / 100;

			GetIndicatorPositions();
			OnProgressChange();
		}
#endregion

#region API
#endregion

#region Implementation
        protected abstract void OnProgressChange();
		protected abstract void GetIndicatorPositions();
#endregion
	}
}