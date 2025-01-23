using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;

public class ScoreAPITests
{
    private GameObject testObject;
    private ScoreUploader scoreUploader;
    private LeaderBoardManager leaderBoardManager;
    private GameObject leaderBoardCanvas;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        testObject = new GameObject("TestObject");
        leaderBoardManager = testObject.AddComponent<LeaderBoardManager>();
        leaderBoardCanvas = CreateLeaderBoardUI();
        scoreUploader = testObject.AddComponent<ScoreUploader>();

        yield return null;
    }

    private GameObject CreateLeaderBoardUI()
    {
        var canvas = new GameObject("Canvas");
        canvas.AddComponent<Canvas>();
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();

        var container = new GameObject("LeaderBoardContainer");
        container.transform.SetParent(canvas.transform, false);
        leaderBoardManager.leaderBoardPanel = container;

        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(container.transform, false);
        viewport.AddComponent<RectTransform>();

        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        content.AddComponent<RectTransform>();
        leaderBoardManager.contentParent = content.transform;

        var rowTemplate = new GameObject("RowTemplate");
        rowTemplate.AddComponent<RectTransform>();
        leaderBoardManager.entryPrefab = rowTemplate;

        CreateScrollbar("Scrollbar Horizontal", container);
        CreateScrollbar("Scrollbar Vertical", container);

        return canvas;
    }

    private void CreateScrollbar(string name, GameObject parent)
    {
        var scrollbar = new GameObject(name);
        scrollbar.transform.SetParent(parent.transform, false);

        var slidingArea = new GameObject("Sliding Area");
        slidingArea.transform.SetParent(scrollbar.transform, false);

        var handle = new GameObject("Handle");
        handle.transform.SetParent(slidingArea.transform, false);
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(testObject);
        Object.Destroy(leaderBoardCanvas);
    }

    [UnityTest]
    public IEnumerator ScoreUpload_WhenScoreUpdated_ShouldUploadSuccess()
    {
        PlayerSession.SetLoggedIn("test-user-id", "TestUser");

        LogAssert.Expect(LogType.Log, "Uploading score - PlayerId: test-user-id, Username: TestUser, Score: 1000");
        LogAssert.Expect(LogType.Log, "Score upload successful");

        scoreUploader.UploadScore(1000);

        yield return new WaitForSeconds(1f);

        PlayerSession.SetLoggedOut();
    }

    [UnityTest]
    public IEnumerator LeaderBoard_WhenRefreshing_ShouldLoadData()
    {
        leaderBoardManager.ForceRefresh();

        yield return new WaitForSeconds(1f);

        Assert.That(leaderBoardManager.contentParent.childCount, Is.GreaterThanOrEqualTo(0),
            "Leaderboard should have entries after refresh");
    }
}