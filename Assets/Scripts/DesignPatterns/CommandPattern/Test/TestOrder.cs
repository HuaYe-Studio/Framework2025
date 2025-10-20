using UnityEngine;
/// <summary>
/// 测试用命令
/// </summary>
public class MoveOrder : IOrder
{
    public void Execute()
    {
        
        Debug.Log("Move do");
    }

    public void Undo()
    {
        Debug.Log("Move Undo");
    }
}


public class ColorChangeOrder : IOrder
{
    public void Execute()
    {
        Debug.Log("Color do");
    }

    public void Undo()
    {
        Debug.Log("Color Undo");
    }
}


public class ScaleOrder : IOrder
{


    public void Execute()
    {
 
        Debug.Log("Scale do");
    }

    public void Undo()
    {

        Debug.Log("Scale Undo");
    }
}
