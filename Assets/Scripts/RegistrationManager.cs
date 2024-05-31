using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class RegistrationManager : MonoBehaviour
{
    public string phpScriptURL = "https://testsitegabry.000webhostapp.com/GP3/register_player.php";
    [SerializeField] TMP_InputField usernameInputField;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] TMP_Text logText;
    [SerializeField] List<GameObject> RunScreen;
    public GameManager gameManager;

    public void RegisterPlayer()
    {
        if (string.IsNullOrEmpty(usernameInputField.text) ||
           string.IsNullOrEmpty(passwordInputField.text) ||
           string.IsNullOrEmpty(nameInputField.text))
        {
            logText.text = "Please fill in all fields.";
            logText.color = Color.red;
            return;
        }

        string username = usernameInputField.text;
        string password = passwordInputField.text;
        string name = nameInputField.text;

        StartCoroutine(SendRegistrationRequest(username, password, name));
    }

    private IEnumerator SendRegistrationRequest(string username, string password, string name)
    {
        // Create the form to send
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("name", name);

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


                RegistrationResponse response = JsonUtility.FromJson<RegistrationResponse>(responseText);

                // Handle the response
                if (response.status == "success")
                {
                    logText.text = "Registration successful!";
                    logText.color = Color.green;
                    
                    gameManager.currentUser = usernameInputField.text;
                    gameObject.SetActive(false);
                    foreach (GameObject Run in RunScreen)
                    {
                        Run.SetActive(true);
                    }
                    
                    usernameInputField.text = "";
                    passwordInputField.text = "";
                    nameInputField.text = "";
                }
                else
                {
                    logText.text = response.message;
                    logText.color = Color.red;
                }      
            }
        }
    }
}

[System.Serializable]
public class RegistrationResponse
{
    public string status;
    public string message;
}
