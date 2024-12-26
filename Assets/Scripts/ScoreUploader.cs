using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;

public class ScoreUploader : MonoBehaviour
{
    [Header("MongoDB Connection Info")]
    [SerializeField]
    private string mongoConnectionUri =
        "mongodb+srv://myUser:myPassword@xiaosong.yupunes.mongodb.net/BoostRocket?retryWrites=true&w=majority&appName=BoostRocket";

    [SerializeField] private string databaseName = "BoostRocket";
    [SerializeField] private string collectionName = "Store";

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
    public void UploadScore(string playerID, int currentScore)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("MongoDB is not initialized. Cannot upload score.");
            return;
        }

        try
        {
            // Define a filter to find the existing document by playerID
            var filter = Builders<BsonDocument>.Filter.Eq("playerID", playerID);

            // Build an update definition to set the score and timestamp
            var update = Builders<BsonDocument>.Update
                .Set("score", currentScore)         // Always store the latest total score
                .Set("timestamp", System.DateTime.UtcNow);

            // Use UpdateOptions with IsUpsert = true
            var options = new UpdateOptions { IsUpsert = true };

            // Perform the upsert operation
            scoreCollection.UpdateOne(filter, update, options);

            Debug.Log($"Successfully upserted score: playerID={playerID}, score={currentScore}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to upload score to MongoDB: {ex.Message}");
        }
    }
}
