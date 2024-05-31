using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public string phpScriptURL = "https://testsitegabry.000webhostapp.com/GP3/login_player.php";
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public TMP_Text logText;
    public GameObject loginErrorPanel;
    [SerializeField] List<GameObject> RunScreen;
    public GameManager gameManager;


    public void LoginPlayer()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        // Check that fields are not empty
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            logText.text = "Please fill in all fields.";
            logText.color = Color.red;
            return;
        }
        
        StartCoroutine(SendLoginRequest(username, password));
    }

    private IEnumerator SendLoginRequest(string username, string password)
    {
        // Create the form to send
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        // Send the request to the server
        using (UnityWebRequest www = UnityWebRequest.Post(phpScriptURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error in request: " + www.error);
            }
            else
            {
                // Parse the response
                string responseText = www.downloadHandler.text;

                // Check if the JSON response is valid
                if (!IsValidJson(responseText))
                {
                    Debug.LogError("Invalid JSON response: " + responseText);
                    yield break;
                }

                LoginResponse response = JsonUtility.FromJson<LoginResponse>(responseText);
                
                // Handle the response
                if (response.status == "success")
                {
                    logText.text = "Login successful!";
                    logText.color = Color.green;
                    
                    gameManager.currentUser = username;
                    Debug.Log("Current user: " + gameManager.currentUser);

                    gameObject.SetActive(false);
                    foreach (GameObject Run in RunScreen)
                    {
                        Run.SetActive(true);
                    }
                    
                    usernameInputField.text = "";
                    passwordInputField.text = "";
                }
                else if (response.status == "error")
                {
                    if (response.message == "Username not found.")
                    {
                        RegisterPlayer(username, password);
                        logText.text = "Username not found. Redirecting to registration.";
                        logText.color = Color.red;
                    }
                    else if (response.message == "Incorrect password.")
                    {
                        logText.text = "Incorrect password.";
                        logText.color = Color.red;
                        // Handle incorrect password case
                    }
                    else
                    {
                        Debug.LogError("Error during login: " + response.message);
                    }
                }
            }
        }
        
    }
    
    // Method to check if the JSON response is valid
    private bool IsValidJson(string jsonString)
    {
        jsonString = jsonString.Trim();
        if ((jsonString.StartsWith("{") && jsonString.EndsWith("}")) || // Object
            (jsonString.StartsWith("[") && jsonString.EndsWith("]")))   // Array
        {
            try
            {
                var obj = JsonUtility.FromJson<LoginResponse>(jsonString);
                Debug.Log(jsonString);
                return true;
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    
    private void RegisterPlayer(string username, string password)
    {
        // Implement your registration logic here
        loginErrorPanel.SetActive(true);
    }
}



[System.Serializable]
public class LoginResponse
{
    public string status;
    public string message;
    public User user; // This assumes you want to parse user details if login is successful
}

[System.Serializable]
public class User
{
    public int id;
    public string username;
    public string name;
}