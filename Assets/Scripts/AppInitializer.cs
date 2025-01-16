using UnityEngine;

public class AppInitializer : MonoBehaviour
{
    private void Awake()
    {
        PlayerSession.ValidateSessionIntegrity();

        Debug.Log("[AppInitializer] Session validated.");
    }
}
