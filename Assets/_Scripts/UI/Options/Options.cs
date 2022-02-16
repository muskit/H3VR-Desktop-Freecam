using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopFreecam
{
    // Options window
    public class Options : MonoBehaviour
    {
        private Button btnClose;

        // Use this for initialization
        void Awake()
        {
            btnClose = transform.GetChild(0).GetChild(1).GetComponent<Button>();
            btnClose.onClick.AddListener(OnCloseClick);
        }

        void OnCloseClick()
        {
            Destroy(gameObject);
        }
    }
}