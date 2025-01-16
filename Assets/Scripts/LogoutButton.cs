using UnityEngine;

public class LogoutButton : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject menuPanel;

    public void OnLogoutButtonClicked()
    {
        PlayerSession.SetLoggedOut();
        loginPanel.SetActive(true);
        menuPanel.SetActive(false);
    }
}
