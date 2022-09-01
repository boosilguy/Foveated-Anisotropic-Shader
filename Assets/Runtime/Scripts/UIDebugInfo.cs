using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace foveated.sample
{
    public class UIDebugInfo : MonoBehaviour
    {
        ScrollRect scrollRect;

        [Header("Component")]
        [SerializeField] GameObject logElement;
        [SerializeField] int fontSize = 36;
        [SerializeField] Transform debugInfo;
        [SerializeField] Button debugButton;

        static UIDebugInfo instance = null;
        static Dictionary<string, Text> logTextDic = new Dictionary<string, Text>();

        public static UIDebugInfo Instance => instance;

        static void Init()
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIDebugInfo>();
                if (instance == null)
                {
                    Debug.LogError("<color=#00FF22>[UIDebugInfo]</color> UIDebugInfo를 찾을 수 없습니다.");
                    return;
                }
                instance.scrollRect = instance.GetComponentInChildren<ScrollRect>();
                instance.debugButton.onClick.AddListener(() => instance.debugInfo.gameObject.SetActive(!instance.debugInfo.gameObject.activeSelf));
            }
        }

        public static Text Log(string logMsg = null, string key = null, int fontSize = -1)
        {
            Init();
            Text item;
            if (!string.IsNullOrEmpty(key))
            {
                if (logTextDic.TryGetValue(key, out item))
                {
                    item.text = $"{key} : {logMsg}";
                    return item;
                }
            }

            GameObject logElement = Instantiate(instance.logElement, instance.scrollRect.content);

            logElement.name = "Log Element" + (string.IsNullOrEmpty(key) ? "" : $"({key})");
            
            item = logElement.GetComponent<Text>();
            item.fontSize = (fontSize == -1) ? instance.fontSize : fontSize;
            item.alignment = TextAnchor.MiddleLeft;

            if (!string.IsNullOrEmpty(key))
            {
                item.text = $"{key} : ";
                logTextDic.Add(key, item);
            }

            if (!string.IsNullOrEmpty(logMsg))
                item.text += logMsg;

            return item;
        }
    }
}