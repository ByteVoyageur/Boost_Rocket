// Unity LeaderBoardManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Threading.Tasks;
using System;

public class LeaderBoardManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject leaderBoardPanel;
    public GameObject entryPrefab;
    public Transform contentParent;

    [Header("Sorting Controls")]
    public TMP_Dropdown sortByDropdown;  
    public Button sortOrderButton;        

    private string currentPlayerID = "";
    private bool isPanelVisible = false;
    private string currentSortBy = "scorevalue";
    private string currentSortOrder = "desc";

    void Start()
    {
        leaderBoardPanel.SetActive(false);

        if (PlayerSession.IsLoggedIn)
        {
            currentPlayerID = PlayerSession.CurrentUserId;
        }
        else
        {
            currentPlayerID = SystemInfo.deviceUniqueIdentifier;
        }

        SetupSortingControls();

        Debug.Log($"LeaderBoard initialized with currentPlayerID: {currentPlayerID}");
    }

    private void SetupSortingControls()
    {
        if (sortByDropdown != null)
        {
            sortByDropdown.onValueChanged.AddListener(OnSortByChanged);
        }

        if (sortOrderButton != null)
        {
            sortOrderButton.onClick.AddListener(OnSortOrderChanged);
        }
    }

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
            leaderBoardPanel.SetActive(false);
            isPanelVisible = false;
        }
    }

    private async void RefreshLeaderBoard()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (PlayerSession.IsLoggedIn)
        {
            currentPlayerID = PlayerSession.CurrentUserId;
        }

        try
        {
            string url = $"{APIClient.API_BASE_URL}/leaderboard?sortBy={currentSortBy}&sortOrder={currentSortOrder}&limit=10&currentUserId={currentPlayerID}";
            var leaderboardData = await APIClient.GetLeaderboard(currentSortBy, currentSortOrder, 10, currentPlayerID);

            foreach (var data in leaderboardData)
            {
                CreateLeaderboardEntry(data);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to refresh leaderboard: {ex.Message}");
        }
    }

    private void CreateLeaderboardEntry(LeaderBoardData data)
    {
        GameObject entryGO = Instantiate(entryPrefab, contentParent);
        TextMeshProUGUI[] textFields = entryGO.GetComponentsInChildren<TextMeshProUGUI>();

        if (textFields.Length >= 3)
        {
            string displayUsername = TrimUsername(data.username);
            textFields[0].text = displayUsername;
            textFields[1].text = data.score.ToString();
            if (DateTime.TryParse(data.timestamp, out DateTime parsedTime))
            {
                textFields[2].text = parsedTime.ToString("dd-MM-yyyy");
            }
            else
            {
                textFields[2].text = "N/A"; 
            }

            HighlightCurrentUser(entryGO, data);
        }
    }

    private string TrimUsername(string username, int maxLength = 12)
    {
        if (string.IsNullOrEmpty(username))
            return "Anonymous";

        return username.Length > maxLength ?
            username.Substring(0, maxLength - 3) + "..." : username;
    }

    private void HighlightCurrentUser(GameObject entryGO, LeaderBoardData data)
    {
        if (data.userId == currentPlayerID)
        {
            Image backgroundImage = entryGO.GetComponent<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(1f, 0.92f, 0f, 0.3f);  
            }

            TextMeshProUGUI[] textFields = entryGO.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in textFields)
            {
                text.color = new Color(1f, 0.8f, 0f, 1f);  
                text.fontStyle = FontStyles.Bold | FontStyles.Underline;  
            }
        }
    }

    private void OnSortByChanged(int index)
    {
        currentSortBy = index == 0 ? "ScoreValue" : "Timestamp";
        RefreshLeaderBoard();
    }

    private void OnSortOrderChanged()
    {
        currentSortOrder = currentSortOrder == "desc" ? "asc" : "desc";
        RefreshLeaderBoard();
    }

    public void ForceRefresh()
    {
        if (isPanelVisible)
        {
            RefreshLeaderBoard();
        }
    }
}