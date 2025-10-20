using System;
using UnityEngine;
/// <summary>
/// 测试输入
/// 1、2、3执行不同命令
/// 0撤销命令
/// 9重做撤销命令
/// （键盘数字键）
/// </summary>
public class TestInput : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MoveOrder moveOrder = new MoveOrder();
            
            OrderInvoker.Instance.Execute(moveOrder);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ColorChangeOrder colorChangeOrder = new ColorChangeOrder();
            
            OrderInvoker.Instance.Execute(colorChangeOrder);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ScaleOrder scaleOrder = new ScaleOrder();
            
            OrderInvoker.Instance.Execute(scaleOrder);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            OrderInvoker.Instance.Undo();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            OrderInvoker.Instance.Redo();
        }
    }
}
