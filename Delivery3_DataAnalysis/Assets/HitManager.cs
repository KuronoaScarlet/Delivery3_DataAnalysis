using Gamekit3D;
using Gamekit3D.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using static Gamekit3D.Damageable;

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
    public HitType hitType;
    public string damager;
    public Vector3 postion;
    public float time;

    public HitData() { }
    public HitData(HitType h, string d, Vector3 pos, float t)
    {
        hitType = h;
        damager = d;
        postion = pos;
        time = t;
    }
}

public class HitManager : MonoBehaviour, IMessageReceiver
{
    public string url = "https://citmalumnes.upc.es/~carlesgdlm/";
    public Vector3 playerPos, enemyPos;
    public string enemyName, hitNum, gameTime;
    Damageable m_Damageable;
    public string lastQuery;

    SendData data = new SendData();
    private void OnEnable()
    {
        m_Damageable = GameObject.Find("Ellen").GetComponent<Damageable>();
        m_Damageable.onDamageMessageReceivers.Add(this);

    }
    private void OnDisable()
    {
        m_Damageable.onDamageMessageReceivers.Remove(this);
    }
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
            Debug.Log(lastQuery);
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

