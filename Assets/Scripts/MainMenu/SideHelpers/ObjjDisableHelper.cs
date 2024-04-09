using UnityEngine;

namespace MainMenuUICode.MainCode
{
    public class ObjjDisableHelper : MonoBehaviour
    {
        private GameObject thisObj;
        private ObjjDisableHelper script;
        public bool disableScript = false;
        // Start is called before the first frame update
        void Start()
        {
            script = this.GetComponent<ObjjDisableHelper>();

            if (disableScript == true)
            {
                script.enabled = false;
            }
        }
    }
}
