using System;

[System.Serializable]
public class LeaderBoardData
{
    public string userId;
    public string username;
    public int score;
    public DateTime timestamp;
}

[System.Serializable]
public class LeaderBoardResponse
{
    public LeaderBoardData[] items;  
}