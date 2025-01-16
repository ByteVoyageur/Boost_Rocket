using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

public class APIClient
{
    public const string API_BASE_URL = "https://localhost:7233/api"; 

    public static async Task<UserResponse> Login(string username, string password)
    {
        var loginData = new LoginRequest
        {
            Username = username,
            Password = password
        };

        using (var request = CreateJsonRequest($"{API_BASE_URL}/user/login", loginData))
        {
            try
            {
                var response = await SendRequest<UserResponse>(request);
                return response;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Login failed: {ex.Message}");
                throw;
            }
        }
    }

    public static async Task<UserResponse> Register(string username, string password)
    {
        var registerData = new RegisterRequest
        {
            Username = username,
            Password = password
        };

        using (var request = CreateJsonRequest($"{API_BASE_URL}/user/register", registerData))
        {
            try
            {
                var response = await SendRequest<UserResponse>(request);
                return response;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Registration failed: {ex.Message}");
                throw;
            }
        }
    }

    private static UnityWebRequest CreateJsonRequest<T>(string url, T data)
    {
        var request = new UnityWebRequest(url, "POST");
        var jsonData = JsonUtility.ToJson(data);
        Debug.Log($"Request URL: {url}"); 
        Debug.Log($"Request data: {jsonData}"); 

        var bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }

    private static async Task<T> SendRequest<T>(UnityWebRequest request)
    {
        var operation = request.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            var errorMessage = $"Request failed: {request.error}";
            if (request.downloadHandler != null)
            {
                errorMessage += $"\nResponse: {request.downloadHandler.text}";
            }
            throw new Exception(errorMessage);
        }

        var json = request.downloadHandler.text;
        Debug.Log($"Response received: {json}"); 
        return JsonUtility.FromJson<T>(json);
    }

    public static async Task<bool> UploadScore(string playerId, int scoreValue, string username)
    {
        var scoreData = new ScoreUploadRequest
        {
            PlayerId = playerId,
            ScoreValue = scoreValue,
            Username = username  
        };

        using (var request = CreateJsonRequest($"{API_BASE_URL}/score", scoreData))
        {
            try
            {
                var jsonData = JsonUtility.ToJson(scoreData);
                Debug.Log($"Sending score data: {jsonData}");

                await SendRequest<ScoreResponse>(request);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Score upload failed: {ex.Message}");
                if (request.downloadHandler != null)
                {
                    Debug.LogError($"Response content: {request.downloadHandler.text}");
                }
                return false;
            }
        }
    }

    public static async Task<List<LeaderBoardData>> GetLeaderboard(
    string sortBy = "ScoreValue",
    string sortOrder = "desc",
    int limit = 10,
    string currentUserId = null)
    {
        try
        {
            string url = $"{API_BASE_URL}/leaderboard?sortBy={sortBy}&sortOrder={sortOrder}&limit={limit}&currentUserId={currentUserId}";

            using (var request = UnityWebRequest.Get(url))
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"Failed to get leaderboard: {request.error}");
                }

                var json = request.downloadHandler.text;
                var wrapper = JsonUtility.FromJson<LeaderBoardResponse>("{\"items\":" + json + "}");
                return new List<LeaderBoardData>(wrapper.items);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error getting leaderboard: {ex.Message}");
            throw;
        }
    }
}



[Serializable]
public class LoginRequest
{
    public string Username;
    public string Password;
}

[Serializable]
public class RegisterRequest
{
    public string Username;
    public string Password;
}

[Serializable]
public class UserResponse
{
    public string Id;
    public string Username;
}

[Serializable]
public class ScoreUploadRequest
{
    public string PlayerId;
    public int ScoreValue;
    public string Username;  
}

[Serializable]
public class ScoreResponse
{
    public string Id;
    public string PlayerId;
    public int ScoreValue;
    public DateTime Timestamp;
}