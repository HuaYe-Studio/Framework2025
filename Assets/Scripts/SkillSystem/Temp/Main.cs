using System;
using SkillSystem.Config;
using UnityEngine;

public class Main : MonoBehaviour
{

    public PlayerLogic PlayerLogic { get; private set; }
    
    private float _accLogicRunTime;
    private float _nextLogicFrameTime;
    public float LogicDeltaTime;
    
    public GameObject _player;



    private void Awake()
    {
        PlayerRender render = _player.GetComponent<PlayerRender>();
        PlayerCombatControl control = _player.GetComponent<PlayerCombatControl>();
        PlayerLogic playerLogic = new PlayerLogic(1, render);
        PlayerLogic = playerLogic;
        
        render.SetLogicObject(playerLogic);
        control.SetLoigc(playerLogic);
        playerLogic.OnCreate();
        render.OnCreate();
        
    }

    private void Update()
    {
        
        _accLogicRunTime += Time.deltaTime;

        while (_accLogicRunTime > _nextLogicFrameTime)
        {
            OnLogicFrameUpdate();

            _nextLogicFrameTime += LogicFrameConfig.LogicFrameInterval;

            LogicFrameConfig.LogicFrameid++;
            
        }

        LogicDeltaTime = (_accLogicRunTime + LogicFrameConfig.LogicFrameInterval - _nextLogicFrameTime) /
                         LogicFrameConfig.LogicFrameInterval;


        
        
        


    }

    public void OnLogicFrameUpdate()
    {
        PlayerLogic.OnLogicFrameUpdate();
        
        
    }
    
    
    
    
    
    
}
