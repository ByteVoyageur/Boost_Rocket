using UnityEngine;

public static class PlayerIDManager
{
    /// <summary>
    /// GetOrCreatePlayerID checks if a local ID exists in PlayerPrefs. 
    /// If not, it generates a new GUID and stores it.
    /// </summary>
    /// <returns>A string representing the local user's ID.</returns>
    public static string GetOrCreatePlayerID()
    {
        if (PlayerPrefs.HasKey("playerID"))
        {
            return PlayerPrefs.GetString("playerID");
        }
        else
        {
            string newID = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("playerID", newID);
            return newID;
        }
    }
}
