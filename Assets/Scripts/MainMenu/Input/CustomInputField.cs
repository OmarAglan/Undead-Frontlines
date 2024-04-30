using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MainMenuUICode.MainCode
{
    public class CustomInputField : MonoBehaviour
    {
        [Header("ANIMATORS")]
        public Animator inputFieldAnimator;
        [Header("Objects")]
        public GameObject fieldTrigger;
        public Text inputFieldText;

        private bool isEmpty = true;
        private bool isClicked = false;
        private string inAnim = "In";
        private string outAnim = "Out";

        void Start()
        {
            // Check if text is empty or not
            if (inputFieldText.text.Length == 0 || inputFieldText.text.Length <= 0)
            {
                isEmpty = true;
            }

            else
            {
                isEmpty = false;
            }

            // Animate if it's empty
            if (isEmpty == true)
            {
                inputFieldAnimator.Play(outAnim);
            }

            else
            {
                inputFieldAnimator.Play(inAnim);
            }
        }
        void Update()
        {
            if (inputFieldText.text.Length == 1 || inputFieldText.text.Length >= 1)
            {
                isEmpty = false;
                inputFieldAnimator.Play(inAnim);
            }

            else if (isClicked == false)
            {
                inputFieldAnimator.Play(outAnim);
            }
        }
        public void Animate()
        {
            isClicked = true;
            inputFieldAnimator.Play(inAnim);
            fieldTrigger.SetActive(true);
        }

        public void FieldTrigger()
        {
            if (isEmpty == true)
            {
                inputFieldAnimator.Play(outAnim);
                fieldTrigger.SetActive(false);
                isClicked = false;
            }

            else
            {
                fieldTrigger.SetActive(false);
                isClicked = false;
            }
        }
    }
}
