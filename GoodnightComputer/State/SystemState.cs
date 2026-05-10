using System;
using System.Collections.Generic;

namespace GoodnightComputer.State;
public static class SystemState
{
    public static List<string> STATIC_APPLICATION_NAMES = null!;

    public static bool IsSignedIn { get; private set; }

    public static event Action<bool>? SignedInChanged;

    public static void SetSignedIn(bool value)
    {
        if (IsSignedIn == value)
            return;

        IsSignedIn = value;
        SignedInChanged?.Invoke(value);
    }
}
