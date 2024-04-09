using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainMenuUICode.MainCode
{
    public class MangeSplashScreen : MonoBehaviour
    {
        [Header("Resources")]
        public GameObject splashScreen;
        public GameObject splashScreenLogin;
        public GameObject splashScreenRegister;
        public GameObject mainPanels;
        private Animator mainPanelsAnimator;

        [Header("Settings")]
        public bool isLogged;
        public bool alwaysShowSplashScreen;
        public bool disableSplashScreen;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {

        }
    }
}
