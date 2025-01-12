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
        Debug.Log($"[UserAuthUI] PlayerSession.IsLoggedIn: {PlayerSession.IsLoggedIn}");

        InitializeMongoDB();

        // Check if user is already logged in
        if (PlayerSession.IsLoggedIn)
        {
            // Hide login panel if user is already logged in
            loginPanel.SetActive(false);
        }
        else
        {
            // Otherwise, show the login panel
            loginPanel.SetActive(true);
        }
    }

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

    public void OnRegisterButtonClicked()
    {
        string inputUsername = usernameField.text;
        string inputPassword = passwordField.text;

        // 1) Check if the username already exists
        var filter = Builders<BsonDocument>.Filter.Eq("username", inputUsername);
        var existing = userCollection.Find(filter).FirstOrDefault();

        if (existing != null)
        {
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
        PlayerSession.SetLoggedIn(userId, inputUsername);

        // Hide the panel
        loginPanel.SetActive(false);
    }

    public void OnLoginButtonClicked()
    {
        string inputUsername = usernameField.text;
        string inputPassword = passwordField.text;

        var filter = Builders<BsonDocument>.Filter.Eq("username", inputUsername);
        var userDoc = userCollection.Find(filter).FirstOrDefault();

        if (userDoc == null)
        {
            feedbackText.text = "User does not exist.";
            feedbackText.gameObject.SetActive(true);
            return;
        }

        string storedHash = userDoc["passwordHash"].AsString;
        bool isMatch = BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);

        if (!isMatch)
        {
            feedbackText.text = "Wrong password.";
            feedbackText.gameObject.SetActive(true);
            return;
        }

        // Login success
        ObjectId userId = userDoc["_id"].AsObjectId;
        PlayerSession.SetLoggedIn(userId, inputUsername);

        loginPanel.SetActive(false);
    }

    public void OnSkipButtonClicked()
    {
        // If user wants to skip login/registration
        PlayerSession.SetLoggedOut();
        // or set up DeviceID-based logic
        loginPanel.SetActive(false);
    }
}
