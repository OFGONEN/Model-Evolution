using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
    public class SharedReferanceSetter : MonoBehaviour
    {
        #region Fields
        public SharedReferenceProperty sharedReferanceProperty;
        public Component referanceComponent;
        #endregion

        #region UnityAPI
        private void Awake()
        {
            sharedReferanceProperty.SetValue(referanceComponent);
        }
        #endregion
    }
}