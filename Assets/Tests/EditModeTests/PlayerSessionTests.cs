using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class PlayerSessionTests
{
    [SetUp]
    public void Setup()
    {
        PlayerPrefs.DeleteAll();
    }

    [TearDown]
    public void TearDown()
    {
        PlayerPrefs.DeleteAll();
    }

    [Test]
    public void SetLoggedIn_ShouldStoreUserData()
    {
        // Arrange
        string testUserId = "test123";
        string testUsername = "testUser";

        // Act
        PlayerSession.SetLoggedIn(testUserId, testUsername);

        // Assert
        Assert.That(PlayerSession.IsLoggedIn, Is.True);
        Assert.That(PlayerSession.CurrentUserId, Is.EqualTo(testUserId));
        Assert.That(PlayerSession.CurrentUsername, Is.EqualTo(testUsername));
    }

    [Test]
    public void SetLoggedOut_ShouldClearUserData()
    {
        // Arrange
        PlayerSession.SetLoggedIn("test123", "testUser");

        // Act
        PlayerSession.SetLoggedOut();

        // Assert
        Assert.That(PlayerSession.IsLoggedIn, Is.False);
        Assert.That(PlayerSession.CurrentUserId, Is.Empty);
        Assert.That(PlayerSession.CurrentUsername, Is.Empty);
    }

    [Test]
    public void GetActivePlayerId_WhenLoggedIn_ShouldReturnUserId()
    {
        // Arrange
        string testUserId = "test123";
        PlayerSession.SetLoggedIn(testUserId, "testUser");

        // Act
        string activePlayerId = PlayerSession.GetActivePlayerId();

        // Assert
        Assert.That(activePlayerId, Is.EqualTo(testUserId));
    }

    [Test]
    public void GetActivePlayerId_WhenNotLoggedIn_ShouldReturnDeviceId()
    {
        // Arrange
        PlayerSession.SetLoggedOut();

        // Act
        string activePlayerId = PlayerSession.GetActivePlayerId();

        // Assert
        Assert.That(activePlayerId, Is.EqualTo(PlayerSession.DeviceId));
    }

    [Test]
    public void GetActiveUsername_WhenLoggedIn_ShouldReturnUsername()
    {
        // Arrange
        string testUsername = "testUser";
        PlayerSession.SetLoggedIn("test123", testUsername);

        // Act
        string activeUsername = PlayerSession.GetActiveUsername();

        // Assert
        Assert.That(activeUsername, Is.EqualTo(testUsername));
    }

    [Test]
    public void GetActiveUsername_WhenNotLoggedIn_ShouldReturnGuest()
    {
        // Arrange
        PlayerSession.SetLoggedOut();

        // Act
        string activeUsername = PlayerSession.GetActiveUsername();

        // Assert
        Assert.That(activeUsername, Is.EqualTo("Guest"));
    }

    [Test]
    public void ValidateSessionIntegrity_WhenInvalidState_ShouldForceLogout()
    {
        // Arrange - ?????????IsLoggedIn = true ? UserId ???
        PlayerPrefs.SetInt("isLoggedIn", 1);
        PlayerPrefs.SetString("userId", "");
        PlayerPrefs.Save();

        // Act
        PlayerSession.ValidateSessionIntegrity();

        // Assert
        Assert.That(PlayerSession.IsLoggedIn, Is.False);
        Assert.That(PlayerSession.CurrentUserId, Is.Empty);
    }
}