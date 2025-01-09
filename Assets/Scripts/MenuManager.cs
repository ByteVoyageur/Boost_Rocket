using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menu References")]
    public GameObject menuPanel;         
    public Button exitGameButton;
    public Button playAgainButton;
    public Button leaderBoardButton;
    public Button closeButton;

    [Header("Leader Board")]
    public LeaderBoardManager leaderBoardManager; 
                                                  

    private bool isGamePaused = false;



    void Start()
    {
        // Hide the menu panel at start
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        // Bind buttons
        if (exitGameButton != null)
            exitGameButton.onClick.AddListener(OnExitGameClicked);

        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);

        if (leaderBoardButton != null)
            leaderBoardButton.onClick.AddListener(OnLeaderBoardClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseMenuClicked);
    }

    /// <summary>
    /// Called by the on-screen "Menu" button to open the menu panel.
    /// </summary>
    public void OpenMenu()
    {
        isGamePaused = true;
        Time.timeScale = 0f; // Pause the game
        menuPanel.SetActive(true);
    }

    /// <summary>
    /// Called when user clicks "Close" button to resume the game.
    /// </summary>
    private void OnCloseMenuClicked()
    {
        isGamePaused = false;
        Time.timeScale = 1f; // Unpause the game
        menuPanel.SetActive(false);
    }

    /// <summary>
    /// Called when user clicks "Play Again" button to reload the first scene.
    /// </summary>
    private void OnPlayAgainClicked()
    {
        // Unpause before changing scene
        Time.timeScale = 1f;
        // Load your first scene (e.g. "Scene_0")
        SceneManager.LoadScene("Scene_0");
    }

    /// <summary>
    /// Called when user clicks "Leader Board" button to show the ranking panel.
    /// </summary>
    private void OnLeaderBoardClicked()
    {
        // If you still have a separate LeaderBoardManager, just call ToggleLeaderBoard()
        if (leaderBoardManager != null)
        {
            // Option 1: Open the LeaderBoard
            leaderBoardManager.ToggleLeaderBoard();

            // Option 2: Or directly call a ShowLeaderBoard() method
            // leaderBoardManager.ShowLeaderBoard();
        }
    }

    /// <summary>
    /// Called when user clicks "Exit Game" button.
    /// </summary>
    private void OnExitGameClicked()
    {
        // In Desktop platforms, it will quit
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
