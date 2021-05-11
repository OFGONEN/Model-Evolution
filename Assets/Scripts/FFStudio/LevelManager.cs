using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace FFStudio
{
    public class LevelManager : MonoBehaviour
    {
        #region Fields
        [Header("Event Listeners")]
        public EventListenerDelegateResponse levelLoadedListener;
        public EventListenerDelegateResponse levelRevealedListener;
        public EventListenerDelegateResponse levelStartedListener;

        [Header("Fired Events")]
        public GameEvent levelFailedEvent;
        public GameEvent levelCompleted;

        [Header("Level Releated")]
        public SharedFloatProperty levelProgress;

        #endregion

        #region UnityAPI

        private void OnEnable()
        {
            levelLoadedListener  .OnEnable();
            levelRevealedListener.OnEnable();
            levelStartedListener .OnEnable();
        }

        private void OnDisable()
        {
            levelLoadedListener  .OnDisable();
            levelRevealedListener.OnDisable();
            levelStartedListener .OnDisable();
        }

        private void Awake()
        {

            levelLoadedListener.response   = LevelLoadedResponse;
            levelRevealedListener.response = LevelRevealedResponse;
            levelStartedListener.response  = LevelStartedResponse;
        }

        #endregion

        #region Implementation
        void LevelLoadedResponse()
        {
            levelProgress.SetValue(0);
        }

        void LevelRevealedResponse()
        {

        }

        void LevelStartedResponse()
        {

        }

        #endregion
    }
}