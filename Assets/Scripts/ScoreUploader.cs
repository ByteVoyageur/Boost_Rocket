// Unity: ScoreUploader.cs :
using UnityEngine;
using System;
using System.Threading.Tasks;

public class ScoreUploader : MonoBehaviour
{
    private static ScoreUploader instance;
    public static ScoreUploader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScoreUploader>();
            }
            return instance;
        }
    }

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

    /// <summary>
    /// UploadScore is used to update or insert player's score through the API
    /// </summary>
    public async void UploadScore(int scoreValue)
    {
        try
        {
            string playerId = PlayerSession.GetActivePlayerId();
            string username = PlayerSession.GetActiveUsername();

            Debug.Log($"Uploading score - PlayerId: {playerId}, Username: {username}, Score: {scoreValue}");

            bool success = await APIClient.UploadScore(playerId, scoreValue, username);
            if (success)
            {
                Debug.Log($"Score upload successful");
            }
            else
            {
                Debug.LogWarning("Score upload completed but may not have succeeded");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to upload score: {ex.Message}");
        }
    }

    /// <summary>
    /// Retries uploading score in case of failure
    /// </summary>
    private async Task RetryUploadScore(int scoreValue, int maxRetries = 3)
    {
        for (int i = 1; i <= maxRetries; i++)
        {
            try
            {
                await Task.Delay(i * 1000);

                string playerId = PlayerSession.GetActivePlayerId();
                string username = PlayerSession.GetActiveUsername();

                bool success = await APIClient.UploadScore(
                    playerId,
                    scoreValue,
                    username
                );

                if (success)
                {
                    Debug.Log($"Successfully uploaded score on retry attempt {i}");
                    return;
                }
            }
            catch (Exception ex)
            {
                if (i == maxRetries)
                {
                    Debug.LogError($"Failed to upload score after {maxRetries} retry attempts: {ex.Message}");
                }
            }
        }
    }
}