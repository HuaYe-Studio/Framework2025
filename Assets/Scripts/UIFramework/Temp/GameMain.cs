using System;
using UIFramework.Core;

using UnityEngine;

namespace UIFramework.Temp
{
    public class GameMain : MonoBehaviour
    {
        
        private void Start()
        {
            
           
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                // UIManager.Instance.HidePanel<MainPanelView>();
            }
        }
        
       
    }
}