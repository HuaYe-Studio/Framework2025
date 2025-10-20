using System;

namespace BuffSystem.BuffDesign
{
   /// <summary>
   /// 用于支持自定义传入委托
   /// </summary>
    public interface IBuffAction
    {
        void RegisterBuffAction(string callbackName, Action action);

        void TriggerBuffAction(string callbackName);
    }
}