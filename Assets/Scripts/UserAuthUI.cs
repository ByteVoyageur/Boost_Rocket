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

        // Check if user is already logged in
        if (PlayerSession.IsLoggedIn)
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
        string inputUsername = usernameField.text;
        string inputPassword = passwordField.text;

        if (string.IsNullOrEmpty(inputUsername) || string.IsNullOrEmpty(inputPassword))
        {
            ShowFeedback("Username and password cannot be empty");
            return;
        }

        try
        {
            var response = await APIClient.Register(inputUsername, inputPassword);
            PlayerSession.SetLoggedIn(response.Id, response.Username);
            loginPanel.SetActive(false);
        }
        catch (Exception ex)
        {
            ShowFeedback(ex.Message);
        }
    }

    public async void OnLoginButtonClicked()
    {
        string inputUsername = usernameField.text;
        string inputPassword = passwordField.text;

        if (string.IsNullOrEmpty(inputUsername) || string.IsNullOrEmpty(inputPassword))
        {
            ShowFeedback("Username and password cannot be empty");
            return;
        }

        try
        {
            var response = await APIClient.Login(inputUsername, inputPassword);
            PlayerSession.SetLoggedIn(response.Id, response.Username);
            loginPanel.SetActive(false);
        }
        catch (Exception ex)
        {
            ShowFeedback(ex.Message);
        }
    }

    public void OnSkipButtonClicked()
    {
        PlayerSession.SetLoggedOut();
        loginPanel.SetActive(false);
    }

    private void ShowFeedback(string message)
    {
        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);
    }
}