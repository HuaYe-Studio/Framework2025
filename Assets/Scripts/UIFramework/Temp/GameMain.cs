using System;
using UIFramework.Core;
using UIFramework.ViewPath;
using UnityEngine;

namespace UIFramework.Temp
{
    public class GameMain : MonoBehaviour
    {
        
        private void Start()
        {
            MainPanelView mainPanelView = UIManager.Instance.GetPanel<MainPanelView>();
            Debug.Log(mainPanelView.gameObject.name);
        }

        // private void Update()
        // {
        //     if(Input.GetKeyDown(KeyCode.Escape))
        //     {
        //         Debug.Log("Escape Pressed");
        //     }
        // }
        
       
    }
}