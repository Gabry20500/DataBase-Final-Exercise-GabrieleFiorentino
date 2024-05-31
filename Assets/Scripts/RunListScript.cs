using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RunListScript : MonoBehaviour
{
    public List<GameObject> runList;
    public GameObject runTilePrefab; // Prefab da instanziare
    public GameObject runListParent; // Il GameObject padre dove inserire i nuovi tile
    public string phpScriptURL = "https://testsitegabry.000webhostapp.com/GP3/get_runs.php";

    [Header("Reference for GoBack Button")]
    [SerializeField] private List<GameObject> ObjectsToDeactivate;
    [SerializeField] private GameObject ObjectToActivate;
    
    private RunData[] runs;
    private RunData[] originalRuns;

    void Start()
    {
        StartCoroutine(GetRunsData());
    }

    public IEnumerator GetRunsData() // Cambiato da private a public
    {
        using (UnityWebRequest www = UnityWebRequest.Get(phpScriptURL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error in request: " + www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;

                if (!IsValidJson(responseText))
                {
                    Debug.LogError("Invalid JSON response: " + responseText);
                    yield break;
                }

                RunsResponse response = JsonUtility.FromJson<RunsResponse>(responseText);

                if (response.status == "success")
                {
                    runs = response.data;
                    originalRuns = (RunData[])runs.Clone();
                    PopulateRunList(runs);
                }
                else
                {
                    Debug.LogError("Error in response: " + response.message);
                }
            }
        }
    }

    private void PopulateRunList(RunData[] runs)
    {
        // Clear the existing runList
        foreach (GameObject runItem in runList)
        {
            Destroy(runItem);
        }
        runList.Clear();

        for (int i = 0; i < runs.Length; i++)
        {
            GameObject runItem = Instantiate(runTilePrefab, runListParent.transform);
            runList.Add(runItem);

            TMP_Text[] texts = runItem.GetComponentsInChildren<TMP_Text>();

            if (texts.Length >= 4)
            {
                texts[0].text = runs[i].Username; // Username
                texts[1].text = runs[i].CurrentLevel.ToString(); // CurrentLevel
                texts[2].text = runs[i].PlayTime; // PlayTime
                texts[3].text = runs[i].Score.ToString(); // Score
            }
        }
    }

    public void SortByTimeAscending()
    {
        if (runs == null) return;

        System.Array.Sort(runs, (x, y) => x.PlayTime.CompareTo(y.PlayTime));
        PopulateRunList(runs);
    }

    public void SortByScoreDescending()
    {
        if (runs == null) return;

        System.Array.Sort(runs, (x, y) => y.Score.CompareTo(x.Score));
        PopulateRunList(runs);
    }

    public void SortByDefault()
    {
        if (originalRuns == null) return;

        runs = (RunData[])originalRuns.Clone();
        PopulateRunList(runs);
    }

    public void GoBack()
    {
        ObjectToActivate.SetActive(true);
        
        foreach (GameObject gameObject in ObjectsToDeactivate)
        {
            gameObject.SetActive(false);
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
                var obj = JsonUtility.FromJson<RunsResponse>(jsonString);
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
public class RunData
{
    public string Username;
    public int CurrentLevel;
    public string PlayTime;
    public int Score;
}

[System.Serializable]
public class RunsResponse
{
    public string status;
    public string message;
    public RunData[] data;
}
