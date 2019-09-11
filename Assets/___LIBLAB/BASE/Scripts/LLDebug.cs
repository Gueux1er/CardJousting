using UnityEngine;

namespace LibLabSystem
{
    public class LLDebug : MonoBehaviour
    {
        public bool DEBUG_KEY_ACTIVE;

        public KeyCode DebugRestartScene;
        public KeyCode DebugDisplayKey;

        private bool DEBUG_DISPLAY;

        private GUIStyle debugDisplayStyle;
        private Texture2D debugBackground;
        private string textProduct;
        private string textFPS;
        private float deltaTime = 0.0f;
        private float msec;
        private float fps;


        private void Awake()
        {
            debugBackground = new Texture2D(1, 1);
            debugBackground.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.5f));
            debugBackground.Apply();

            debugDisplayStyle = new GUIStyle();
            debugDisplayStyle.normal.background = debugBackground;
            debugDisplayStyle.fontSize = Screen.height * 3 / 100;
            debugDisplayStyle.normal.textColor = Color.white;
            debugDisplayStyle.margin = new RectOffset(0, 0, 2, 0);
            debugDisplayStyle.padding = new RectOffset(3, 3, 1, 1);
            debugDisplayStyle.stretchWidth = false;

            textProduct = string.Format("<i>{0} {1}</i>", Application.productName, Application.version);

            textFPS = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        }

        private void Update()
        {
            if (!DEBUG_KEY_ACTIVE) return;

            if (Input.GetKeyDown(DebugDisplayKey))
            {
                DEBUG_DISPLAY = !DEBUG_DISPLAY;
            }
            else if (Input.GetKey(DebugRestartScene))
            {
                LLUtils.ReloadCurrentScene();
            }
        }

        private void OnGUI()
        {
            if (DEBUG_DISPLAY)
            {
                deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

                msec = deltaTime * 1000.0f;
                fps = 1.0f / deltaTime;
                textFPS = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

                GUILayout.Label(textProduct, debugDisplayStyle);
                GUILayout.Label(textFPS, debugDisplayStyle);
            }
        }
    }
}