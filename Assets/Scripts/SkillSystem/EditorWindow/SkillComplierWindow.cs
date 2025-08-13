using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using SkillSystem.Config;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 用于创建技能编辑器窗口的类
/// </summary>
public class SkillComplierWindow : OdinEditorWindow
{
    [TabGroup("skill" , "模型动画数据" , SdfIconType.PersonFill , TextColor = "orange")]
    public SkillCharacterConfig CharacterConfig = new SkillCharacterConfig();
    
    [TabGroup("SkillComplier" , " Skill" , SdfIconType.Robot , TextColor = "lightmagenta")]
    public SkillConfig SkillConfig = new SkillConfig();
    
    [TabGroup("SkillComplier", " Damage", SdfIconType.At, TextColor = "lightred")]
    public List<SkillDamageConfig> SkillDamageConfig = new List<SkillDamageConfig>();
    
    [TabGroup("SkillComplier", " Effect", SdfIconType.OpticalAudio, TextColor = "blue")]
    public List<SkillEffectConfig> EffectConfig = new List<SkillEffectConfig>();
    
    [TabGroup("SkillComplier", " Audio", SdfIconType.Soundwave, TextColor = "lightgreen")]
    public List<SkillAudioConfig> SkillAudioConfig = new List<SkillAudioConfig>();
    
    
    
#if UNITY_EDITOR
    
    
    /// <summary>
    /// 弹出技能编辑器窗口
    /// </summary>
    /// <returns></returns>
    [MenuItem("Skill/技能编译器")]
    public static SkillComplierWindow PopupWindow()
    {
        return GetWindowWithRect<SkillComplierWindow>(new Rect(0, 0, 1000, 600));
    }

    /// <summary>
    /// 获取技能编辑器窗口，提供给外部调用
    /// </summary>
    /// <returns></returns>
    public static SkillComplierWindow GetWindow()
    {
        return GetWindow<SkillComplierWindow>();
    }

    /// <summary>
    /// 读取数据，用于快速配置/更改已有的技能数据
    /// </summary>
    /// <param name="data"></param>
    public void LoadSkillData(SkillDataConfig data)
    {
        
        this.CharacterConfig = SkillDataConfig.CreateCopy(data.CharacterConfig);
        this.SkillConfig = SkillDataConfig.CreateCopy(data.SkillConfig);
        this.SkillDamageConfig = data.DamageConfigs.Select(SkillDataConfig.CreateCopy).ToList();
        this.EffectConfig = data.EffectConfigs.Select(SkillDataConfig.CreateCopy).ToList();
        this.SkillAudioConfig = data.AudioConfigs.Select(SkillDataConfig.CreateCopy).ToList();
        CharacterConfig.Init();
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    public void SaveConfig()
    {
        SkillDataConfig.SaveData(CharacterConfig, SkillConfig, SkillDamageConfig, EffectConfig , SkillAudioConfig);
        
    }

    protected override void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    protected override void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    public void OnEditorUpdate()
    {
        try
        {
            //刷新窗口 
            CharacterConfig.OnUpdate(Focus);
            //在释放技能中去更新逻辑
            if (_isStartPlaySkill)
            {
                OnLogicUpdate();
                OnUpdate();
            }
            
                
            
        }
        catch (Exception e)
        {
            
        }
    }

    private Dictionary<int , SkillEffectConfig> _effectCache = new Dictionary<int , SkillEffectConfig>();
    private Dictionary<int , SkillDamageConfig> _damageCache = new Dictionary<int , SkillDamageConfig>();
    public void OnProgressValueChange(int value)
    {
        //特效
        EffectConfig.ForEach(x =>
        {
            if (value >= x.TriggerFrame * LogicFrameConfig.LogicFrameIntervalms && value <= x.EndFrame * LogicFrameConfig.LogicFrameIntervalms && !_effectCache.ContainsKey(x.GetHashCode()))
            {
                _effectCache.Add(x.GetHashCode(), x);
                x.CreateEffect(false);
            }
            else if (value < x.TriggerFrame * LogicFrameConfig.LogicFrameIntervalms || value > x.EndFrame * LogicFrameConfig.LogicFrameIntervalms)
            {
                if (_effectCache.ContainsKey(x.GetHashCode()))
                {
                    _effectCache.Remove(x.GetHashCode());
                    x.DestroyEffect();
                }
                
            }
        });
        
        //伤害
        SkillDamageConfig.ForEach(x =>
        {
            if (value >= x.TriggerLogicFrame * LogicFrameConfig.LogicFrameIntervalms &&
                value <= x.EndLogicFrame * LogicFrameConfig.LogicFrameIntervalms &&
                !_damageCache.ContainsKey(x.GetHashCode()))
            {
                _damageCache.Add(x.GetHashCode(), x);
                x.CreateCollider();
            }
            else if(value < x.TriggerLogicFrame * LogicFrameConfig.LogicFrameIntervalms || value > x.EndLogicFrame * LogicFrameConfig.LogicFrameIntervalms)
            {
                if (_damageCache.ContainsKey(x.GetHashCode()))
                {
                    _damageCache.Remove(x.GetHashCode());
                    x.DestroyCollider();
                }
            }
        });
        
        //模拟
        _effectCache.Values.ToList().ForEach(x => x.OnSimulate((value - x.TriggerFrame * LogicFrameConfig.LogicFrameIntervalms) / 1000f));
        
        
    }

    private bool _isStartPlaySkill = false;
    private float _accLogicFrameTime;
    private float _nextLogicFrameTime;
    private float _deltaTime;
    private double _lastUpdateTime;
    
    //新的
    private int _accUpdateTimeMS;
    
    
    /// <summary>
    /// 逻辑帧更新
    /// </summary>
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
            // OnLogicFrameHandle();
    
            _nextLogicFrameTime += LogicFrameConfig.LogicFrameInterval;
        }
    }

    public void OnUpdate()
    {
        if(_lastUpdateTime == 0) _lastUpdateTime = EditorApplication.timeSinceStartup;
        
        _accUpdateTimeMS = (int)((EditorApplication.timeSinceStartup - _lastUpdateTime) * 1000f);
        OnUpdateHandle();
    }

    public void OnUpdateHandle()
    {
        EffectConfig.ForEach(x => x.OnUpdate(_accUpdateTimeMS));
    }
    
    

    // public void OnLogicFrameHandle()
    // {
    //     EffectConfig.ForEach(x => x.OnLogicFrameUpdate());
    // }

    /// <summary>
    /// 开始播放技能
    /// </summary>
    public void StartPlaySkill()
    {
        EffectConfig.ForEach(x => x.StartPlaySkill());
        _isStartPlaySkill = true;
        _accLogicFrameTime = 0;
        _nextLogicFrameTime = 0;
        _lastUpdateTime = 0;
    }

    /// <summary>
    /// 播放结束的回调
    /// </summary>
    public void PlaySkillEnd()
    {
        EffectConfig.ForEach(x => x.PlaySkillEnd());
        _isStartPlaySkill = false;
        _accLogicFrameTime = 0;
        _nextLogicFrameTime = 0;
        _lastUpdateTime = 0;
    }

    /// <summary>
    /// 暂停播放
    /// </summary>
    public void PlayPause()
    {
        EffectConfig.ForEach(x => x.SkillPlayPause());
    }

    /// <summary>
    /// 获取角色的信息
    /// </summary>
    /// <returns>角色位置</returns>
    public static Transform GetCharacterTransform()
    {
        SkillComplierWindow window = GetWindow<SkillComplierWindow>();
        
        if(window.CharacterConfig.SkillCharacter != null)
            return window.CharacterConfig.SkillCharacter.transform;
        return null;
    }

    private void OnLostFocus()
    {
       EffectConfig.ForEach(x => x.DestroyEffect());
       SkillDamageConfig.ForEach(x => x.DestroyCollider());
    }


#endif
}
