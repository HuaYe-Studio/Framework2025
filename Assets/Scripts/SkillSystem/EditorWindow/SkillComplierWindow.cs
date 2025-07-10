using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using SkillSystem.Config;
using UnityEditor;
using UnityEngine;

public class SkillComplierWindow : OdinEditorWindow
{
    [TabGroup("skill" , "模型动画数据" , SdfIconType.PersonFill , TextColor = "orange")]
    public SkillCharacterConfig CharacterConfig = new SkillCharacterConfig();
    
    [TabGroup("SkillComplier" , " Skill" , SdfIconType.Robot , TextColor = "lightmagenta")]
    public SkillConfig SkillConfig = new SkillConfig();
    
    [TabGroup("SkillComplier", " Damage", SdfIconType.At, TextColor = "lightred")]
    public List<SkillDamageConfig> SkillDamageConfig = new List<SkillDamageConfig>();
    
    [TabGroup("SkillComplier", " Effect", SdfIconType.OpticalAudio, TextColor = "blue")]
    public List<EffectConfig> EffectConfig = new List<EffectConfig>();
    
    
    
    
#if UNITY_EDITOR
    
    
    [MenuItem("Skill/技能编译器")]
    public static SkillComplierWindow PopupWindow()
    {
        return GetWindowWithRect<SkillComplierWindow>(new Rect(0, 0, 1000, 600));
    }

    public static SkillComplierWindow GetWindow()
    {
        return GetWindow<SkillComplierWindow>();
    }

    public void LoadSkillData(SkillDataConfig data)
    {
        
        this.CharacterConfig = SkillDataConfig.CreateCopy(data.CharacterConfig);
        this.SkillConfig = SkillDataConfig.CreateCopy(data.SkillConfig);
        this.SkillDamageConfig = data.DamageConfigs.Select(SkillDataConfig.CreateCopy).ToList();
        this.EffectConfig = data.EffectConfigs.Select(SkillDataConfig.CreateCopy).ToList();
    }

    public void SaveConfig()
    {
        SkillDataConfig.SaveData(CharacterConfig, SkillConfig, SkillDamageConfig, EffectConfig);
        
    }

    protected override void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
        SkillDamageConfig.ForEach(x => x.OnInit());
    }

    protected override void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
        SkillDamageConfig.ForEach(x => x.OnRelease());
    }

    public void OnEditorUpdate()
    {
        try
        {
            //刷新窗口 
            CharacterConfig.OnUpdate((() =>  Focus() ));
            if(_isStartPlaySkill) OnLogicUpdate();
                
            
        }
        catch (Exception e)
        {
            
        }
    }

    private bool _isStartPlaySkill = false;
    private float _accLogicFrameTime;
    private float _nextLogicFrameTime;
    private float _deltaTime;
    private double _lastUpdateTime;
    
    public void OnLogicUpdate()
    {
        //模拟帧同步更新
        if (_lastUpdateTime == 0)
        {
            _lastUpdateTime = EditorApplication.timeSinceStartup;
        }
        
        _accLogicFrameTime = (float)(EditorApplication.timeSinceStartup - _lastUpdateTime);

        while (_accLogicFrameTime > _nextLogicFrameTime)
        {
            OnLogicFrameHandle();

            _nextLogicFrameTime += LogicFrameConfig.LogicFrameInterval;
        }
    }

    public void OnLogicFrameHandle()
    {
        EffectConfig.ForEach(x => x.OnLogicFrameUpdate());
        SkillDamageConfig.ForEach(x => x.UpdateLogic());
    }

    public void StartPlaySkill()
    {
        EffectConfig.ForEach(x => x.StartPlaySkill());
        SkillDamageConfig.ForEach(x => x.StartPlaySkill());
        _isStartPlaySkill = true;
        _accLogicFrameTime = 0;
        _nextLogicFrameTime = 0;
        _lastUpdateTime = 0;
    }

    public void PlaySkillEnd()
    {
        EffectConfig.ForEach(x => x.PlaySkillEnd());
        SkillDamageConfig.ForEach(x => x.PlaySkillEnd());
        _isStartPlaySkill = false;
        _accLogicFrameTime = 0;
        _nextLogicFrameTime = 0;
        _lastUpdateTime = 0;
    }

    public void PlayPause()
    {
        EffectConfig.ForEach(x => x.SkillPlayPause());
        SkillDamageConfig.ForEach(x => x.PlaySkillEnd());
    }

    public static Vector3 GetCharacterPosition()
    {
        SkillComplierWindow window = GetWindow<SkillComplierWindow>();
        
        if(window.CharacterConfig.SkillCharacter != null)
            return window.CharacterConfig.SkillCharacter.transform.position;
        return Vector3.zero;
    }
    
    
    
#endif
}
