/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FFStudio
{
	public class Debug_UI_Text : MonoBehaviour
	{
#region Fields
		public Pool_Debug_UI_Text pool;

        private UI_Float ui_float; 
        private UI_Fade_Text ui_text; 
#endregion

#region Properties
#endregion

#region Unity API
        private void Awake()
        {
            ui_float = GetComponentInChildren< UI_Float >();
            ui_text  = GetComponentInChildren< UI_Fade_Text >();
        }
#endregion

#region API
        public void Spawn( Vector3 position ,string text )
        {
			gameObject.SetActive( true );

			ui_text.UI_Text.text  = text;
			ui_text.UI_Text.color = ui_text.UI_Text.color.SetAlpha( 1 );

			ui_float.UI_RectTransform.position = position;

			ui_float.DoFloat( GameSettings.Instance.debug_ui_text_float_height,
				GameSettings.Instance.debug_ui_text_float_duration );

			ui_text.DoFade( 0, GameSettings.Instance.debug_ui_text_float_duration );

			ui_float.ui_OnComplete.AddListener( OnFloatComplete );
		}
#endregion

#region Implementation
		private void OnFloatComplete()
		{
			ui_float.ui_OnComplete.RemoveListener( OnFloatComplete );
			pool.ReturnEntity( this );
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}