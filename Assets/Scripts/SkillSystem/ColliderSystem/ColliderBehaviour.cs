using UnityEngine;

namespace SkillSystem.ColliderSystem
{
    public enum ColliderType
    {
        Cube,
        Sphere,
        Cylinder,
    }
    
    public abstract class ColliderBehaviour
    {
        public bool Active = true;

        public ColliderType ColliderType { get; protected set; }
        
        public Vector3 LocalPosition { get;set; }
        
        public Vector3 Center { get;set; }
        
        public Vector3 Size { get;set; }

        public virtual void UpdateColliderInfo(Vector3 pos, Vector3 size = default, float radius = default)
        {
            
        }
        
        public virtual void SetBoxData(Vector3 center , Vector3 size , bool isFollowTarget = false) {}
        
        public virtual void SetBoxData(Vector3 center , float radius, bool isFollowTarget = false) {}
        
        public virtual void OnRelease() {}
        
        
    }
}