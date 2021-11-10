/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
    public abstract class SharedDataNotifier< SharedDataType > : ScriptableObject
    {
#region Fields (Public)
        public event ChangeEvent changeEvent;
#endregion

#region Fields (Private)
        [ SerializeField ]
        private SharedDataType sharedValue;
#endregion

#region Properties
        public SharedDataType SharedValue
        {
            get => sharedValue;
            set
            {
                if( !EqualityComparer< SharedDataType >.Default.Equals( sharedValue, value ) )
                {
                    sharedValue = value;

                    changeEvent?.Invoke();
                }
            }
        }
#endregion

#region API
        public void SetValue_DontNotify( SharedDataType value )
        {
            sharedValue = value;
        }

		public void SetValue_NotifyAlways( SharedDataType value )
		{
			sharedValue = value;
			changeEvent?.Invoke();
		}
#endregion
    }
}
