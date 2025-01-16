using UnityEngine;

public static class PlayerSession
{
    // These keys are used to store player session state in PlayerPrefs.
    private const string LOGGED_IN_KEY = "isLoggedIn";
    private const string USER_ID_KEY = "userId";
    private const string USERNAME_KEY = "username";
    private const string DEVICE_ID_KEY = "deviceId";
    private static string deviceId;
    private const string SKIPPED_KEY = "hasSkippedLogin";

    public static bool HasSkippedLogin
    {
        get => PlayerPrefs.GetInt(SKIPPED_KEY, 0) == 1;
        private set => PlayerPrefs.SetInt(SKIPPED_KEY, value ? 1 : 0);
    }

    /// <summary>
    /// Determines if the player is logged in. True if 'isLoggedIn' key == 1.
    /// </summary>
    public static bool IsLoggedIn
    {
        get => PlayerPrefs.GetInt(LOGGED_IN_KEY, 0) == 1;
        private set => PlayerPrefs.SetInt(LOGGED_IN_KEY, value ? 1 : 0);
    }

    /// <summary>
    /// Stores the MongoDB ObjectId for the logged-in user, or empty if not logged in.
    /// </summary>
    public static string CurrentUserId
    {
        get => PlayerPrefs.GetString(USER_ID_KEY, "");
        private set => PlayerPrefs.SetString(USER_ID_KEY, value);
    }

    /// <summary>
    /// Stores the username for the logged-in user, or empty if not logged in.
    /// </summary>
    public static string CurrentUsername
    {
        get => PlayerPrefs.GetString(USERNAME_KEY, "");
        private set => PlayerPrefs.SetString(USERNAME_KEY, value);
    }

    /// <summary>
    /// A unique device identifier for "guest" or offline play.
    /// We lazily load it from PlayerPrefs or SystemInfo.deviceUniqueIdentifier.
    /// </summary>
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

    /// <summary>
    /// Marks the player as logged in, storing the userId and username to PlayerPrefs.
    /// </summary>
    public static void SetLoggedIn(string userId, string username)
    {
        CurrentUserId = userId;
        CurrentUsername = username;
        IsLoggedIn = true;
        PlayerPrefs.Save();
        Debug.Log($"[PlayerSession] Logged in - UserId: {userId}, Username: {username}");
    }

    /// <summary>
    /// Marks the player as logged out and clears the relevant keys in PlayerPrefs.
    /// </summary>
    public static void SetLoggedOut()
    {
        // If we want to preserve the deviceId, do not delete it here.
        // Otherwise, we can also delete it to force a new deviceId next time.
        IsLoggedIn = false;
        CurrentUserId = "";
        CurrentUsername = "";
        ClearPlayerPrefs();
        Debug.Log("[PlayerSession] Logged out and data cleared");
    }

    /// <summary>
    /// Deletes the main login keys and resets the in-memory deviceId so that
    /// it will be reloaded from PlayerPrefs or SystemInfo on next access.
    /// </summary>
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteKey(LOGGED_IN_KEY);
        PlayerPrefs.DeleteKey(USER_ID_KEY);
        PlayerPrefs.DeleteKey(USERNAME_KEY);
        // If you want to also regenerate device ID after logout, uncomment below:
        // PlayerPrefs.DeleteKey(DEVICE_ID_KEY);

        PlayerPrefs.Save();
        deviceId = null;
    }

    /// <summary>
    /// Gets the active player ID. This is either the logged-in user's ID
    /// or the device ID if not logged in.
    /// </summary>
    public static string GetActivePlayerId()
    {
        return IsLoggedIn ? CurrentUserId : DeviceId;
    }

    /// <summary>
    /// Gets the active username. This is either the logged-in user's name
    /// or "Guest" if not logged in.
    /// </summary>
    public static string GetActiveUsername()
    {
        return IsLoggedIn ? CurrentUsername : "Guest";
    }

    /// <summary>
    /// This method can be called at the very beginning (e.g., in a bootstrap scene)
    /// to ensure there's no contradiction like "IsLoggedIn = true but userId is empty".
    /// </summary>
    public static void ValidateSessionIntegrity()
    {
        if (IsLoggedIn && string.IsNullOrEmpty(CurrentUserId))
        {
            Debug.LogWarning("[PlayerSession] Detected IsLoggedIn but UserId is empty. Forcing logout.");
            SetLoggedOut();
        }
    }

    public static void SetSkipMode()
    {
        HasSkippedLogin = true;
        IsLoggedIn = false;
        CurrentUserId = "";
        CurrentUsername = "";
        PlayerPrefs.Save();
    }

    public static void ClearSkip()
    {
        HasSkippedLogin = false;
        PlayerPrefs.Save();
    }
}
