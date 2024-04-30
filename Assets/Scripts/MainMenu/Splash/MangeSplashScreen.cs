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
        public bool alwaysShowLoginScreen = true;
        public bool disableSplashScreen;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            if (disableSplashScreen == true)
            {
                splashScreen.SetActive(false);
                splashScreenLogin.SetActive(false);
                splashScreenRegister.SetActive(false);
                mainPanels.SetActive(true);

                mainPanelsAnimator = mainPanels.GetComponent<Animator>();
                mainPanelsAnimator.Play("Main Panel Opening");

            }
            else if (isLogged == false && alwaysShowLoginScreen == true)
            {
                splashScreen.SetActive(false);
                splashScreenLogin.SetActive(true);
                splashScreenRegister.SetActive(true);
            }

            else if (isLogged == false && alwaysShowLoginScreen == false)
            {
                splashScreen.SetActive(false);
                splashScreenLogin.SetActive(true);
                splashScreenRegister.SetActive(true);
            }

            else if (isLogged == false && alwaysShowLoginScreen == false)
            {
                splashScreen.SetActive(false);
                splashScreenLogin.SetActive(true);
                splashScreenRegister.SetActive(true);
            }

            else if (isLogged == true && alwaysShowLoginScreen == true)
            {
                splashScreen.SetActive(false);
                splashScreenLogin.SetActive(true);
                splashScreenRegister.SetActive(true);
            }

            else if (isLogged == true && alwaysShowLoginScreen == false)
            {
                splashScreen.SetActive(true);
                splashScreenLogin.SetActive(false);
                splashScreenRegister.SetActive(false);
            }
        }
    }
}
