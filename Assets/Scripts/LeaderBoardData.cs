using System;

/// <summary>
/// LeaderBoardData stores a single record for leaderboard display.
/// </summary>
[System.Serializable]
public class LeaderBoardData
{
    public string userId;       // The player's ID (MongoDB _id in the Users or deviceID)
    public string username;     // The player's username from Users collection
    public int score;           // Score
    public DateTime timestamp;  // When this score was updated
}
