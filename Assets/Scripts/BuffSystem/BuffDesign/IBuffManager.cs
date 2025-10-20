using UnityEngine;

namespace BuffSystem.BuffDesign
{
    public interface IBuffManager
    {
        /// <summary>
        /// 清除所有Buff
        /// </summary>
        public void ClearBuff();

        /// <summary>
        /// 添加Buff
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public bool AddBuff(string buffId, GameObject creator);

        /// <summary>
        /// 删除指定Buff
        /// </summary>
        /// <param name="buffId"></param>
        /// <returns></returns>
        public bool RemoveBuff(string buffId);
        
    }
}