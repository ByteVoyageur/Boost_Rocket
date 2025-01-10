using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;

public class ScoreUploader : MonoBehaviour
{
    [Header("MongoDB Connection Info")]
    [SerializeField]
    private string mongoConnectionUri =
    "mongodb+srv://BoostRocket_admin:Stubborn0310@xiaosong.yupunes.mongodb.net/BoostRocket?retryWrites=true&w=majority&appName=BoostRocket&authMechanism=SCRAM-SHA-256";


    [SerializeField] private string databaseName = "BoostRocket";
    [SerializeField] private string collectionName = "Score";

    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> scoreCollection;

    private bool isInitialized = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeMongoDB();
    }

    private void InitializeMongoDB()
    {
        try
        {
            var settings = MongoClientSettings.FromConnectionString(mongoConnectionUri);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            client = new MongoClient(settings);
            database = client.GetDatabase(databaseName);
            scoreCollection = database.GetCollection<BsonDocument>(collectionName);

            isInitialized = true;
            Debug.Log("MongoDB initialization succeeded.");
        }
        catch (System.Exception ex)
        {
            isInitialized = false;
            Debug.LogError($"MongoDB initialization failed: {ex.Message}");
        }
    }

    /// <summary>
    /// UploadScore is used to upsert (update or insert) player's score 
    /// under the same document in MongoDB based on the playerID.
    /// </summary>
    public void UploadScore(int currentScore)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("MongoDB is not initialized. Cannot upload score.");
            return;
        }

        try
        {
            // Decide which ID to use
            string playerID;
            if (PlayerSession.IsLoggedIn)
            {
                // If the user logged in, get the ObjectId.ToString()
                playerID = PlayerSession.CurrentUserId.ToString();
            }
            else
            {
                // If the user skipped, use device ID (or any fallback)
                playerID = SystemInfo.deviceUniqueIdentifier;
            }

            var filter = Builders<BsonDocument>.Filter.Eq("playerID", playerID);
            var update = Builders<BsonDocument>.Update
                .Set("score", currentScore)
                .Set("timestamp", System.DateTime.UtcNow);

            var options = new UpdateOptions { IsUpsert = true };
            scoreCollection.UpdateOne(filter, update, options);

            Debug.Log($"Successfully upserted score: playerID={playerID}, score={currentScore}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to upload score to MongoDB: {ex.Message}");
        }
    }


}
