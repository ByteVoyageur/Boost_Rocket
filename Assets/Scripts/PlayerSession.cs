using UnityEngine;

public static class PlayerSession
{
    public static bool IsLoggedIn = false;
    public static string CurrentUserId = "";  
    public static string CurrentUsername = "";
    public static string DeviceId = "";

    /// <summary>
    /// Call this when user logs in successfully
    /// </summary>
    public static void SetLoggedIn(string userId, string username)  
    {
        CurrentUserId = userId;
        CurrentUsername = username;
        IsLoggedIn = true;
    }

    /// <summary>
    /// Call this when user logs out or chooses "skip"
    /// </summary>
    public static void SetLoggedOut()
    {
        IsLoggedIn = false;
        CurrentUserId = "";  
        CurrentUsername = "";
    }
}