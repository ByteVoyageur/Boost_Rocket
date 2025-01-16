using UnityEngine;

public static class PlayerSession
{
    private const string LOGGED_IN_KEY = "isLoggedIn";
    private const string USER_ID_KEY = "userId";
    private const string USERNAME_KEY = "username";
    private const string DEVICE_ID_KEY = "deviceId";

    private static string deviceId;

    public static bool IsLoggedIn
    {
        get => PlayerPrefs.GetInt(LOGGED_IN_KEY, 0) == 1;
        private set => PlayerPrefs.SetInt(LOGGED_IN_KEY, value ? 1 : 0);
    }

    public static string CurrentUserId
    {
        get => PlayerPrefs.GetString(USER_ID_KEY, "");
        private set => PlayerPrefs.SetString(USER_ID_KEY, value);
    }

    public static string CurrentUsername
    {
        get => PlayerPrefs.GetString(USERNAME_KEY, "");
        private set => PlayerPrefs.SetString(USERNAME_KEY, value);
    }

    public static string DeviceId
    {
        get
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = PlayerPrefs.GetString(DEVICE_ID_KEY, "");
                if (string.IsNullOrEmpty(deviceId))
                {
                    deviceId = SystemInfo.deviceUniqueIdentifier;
                    PlayerPrefs.SetString(DEVICE_ID_KEY, deviceId);
                }
            }
            return deviceId;
        }
    }

    public static void SetLoggedIn(string userId, string username)
    {
        CurrentUserId = userId;
        CurrentUsername = username;
        IsLoggedIn = true;
        PlayerPrefs.Save();
        Debug.Log($"Player logged in - UserId: {userId}, Username: {username}");
    }

    public static void SetLoggedOut()
    {
        IsLoggedIn = false;
        CurrentUserId = "";
        CurrentUsername = "";
        ClearPlayerPrefs();
        Debug.Log("Player logged out and data cleared");
    }

    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteKey(LOGGED_IN_KEY);
        PlayerPrefs.DeleteKey(USER_ID_KEY);
        PlayerPrefs.DeleteKey(USERNAME_KEY);
        PlayerPrefs.Save();
        deviceId = null;
    }

    public static string GetActivePlayerId()
    {
        return IsLoggedIn ? CurrentUserId : DeviceId;
    }

    public static string GetActiveUsername()
    {
        return IsLoggedIn ? CurrentUsername : "Guest";
    }
}