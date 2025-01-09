using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // If you are using TextMeshPro

public class LeaderBoardManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject leaderBoardPanel;         
    public Transform contentParent;             
    public GameObject leaderBoardEntryPrefab;   
    public Button exitGameButton;

    private bool isPanelVisible = false;

    // Reference to our new ScoreFetcher
    public ScoreFetcher scoreFetcher;

    // Current player's ID, used for tie-breaking and highlighting
    private string currentPlayerID = "";

    void Start()
    {
        // Ensure panel is initially hidden
        if (leaderBoardPanel != null)
        {
            leaderBoardPanel.SetActive(false);
        }

        // Find ScoreFetcher in the scene (or you can assign via Inspector)
        scoreFetcher = FindObjectOfType<ScoreFetcher>();
        if (scoreFetcher == null)
        {
            Debug.LogWarning("ScoreFetcher not found in the scene. LeaderBoardManager cannot retrieve scores.");
        }

        // Get current player ID from ScoreManager (if needed)
        if (ScoreManager.Instance != null)
        {
            currentPlayerID = ScoreManager.Instance.GetCurrentPlayerID();
        }

        // Bind the exit button event for quitting the game
        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener(OnExitGameClicked);
        }
    }

    /// <summary>
    /// Show or hide the leaderboard panel based on current state.
    /// </summary>
    public void ToggleLeaderBoard()
    {
        if (isPanelVisible)
        {
            if (leaderBoardPanel != null)
            {
                leaderBoardPanel.SetActive(false);
            }
            isPanelVisible = false;
        }
        else
        {
            // If panel is hidden, fetch data from DB and show it
            RefreshLeaderBoard();

            if (leaderBoardPanel != null)
            {
                leaderBoardPanel.SetActive(true);
            }
            isPanelVisible = true;
        }
    }

    /// <summary>
    /// RefreshLeaderBoard will fetch top scores from DB (via ScoreFetcher) and update the UI.
    /// </summary>
    private async void RefreshLeaderBoard()
    {
        // Clear old entries
        if (contentParent != null)
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
        }

        // If no fetcher is found, just return
        if (scoreFetcher == null)
        {
            Debug.LogWarning("No ScoreFetcher available. Cannot retrieve top scores.");
            return;
        }

        // Get top 10 from DB
        List<LeaderBoardData> topScores = await scoreFetcher.GetTopScores(10, currentPlayerID);

        // Render each entry in UI
        for (int i = 0; i < topScores.Count; i++)
        {
            if (leaderBoardEntryPrefab == null || contentParent == null) { break; }

            GameObject entryObj = Instantiate(leaderBoardEntryPrefab, contentParent);
            LeaderBoardEntry entry = entryObj.GetComponent<LeaderBoardEntry>();

            // Set rank, name, score
            entry.rankText.text = (i + 1).ToString();
            entry.nameText.text = topScores[i].username;
            entry.scoreText.text = topScores[i].score.ToString();

            // Highlight if this is the current player
            if (topScores[i].userId == currentPlayerID)
            {
                entry.nameText.color = Color.yellow;
                entry.scoreText.color = Color.yellow;
            }
        }
    }
}
