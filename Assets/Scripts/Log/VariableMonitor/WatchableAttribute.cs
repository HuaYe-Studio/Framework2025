using UnityEngine;



public class WatchableAttribute : PropertyAttribute
{
    public string DisplayName { get; }
    
    // 基础构造函数
    public WatchableAttribute(string displayName = "")
    {
        DisplayName = string.IsNullOrEmpty(displayName) 
            ? "Unnamed" 
            : displayName;
    }
}
