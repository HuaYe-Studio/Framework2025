using System;
using UnityEngine;

namespace RenderLayer
{
    public class RenderObject : MonoBehaviour
    {
        
        public LogicObject LogicObject;
        
        protected float _smoothPosSpeed = 10;

        protected Vector2 _renderDir;

        private void UpdatePosition()
        {
            transform.position = Vector3.Lerp(transform.position , LogicObject.LogicPos.ToVector3() , Time.deltaTime * _smoothPosSpeed);
        }

        private void UpdateDir()
        {
            // transform.rotation = Quaternion.Euler(LogicObject.LogicDir.ToVector3());
            _renderDir.x = LogicObject.LogicAxis >= 0 ? 0 : -20;
            _renderDir.y = LogicObject.LogicAxis >= 0 ? 0 : 180;
            transform.localEulerAngles = _renderDir;
        }

        public void SetLogicObject(LogicObject logicObject)
        {
            LogicObject = logicObject;
            transform.position = logicObject.LogicPos.ToVector3();
        }


        public virtual void OnCreate()
        {
            
            
            
        }

        public virtual void OnRelease()
        {
            
            
        }

        public virtual void PlayAnim(AnimationClip clip)
        {
            
        }

        public virtual void PlayAnim(string animName)
        {
            
        }
        
        
        /// <summary>
        /// Unity引擎渲染帧，根据程序配置，一般在 30 , 60 , 120 帧
        /// </summary>
        public virtual void Update()
        {
            UpdatePosition();
            UpdateDir();
        }
    }
}
