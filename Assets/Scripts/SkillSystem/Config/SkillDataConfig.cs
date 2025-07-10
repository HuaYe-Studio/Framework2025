using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;

namespace SkillSystem.Config
{
    
    
    
    [CreateAssetMenu(fileName = "SkillConfig",menuName = "SkillConfig" ,order = 0)]
    public class SkillDataConfig : ScriptableObject
    {
        public SkillCharacterConfig CharacterConfig;

        public SkillConfig SkillConfig;

        public List<SkillDamageConfig> DamageConfigs;

        public List<EffectConfig> EffectConfigs;


#if UNITY_EDITOR
        
        
        public static void SaveData(SkillCharacterConfig characterConfig, SkillConfig skillConfig,
            List<SkillDamageConfig> damageConfigs, List<EffectConfig> effectConfigs)
        {
            // SkillDataConfig data = ScriptableObject.CreateInstance<SkillDataConfig>();
            // data.CharacterConfig = characterConfig;
            // data.SkillConfig = skillConfig;
            // data.DamageConfigs = damageConfigs;
            // data.EffectConfigs = effectConfigs;
            //
            // string Path = "Assets/Scripts/SkillSystem/SkillData/" + skillConfig.SkillId + ".asset";
            // AssetDatabase.DeleteAsset(Path);
            // AssetDatabase.CreateAsset(data, Path);
            // AssetDatabase.SaveAssets();
            
            SkillDataConfig data = ScriptableObject.CreateInstance<SkillDataConfig>();
    
            // 创建所有配置的独立副本
            data.CharacterConfig = CreateCopy(characterConfig);
            data.SkillConfig = CreateCopy(skillConfig);
            data.DamageConfigs = damageConfigs.Select(CreateCopy).ToList();
            data.EffectConfigs = effectConfigs.Select(CreateCopy).ToList();
            
            string Path = "Assets/Scripts/SkillSystem/SkillData/" + skillConfig.SkillId + ".asset";
            AssetDatabase.DeleteAsset(Path);
            AssetDatabase.CreateAsset(data, Path);
            
            
        }

        [Button("配置技能" , ButtonSizes.Large) , GUIColor("green")]
        public void ShowSkillWindow()
        {
            SkillComplierWindow window = SkillComplierWindow.PopupWindow();
            window.LoadSkillData(this);
            
        }
        
        
        
#endif
        
        
        public static T CreateCopy<T>(T original)
        {
            string json = JsonUtility.ToJson(original);
            T copy = (T)Activator.CreateInstance(typeof(T));
            JsonUtility.FromJsonOverwrite(json, copy);
            return copy;
            
        }
        
    }

   
}