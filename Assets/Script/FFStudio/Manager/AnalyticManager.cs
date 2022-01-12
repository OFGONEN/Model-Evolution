using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Facebook.Unity;
using ElephantSDK;

namespace FFStudio
{
	public class AnalyticManager : MonoBehaviour
	{
		#region Fields
		[Header( "Event Listeners" )]
		public EventListenerDelegateResponse elephantEventListener;
		public EventListenerDelegateResponse elephantRemoteConfigListener;

		public SharedStringNotifier build_string;
		#endregion

		#region UnityAPI
		private void OnEnable()
		{
			elephantEventListener.OnEnable();
			elephantRemoteConfigListener.OnEnable();
		}

		private void OnDisable()
		{
			elephantEventListener.OnDisable();
			elephantRemoteConfigListener.OnDisable();
		}

		private void Awake()
		{
			elephantEventListener.response = ElephantEventResponse;
			elephantRemoteConfigListener.response = ElephantRemoteConfigResponse;
		}

		private void Start()
		{
			LoadRemoteConfigs();

			var param = Params.New();
			param.customData = build_string.SharedValue;
			param.CustomString( build_string.SharedValue );

			Elephant.Event( "build_string", 0, param );
		}

		#endregion

		#region Implementation
		void ElephantRemoteConfigResponse()
		{
			var configEvent = elephantRemoteConfigListener.gameEvent as ElephantConfigEvent;

			var value = RemoteConfig.GetInstance().Get( configEvent.configKeyName );

			if( value != null )
				configEvent.source.SetFieldValue( configEvent.fieldName, value );
		}

		void ElephantEventResponse()
		{
			var gameEvent = elephantEventListener.gameEvent as ElephantLevelEvent;

			switch( gameEvent.elephantEventType )
			{
				case ElephantEvent.LevelStarted:
					Elephant.LevelStarted( gameEvent.level );
					FFLogger.Log( "FFAnalytic Elephant LevelStarted: " + gameEvent.level );
					break;
				case ElephantEvent.LevelCompleted:
					Elephant.LevelCompleted( gameEvent.level );
					FFLogger.Log( "FFAnalytic Elephant LevelFinished: " + gameEvent.level );
					break;
				case ElephantEvent.LevelFailed:
					Elephant.LevelFailed( gameEvent.level );
					FFLogger.Log( "FFAnalytic Elephant LevelFailed: " + gameEvent.level );
					break;
			}
		}

		void LoadRemoteConfigs()
		{
			var gameSettings = GameSettings.Instance;
			var useRemoteGameSettings = gameSettings.useRemoteConfig_GameSettings;

			if( !useRemoteGameSettings )
				return;

			var remote = RemoteConfig.GetInstance();
			var settings = remote.Get( "game_settings", "null" );

			if( settings == null )
			{
				Debug.Log( "Remote GameSettings could not configured" );
				return;
			}

			FFLogger.Log( "game_settings\n" + settings );
			var setting_keys = settings.Split( ',' );

			foreach( var settingName in setting_keys )
			{
				var value = remote.Get( settingName );

				if( value != null )
					gameSettings.SetFieldValue( settingName, value );
			}
		}
		#endregion
	}
}