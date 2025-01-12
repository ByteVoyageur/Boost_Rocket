using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LeaderBoardManager : MonoBehaviour
{
    public GameObject leaderBoardPanel;
    public GameObject entryPrefab;      
    public Transform contentParent;     
    public ScoreFetcher scoreFetcher;

    private string currentPlayerID = "";

    private bool isPanelVisible = false;

    void Start()
    {
            leaderBoardPanel.SetActive(false);
            currentPlayerID = ScoreManager.Instance.GetCurrentPlayerID();
    }

    /// <summary>
    /// Called when user wants to toggle the leaderboard panel on/off (e.g. via a button).
    /// </summary>
    public void ToggleLeaderBoard()
    {
        if (!isPanelVisible)
        {
            RefreshLeaderBoard();
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
    /// Pull top scores from database and display in the ScrollView.
    /// </summary>
    private async void RefreshLeaderBoard()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Cleared previous leaderboard entries.");

        List<LeaderBoardData> topScores = await scoreFetcher.GetTopScores(10, currentPlayerID);
        Debug.Log($"Retrieved {topScores.Count} top scores from database.");

        foreach (LeaderBoardData data in topScores)
        {
            GameObject entryGO = Instantiate(entryPrefab, contentParent);
            Debug.Log("Instantiated a leaderboard entry prefab.");

            TextMeshProUGUI[] textFields = entryGO.GetComponentsInChildren<TextMeshProUGUI>();

            if (textFields.Length >= 3)
            {
                textFields[0].text = data.username;
                textFields[1].text = data.score.ToString();
                textFields[2].text = data.timestamp.ToLocalTime().ToString("dd-MM-yyyy");
            }
        }

    }
}
