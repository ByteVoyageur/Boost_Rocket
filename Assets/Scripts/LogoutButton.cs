// Unity LogoutButton.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutButton : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject menuPanel;

    public void OnLogoutButtonClicked()
    {
        PlayerSession.ClearSkip();
        PlayerSession.SetLoggedOut();
        loginPanel.SetActive(true);
        menuPanel.SetActive(false);
        SceneManager.LoadScene("Scene_0");

        if (ScoreManager.Instance != null )
        {
            ScoreManager.Instance.ResetScore();
        }
    }
}
