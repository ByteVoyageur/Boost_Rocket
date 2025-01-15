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
    public async void UploadScore(int currentScore)
    {
        try
        {
            // Decide which ID to use
            string playerId;
            if (PlayerSession.IsLoggedIn)
            {
                // If the user logged in, use their user ID
                playerId = PlayerSession.CurrentUserId;
            }
            else
            {
                // If the user skipped, use device ID
                playerId = SystemInfo.deviceUniqueIdentifier;
            }

            bool success = await APIClient.UploadScore(playerId, currentScore);

            if (success)
            {
                Debug.Log($"Successfully uploaded score: playerId={playerId}, score={currentScore}");
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

    private async void UploadScoreWithRetry(int currentScore, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await Task.Delay(i * 1000); 
                UploadScore(currentScore);
                return;
            }
            catch (Exception ex)
            {
                if (i == maxRetries - 1)
                {
                    Debug.LogError($"Failed to upload score after {maxRetries} attempts: {ex.Message}");
                }
            }
        }
    }
}