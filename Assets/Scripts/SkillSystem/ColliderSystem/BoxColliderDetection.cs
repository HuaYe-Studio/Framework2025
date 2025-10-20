using UnityEngine;

namespace SkillSystem.ColliderSystem
{
    
    public class BoxColliderDetection : ColliderBehaviour
    {
        private bool _isFollowTarget;
        
        public BoxColliderGizmo BoxGizmo;

        public BoxColliderDetection(Vector3 size, Vector3 center)
        {
            Size = size;
            Center = center;
            ColliderType = ColliderType.Cube;
        }

        public override void UpdateColliderInfo(Vector3 pos, Vector3 size = default, float radius = default)
        {
            
            Size = size;
            if (BoxGizmo != null)
            {
                BoxGizmo.transform.position = pos;
            }
        }

        public override void SetBoxData(Vector3 center, Vector3 size, bool isFollowTarget = false)
        {
            if (BoxGizmo == null)
            {
                GameObject obj = new GameObject();
                BoxGizmo = obj.AddComponent<BoxColliderGizmo>();
                BoxGizmo.SetBoxData(center, size , isFollowTarget);
            }
            
            _isFollowTarget = isFollowTarget;
            Center = center;
            Size = size;
            BoxGizmo?.SetBoxData(center, size, isFollowTarget);
            
        }

        public override void OnRelease()
        {
            if (BoxGizmo != null && BoxGizmo.gameObject != null)
            {
                GameObject.DestroyImmediate(BoxGizmo.gameObject);
            }
            BoxGizmo = null;
        }
    }
}
