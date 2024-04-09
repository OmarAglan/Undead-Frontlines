using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MainMenuUICode.MainCode
{
    public class SoundElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [Header("RESOURCES")]
        public AudioClip hoverSound;
        public AudioClip clickSound;
        public AudioClip notificationSound;

        [Header("Settings")]
        public bool enableHoverSound = true;
        public bool enableClickSound = true;
        public bool isNotificationSound = false;

        private AudioSource HoverAudioSource { get { return GetComponent<AudioSource>(); } }
        private AudioSource ClickAudioSource { get { return GetComponent<AudioSource>(); } }
        private AudioSource NotificationAudioSource { get { return GetComponent<AudioSource>(); } }


        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            gameObject.AddComponent<AudioSource>();
            HoverAudioSource.clip = hoverSound;
            ClickAudioSource.clip = clickSound;
            NotificationAudioSource.clip = notificationSound;

            HoverAudioSource.playOnAwake = false;
            ClickAudioSource.playOnAwake = false;

        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enableHoverSound == true)
            {
                HoverAudioSource.PlayOneShot(hoverSound);
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (enableClickSound == true)
            {
                ClickAudioSource.PlayOneShot(clickSound);
            }
        }
        public void Notification()
        {
            if (isNotificationSound == true)
            {
                NotificationAudioSource.PlayOneShot(notificationSound);
            }
        }

    }

}