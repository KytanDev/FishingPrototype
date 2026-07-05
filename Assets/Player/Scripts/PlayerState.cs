public static class PlayerState
{
    public static bool RodControlMode { get; private set; }

    public static void ToggleRodControlMode()
    {
        RodControlMode = !RodControlMode;
    }
}