using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_CameraControl : MonoBehaviour
{
    
    //相机的移动速度
    [SerializeField, Header("相机参数配置")] private float _controlSpeed;  //摄像机移动速度
    [SerializeField] private Vector2 _cameraVerticalMaxAngle;  //限制相机上下最大旋转角度
    [SerializeField] private float _smoothSpeed;   //摄像机平滑速度
    [SerializeField] private float _positionOffset;   //摄像机与目标物体的距离偏移
    [SerializeField] private float _positionSmoothTime;    //摄像机位置平滑时间
    
   
    
    // [SerializeField , Header("相机锁定时的固定偏移量")]private Vector3 _lockOffset;   //相机锁定时的固定偏移量
    private Vector3 _lockPos;   //相机锁定时的固定位置
    
    [SerializeField]private Transform _lookTarget;
    
    private Vector3 _smoothDampVelocity = Vector3.zero;
    
    private Vector2 _input;    //相机的输入 旋转角度
    
    private Vector3 _cameraRotation;   //当前摄像机的旋转角度
    private Transform _camera;

    private Quaternion _currentRotation;
    
    //锁定的目标 
    [SerializeField]private Transform _locktarget;
    
    [SerializeField]private bool _cameraLocked = false;   //相机是否锁定

    private void Awake()
    {
        _lookTarget = GameObject.FindWithTag("CameraTarget").transform;
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        CameraInput();
    }

    private void LateUpdate()
    {
        UpdateCameraRotation();
        CameraPosition();
        
    }

    private void CameraInput()
    {
        if (_cameraLocked) 
            return;
        
        _input.y += InputManager.Instance.Look.x * _controlSpeed;
        _input.x -= InputManager.Instance.Look.y * _controlSpeed;
        
        
        
        _input.x = Mathf.Clamp(_input.x, _cameraVerticalMaxAngle.x, _cameraVerticalMaxAngle.y);
    }
    
    //更新相机的旋转
    private void UpdateCameraRotation()
    {
        if (_cameraLocked)
        {
            //让摄像机始终朝向锁定的目标
            // Quaternion targetRotation = Quaternion.LookRotation(_locktarget.position - _camera.position);
            // Vector3 targetRotationEuler = targetRotation.eulerAngles;
            // _cameraRotation = Vector3.SmoothDamp(_cameraRotation,
            //     new Vector3(targetRotationEuler.x, targetRotationEuler.y, 0), ref _smoothDampVelocity, _smoothSpeed * 0.2f);
            // transform.eulerAngles = _cameraRotation;
            
            // 计算目标方向
            Vector3 direction = _locktarget.position + Vector3.up - _camera.position;
            if (direction == Vector3.zero) return;

            // 生成目标旋转
            Quaternion targetRotation = Quaternion.LookRotation(direction);
        
            // 使用四元数插值
            _currentRotation = Quaternion.Slerp(
                _currentRotation,
                targetRotation,
                _smoothSpeed * Time.deltaTime * 40f
            );
            transform.rotation = _currentRotation;
           
        }
        else
        {
            Vector3 targetEuler = new Vector3(_input.x, _input.y, 0);
            
            Quaternion targetRotation = Quaternion.Euler(targetEuler);
        
            // 四元数插值
            _currentRotation = Quaternion.Slerp(
                _currentRotation,
                targetRotation,
                _smoothSpeed * Time.deltaTime * 80f
            );
            
            // 强制消除 Z 轴旋转
            Vector3 finalEuler = _currentRotation.eulerAngles;
            finalEuler.z = 0;
            _currentRotation = Quaternion.Euler(finalEuler);
        
            transform.rotation = _currentRotation;
            // transform.rotation = _currentRotation;
             
            // _cameraRotation = Vector3.SmoothDamp(_cameraRotation , new Vector3(_input.x, _input.y, 0), ref _smoothDampVelocity, _smoothSpeed);
            // transform.eulerAngles = _cameraRotation;
        }
        
    }

    //如果锁定相机 则固定位置进行更新
    private void CameraPosition()
    {
        if (_cameraLocked)
        {
            //如果锁定了 则根据固定的位置 更新相机的位置
            _lockPos = _lookTarget.position   -   (_locktarget.position   - _lookTarget.position).normalized * _positionOffset - Vector3.Cross(_locktarget.position - _lookTarget.position, Vector3.up).normalized * 0.5f;

            transform.position = Vector3.Lerp(transform.position, _lockPos, DevelopmentTools.UnTetheredLerp(_positionSmoothTime));
        }
        else
        {
            var newPos = (_lookTarget.position + (-transform.forward * _positionOffset));
            transform.position = Vector3.Lerp(transform.position , newPos , DevelopmentTools.UnTetheredLerp(_positionSmoothTime));
        }
            
        
    }
    
    //交给外界控制自己的锁定状态
    public void SetLock(bool _isLocked, Transform target)
    {

        //锁定
        if (_isLocked)
        {
            _cameraLocked = true;
            _locktarget = target;
            // UIManager.Instance.GetPanel<PlayerPanel>().ShowLock(target , true);
        }
        else
        {
            //解锁
            _cameraLocked = false;
            _locktarget = target;
            _input = new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y);
            // UIManager.Instance.GetPanel<PlayerPanel>().ShowLock(target , false);

        }
        
            
            
       
        
    }
    
    
    
    
    
    
}
    
    

