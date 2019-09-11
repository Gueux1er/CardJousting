using UnityEngine;

namespace LibLabSystem
{
    public class LLSystem : MonoBehaviour
    {
        public static LLSystem instance;

        public static bool isInit
        {
            get { return instance != null; }
        }

        private void Awake()
        {
            if (LLSystem.instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            DontDestroyOnLoad(gameObject);

            LLUtils.LoadScene(1);
        }
    }
}