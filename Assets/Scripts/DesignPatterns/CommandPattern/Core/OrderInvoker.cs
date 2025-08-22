using System.Collections.Generic;
using UnityEngine;

public class OrderInvoker : Singleton<OrderInvoker>
{
    private Stack<IOrder>  _orderHistory = new Stack<IOrder>();
    
    private Stack<IOrder> _redoStack = new Stack<IOrder>();

    /// <summary>
    /// 执行命令,并记录入历史中
    /// </summary>
    /// <param name="order">被执行的命令</param>
    public void Execute(IOrder order)
    {
        order.Execute();
        _orderHistory.Push(order);
        _redoStack.Clear();
    }

    /// <summary>
    /// 撤销上一个命令
    /// </summary>
    public void Undo()
    {
        if (_orderHistory.Count > 0)
        {
            IOrder order = _orderHistory.Pop();
            order.Undo();
            _redoStack.Push(order);
        }
    }

    /// <summary>
    /// 重新执行上一条被撤销的命令
    /// </summary>
    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            IOrder order = _redoStack.Pop();
            order.Execute();
            _orderHistory.Push(order);
        }
    }

    /// <summary>
    /// 清空命令历史
    /// </summary>
    public void ClearHistory()
    {
        _orderHistory.Clear();
        _redoStack.Clear();
    }
}
