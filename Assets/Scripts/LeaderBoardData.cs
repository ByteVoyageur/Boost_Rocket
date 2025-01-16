using System;

[Serializable]
public class LeaderBoardData
{
    public string UserId;        
    public string Username;      
    public int ScoreValue;       
    public DateTime Timestamp;   
}

[Serializable]
public class LeaderBoardResponse
{
    public LeaderBoardData[] items;  
}