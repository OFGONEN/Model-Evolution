/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using NaughtyAttributes;
using TMPro;

namespace FFStudio
{
    public class UI_Update_Text< SharedDataNotifierType, SharedDataType > : MonoBehaviour
        where SharedDataNotifierType : SharedDataNotifier< SharedDataType >
    {
#region Fields
        [ BoxGroup( "Setup" ), SerializeField ]
        protected SharedDataNotifierType sharedDataNotifier;

        protected TextMeshProUGUI ui_Text; 
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

        private void Awake()
        {
            ui_Text = GetComponentInChildren< TextMeshProUGUI >();
        }
#endregion

#region Base Class API
        protected virtual void OnSharedDataChange()
        {
			ui_Text.text = sharedDataNotifier.SharedValue.ToString();
        }
#endregion
    }
}
