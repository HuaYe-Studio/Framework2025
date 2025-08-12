using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 模拟产品注册
/// </summary>
public class TestProduct : MonoBehaviour
{
    private TestProduct1 _testProduct1;
    private string type;
    
    private ITest[]  _tests;

    private void Awake()
    {
        _testProduct1 = new TestProduct1();
        type = "Test";
        
        SingleFactory.Instance.Register(type, _testProduct1);
    }

}
