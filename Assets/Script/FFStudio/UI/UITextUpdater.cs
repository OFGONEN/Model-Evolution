/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using NaughtyAttributes;

namespace FFStudio
{
    public class UITextUpdater< SharedDataNotifierType, SharedDataType > : UIText
        where SharedDataNotifierType : SharedDataNotifier< SharedDataType >
    {
#region Fields
        [ BoxGroup( "Setup" ), SerializeField ]
        private SharedDataNotifierType sharedDataNotifier;
#endregion

#region Unity API
        private void OnEnable()
        {
            sharedDataNotifier.Subscribe( OnSharedDataChange );
        }
        
        private void OnDisable()
        {
            sharedDataNotifier.Unsubscribe( OnSharedDataChange );
        }
#endregion

#region Base Class API
        protected virtual void OnSharedDataChange()
        {
            textRenderer.text = sharedDataNotifier.SharedValue.ToString();
        }
#endregion
    }
}
