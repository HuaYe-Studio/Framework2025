using UnityEngine;
/// <summary>
/// 测试接口
/// </summary>
public interface ITest : IProduct
{
    public void Say(int number);
}

/// <summary>
/// 测试产品1
/// </summary>
public class TestProduct1 : ITest
{
    public void Say(int number)
    {
        if (number == 1)
        {
            Debug.Log("TestProduct1-1");
        }
        else if (number == 2)
        {
            Debug.Log("TestProduct1-2");
        }
        else
        {
            Debug.Log("TestProduct1-O");
        }
    }
}



