using UnityEngine;
using UnityEngine.UI;
using MongoDB.Driver;
using MongoDB.Bson;
using BCrypt.Net;
using TMPro;

public class UserAuthUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private GameObject loginPanel;


    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> userCollection;
    private IMongoCollection<BsonDocument> scoreCollection;

    private void Start()
    {
        InitializeMongoDB();
    }

    /// <summary>
    /// Initialize the MongoDB client and get collections
    /// </summary>
    private void InitializeMongoDB()
    {
        var settings = MongoClientSettings.FromConnectionString(
            "mongodb+srv://BoostRocket_admin:Stubborn0310@xiaosong.yupunes.mongodb.net/BoostRocket?retryWrites=true&w=majority&appName=BoostRocket");
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        client = new MongoClient(settings);
        database = client.GetDatabase("BoostRocket");

        // Prepare "Users" and "Score" collections
        userCollection = database.GetCollection<BsonDocument>("Users");
        scoreCollection = database.GetCollection<BsonDocument>("Score");
    }

    /// <summary>
    /// Called when "Register" button is clicked
    /// </summary>
    public void OnRegisterButtonClicked()
    {
        string inputUsername = usernameField.text;
        string inputPassword = passwordField.text;

        // 1) Check if the username already exists
        var filter = Builders<BsonDocument>.Filter.Eq("username", inputUsername);
        var existing = userCollection.Find(filter).FirstOrDefault();

        if (existing != null)
        {
            Debug.Log("Username already taken");
            feedbackText.text = "Username already taken. Try another name.";
            feedbackText.gameObject.SetActive(true);
            return;
        }

        // 2) Create a new user document
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(inputPassword);
        var userDoc = new BsonDocument
    {
        { "username", inputUsername },
        { "passwordHash", passwordHash },
        { "createdAt", System.DateTime.UtcNow }
    };
        userCollection.InsertOne(userDoc);

        var userId = userDoc["_id"].AsObjectId;
        Debug.Log("User registered with id: " + userId);
        feedbackText.text = "Register success! Welcome, " + inputUsername;

        // 3) Save the current user data in PlayerSession
        PlayerSession.CurrentUserId = userId;
        PlayerSession.CurrentUsername = inputUsername;
        PlayerSession.IsLoggedIn = true;

        loginPanel.SetActive(false);

    }



    /// <summary>
    /// Called when "Login" button is clicked
    /// </summary>
    public void OnLoginButtonClicked()
    {
        string inputUsername = usernameField.text;
        string inputPassword = passwordField.text;

        // Find user by username
        var filter = Builders<BsonDocument>.Filter.Eq("username", inputUsername);
        var userDoc = userCollection.Find(filter).FirstOrDefault();

        if (userDoc == null)
        {
            Debug.Log("User does not exist.");
            return;
        }

        // Compare password
        string storedHash = userDoc["passwordHash"].AsString;
        bool isMatch = BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);

        if (!isMatch)
        {
            Debug.Log("Wrong password.");
            return;
        }

        // Login success
        ObjectId userId = userDoc["_id"].AsObjectId;
        Debug.Log("Login success. userId: " + userId);

        PlayerSession.CurrentUserId = userId;
        loginPanel.SetActive(false);
    }

    /// <summary>
    /// Called when "Skip" button is clicked
    /// </summary>
    public void OnSkipButtonClicked()
    {
        // The user does not want to register or log in,
        // fallback to device ID or something else
        PlayerSession.CurrentUserId = ObjectId.Empty; // or keep a device ID-based system
        loginPanel.SetActive(false);
    }
}
