using System;
using FixMath;
using LogicLayer;
using RenderLayer;
using UnityEngine;

public class PlayerRender : RenderObject
{
    private PlayerLogic _playerLogic;
    
    private Vector2 _inputDirection;
    
    private Animator _animator;

    public override void OnCreate()
    {
        base.OnCreate();
        _playerLogic = LogicObject as PlayerLogic;
    }


    public override void Update()
    {
        base.Update();
        UpdateMove();

        if (_playerLogic != null &&  _playerLogic.ReleaseSkills.Count == 0)
        {
            //判断输出是否有值
            //需要控制状态机 ，我们主要做技能系统 因此先忽略这一点。
            
        }
        
        
        
        
    }

    private void UpdateMove()
    {
        _inputDirection = InputManager.MainInstance.Move;
        FixIntVector3 logicDirection = FixIntVector3.zero;
        if (_inputDirection != Vector2.zero)
        {
            logicDirection.x = _inputDirection.x;
            logicDirection.y = 0;
            logicDirection.z = _inputDirection.y;
            
        }

        if (_playerLogic != null)
        {
            _playerLogic.InputLogicFrameEvent(logicDirection);
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public override void PlayAnim(string animName)
    {
        base.PlayAnim(animName);
        _animator.CrossFadeInFixedTime(animName, 0.15555f , 0, 0.0f);
    }
}
