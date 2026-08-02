#if STEAMBUILD
using GoodnightComputer.Constants;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace GoodnightComputer.Utils;

/// <summary>
/// Handles Steam DRM enforcement for RELEASE-STEAM and RELEASE-BETA-STEAM builds.
/// Must be called as early as possible in Main(), before any other initialization.
/// </summary>
public static class SteamDRM
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = false)]
    private static extern int MessageBoxW(nint hWnd, string text, string caption, uint type);

    private const uint MB_OK = 0x00000000;
    private const uint MB_ICONERROR = 0x00000010;

    /// <summary>
    /// Enforces Steam DRM and validates the user has a legitimate Steam account.
    ///
    /// Steps:
    ///  1. RestartAppIfNecessary — if the app was not launched through Steam, Steam will
    ///     relaunch it correctly and this process should exit immediately. This is the
    ///     primary Steamworks DRM mechanism.
    ///  2. SteamAPI.Init — verifies the Steam client is running and a user is logged in.
    ///     Returns false (and we exit) if Steam is not present or no account is active.
    ///  3. Validates the returned SteamID is valid as a final account check.
    /// </summary>
    /// <returns>
    /// True if Steam is valid and the app should continue launching.
    /// False if the process should exit.
    /// </returns>
    public static bool EnforceAndInit()
    {
        try
        {
            // Step 1: Steam DRM — if not launched via Steam, Steam will relaunch the app
            // and this instance should terminate immediately.
            if (SteamAPI.RestartAppIfNecessary(new AppId_t(Constants.Constants.STEAM_APP_ID)))
                return false;

            // Step 2: Initialize the Steam API — fails if Steam client isn't running
            // or no account is logged in.
            if (!SteamAPI.Init())
            {
                ShowFatalError(
                    "Steam is required to run GoodnightComputer.\n\n" +
                    "Please ensure Steam is running and you are logged in, then try again.");
                return false;
            }

            // Step 3: Verify a valid SteamID is present (logged-in account check).
            CSteamID steamId = SteamUser.GetSteamID();
            if (!steamId.IsValid())
            {
                SteamAPI.Shutdown();
                ShowFatalError(
                    "Could not verify your Steam account.\n\n" +
                    "Please ensure you are logged into Steam and try again.");
                return false;
            }

            return true;
        }
        catch (DllNotFoundException)
        {
            ShowFatalError(
                "A required Steam library (steam_api64.dll) could not be found.\n\n" +
                "Please verify your installation and ensure Steam is installed.");
            return false;
        }
    }

    /// <summary>
    /// Shuts down the Steamworks API. Call on application exit.
    /// </summary>
    public static void Shutdown()
    {
        try { SteamAPI.Shutdown(); }
        catch { /* best-effort */ }
    }

    private static void ShowFatalError(string message) =>
        MessageBoxW(nint.Zero, message, "GoodnightComputer — Steam Required", MB_OK | MB_ICONERROR);
}
#endif