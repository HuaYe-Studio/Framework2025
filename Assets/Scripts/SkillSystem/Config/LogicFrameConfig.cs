namespace SkillSystem.Config
{
    /// <summary>
    /// 逻辑帧配置文件
    /// </summary>
    public static class LogicFrameConfig
    {
        /// <summary>
        /// 当前逻辑帧id（进行到哪个逻辑帧了）
        /// </summary>
        public static long LogicFrameid;
        
        /// <summary>
        /// 逻辑帧间隔（单位：秒）
        /// </summary>
        public static float LogicFrameInterval = 0.016f;
        
        /// <summary>
        /// 逻辑帧间隔（单位：毫秒）
        /// </summary>
        public static int LogicFrameIntervalms = 16;

    }
}