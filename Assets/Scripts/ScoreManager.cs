using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    public static ScoreManager Instance => instance;

    private int currentScore;
    private string playerID = string.Empty;
    [SerializeField] private TextMeshProUGUI scoreText;

    // Use a HashSet to track visited scenes
    private HashSet<int> visitedScenes = new HashSet<int>();

    // Reference to ScoreUploader
    private ScoreUploader uploader;

    private float startTime;
    private bool isTimerRunning;

    private void Awake()
    {
        // Implement singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Use the same ID logic as ScoreUploader
        if (PlayerSession.IsLoggedIn)
        {
            playerID = PlayerSession.CurrentUserId.ToString();
        }
        else
        {
            playerID = SystemInfo.deviceUniqueIdentifier;
        }
        Debug.Log($"ScoreManager using playerID: {playerID}");

        // Add current scene index to visited set
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        visitedScenes.Add(currentSceneIndex);

        // Find ScoreUploader in the scene
        uploader = GameObject.FindAnyObjectByType<ScoreUploader>();
        if (uploader == null)
        {
            Debug.LogWarning("ScoreUploader is not found in the scene. Score uploading will not work.");
        }

        // Update UI
        UpdateScoreText();
    }

    /// <summary>
    /// OnSceneTransition is called before loading the next scene. 
    /// If nextSceneIndex is not visited, add 100 points. Then mark it visited.
    /// </summary>
    public void OnSceneTransition(int nextSceneIndex)
    {
        if (!visitedScenes.Contains(nextSceneIndex))
        {
            //AddScore(100);
            visitedScenes.Add(nextSceneIndex);
        }
    }

    /// <summary>
    /// AddScore increments currentScore and optionally uploads the score to MongoDB.
    /// </summary>
    private void AddScore(int value)
    {
        currentScore += value;
        Debug.Log($"Current Score: {currentScore}");
        UpdateScoreText();

        // Upload to MongoDB Atlas if uploader is available
        if (uploader != null)
        {
            uploader.UploadScore(currentScore);
        }
    }

    /// <summary>
    /// UpdateScoreText refreshes the score text in the UI.
    /// </summary>
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreText();
        Debug.Log("Score has been reset to 0.");
    }


    /// <summary>
    /// Returns the current player's ID - either ObjectId for logged in users
    /// or device ID for non-logged in users
    /// </summary>
    public string GetCurrentPlayerID()
    {
        // Double check the current status and update if needed
        if (PlayerSession.IsLoggedIn && PlayerSession.CurrentUserId != null)
        {
            playerID = PlayerSession.CurrentUserId.ToString();
        }
        else if (string.IsNullOrEmpty(playerID))
        {
            playerID = SystemInfo.deviceUniqueIdentifier;
        }

        return playerID;
    }

    /// <summary>
    /// StartTimer initializes the startTime and begins timing.
    /// </summary>
    public void StartTimer()
    {
        startTime = Time.time;
        isTimerRunning = true;
        Debug.Log("Timer started.");
    }

    /// <summary>
    /// StopTimerAndAddScore stops the timer, calculates time-based score, and adds it to total score.
    /// </summary>
    public void StopTimerAndAddScore()
    {
        if (!isTimerRunning) return;

        float endTime = Time.time;
        float deltaTime = endTime - startTime;
        isTimerRunning = false;

        int timeScore = Mathf.Max(0, 1000 - Mathf.RoundToInt(deltaTime * 10));
        Debug.Log($"Time-based score = {timeScore}, deltaTime = {deltaTime}");
        AddScore(timeScore);
    }

    /// <summary>
    /// ResetTimer can be called when the player crashes or you need to discard current timing.
    /// </summary>
    public void ResetTimer()
    {
        isTimerRunning = false;
        startTime = 0f;
        Debug.Log("Timer reset.");
    }
}