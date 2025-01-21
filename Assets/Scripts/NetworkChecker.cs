using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class NetworkChecker : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private float retryInterval = 5f;
    [SerializeField] private float successMessageDuration = 5f;

    private bool isCheckingConnection = false;

    private void Start()
    {
        CheckAPIConnection();
    }

    public async void CheckAPIConnection()
    {
        if (isCheckingConnection) return;
        isCheckingConnection = true;

        try
        {
            bool isConnected = await TestAPIConnection();
            await UpdateStatusDisplay(isConnected);
        }
        finally
        {
            isCheckingConnection = false;
        }
    }

    private async Task<bool> TestAPIConnection()
    {
        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"{APIClient.API_BASE_URL}/leaderboard?limit=1"))
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                bool isSuccess = request.result == UnityWebRequest.Result.Success;

                if (isSuccess)
                {
                    Debug.Log("API server connection successful");
                }
                else
                {
                    Debug.LogWarning($"API server connection failed: {request.error}");
                }

                return isSuccess;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"API connection test failed: {ex.Message}");
            return false;
        }
    }

    private async Task UpdateStatusDisplay(bool isConnected)
    {
        if (statusText == null) return;

        if (!isConnected)
        {
            statusText.text = "API Server Connection Failed, Retrying...";
            statusText.color = Color.red;
            statusText.gameObject.SetActive(true);

            await Task.Delay((int)(retryInterval * 1000));
            CheckAPIConnection();
        }
        else
        {
            statusText.text = "API Server Connected Successfully";
            statusText.color = Color.green;
            statusText.gameObject.SetActive(true);

            await Task.Delay((int)(successMessageDuration * 1000));

            if (statusText != null && this != null)
            {
                statusText.text = "";
                statusText.gameObject.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        isCheckingConnection = false;
    }
}