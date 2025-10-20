using UnityEngine;

/// <summary>
/// 测试，护盾类,
/// 假设护盾值固定20，超过护盾值会直接破甲且不被护盾吸收
/// </summary>
public class FirstHandler : Handler<TestRequest>
{
    protected override bool CanHandle(TestRequest request)
    {
        if (request.Attackvalue <= 20)
        {
            return true;
        }
        
        return false;
    }

    protected override void Process(TestRequest request)
    {
        Debug.Log($"护盾吸收了{request.Attackvalue}点伤害");
    }
}
