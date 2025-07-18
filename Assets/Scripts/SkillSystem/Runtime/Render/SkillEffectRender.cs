using RenderLayer;
using UnityEngine;

namespace SkillSystem.Runtime.Render
{
    public class SkillEffectRender : RenderObject
    {
        public override void Update()
        {
            base.Update();
            
        }

        public override void OnRelease()
        {
            base.OnRelease();
            GameObject.Destroy(gameObject);
        }
    }
}