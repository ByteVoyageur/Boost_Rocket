using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LeaderBoardManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject leaderBoardPanel;
    public GameObject entryPrefab;
    public Transform contentParent;

    [Header("Dependencies")]
    public ScoreFetcher scoreFetcher;

    private string currentPlayerID = "";
    private bool isPanelVisible = false;

    void Start()
    {
        leaderBoardPanel.SetActive(false);

        // Use the same ID logic as ScoreManager and ScoreUploader
        if (PlayerSession.IsLoggedIn)
        {
            currentPlayerID = PlayerSession.CurrentUserId.ToString();
        }
        else
        {
            currentPlayerID = SystemInfo.deviceUniqueIdentifier;
        }
        Debug.Log($"LeaderBoard initialized with currentPlayerID: {currentPlayerID}");
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
            Debug.Log("Leaderboard panel shown");
        }
        else
        {
            if (leaderBoardPanel != null)
            {
                leaderBoardPanel.SetActive(false);
                isPanelVisible = false;
                Debug.Log("Leaderboard panel hidden");
            }
        }
    }

    /// <summary>
    /// Pull top scores from database and display in the ScrollView.
    /// </summary>
    private async void RefreshLeaderBoard()
    {
        // Clear existing entries
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Cleared previous leaderboard entries.");

        // Update current player ID before fetching scores
        if (PlayerSession.IsLoggedIn)
        {
            currentPlayerID = PlayerSession.CurrentUserId.ToString();
        }
        else
        {
            currentPlayerID = SystemInfo.deviceUniqueIdentifier;
        }
        Debug.Log($"Refreshing leaderboard with currentPlayerID: {currentPlayerID}");

        // Fetch and display scores
        List<LeaderBoardData> topScores = await scoreFetcher.GetTopScores(10, currentPlayerID);
        Debug.Log($"Retrieved {topScores.Count} top scores from database.");

        foreach (LeaderBoardData data in topScores)
        {
            GameObject entryGO = Instantiate(entryPrefab, contentParent);
            TextMeshProUGUI[] textFields = entryGO.GetComponentsInChildren<TextMeshProUGUI>();

            if (textFields.Length >= 3) // Username, Score, Date fields
            {
                // Format displayed data
                string displayUsername = TrimUsername(data.username);
                string displayScore = data.score.ToString();
                string displayDate = data.timestamp.ToLocalTime().ToString("dd/MM/yyyy");

                // Assign to text fields
                textFields[0].text = displayUsername;
                textFields[1].text = displayScore;
                textFields[2].text = displayDate;

                Debug.Log($"Added entry - User: {displayUsername}, ID: {data.userId}, Score: {displayScore}");

                // Highlight if this is the current user's entry
                HighlightCurrentUser(entryGO, data);
            }
            else
            {
                Debug.LogWarning($"Entry prefab doesn't have enough text fields. Expected 3, found {textFields.Length}");
            }
        }
    }

    /// <summary>
    /// Trims username if it exceeds maxLength
    /// </summary>
    private string TrimUsername(string username, int maxLength = 12)
    {
        if (string.IsNullOrEmpty(username))
            return "Anonymous";

        if (username.Length > maxLength)
            return username.Substring(0, maxLength - 3) + "...";

        return username;
    }

    /// <summary>
    /// Highlights the entry if it belongs to the current user
    /// </summary>
    private void HighlightCurrentUser(GameObject entryGO, LeaderBoardData data)
    {
        // Add more robust ID comparison
        bool isCurrentUser = false;

        if (PlayerSession.IsLoggedIn)
        {
            // For logged-in users, compare ObjectId strings
            isCurrentUser = data.userId == currentPlayerID;
            Debug.Log($"Comparing logged-in user IDs - Current: {currentPlayerID}, Entry: {data.userId}");
        }
        else
        {
            // For device IDs, compare directly
            isCurrentUser = data.userId == currentPlayerID;
            Debug.Log($"Comparing device IDs - Current: {currentPlayerID}, Entry: {data.userId}");
        }

        if (isCurrentUser)
        {
            // Get the background image component if it exists
            Image backgroundImage = entryGO.GetComponent<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(1f, 1f, 0f, 0.2f); // Light yellow background
            }

            // Highlight the text
            TextMeshProUGUI[] textFields = entryGO.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in textFields)
            {
                text.color = Color.yellow;
                text.fontStyle = FontStyles.Bold;
            }

            Debug.Log($"Highlighted entry for current user: {data.username} (ID: {data.userId})");
        }
    }

    /// <summary>
    /// Public method to force a refresh of the leaderboard
    /// </summary>
    public void ForceRefresh()
    {
        if (isPanelVisible)
        {
            RefreshLeaderBoard();
        }
    }
}