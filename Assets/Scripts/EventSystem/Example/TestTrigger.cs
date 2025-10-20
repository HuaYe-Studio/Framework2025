using System.Collections.Generic;
using EventSystem.Core;
using UnityEngine;


public class TestTrigger : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //通过new一个事件 去触发事件 且通过构造函数传参
            EventCenter.Trigger(new TestEvent(1.22f , "Hello World" , new List<string>() {"1", "2", "3" }));
        }
    }
}
