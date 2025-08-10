namespace BuffSystem.BuffDesign
{
    /// <summary>
    /// 用于限制给BuffManager控制的变量被外界访问（参阅C#接口的显示实现）
    /// </summary>
    public interface IBuffTicker
    {
        float DurationTimer { get; set; }
        float TickTimer     { get; set; }
        int   CurStack      { get; set; }
    }
}