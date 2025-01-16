using System;

[Serializable]
public class LeaderBoardData
{
    public string userId;        
    public string username;      
    public int score;       
    public string timestamp;   
}

[Serializable]
public class LeaderBoardResponse
{
    public LeaderBoardData[] items;  
}