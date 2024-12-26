using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;  

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    public static ScoreManager Instance => instance;

    private int currentScore;
    [SerializeField] private TextMeshProUGUI scoreText;

    // Use a HashSet to track visited scenes
    private HashSet<int> visitedScenes = new HashSet<int>();

    private void Awake()
    {
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
        // Mark the initial scene as visited
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        visitedScenes.Add(currentSceneIndex);

        UpdateScoreText();
    }

    /// <summary>
    /// This method is called before loading the next scene.
    /// If the next scene hasn't been visited, grant points.
    /// Then mark the next scene as visited.
    /// </summary>
    public void OnSceneTransition(int nextSceneIndex)
    {
        if (!visitedScenes.Contains(nextSceneIndex))
        {
            AddScore(100);
            visitedScenes.Add(nextSceneIndex);
        }
    }

    private void AddScore(int value)
    {
        currentScore += value;
        Debug.Log($"Current Score: {currentScore}");
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }
}
