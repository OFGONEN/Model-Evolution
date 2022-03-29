/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

namespace FFStudio
{
    public class UI_Update_Text< SharedDataNotifierType, SharedDataType > : MonoBehaviour
        where SharedDataNotifierType : SharedDataNotifier< SharedDataType >
    {
#region Fields
        [ BoxGroup( "Setup" ), SerializeField ] protected SharedDataNotifierType sharedDataNotifier;
        [ BoxGroup( "Setup" ), SerializeField ] protected string text_prefix;
        [ BoxGroup( "Setup" ), SerializeField ] protected string text_suffix;

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
			ui_Text.text = text_prefix + sharedDataNotifier.SharedValue.ToString() + text_suffix;
        }
#endregion
    }
}
