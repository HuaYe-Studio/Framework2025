using System;
using UnityEngine;

/// <summary>
/// 组装责任链函数
/// 以无Mono版本为例，展示一下组装，就不运行了
/// </summary>
public class TestChain : MonoBehaviour
{
    private FirstHandler firstHandler;
    private SecondHandler secondHandler;

    private TestRequest request;

    /// <summary>
    /// 在start中组装（或者别的合适的生命周期中）
    /// </summary>
    private void Start()
    {
        firstHandler = new FirstHandler();
        secondHandler = new SecondHandler();
        request = new TestRequest();
        
        firstHandler.SetNext(secondHandler);
    }

    /// <summary>
    /// 攻击函数，调用攻击函数就会进行一次伤害的判定传递
    /// </summary>
    /// <param name="value"></param>
    public void Attack(int value)
    {
        request.Attackvalue = value;
        
        firstHandler.Handle(request);
    }
}
