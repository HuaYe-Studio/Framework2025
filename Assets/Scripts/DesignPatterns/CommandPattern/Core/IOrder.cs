using UnityEngine;

public interface IOrder
{
    /// <summary>
    /// 执行命令
    /// </summary>
    public void Execute();
    
    /// <summary>
    /// 撤销命令
    /// </summary>
    public void Undo();
}
