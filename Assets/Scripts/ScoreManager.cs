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
        // Get or create player ID
        playerID = PlayerIDManager.GetOrCreatePlayerID();
        Debug.Log("Current player ID: " + playerID);

        // Add current scene index to visited set
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        visitedScenes.Add(currentSceneIndex);

        // Find ScoreUploader in the scene
        uploader = FindObjectOfType<ScoreUploader>();
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
            AddScore(100);
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
            uploader.UploadScore(playerID, currentScore);
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
}
