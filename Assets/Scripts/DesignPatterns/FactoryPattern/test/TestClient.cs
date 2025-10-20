using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模拟客户端获取并使用产品
/// </summary>
public class TestClient : MonoBehaviour
{
    private List<ITest> _tests;

    void Start()
    {
        _tests = SingleFactory.Instance.GetProduct<ITest>("Test");
        
        _tests[0].Say(1);
    }

}
