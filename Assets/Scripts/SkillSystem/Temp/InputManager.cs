using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoSingleton<InputManager>
{
    
    private InputSystem_Actions _inputSystem;

    public bool LAttack => _inputSystem.Player.Attack.triggered;
    
    public Vector2 Move => _inputSystem.Player.Move.ReadValue<Vector2>();
    
    public Vector2 Look => _inputSystem.Player.Look.ReadValue<Vector2>();

    public bool Run => _inputSystem.Player.Sprint.phase == InputActionPhase.Performed;










    // #region 单例模板
    //
    //

    // private static InputManager _instance;
    // private static object _lock = new object();
    //
    // public static InputManager MainInstance
    // {
    //     get
    //     {
    //         if (_instance == null)
    //         {
    //             lock (_lock)
    //             {
    //                 _instance = FindObjectOfType<InputManager>();
    //                 
    //                 if (_instance == null)//如果没有，那么我们自己创建一个Gameobject然后给他加一个T这个类型的脚本，并赋值给instance;
    //                 {
    //                     GameObject go = new GameObject("InputManager");
    //                     _instance = go.AddComponent<InputManager>();
    //                 }
    //             }
    //         }
    //
    //         return _instance;
    //     }
    // }
    //     
    //
    //
    // protected  virtual void Awake()
    // {
    //     if (_instance == null)
    //     {
    //         _instance = this;
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    //
    //     if (_inputSystem == null)
    //     {
    //         _inputSystem = new InputSystem_Actions();
    //     }
    //     
    //     
    // }
    // #endregion


    protected override void Init()
    {
        _inputSystem = new InputSystem_Actions();
    }


    private void OnEnable()
    {
        _inputSystem.Enable();
    }

    private void OnDisable()
    {
        _inputSystem.Disable();
    }
    
    
    
}
