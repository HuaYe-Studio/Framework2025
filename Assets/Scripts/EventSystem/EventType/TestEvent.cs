using System.Collections;
using System.Collections.Generic;
using EventSystem.Core;
using UnityEngine;




/// <summary>
/// 用于演示测试的事件 必须继承IEvent接口 表明这是一个事件
///
///这样写的好处是 可以查看谁调用了此事件 方便DeBug ， 且避免了装箱拆箱
/// 
/// </summary>
public struct TestEvent : IEvent
{
    //想要传递什么参数 就在这里声明 且加入构造函数。
    
    
    public float testfloat;

    public string teststring;

    public List<string> testList;

    public TestEvent(float testfloat, string teststring, List<string> testList)
    {
       this.testfloat = testfloat;
       this.teststring = teststring;
       this.testList = testList;

    }
}
