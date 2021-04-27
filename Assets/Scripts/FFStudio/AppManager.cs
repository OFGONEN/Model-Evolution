/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FFStudio
{
	public class AppManager : MonoBehaviour
	{
		#region Fields
		[Header( "Event Listeners" )]
		public EventListenerDelegateResponse loadNewLevelListener;
		public EventListenerDelegateResponse resetLevelListener;

		[Header( "Fired Events" )]
		public GameEvent levelLoaded;
		public GameEvent cleanUpEvent;

		public CurrentLevelData currentLevel;
		#endregion

		#region Unity API
		private void OnEnable()
		{
			loadNewLevelListener.OnEnable();
			resetLevelListener.OnEnable();
		}
		private void OnDisable()
		{
			loadNewLevelListener.OnDisable();
			resetLevelListener.OnDisable();

		}
		private void Awake()
		{
			loadNewLevelListener.response = LoadNewLevel;
			resetLevelListener.response = ResetLevel;
		}

		private void Start()
		{
			LoadLevel();
		}
		#endregion

		#region API
		public void ResetScene()
		{
			var operation = SceneManager.UnloadSceneAsync( currentLevel.levelData.sceneIndex ); // Unload current scene

			cleanUpEvent.Raise();

			// When unloading done load the same scene again
			operation.completed += ( AsyncOperation operation ) =>
			SceneManager.LoadScene( currentLevel.levelData.sceneIndex, LoadSceneMode.Additive );

		}
		#endregion

		#region Implementation
		/// <summary>
		/// Same as ResetScene method but raises level loaded event
		/// </summary>
		public void ResetLevel()
		{
			ResetScene();

			levelLoaded.Raise();
		}
		private void LoadLevel()
		{
			currentLevel.currentLevel = PlayerPrefs.GetInt( "Level", 1 );
			currentLevel.currentConsecutiveLevel = PlayerPrefs.GetInt( "Consecutive Level", 1 );

			currentLevel.LoadCurrentLevelData();

			cleanUpEvent.Raise();
			SceneManager.LoadScene( currentLevel.levelData.sceneIndex, LoadSceneMode.Additive );

			levelLoaded.Raise();
		}
		private void LoadNewLevel()
		{
			currentLevel.currentLevel++;
			currentLevel.currentConsecutiveLevel++;
			PlayerPrefs.SetInt( "Level", currentLevel.currentLevel );
			PlayerPrefs.SetInt( "Consecutive Level", currentLevel.currentConsecutiveLevel );


			var _operation = SceneManager.UnloadSceneAsync( currentLevel.levelData.sceneIndex );
			_operation.completed += ( AsyncOperation operation ) => LoadLevel();
		}
		#endregion
	}
}