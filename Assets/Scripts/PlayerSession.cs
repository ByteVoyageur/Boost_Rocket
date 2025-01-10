using UnityEngine;
using MongoDB.Bson;

public static class PlayerSession
{
    // True if user has successfully logged in, false if not logged in or skipped
    public static bool IsLoggedIn = false;

    // Store the current user's ObjectId from "Users" collection
    public static ObjectId CurrentUserId = ObjectId.Empty;

    // Store the username if needed
    public static string CurrentUsername = "";

    // Optionally store the deviceID if you also want to track that
    public static string DeviceId = "";

    /// <summary>
    /// Call this when user logs in successfully
    /// </summary>
    public static void SetLoggedIn(ObjectId userId, string username)
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
        CurrentUserId = ObjectId.Empty;
        CurrentUsername = "";
    }
}
