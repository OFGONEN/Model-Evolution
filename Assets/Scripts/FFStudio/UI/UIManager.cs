using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NaughtyAttributes;

namespace FFStudio
{
    public class UIManager : MonoBehaviour
    {
        #region Fields

        [Header("Event Listeners")]
        public EventListenerDelegateResponse levelLoadedResponse;
        public EventListenerDelegateResponse levelCompleteResponse;
        public EventListenerDelegateResponse levelFailResponse;
        public EventListenerDelegateResponse tapInputListener;

        [Header("Shared Variables")]
        public SharedFloatProperty levelLoadingProgressProperty;
        public SharedFloatProperty levelProgressProperty;

        [Header("UI Elements")]
        public UIText informationText;
        public Image loadingScreenImage;
        public UIImage levelLoadingProgressImage;
        public UIText levelCountText;
        public UIImage levelProgressImage;
        public Image foreGroundImage;
        public RectTransform tutorialObjects;

        [Header("Fired Events")]
        public GameEvent levelRevealedEvent;
        public GameEvent loadNewLevelEvent;
        public GameEvent resetLevelEvent;
        public ElephantLevelEvent elephantLevelEvent;


        #endregion

        #region UnityAPI

        private void OnEnable()
        {
            levelLoadingProgressProperty.changeEvent += LevelLoadingProgressResponse;
            levelProgressProperty.changeEvent += LevelProgressResponse;

            levelLoadedResponse.OnEnable();
            levelFailResponse.OnEnable();
            levelCompleteResponse.OnEnable();
            tapInputListener.OnEnable();
        }

        private void OnDisable()
        {
            levelLoadingProgressProperty.changeEvent -= LevelLoadingProgressResponse;
            levelProgressProperty.changeEvent -= LevelProgressResponse;

            levelLoadedResponse.OnDisable();
            levelFailResponse.OnDisable();
            levelCompleteResponse.OnDisable();
            tapInputListener.OnDisable();
        }

        private void Awake()
        {
            levelLoadedResponse.response = LevelLoadedResponse;
            levelFailResponse.response = LevelFailResponse;
            levelCompleteResponse.response = LevelCompleteResponse;
            tapInputListener.response = ExtensionMethods.EmptyMethod;

            informationText.textRenderer.text = "Tap to Foo";
        }
        #endregion

        #region Implementation

        void LevelLoadingProgressResponse()
        {
            levelLoadingProgressImage.imageRenderer.fillAmount = levelLoadingProgressProperty.sharedValue;
        }

        void LevelProgressResponse()
        {
            levelProgressImage.imageRenderer.fillAmount = levelProgressProperty.sharedValue;
        }

        void LevelLoadedResponse()
        {
            var sequance = DOTween.Sequence();

            sequance.Append(levelLoadingProgressImage.GoPopIn());
            sequance.Append(loadingScreenImage.DOFade(0, GameSettings.Instance.ui_Entity_Fade_TweenDuration)); 
            sequance.AppendCallback(() => tapInputListener.response = StartLevel);

            levelCountText.textRenderer.text = "Level " + CurrentLevelData.Instance.currentConsecutiveLevel;

            levelLoadedResponse.response = NewLevelLoaded;
        }

        void LevelCompleteResponse()
        {
            var sequence = DOTween.Sequence();

            Tween tween = null;

            informationText.textRenderer.text = "Completed \n\n Tap to Contiune";

            sequence.Append(tween); //TODO: UIElements tween
            sequence.Append(foreGroundImage.DOFade(0.5f, GameSettings.Instance.ui_Entity_Fade_TweenDuration));
            sequence.Append(informationText.GoPopOut());
            sequence.AppendCallback(() => tapInputListener.response = LoadNewLevel);

            elephantLevelEvent.level = CurrentLevelData.Instance.currentConsecutiveLevel;
            elephantLevelEvent.elephantEventType = ElephantEvent.LevelCompleted;
            elephantLevelEvent.Raise();
        }

        [Button]
        void LevelFailResponse()
        {
            var sequence = DOTween.Sequence();

            Tween tween = null;

            informationText.textRenderer.text = "Level Failed \n\n Tap to Contiune";

            sequence.Append(tween); //TODO: UIElements tween
            sequence.Append(foreGroundImage.DOFade(0.5f, GameSettings.Instance.ui_Entity_Fade_TweenDuration));
            sequence.Append(informationText.GoPopOut());
            sequence.AppendCallback(() => tapInputListener.response = Resetlevel);

            elephantLevelEvent.level = CurrentLevelData.Instance.currentConsecutiveLevel;
            elephantLevelEvent.elephantEventType = ElephantEvent.LevelFailed;
            elephantLevelEvent.Raise();
        }

        void NewLevelLoaded()
        {
            levelCountText.textRenderer.text = "Level " + CurrentLevelData.Instance.currentConsecutiveLevel;

            var sequence = DOTween.Sequence();

            Tween tween = null;

            sequence.Append(foreGroundImage.DOFade(0, GameSettings.Instance.ui_Entity_Fade_TweenDuration));
            sequence.Append(tween); //TODO: UIElements tween
            sequence.AppendCallback(levelRevealedEvent.Raise);

            elephantLevelEvent.level = CurrentLevelData.Instance.currentConsecutiveLevel;
            elephantLevelEvent.elephantEventType = ElephantEvent.LevelStarted;
            elephantLevelEvent.Raise();
        }

        void StartLevel()
        {
            foreGroundImage.DOFade(0, GameSettings.Instance.ui_Entity_Fade_TweenDuration);
            informationText.GoPopIn().OnComplete(levelRevealedEvent.Raise);
            tutorialObjects.gameObject.SetActive(false);

            tapInputListener.response = ExtensionMethods.EmptyMethod;

            elephantLevelEvent.level = CurrentLevelData.Instance.currentConsecutiveLevel;
            elephantLevelEvent.elephantEventType = ElephantEvent.LevelStarted;
            elephantLevelEvent.Raise();
        }

        void LoadNewLevel()
        {
            FFLogger.Log("Load New Level");
            tapInputListener.response = ExtensionMethods.EmptyMethod;

            var sequence = DOTween.Sequence();

            sequence.Append(foreGroundImage.DOFade(1f, GameSettings.Instance.ui_Entity_Fade_TweenDuration));
            sequence.Join(informationText.GoPopIn());
            sequence.AppendCallback(loadNewLevelEvent.Raise);
        }

        void Resetlevel()
        {
            FFLogger.Log("Reset Level");
            tapInputListener.response = ExtensionMethods.EmptyMethod;

            var sequence = DOTween.Sequence();

            sequence.Append(foreGroundImage.DOFade(1f, GameSettings.Instance.ui_Entity_Fade_TweenDuration));
            sequence.Join(informationText.GoPopIn());
            sequence.AppendCallback(resetLevelEvent.Raise);

            elephantLevelEvent.level = CurrentLevelData.Instance.currentConsecutiveLevel;
            elephantLevelEvent.elephantEventType = ElephantEvent.LevelStarted;
            elephantLevelEvent.Raise();
        }


        #endregion
    }
}