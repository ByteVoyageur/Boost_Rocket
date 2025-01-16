using System;

[Serializable]
public class LeaderBoardData
{
    public string playerId;        
    public string username;      
    public int score;       
    public DateTime timestamp;   
}

[Serializable]
public class LeaderBoardResponse
{
    public LeaderBoardData[] items;  
}