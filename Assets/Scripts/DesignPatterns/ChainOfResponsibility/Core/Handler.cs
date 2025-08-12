using UnityEngine;

/// <summary>
/// 责任节点父类
/// </summary>
/// <typeparam name="T">待处理的任务类</typeparam>
public abstract class Handler<T> where T : class
{
    /// <summary>
    /// 下个责任节点
    /// </summary>
    private Handler<T> _next;
    
    /// <summary>
    /// 设置下个责任节点函数
    /// </summary>
    /// <param name="next">下个节点的引用注入</param>
    public void SetNext(Handler<T> next) => _next = next;
    
    /// <summary>
    /// 处理函数
    /// 先判断能否处理，能处理则解决，
    /// 如果不能处理有下个责任节点则传递，否则提示无人处理
    /// </summary>
    /// <param name="request">待处理任务</param>
    public void Handle(T request)
    {
        if (CanHandle(request))
        {
            Process(request);
        }
        else if (_next != null)
        {
            _next.Handle(request);
        }
        else
        {
            OnUnhandled(request);
        }
    }

    /// <summary>
    /// 判断符合处理条件函数
    /// </summary>
    /// <param name="request"></param>
    /// <returns>符合处理条件返回True，否则返回False</returns>
    protected abstract bool CanHandle(T request);
    
    /// <summary>
    /// 具体处理逻辑函数，子类覆写
    /// </summary>
    /// <param name="request">待处理任务</param>
    protected abstract void Process(T request);
    
    /// <summary>
    /// 无人处理反馈函数
    /// </summary>
    /// <param name="request"></param>
    protected virtual void OnUnhandled(T request) => Debug.LogWarning($"Unhandled: {request}");
}
