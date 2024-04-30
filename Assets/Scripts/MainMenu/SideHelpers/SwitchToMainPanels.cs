using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainMenuUICode.MainCode
{
    public class SwitchToMainPanels : MonoBehaviour
    {
        [Header("RESOURCES")]
        private Animator loginScreenAnimator;
        public Animator mainPanelAnimator;
        public Animator shadowsAnimator;
        [Header("Settings")]
        public bool isLoginScreen;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            loginScreenAnimator = this.GetComponent<Animator>();
            if (isLoginScreen == false)
            {
                loginScreenAnimator.Play("SS Fade-out");
                mainPanelAnimator.Play("Main Panel Opening");
                shadowsAnimator.Play("CG Fade-in");
            }
        }

        public void Animate()
        {
            loginScreenAnimator = GetComponent<Animator>();
            if (isLoginScreen == false)
            {
                loginScreenAnimator.Play("SS w Login Fade-out");

            }

            mainPanelAnimator.Play("Main Panel Opening");
            shadowsAnimator.Play("CG Fade-in");
        }

    }
}
