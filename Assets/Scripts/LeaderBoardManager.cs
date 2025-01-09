using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LeaderBoardManager : MonoBehaviour
{
    [Header("Leaderboard UI")]
    public GameObject leaderBoardPanel;  // The parent panel or container for the leaderboard

    [Header("Top 10 Ranks (fixed)")]
    public TextMeshProUGUI[] rankTexts;  // Array of size 10, each referencing Rank01 ~ Rank10

    // Reference to ScoreFetcher
    public ScoreFetcher scoreFetcher;

    // For identifying current player
    private string currentPlayerID = "";

    // Internal state
    private bool isPanelVisible = false;

    void Start()
    {
        // Hide leaderboard panel at start (optional)
        if (leaderBoardPanel != null)
        {
            leaderBoardPanel.SetActive(false);
        }

        // If you have a ScoreFetcher in the scene or via Inspector
        if (scoreFetcher == null)
        {
            scoreFetcher = FindObjectOfType<ScoreFetcher>();
            if (scoreFetcher == null)
            {
                Debug.LogWarning("ScoreFetcher not found in the scene. LeaderBoardManager cannot retrieve scores.");
            }
        }

        // Get current player ID from ScoreManager if needed
        if (ScoreManager.Instance != null)
        {
            // Suppose you have a method GetCurrentPlayerID() or PlayerID property
            currentPlayerID = ScoreManager.Instance.GetCurrentPlayerID();
        }
    }

    /// <summary>
    /// Called when user wants to toggle the leaderboard panel on/off (e.g. via a button).
    /// </summary>
    public void ToggleLeaderBoard()
    {
        if (!isPanelVisible)
        {
            RefreshLeaderBoard();
            if (leaderBoardPanel != null)
                leaderBoardPanel.SetActive(true);

            isPanelVisible = true;
        }
        else
        {
            if (leaderBoardPanel != null)
                leaderBoardPanel.SetActive(false);

            isPanelVisible = false;
        }
    }

    /// <summary>
    /// Pull top 10 from database and display in the rankTexts array.
    /// </summary>
    private async void RefreshLeaderBoard()
    {
        if (scoreFetcher == null)
        {
            Debug.LogWarning("No ScoreFetcher available. Cannot retrieve top scores.");
            return;
        }

        // 1) Get top 10 from DB
        List<LeaderBoardData> topScores = await scoreFetcher.GetTopScores(10, currentPlayerID);

        // 2) Loop through rankTexts to fill them
        for (int i = 0; i < rankTexts.Length; i++)
        {
            if (i < topScores.Count)
            {
                // i-th data
                LeaderBoardData data = topScores[i];
                rankTexts[i].text = $"{i + 1}. {data.username}  {data.score}";

                // Highlight if current player
                if (data.userId == currentPlayerID)
                    rankTexts[i].color = Color.yellow;
                else
                    rankTexts[i].color = Color.white;
            }
            else
            {
                // If no more data, show placeholder or blank
                rankTexts[i].text = $"---";
                rankTexts[i].color = Color.white;
            }
        }
    }

    /// <summary>
    /// Called when "Exit Game" button is clicked.
    /// </summary>
}
