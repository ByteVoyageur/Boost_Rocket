using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject menuPanel;
    public Button playAgainButton;
    public Button leaderBoardButton;
    public Button closeButton;
    public GameObject leaderBoardContainer;

    void Start()
    {
        menuPanel?.SetActive(false);

        BindButton(playAgainButton, OnPlayAgainClicked);
        BindButton(closeButton, OnCloseMenuClicked);
        BindButton(leaderBoardButton, OnLeaderBoardClicked);
    }

    private void BindButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
    }

    public void OpenMenu()
    {
        Time.timeScale = 0f;
        menuPanel.SetActive(true);
    }

    private void OnCloseMenuClicked()
    {
        Time.timeScale = 1f;
        menuPanel.SetActive(false);
        leaderBoardContainer?.SetActive(false);
    }

    private void OnPlayAgainClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scene_0");
    }

    private void OnLeaderBoardClicked()
    {
        leaderBoardContainer?.SetActive(true);
    }
}
