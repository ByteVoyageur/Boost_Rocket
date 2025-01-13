using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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
    private IMongoCollection<BsonDocument> usersCollection; 

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

            // 1) Score collection
            scoreCollection = database.GetCollection<BsonDocument>(collectionName);

            // 2) Users collection
            usersCollection = database.GetCollection<BsonDocument>("Users");

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
    /// GetTopScores queries the 'Score' collection, sorts by 'score' descending,
    /// and returns the top 'limit' results as LeaderBoardData list.
    /// 
    /// For each record, we also retrieve username from 'Users' collection by playerID.
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
            // Sort by 'score' descending
            var sort = Builders<BsonDocument>.Sort.Descending("score");
            var findOptions = new FindOptions<BsonDocument>
            {
                Sort = sort,
                Limit = limit * 3 // we fetch more to handle tie or further sorting
            };

            // Query the Score collection
            var cursor = await scoreCollection.FindAsync(new BsonDocument(), findOptions);
            var documents = await cursor.ToListAsync();

            foreach (var doc in documents)
            {
                LeaderBoardData data = new LeaderBoardData();

                // 1) Score doc fields
                if (doc.Contains("playerID"))
                {
                    data.userId = doc["playerID"].AsString;
                }
                else
                {
                    data.userId = "";
                }

                if (doc.Contains("score"))
                {
                    data.score = doc["score"].AsInt32;
                }
                else
                {
                    data.score = 0;
                }

                // 2) Timestamp
                if (doc.Contains("timestamp"))
                {
                    // BsonDateTime -> C# DateTime (UTC)
                    data.timestamp = doc["timestamp"].ToUniversalTime();
                }
                else
                {
                    data.timestamp = DateTime.MinValue;
                }

                // 3) Look up username from Users collection by ObjectId
                //    Note: If 'playerID' is an ObjectId in the DB, parse it as ObjectId:
                //    var userObjId = new ObjectId(data.userId);
                //    If it's a deviceID, you might do a different approach.
                BsonDocument userDoc = null;
                try
                {
                    // Attempt to parse userId as ObjectId
                    var userObjId = new ObjectId(data.userId);
                    var userFilter = Builders<BsonDocument>.Filter.Eq("_id", userObjId);
                    userDoc = await usersCollection.Find(userFilter).FirstOrDefaultAsync();
                }
                catch (FormatException)
                {
                    // Means data.userId is not a valid ObjectId
                    // Possibly a device-based ID if user skipped login
                }

                // If found user doc, get username. Otherwise fallback to userId
                if (userDoc != null && userDoc.Contains("username"))
                {
                    data.username = userDoc["username"].AsString;
                }
                else
                {
                    // Fallback: if we can't find a user doc, or user doc doesn't have 'username'
                    data.username = data.userId;
                }

                // Add to result
                result.Add(data);
            }

            // Sort the result again to ensure tie-breaking for current user
            result.Sort((x, y) =>
            {
                int compare = y.score.CompareTo(x.score);
                if (compare == 0)
                {
                    // Put current user on top if there's a tie
                    if (x.userId == currentUserId) return -1;
                    if (y.userId == currentUserId) return 1;
                    return 0;
                }
                return compare;
            });

            // Take top 'limit'
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
