using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenuUICode.mainCode
{
    public class ToggleMang : MonoBehaviour
    {
        [Header("Toggles")]
        public Toggle toggleObject;

        [Header("animation")]
        public Animator toggleAnimator;


        //
        private string toggleOn = "Toggle On";
        private string toggleOff = "Toggle Off";

        void Start()
        {
            this.toggleObject.GetComponent<Toggle>();
            toggleObject.onValueChanged.AddListener(TaskClicked);

            if (toggleObject.isOn)
            {
                toggleAnimator.Play(toggleOn);
            }
            else
            {
                toggleAnimator.Play(toggleOff);
            }
        }

        void TaskClicked(bool value)
        {
            if (toggleObject.isOn)
            {
                toggleAnimator.Play(toggleOn);
            }
            else
            {
                toggleAnimator.Play(toggleOff);
            }
        }
    }
}
