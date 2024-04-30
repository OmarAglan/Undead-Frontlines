using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace MainMenuUICode.MainCode
{
    public class TooltipManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("settings")]
        public Text textObj;
        public string text;

        private Animator tooltipAnim;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            tooltipAnim = GetComponent<Animator>();
            textObj.text = text;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipAnim.Play("Tooltip In");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipAnim.Play("Tooltip Out");
        }
    }
}