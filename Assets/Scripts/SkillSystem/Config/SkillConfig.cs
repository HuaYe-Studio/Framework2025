using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SkillSystem.Config
{
    [HideMonoScript]
    [Serializable]
    public class SkillConfig
    {
        
        [LabelText("技能图标")]
        [LabelWidth(0.1f)]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SuffixLabel("技能图标")]
        public Sprite SkillIcon;

        [LabelText("技能ID")]
        public int SkillId;
        
        [LabelText("技能名称")]
        public string SkillName;
        
        [LabelText("技能所需蓝量")]
        public int SkillManaCost = 100;
        
        [LabelText("技能前摇时间ms")]
        public int SkillShakeBeforeTimeMS;
        
        [LabelText("技能攻击持续时间ms")]
        public int SkillDurationMS;
        
        [LabelText("技能后摇时间ms")]
        public int SkillShakeAfterTimeMS;
        
        [LabelText("技能冷却时间ms")]
        public int SkillCooldownMS;
        
        [LabelText("技能类型")]
        [OnValueChanged("OnSkillTypeChange")]
        public SkillType SkillType;
        

        [LabelText("蓄力阶段配置(若第一阶段触发时间不为0,则空档时间为动画表现时间)")]
        [ShowIf("_showStockPileStageData")]
        public List<StockPileStageData> StockPileStageData;

        [LabelText("技能引导特效")]
        [ShowIf("_showSkillGuide")]
        public GameObject SkillGuideEffect;
        
        [LabelText("技能引导范围")]
        [ShowIf("_showSkillGuide")]
        public float SkillGuideRange;
        
        [LabelText("衔接技能ID")]
        public int NextSkillId;

        
        
        [LabelText("技能命中特效"),TitleGroup("技能渲染" , "所有渲染数据会在释放技能时触发")]
        public GameObject SkillHitEffect;
        
        [LabelText("特效激活时间ms"),TitleGroup("技能渲染" , "所有渲染数据会在释放技能时触发")]
        public int SkillHitEffectDurationMS;
        
        [LabelText("技能命中音效"),TitleGroup("技能渲染" , "所有渲染数据会在释放技能时触发")]
        public AudioClip SkillHitSound;
        
        [LabelText("是否开启技能立绘"),TitleGroup("技能渲染" , "所有渲染数据会在释放技能时触发")]
        public bool ShowSkillPortrait;
        
        [LabelText("技能立绘对象"),TitleGroup("技能渲染" , "所有渲染数据会在释放技能时触发")]
        [ShowIf("ShowSkillPortrait")]
        public GameObject SkillPortrait;
        
        [LabelText("技能描述"),TitleGroup("技能渲染" , "所有渲染数据会在释放技能时触发")]
        public string SkillDescription;

        private bool _showStockPileStageData;
        private bool _showSkillGuide;
        
        public void OnSkillTypeChange(SkillType skillType)
        {
           _showStockPileStageData = skillType == SkillType.StockPile;
           _showSkillGuide = skillType == SkillType.PosGuide;


        }
        
        
        
        
        
        
    }

    public enum SkillType
    {
        [LabelText("瞬发技能")]
        None,
        [LabelText("吟唱型技能")]
        Chant,
        [LabelText("弹道型技能")]
        Ballistic,
        [LabelText("蓄力技能")]
        StockPile,
        [LabelText("位置引导技能")]
        PosGuide,
    }

    public class PosGuide
    {
        
        
        
    }

    [Serializable]
    public class StockPileStageData
    {
        [LabelText("蓄力阶段ID")]
        public int stage;

        [LabelText("当前蓄力阶段触发的技能ID")]
        public int skillid;

        [LabelText("当前阶段触发开始时间")]
        public int startTimeMS;

        [LabelText("当前阶段结束时间")]
        public int endTimeMS;


    }
}