/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace FFStudio
{
    public class UI_PunchScale_Base< NotifierType > : UIEntity
    {
#region Fields (Inspector Interface)
    [ Title( "Parameters" ) ]
        [ SerializeField ] private float punchScale;

    [ Title( "Observed Shared Data" ) ]
        [ SerializeField ] private SharedDataNotifier< NotifierType > notifier_count;
#endregion

#region Fields (Private)
        private Vector3 originalScale;
        private RecycledTween punchScaleTween;
        private float duration;
#endregion

#region Properties
#endregion

#region Unity API
        private void OnEnable()
        {
            notifier_count.Subscribe( OnCountChange );
        }
        
        private void OnDisable()
        {
            notifier_count.Unsubscribe( OnCountChange );
        }
        
        private void Awake()
        {
            originalScale = uiTransform.localScale;

            punchScaleTween = new RecycledTween();

            duration = GameSettings.Instance.ui_Entity_Scale_TweenDuration;
        }
#endregion

#region API
        public void PunchScale()
        {
            punchScaleTween.Kill();
            uiTransform.localScale = originalScale;
            punchScaleTween.Recycle( uiTransform.DOPunchScale( Vector3.one * punchScale, duration, 1, 1 ), OnPunchScaleComplete );

    #if UNITY_EDITOR
            punchScaleTween.Tween.SetId( name + "_ff_UIPunchScale" );
    #endif
        }

        public void PunchScale( float strength )
        {
            punchScaleTween.Kill();
            uiTransform.localScale = originalScale;
            punchScaleTween.Recycle( uiTransform.DOPunchScale( Vector3.one * strength, duration, 1, 1 ), OnPunchScaleComplete );

    #if UNITY_EDITOR
            punchScaleTween.Tween.SetId( name + "_ff_UIPunchScale" );
    #endif
        }
        
        public void CancelAllShakes()
        {
            punchScaleTween.Kill();
        }
#endregion

#region Implementation
        protected virtual void OnCountChange()
        {
            PunchScale();
        }
        
        protected virtual void OnPunchScaleComplete()
        {
            uiTransform.localScale = originalScale;
        }
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
    }
}