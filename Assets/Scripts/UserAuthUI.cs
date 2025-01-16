// Unity: UserAuthUI.cs:
using UnityEngine;
using TMPro;
using System;

public class UserAuthUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private GameObject loginPanel;

    private void Start()
    {
        Debug.Log($"[UserAuthUI] PlayerSession.IsLoggedIn: {PlayerSession.IsLoggedIn}");
        Debug.Log($"[UserAuthUI] PlayerSession.HasSkippedLogin: {PlayerSession.HasSkippedLogin}");

        if (PlayerSession.IsLoggedIn || PlayerSession.HasSkippedLogin)
        {
            loginPanel.SetActive(false);
        }
        else
        {
            loginPanel.SetActive(true);
        }
    }


    public async void OnRegisterButtonClicked()
    {
        if (!ValidateInput(out string inputUsername, out string inputPassword))
            return;

        try
        {
            var response = await APIClient.Register(inputUsername, inputPassword);
            Debug.Log($"Registration successful - Id: {response.id}, Username: {response.username}");
            PlayerSession.SetLoggedIn(response.id, response.username);
            loginPanel.SetActive(false);
        }
        catch (Exception ex)
        {
            ShowFeedback($"Registration failed: {ex.Message}");
        }
    }

    public async void OnLoginButtonClicked()
    {
        if (!ValidateInput(out string inputUsername, out string inputPassword))
            return;

        try
        {
            var response = await APIClient.Login(inputUsername, inputPassword);
            Debug.Log($"Login successful - Id: {response.id}, Username: {response.username}");
            PlayerSession.SetLoggedIn(response.id, response.username);
            loginPanel.SetActive(false);
        }
        catch (Exception ex)
        {
            ShowFeedback($"Login failed: {ex.Message}");
        }
    }

    private bool ValidateInput(out string username, out string password)
    {
        username = usernameField.text.Trim();
        password = passwordField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowFeedback("Username and password cannot be empty");
            return false;
        }
        return true;
    }

    public void OnSkipButtonClicked()
    {
        PlayerSession.SetSkipMode();
        //PlayerSession.SetLoggedOut();
        loginPanel.SetActive(false);
        Debug.Log("Skipped login, using device ID");
    }

    private void ShowFeedback(string message)
    {
        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);
        Debug.Log($"Auth feedback: {message}");
    }
}