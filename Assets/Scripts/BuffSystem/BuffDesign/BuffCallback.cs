namespace BuffSystem.BuffDesign
{
    public abstract class BuffCallback
    {
        public abstract void Apply(BuffRunTimeInfo info, params object[] customParams);
    }
}