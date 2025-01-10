using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LeaderBoardManager : MonoBehaviour
{
    public GameObject leaderBoardPanel;  

    public TextMeshProUGUI[] rankTexts;  

    public ScoreFetcher scoreFetcher;

    private string currentPlayerID = "";

    private bool isPanelVisible = false;

    void Start()
    {
        if (leaderBoardPanel != null)
        {
            leaderBoardPanel.SetActive(false);
        }

        if (scoreFetcher == null)
        {
            scoreFetcher = FindObjectOfType<ScoreFetcher>();
            if (scoreFetcher == null)
            {
                Debug.LogWarning("ScoreFetcher not found in the scene. LeaderBoardManager cannot retrieve scores.");
            }
        }

        if (ScoreManager.Instance != null)
        {
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
