using System;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using SkillSystem.Config;
using UnityEditor;
using UnityEngine;

namespace SkillSystem.EditorWindow
{
    public class SkillComplierWindow : OdinEditorWindow
    {
        [MenuItem("Skill/技能编译器")]
        public static SkillComplierWindow OpenWindow()
        {
            return GetWindowWithRect<SkillComplierWindow>(new Rect(0,0,1000,600));
        }
        
        [TabGroup("Skill" , "模型动画数据" , SdfIconType.PersonFill , TextColor = "orange")]
        public SkillCharacterConfig CharacterConfig = new SkillCharacterConfig();

        protected override void OnEnable()
        {
            EditorApplication.update += OnUpdate;
        }

        protected override void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
        }

        public void OnUpdate()
        {
            try
            {
                CharacterConfig.OnUpdate(() => Focus());
            }
            catch (Exception e)
            {
                
            }
        }
    }
}
