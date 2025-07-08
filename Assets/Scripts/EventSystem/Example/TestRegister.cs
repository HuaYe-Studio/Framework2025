using EventSystem.Core;
using UnityEngine;


/// <summary>
///  使用事件系统的类必须实现IEventRegister接口
/// </summary>
public class TestRegister : MonoBehaviour , IEventRegister
{
    private void Awake()
    {
        //要使用此事件系统的类，必须在Awake()方法中将自己注册到事件中心
        EventCenter.Register(this);
    }
    
    private void OnDestroy()
    {
        //要使用此事件系统的类，必须在OnDestroy()方法中注销自己，防止空引用报错
        EventCenter.UnRegister(this);
    }
    
    //好处是 无论使用多少个事件 ， 只需要注册和注销一次即可

    
    //标记Event特性 ， 表示此方法是一个事件方法 ， 且参数类型为一个继承了IEvent接口的类
    //订阅的事件就是参数 TestEvent 事件 因为我们是以Type为参数的形式订阅的 而不是字符串
    [Event]
    public void Test(TestEvent e)
    {
        //在这里处理事件
        //使用TestEvent参数可以获取事件传递的数据
        
        print(e.testfloat);
        print(e.teststring);
        e.testList.ForEach(x => print(x));
        
    }
    
}
