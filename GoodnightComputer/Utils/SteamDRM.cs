#if STEAMBUILD
using GoodnightComputer.Constants;
using Steamworks;
using System;
using System.Windows;

namespace GoodnightComputer.Utils;

/// <summary>
/// Handles Steam DRM enforcement for RELEASE-STEAM and RELEASE-BETA-STEAM builds.
/// Must be called as early as possible in Main(), before any other initialization.
/// </summary>
public static class SteamDRM
{
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
    /// False if the process should exit (Steam has been asked to relaunch us, or validation failed).
    /// </returns>
    public static bool EnforceAndInit()
    {
        // Step 1: Steam DRM — if not launched via Steam, Steam will relaunch the app
        // and this instance should terminate immediately (no error dialog needed).
        if (SteamAPI.RestartAppIfNecessary(new AppId_t(Constants.Constants.STEAM_APP_ID)))
        {
            // Steam is relaunching us through the Steam client. Exit silently.
            return false;
        }

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

    /// <summary>
    /// Shuts down the Steamworks API. Call on application exit.
    /// </summary>
    public static void Shutdown()
    {
        try { SteamAPI.Shutdown(); }
        catch { /* best-effort */ }
    }

    private static void ShowFatalError(string message)
    {
        // MessageBox is available because this is a WinExe targeting net9.0-windows.
        System.Windows.MessageBox.Show(
            message,
            "GoodnightComputer — Steam Required",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Error);
    }
}
#endif