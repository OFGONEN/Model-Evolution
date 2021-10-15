/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class UIVerticalProgressIndicator : UIProgressIndicator
	{
#region Fields
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
        protected override void OnProgressChange()
        {
			var position             = indicator_BasePosition;
			    position.y           = Mathf.Lerp( indicator_BasePosition.y, indicator_EndPosition.y, indicatorProgress.sharedValue );
			    uiTransform.position = position;
		}
		
		protected override void GetIndicatorPositions()
        {
			indicator_BasePosition = ( indicatingParentWorldPos[ 0 ] + indicatingParentWorldPos[ 3 ] ) / 2;
			indicator_EndPosition  = ( indicatingParentWorldPos[ 1 ] + indicatingParentWorldPos[ 2 ] ) / 2;
        }
#endregion
	}
}