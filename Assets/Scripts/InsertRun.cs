using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InsertRun : MonoBehaviour
{
    public string phpScriptURL = "https://testsitegabry.000webhostapp.com/GP3/insert_run.php";
    public TMP_InputField currentLevelInput;
    public TMP_InputField playTimeInput;
    public TMP_InputField scoreInput;
    public TMP_Text logText;
    public GameManager gameManager;
 public RunListScript runListScript; // Riferimento allo script RunListScript

    public void OnSubmit()
    {
        string username = gameManager.currentUser;
        string currentLevel = currentLevelInput.text;
        string playTime = playTimeInput.text;
        string score = scoreInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(currentLevel) ||
            string.IsNullOrEmpty(playTime) || string.IsNullOrEmpty(score))
        {
            logText.text = "Please fill in all fields.";
            logText.color = Color.red;
            return;
        }

        StartCoroutine(SendData(username, currentLevel, playTime, score));
    }

    private IEnumerator SendData(string username, string currentLevel, string playTime, string score)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("currentLevel", currentLevel);
        form.AddField("playTime", playTime);
        form.AddField("score", score);

        using (UnityWebRequest www = UnityWebRequest.Post(phpScriptURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error in request: " + www.error);
                logText.text = "Error in request: " + www.error;
                logText.color = Color.red;
            }
            else
            {
                string responseText = www.downloadHandler.text;

                if (!IsValidJson(responseText))
                {
                    Debug.LogError("Invalid JSON response: " + responseText);
                    logText.text = "Invalid server response.";
                    logText.color = Color.red;
                    yield break;
                }

                DataResponse response = JsonUtility.FromJson<DataResponse>(responseText);

                if (response.status == "success")
                {
                    logText.text = "Data sent successfully!";
                    logText.color = Color.green;

                    currentLevelInput.text = "";
                    playTimeInput.text = "";
                    scoreInput.text = "";
                    // Aggiorna la lista delle run
                    if (runListScript != null)
                    {
                        runListScript.StartCoroutine(runListScript.GetRunsData());
                    }
                }
                else if (response.status == "error")
                {
                    logText.text = response.message;
                    logText.color = Color.red;
                }
            }
        }
    }

    private bool IsValidJson(string jsonString)
    {
        jsonString = jsonString.Trim();
        if ((jsonString.StartsWith("{") && jsonString.EndsWith("}")) ||
            (jsonString.StartsWith("[") && jsonString.EndsWith("]")))
        {
            try
            {
                var obj = JsonUtility.FromJson<DataResponse>(jsonString);
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
}

[System.Serializable]
public class DataResponse
{
    public string status;
    public string message;
}