using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EventSystem.Core
{
    
    
    

    public static class EventCenter
    {
        // 存储结构：事件类型 -> 处理程序列表
        private static readonly Dictionary<Type, List<Delegate>> EventHandlers = 
            new Dictionary<Type, List<Delegate>>();
        
        // 注册对象 -> 事件类型映射（用于注销）
        private static readonly Dictionary<IEventRegister, List<Type>> RegisterMap = 
            new Dictionary<IEventRegister, List<Type>>();
        
        
        public static void Register(IEventRegister eventRegister)
        {
            
            Type type = eventRegister.GetType();
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            
            foreach (var method in type.GetMethods(flags))
            {
                if (!method.IsDefined(typeof(EventAttribute), false)) 
                    continue;
                
                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                {
                    Debug.LogError($"事件方法 {method.Name} 必须有且只有一个参数");
                    continue;
                }
                Type eventType = parameters[0].ParameterType;
                if (!typeof(IEvent).IsAssignableFrom(eventType))
                {
                    Debug.LogError($"事件方法 {method.Name} 的参数必须实现 IEvent 接口");
                    continue;
                }
                
                // 创建泛型委托类型
                Type delegateType = typeof(Action<>).MakeGenericType(eventType);
               
                try
                {
                    // 创建特定事件类型的委托
                    var handler = Delegate.CreateDelegate(delegateType, eventRegister, method);
                    
                    // 注册事件处理程序
                    if (!EventHandlers.TryGetValue(eventType, out var handlers))
                    {
                        handlers = new List<Delegate>();
                        EventHandlers[eventType] = handlers;
                    }
                    handlers.Add(handler);
                    
                    // 添加到注册映射
                    if (!RegisterMap.TryGetValue(eventRegister, out var eventTypes))
                    {
                        eventTypes = new List<Type>();
                        RegisterMap[eventRegister] = eventTypes;
                    }
                    eventTypes.Add(eventType);
                    
                }
                catch (Exception ex)
                {
                    Debug.LogError($"创建委托失败: {method.Name}. 错误: {ex.Message}");
                }
            }
        }

        public static void UnRegister(IEventRegister eventRegister)
        {
            if (RegisterMap.TryGetValue(eventRegister, out var eventTypes))
            {
                foreach (var eventType in eventTypes)
                {
                    if (EventHandlers.TryGetValue(eventType, out var handlers))
                    {
                        // 移除该对象的所有处理程序
                        handlers.RemoveAll(d => d.Target == eventRegister);
                        
                        // 如果事件类型没有处理程序，移除空列表
                        if (handlers.Count == 0)
                        {
                            EventHandlers.Remove(eventType);
                        }
                    }
                }
                
                RegisterMap.Remove(eventRegister);
            }
        }

        public static void Trigger<TEvent>(TEvent eventData) where TEvent : struct, IEvent
        {
            Type eventType = typeof(TEvent);
            
            if (EventHandlers.TryGetValue(eventType, out var handlers))
            {
            
                for (int i = 0; i < handlers.Count; i++)
                {
                    (handlers[i] as Action<TEvent>)(eventData);
                }
            }
            
        }
    }
}
