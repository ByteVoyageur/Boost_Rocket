using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// ScoreFetcher is responsible for retrieving top scores (leaderboard) from MongoDB.
/// </summary>
public class ScoreFetcher : MonoBehaviour
{
    [Header("MongoDB Connection Info")]
    [SerializeField]
    private string mongoConnectionUri =
    "mongodb+srv://BoostRocket_admin:Stubborn0310@xiaosong.yupunes.mongodb.net/BoostRocket?retryWrites=true&w=majority&appName=BoostRocket";



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

    /// <summary>
    /// InitializeMongoDB connects to the MongoDB using provided credentials.
    /// </summary>
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
            Debug.Log("MongoDB initialization (ScoreFetcher) succeeded.");
        }
        catch (System.Exception ex)
        {
            isInitialized = false;
            Debug.LogError($"MongoDB initialization (ScoreFetcher) failed: {ex.Message}");
        }
    }

    /// <summary>
    /// GetTopScores queries the collection, sorts by 'score' descending,
    /// and returns the top 'limit' results as LeaderBoardData list.
    /// If there's a tie in scores, current user will be placed before others with the same score.
    /// </summary>
    public async Task<List<LeaderBoardData>> GetTopScores(int limit, string currentUserId)
    {
        List<LeaderBoardData> result = new List<LeaderBoardData>();

        if (!isInitialized)
        {
            Debug.LogWarning("MongoDB is not initialized (ScoreFetcher). Cannot get top scores.");
            return result;
        }

        try
        {
            var sort = Builders<BsonDocument>.Sort.Descending("score");

            var findOptions = new FindOptions<BsonDocument>
            {
                Sort = sort,
                Limit = limit * 3
            };

            var cursor = await scoreCollection.FindAsync(new BsonDocument(), findOptions);
            var documents = await cursor.ToListAsync();

            foreach (var doc in documents)
            {
                LeaderBoardData data = new LeaderBoardData
                {
                    userId = doc.Contains("playerID") ? doc["playerID"].AsString : "",
                    username = doc.Contains("playerID") ? doc["playerID"].AsString : "",
                    score = doc.Contains("score") ? doc["score"].AsInt32 : 0
                };
                result.Add(data);
            }

            result.Sort((x, y) =>
            {
                int compare = y.score.CompareTo(x.score);
                if (compare == 0)
                {
                    if (x.userId == currentUserId) return -1;
                    if (y.userId == currentUserId) return 1;
                    return 0;
                }
                return compare;
            });

            if (result.Count > limit)
            {
                result = result.GetRange(0, limit);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to retrieve top scores from MongoDB (ScoreFetcher): {ex.Message}");
        }

        return result;
    }
}
