using UnityEngine;

namespace OnlineMap
{
    public partial class MapStart : MonoBehaviour
    {
        static MapStart _instance;


        public static MapStart Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Misc.FindObjectOfType<MapStart>();

                    if (_instance == null)
                    {
                        Debug.LogWarning(" 'MapStart' Object not found. Create it in the scene.");
                    }
                }
                return _instance;
            }
        }

    }

}
