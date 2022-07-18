using System;
using UnityEngine;

namespace Demo._99_Shared.Scripts
{
    public sealed class EventSystemController : MonoBehaviour
    {
        private static EventSystemController _instance;

        public static EventSystemController Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException(
                        "The singleton instance of the EventSystemController does not exits.");
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                return;
            }

            Destroy(gameObject);
        }
    }
}
