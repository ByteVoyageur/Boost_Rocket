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
        // 创建测试对象
        testObject = new GameObject("TestObject");

        // 设置LeaderBoardManager及其UI层级
        leaderBoardManager = testObject.AddComponent<LeaderBoardManager>();

        // 创建完整的UI层级结构
        leaderBoardCanvas = CreateLeaderBoardUI();

        // 设置ScoreUploader
        scoreUploader = testObject.AddComponent<ScoreUploader>();

        yield return null;
    }

    private GameObject CreateLeaderBoardUI()
    {
        // 创建Canvas
        var canvas = new GameObject("Canvas");
        canvas.AddComponent<Canvas>();
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();

        // 创建LeaderBoardContainer
        var container = new GameObject("LeaderBoardContainer");
        container.transform.SetParent(canvas.transform, false);
        leaderBoardManager.leaderBoardPanel = container;

        // 创建Viewport
        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(container.transform, false);
        viewport.AddComponent<RectTransform>();

        // 创建Content
        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        content.AddComponent<RectTransform>();
        leaderBoardManager.contentParent = content.transform;

        // 创建RowTemplate作为预制体
        var rowTemplate = new GameObject("RowTemplate");
        rowTemplate.AddComponent<RectTransform>();
        leaderBoardManager.entryPrefab = rowTemplate;

        // 创建滚动条
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
        // 设置测试用户
        PlayerSession.SetLoggedIn("test-user-id", "TestUser");

        // 预期的日志消息
        LogAssert.Expect(LogType.Log, "Uploading score - PlayerId: test-user-id, Username: TestUser, Score: 1000");
        LogAssert.Expect(LogType.Log, "Score upload successful");

        // Act
        scoreUploader.UploadScore(1000);

        // 等待上传完成
        yield return new WaitForSeconds(1f);

        // 清理
        PlayerSession.SetLoggedOut();
    }

    [UnityTest]
    public IEnumerator LeaderBoard_WhenRefreshing_ShouldLoadData()
    {
        // Act
        leaderBoardManager.ForceRefresh();

        // 等待数据加载
        yield return new WaitForSeconds(1f);

        // Assert
        Assert.That(leaderBoardManager.contentParent.childCount, Is.GreaterThanOrEqualTo(0),
            "Leaderboard should have entries after refresh");
    }
}