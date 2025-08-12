using UnityEngine;

/// <summary>
/// 测试，血量类，护盾无法阻挡的伤害由血量承受，超过血量上线则无法处理
/// 假设血量为100点
/// </summary>
public class SecondHandler : Handler<TestRequest>
{
    protected override bool CanHandle(TestRequest request)
    {
        if (request.Attackvalue < 100)
        {
            return true;
        }
        return false;
    }

    protected override void Process(TestRequest request)
    {
        Debug.Log($"受到了{request.Attackvalue}点伤害");
    }
    
    
}
