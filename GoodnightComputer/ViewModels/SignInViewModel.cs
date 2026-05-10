using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaToaster;
using AvaloniaToaster.Themes;
using GoodnightComputer.Models;
using GoodnightComputer.State;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace GoodnightComputer.ViewModels;

public partial class SignInViewModel : UserControl
{
    private readonly ToastNotificationService _toastService;
    private readonly IPublicClientApplication _msalApp;

    public SignInViewModel(ToastNotificationService toastService)
    {
        _toastService = toastService;
        InitializeComponent();
        _msalApp = PublicClientApplicationBuilder
            .Create(Constants.Constants.ENTRA_CLIENT_ID)
            .WithTenantId(Constants.Constants.ENTRA_TENANT_ID)
            .WithRedirectUri(Constants.Constants.ENTRA_REDIRECT_URI)
            .Build();
    }

    public async void SignIn_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var result = await AcquireTokenAsync();
            if (result == null)
            {
                _toastService.Show("Sign-in was canceled.", 3000, new AvaloniaToasterErrorTheme());
                return;
            }

            var verified = await VerifyTokenAsync(result.AccessToken);
            if (!verified.Success)
            {
                _toastService.Show(verified.Message ?? "Sign-in failed.", 3000, new AvaloniaToasterErrorTheme());
                return;
            }

            SystemState.SetSignedIn(true);
            _toastService.Show("Signed in successfully.", 3000);
        }
        catch (MsalException ex)
        {
            _toastService.Show($"Sign-in error: {ex.Message}", 4000, new AvaloniaToasterErrorTheme());
        }
        catch (Exception ex)
        {
            _toastService.Show($"Unexpected error: {ex.Message}", 4000, new AvaloniaToasterErrorTheme());
        }
    }

    private async Task<AuthenticationResult?> AcquireTokenAsync()
    {
        var scopes = new[] { $"{Constants.Constants.ENTRA_CLIENT_ID}/.default" };
        try
        {
            var account = (await _msalApp.GetAccountsAsync()).FirstOrDefault();
            return await _msalApp.AcquireTokenSilent(scopes, account).ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            return await _msalApp.AcquireTokenInteractive(scopes).ExecuteAsync();
        }
    }

    private static async Task<SignInResult> VerifyTokenAsync(string accessToken)
    {
        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, Constants.Constants.SIGNIN_ENDPOINT);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            return new SignInResult { Success = false, Message = $"Sign-in rejected ({(int)response.StatusCode}): {body}" };
        }

        try
        {
            var result = await response.Content.ReadFromJsonAsync<SignInResult>();
            return result ?? new SignInResult { Success = true };
        }
        catch (JsonException)
        {
            // API returned 2xx but not JSON matching SignInResult — treat as success
            return new SignInResult { Success = true };
        }
    }
}
