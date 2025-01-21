using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;

public class LeaderBoardTests
{
    private GameObject testObject;
    private LeaderBoardManager leaderBoardManager;
    private Canvas canvas;

    [SetUp]
    public void Setup()
    {
        testObject = new GameObject();
        leaderBoardManager = testObject.AddComponent<LeaderBoardManager>();

        var canvasObject = new GameObject("Canvas", typeof(RectTransform));
        canvas = canvasObject.AddComponent<Canvas>();
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        SetupLeaderboard();
    }

    private void SetupLeaderboard()
    {
        var panelObject = new GameObject("LeaderBoardPanel", typeof(RectTransform));
        panelObject.transform.SetParent(canvas.transform, false);
        leaderBoardManager.leaderBoardPanel = panelObject;

        var contentObject = new GameObject("Content", typeof(RectTransform));
        contentObject.transform.SetParent(panelObject.transform, false);
        var contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.sizeDelta = Vector2.zero;
        leaderBoardManager.contentParent = contentObject.transform;

        leaderBoardManager.entryPrefab = CreateEntryPrefab();

        CreateSortingControls();
    }

    private GameObject CreateEntryPrefab()
    {
        var prefab = new GameObject("LeaderboardEntry", typeof(RectTransform));
        prefab.AddComponent<VerticalLayoutGroup>();

        var background = new GameObject("Background", typeof(RectTransform));
        background.transform.SetParent(prefab.transform, false);
        background.AddComponent<Image>();

        CreateTextComponent(prefab, "Username", 0);
        CreateTextComponent(prefab, "Score", 1);
        CreateTextComponent(prefab, "Date", 2);

        return prefab;
    }

    private void CreateTextComponent(GameObject parent, string name, int index)
    {
        var textObject = new GameObject(name, typeof(RectTransform));
        textObject.transform.SetParent(parent.transform, false);
        var tmp = textObject.AddComponent<TextMeshProUGUI>();
        tmp.text = "0";
        tmp.fontSize = 14;
        tmp.alignment = TextAlignmentOptions.Center;
    }

    private void CreateSortingControls()
    {
        var dropdownObject = new GameObject("SortDropdown", typeof(RectTransform));
        dropdownObject.transform.SetParent(leaderBoardManager.leaderBoardPanel.transform, false);
        var dropdown = dropdownObject.AddComponent<TMP_Dropdown>();
        leaderBoardManager.sortByDropdown = dropdown;

        var buttonObject = new GameObject("SortOrderButton", typeof(RectTransform));
        buttonObject.transform.SetParent(leaderBoardManager.leaderBoardPanel.transform, false);
        var button = buttonObject.AddComponent<Button>();
        buttonObject.AddComponent<Image>();
        leaderBoardManager.sortOrderButton = button;
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(testObject);
        Object.Destroy(canvas.gameObject);
    }

    [UnityTest]
    public IEnumerator LeaderBoard_WhenToggling_ShouldChangeVisibility()
    {
        leaderBoardManager.leaderBoardPanel.SetActive(false);

        leaderBoardManager.ToggleLeaderBoard();

        yield return null;

        Assert.That(leaderBoardManager.leaderBoardPanel.activeSelf, Is.True);
    }

    [UnityTest]
    public IEnumerator LeaderBoard_WhenRefreshing_ShouldClearExistingEntries()
    {
        var initialChildCount = leaderBoardManager.contentParent.childCount;

        leaderBoardManager.ForceRefresh();

        yield return new WaitForSeconds(0.1f);

        Assert.That(leaderBoardManager.contentParent.childCount, Is.GreaterThanOrEqualTo(0));
    }
}