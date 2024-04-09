using UnityEngine;
using UnityEngine.UI;

namespace MainMenuUICode.MainCode
{
    public class LoginManager : MonoBehaviour
    {
        [Header("RESOURCES")]
        public SwitchingMainPanels switchingMainPanels;
        public SoundElement soundScript;
        public Animator wrongAnimator;
        public Text usernametext;
        public Text passwordtext;

        [Header("Settings")]
        public string username;
        public string password;

        public void Login()
        {
            if (usernametext.text == username && passwordtext.text == password)
            {
                switchingMainPanels.Animator();
            }
            else
            {
                wrongAnimator.Play("Notification In");
                //soundScript.Notification();
            }
        }


    }
}
