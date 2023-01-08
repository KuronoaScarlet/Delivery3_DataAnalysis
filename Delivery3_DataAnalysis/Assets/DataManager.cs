using Gamekit3D;
using Gamekit3D.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using static Gamekit3D.Damageable;
using static UnityEditor.Progress;

public enum HitType
{
    NONLETHAL,
    LETHAL,
    LAVA,
    NONE,
}
[System.Serializable]
public class HitData
{
    public int hitNum;
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
    public string enemyName;
    public float enemyPosX;
    public float enemyPosY;
    public float enemyPosZ;
    public float gameTime;

}
[System.Serializable]
public class PositionData
{
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
    public float playerForwardX;
    public float playerForwardY;
    public float playerForwardZ;
    public float gameTime;

}
public class DataManager : MonoBehaviour, IMessageReceiver
{
    //General
    public string url = "https://citmalumnes.upc.es/~carlesgdlm/";
    bool sendingPos = false;
    public GameObject positionArrow;
    public GameObject hitsAndDeathsMarker;
    public List<GameObject> allDebugPrefabs;
    //Posititon
    GameObject player;
    public PositionData[] posData;
    string posInfo;
    string[] allPosition;
    string[] posPoint;
    //Hits and Death
    Vector3 playerPos, enemyPos;
    string enemyName, hitNum, gameTime;
    Damageable m_Damageable;
    public HitData[] hitData;
    string lastQuery;
    string[] hitPoints;
    string[] hitDataString;
    public HitData[] deathData;
    string[] deathPoints;
    string[] deathDataString;
    public LayerMask hitLayer;
    public LayerMask deathsLayer;
    GameObject[] heatBoxes;
    float allowableDistance = 5f;
    Material cubeMaterial;
    float red;
    float blue;
    float colorAdjuster = 0.2f;

    SendData data = new SendData();
    private void OnEnable()
    {
        m_Damageable = GameObject.Find("Ellen").GetComponent<Damageable>();
        m_Damageable.onDamageMessageReceivers.Add(this);
        player = GameObject.Find("Ellen");
        heatBoxes = GameObject.FindGameObjectsWithTag("HitTag");
        cubeMaterial = hitsAndDeathsMarker.GetComponent<Material>();
    }
    private void OnDisable()
    {
        m_Damageable.onDamageMessageReceivers.Remove(this);
    }
    private void Update()
    { 
        if(!sendingPos)
            StartCoroutine(PosAdded("PositionTracker.php"));
    }
    #region Hits&Death
    public void OnReceiveMessage(MessageType type, object sender, object msg)
    {
        Vector3 playerPosition = ((MonoBehaviour)sender).transform.position;
        Vector3 enemy = ((DamageMessage)msg).damager.gameObject.transform.position;
        string damagerName = ((DamageMessage)msg).damager.gameObject.transform.root.name;

        switch (type)
        {
            case MessageType.DAMAGED:
                HitAdded(playerPosition, enemy, damagerName, Time.time, "HitTracker.php");
                break;
            case MessageType.DEAD:
                HitAdded(playerPosition, enemy, damagerName, Time.time, "HitTracker.php");
                HitAdded(playerPosition, enemy, damagerName, Time.time, "DeathTracker.php");
                break;
            default:
                break;
        }
    }

    void HitAdded(Vector3 player, Vector3 enemy, string eN, float time, string php)
    {
        playerPos = player;
        enemyPos = enemy;
        enemyName = eN;
        gameTime = time.ToString().Replace(",", ".");

        WWWForm form = new WWWForm();
        form.AddField("playerPosX", playerPos.x.ToString().Replace(",", "."));
        form.AddField("playerPosY", playerPos.y.ToString().Replace(",", "."));
        form.AddField("playerPosZ", playerPos.z.ToString().Replace(",", "."));
        form.AddField("enemyName", enemyName);
        form.AddField("enemyPosX", enemyPos.x.ToString().Replace(",", "."));
        form.AddField("enemyPosY", enemyPos.y.ToString().Replace(",", "."));
        form.AddField("enemyPosZ", enemyPos.z.ToString().Replace(",", "."));
        form.AddField("gameTime", gameTime);

        data.SetData(form, php);

        StartCoroutine(SendUpload(data));
    }
    IEnumerator GetData(string php)
    {
        using(UnityWebRequest www = UnityWebRequest.Get("https://citmalumnes.upc.es/~carlesgdlm/" + php))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                lastQuery = www.downloadHandler.text;
                Debug.Log(lastQuery);
                if (php == "GetHits.php")
                {
                    hitPoints = lastQuery.Split("*");
                    hitData = new HitData[hitPoints.Length-1];
                    for (int i = 1; i < hitPoints.Length; i++)
                    {
                        hitDataString = hitPoints[i].Split("/");
                        hitData[i - 1] = new HitData();
                        hitData[i - 1].hitNum = int.Parse(hitDataString[0]);
                        hitData[i - 1].playerPosX = float.Parse(hitDataString[1].Replace(".", ","));
                        hitData[i - 1].playerPosY = float.Parse(hitDataString[2].Replace(".", ","));
                        hitData[i - 1].playerPosZ = float.Parse(hitDataString[3].Replace(".", ","));
                        hitData[i - 1].enemyName = hitDataString[4];
                        hitData[i - 1].enemyPosX = float.Parse(hitDataString[5].Replace(".", ","));
                        hitData[i - 1].enemyPosY = float.Parse(hitDataString[6].Replace(".", ","));
                        hitData[i - 1].enemyPosZ = float.Parse(hitDataString[7].Replace(".", ","));
                        hitData[i - 1].gameTime = float.Parse(hitDataString[8].Replace(".", ","));

                        GameObject debugprefab = Instantiate(hitsAndDeathsMarker, new Vector3(hitData[i - 1].playerPosX, hitData[i - 1].playerPosY, hitData[i - 1].playerPosZ), Quaternion.identity, GameObject.Find("Trash").transform);
                        debugprefab.layer = hitLayer;
                        debugprefab.tag = "HitTag";
                        SetColor(debugprefab);
                        
                        allDebugPrefabs.Add(debugprefab);

                        for (int j = 0; j < allDebugPrefabs.Count; j++)
                        {
                            if (j != (allDebugPrefabs.Count - 1))
                                allDebugPrefabs[j].transform.LookAt(allDebugPrefabs[j + 1].transform);
                        }
                    }
                }
                else if(php == "GetDeaths.php")
                {
                    deathPoints = lastQuery.Split("*");
                    deathData = new HitData[deathPoints.Length-1];
                    for (int i = 1; i < deathPoints.Length; i++)
                    {
                        deathDataString = deathPoints[i].Split("/");
                        deathData[i - 1] = new HitData();
                        deathData[i - 1].hitNum = int.Parse(deathDataString[0]);
                        deathData[i - 1].playerPosX = float.Parse(deathDataString[1].Replace(".", ","));
                        deathData[i - 1].playerPosY = float.Parse(deathDataString[2].Replace(".", ","));
                        deathData[i - 1].playerPosZ = float.Parse(deathDataString[3].Replace(".", ","));
                        deathData[i - 1].enemyName = deathDataString[4];
                        deathData[i - 1].enemyPosX = float.Parse(deathDataString[5].Replace(".", ","));
                        deathData[i - 1].enemyPosY = float.Parse(deathDataString[6].Replace(".", ","));
                        deathData[i - 1].enemyPosZ = float.Parse(deathDataString[7].Replace(".", ","));
                        deathData[i - 1].gameTime = float.Parse(deathDataString[8].Replace(".", ","));

                        GameObject debugprefab = Instantiate(hitsAndDeathsMarker, new Vector3(deathData[i - 1].playerPosX, deathData[i - 1].playerPosY, deathData[i - 1].playerPosZ), Quaternion.identity, GameObject.Find("Trash").transform);
                        allDebugPrefabs.Add(debugprefab);

                        for (int j = 0; j < allDebugPrefabs.Count; j++)
                        {
                            if (j != (allDebugPrefabs.Count - 1))
                                allDebugPrefabs[j].transform.LookAt(allDebugPrefabs[j + 1].transform);
                        }
                    }
                }
                else if(php == "GetPosition.php")
                {
                    allPosition = lastQuery.Split("*");
                    posData = new PositionData[allPosition.Length-1];
                    for (int i = 1; i < allPosition.Length; i++)
                    {
                        posPoint = allPosition[i].Split("/");
                        posData[i - 1] = new PositionData();
                        posData[i - 1].playerPosX = float.Parse(posPoint[0].Replace(".", ","));
                        posData[i - 1].playerPosY = float.Parse(posPoint[1].Replace(".", ","));
                        posData[i - 1].playerPosZ = float.Parse(posPoint[2].Replace(".", ","));
                        posData[i - 1].playerForwardX = float.Parse(posPoint[3].Replace(".", ","));
                        posData[i - 1].playerForwardY = float.Parse(posPoint[4].Replace(".", ","));
                        posData[i - 1].playerForwardZ = float.Parse(posPoint[5].Replace(".", ","));
                        posData[i - 1].gameTime = float.Parse(posPoint[6].Replace(".", ","));

                        GameObject debugprefab = Instantiate(positionArrow, new Vector3(posData[i - 1].playerPosX, posData[i - 1].playerPosY, posData[i - 1].playerPosZ), Quaternion.identity, GameObject.Find("Trash").transform);
                        allDebugPrefabs.Add(debugprefab);

                        for(int j = 0; j < allDebugPrefabs.Count; j++)
                        {
                            if(j != (allDebugPrefabs.Count - 1))
                                allDebugPrefabs[j].transform.LookAt(allDebugPrefabs[j+1].transform);
                        }
                    }
                }
            }
        }
    }
    #endregion
    #region PositionTracking
    IEnumerator PosAdded(string php)
    {
        gameTime = Time.time.ToString().Replace(",", ".");
        sendingPos = true;
        yield return new WaitForSeconds(0.1f);//3
        WWWForm formPosiiton = new WWWForm();
        formPosiiton.AddField("playerPosX", player.transform.position.x.ToString().Replace(",", "."));
        formPosiiton.AddField("playerPosY", player.transform.position.y.ToString().Replace(",", "."));
        formPosiiton.AddField("playerPosZ", player.transform.position.z.ToString().Replace(",", "."));
        formPosiiton.AddField("playerFwdX", player.transform.forward.x.ToString().Replace(",", "."));
        formPosiiton.AddField("playerFwdY", player.transform.forward.y.ToString().Replace(",", "."));
        formPosiiton.AddField("playerFwdZ", player.transform.forward.z.ToString().Replace(",", "."));
        formPosiiton.AddField("gameTime", gameTime);

        data.SetData(formPosiiton, php);

        StartCoroutine(SendUpload(data));
        sendingPos = false;
    }
    
    #endregion
    IEnumerator SendUpload(SendData data)
    {
        UnityWebRequest www = UnityWebRequest.Post(url + data.GetPHP(), data.GetForm());

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            lastQuery = www.downloadHandler.text;
            if (data.GetPHP() == "HitTracker.php")
            {
                hitNum = lastQuery;
            }
            else if (data.GetPHP() == "DeathTracker.php")
            {
                hitNum = lastQuery;
            }
            else if (data.GetPHP() == "PositionTracker.php")
            {
                posInfo = lastQuery;
            }
            Debug.Log(lastQuery);
        }
    }
    public void EditorStartHeatMap(string php)
    {
        allDebugPrefabs.Clear();
        StartCoroutine(GetData(php));
        Debug.Log(php);
    }
    public void EditorFinishHeatMap()
    {
        for (int i = 0; i < allDebugPrefabs.Count; i++)
        {
            DestroyImmediate(allDebugPrefabs[i]);
        }
        allDebugPrefabs.Clear();
    }

    public void SetColor(GameObject hitCube)
    {
        foreach (GameObject item in heatBoxes)
        {
            Vector2 distCheck = new Vector2(item.transform.position.x, item.transform.position.z) - new Vector2(hitCube.transform.position.x, hitCube.transform.position.z);
            float dist = distCheck.sqrMagnitude;
            Debug.Log("Distances " + dist);
            if (dist <= allowableDistance)
            {
                // Here's where the color is actually adjusted.
                // It starts blue (cool) by default and moves towards red (hot)
                red += colorAdjuster;
                blue -= colorAdjuster;

                cubeMaterial.color = new Color(red, cubeMaterial.color.g, blue);
            }
        }
    }
}

///////////////PHP////////////////
public class SendData
{
    WWWForm form;
    string php;
    public void SetData(WWWForm _form, string _php)
    {
        form = _form;
        php = _php;
    }
    public WWWForm GetForm() { return form; }
    public string GetPHP() { return php; }
}