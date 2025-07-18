using System;
using UnityEngine;

public class PlayerCombatControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private PlayerLogic _playerLogic;

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.MainInstance.LAttack)
        {
            _playerLogic.OnReleaseSkill(1001);
            
            
            
            
        }
    }


    public void SetLoigc(PlayerLogic playerLogic)
    {
        _playerLogic = playerLogic;
    }
}
