using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;

public class ScoreUploader : MonoBehaviour
{
    [Header("MongoDB Connection Info")]
    [SerializeField]
    private string mongoConnectionUri =
        "mongodb+srv://username:password@clustername.mongodb.net/?retryWrites=true&w=majority&appName=BoostRocket";

    [SerializeField] private string databaseName = "BoostRocket";
    [SerializeField] private string collectionName = "Score";

    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> scoreCollection;

    private bool isInitialized = false;

    private void Awake()
    {
        // Optional: Make this GameObject persistent across scenes
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeMongoDB();
    }

    /// <summary>
    /// InitializeMongoDB is used to connect to the MongoDB Atlas database and retrieve the target collection.
    /// </summary>
    private void InitializeMongoDB()
    {
        try
        {
            // Create settings from connection string
            var settings = MongoClientSettings.FromConnectionString(mongoConnectionUri);
            // Specify the ServerApi version
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            // Create MongoClient
            client = new MongoClient(settings);

            // Get database
            database = client.GetDatabase(databaseName);

            // Get collection
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
    /// UploadScore is used to insert a new document into the Score collection.
    /// </summary>
    /// <param name="playerID">The ID of the current player.</param>
    /// <param name="currentScore">The player's total score.</param>
    public void UploadScore(string playerID, int currentScore)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("MongoDB is not initialized. Cannot upload score.");
            return;
        }

        try
        {
            var doc = new BsonDocument
            {
                { "playerID", playerID },
                { "score", currentScore },
                { "timestamp", System.DateTime.UtcNow }
            };

            scoreCollection.InsertOne(doc);
            Debug.Log($"Successfully uploaded score: playerID={playerID}, score={currentScore}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to upload score to MongoDB: {ex.Message}");
        }
    }
}
