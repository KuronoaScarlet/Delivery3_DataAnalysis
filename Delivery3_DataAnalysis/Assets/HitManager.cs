using Gamekit3D;
using Gamekit3D.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Gamekit3D.Damageable;

public enum HitType
{
    NONLETHAL,
    LETHAL,
    LAVA,
    NONE,
}
public class HitData
{
    public HitType hitType;
    public Vector3 postion;
    public Vector3 forward;
    public float time;

    public HitData() { }
    public HitData(HitType h, Vector3 pos, Vector3 fw, float t)
    {
        hitType = h;
        postion = pos;
        forward = fw;
        time = t;
    }
}
public class HitManager : MonoBehaviour, IMessageReceiver
{
    public List<HitData> hits = new List<HitData>();
    Damageable m_Damageable;
    private void OnEnable()
    {
        m_Damageable = GameObject.FindObjectOfType<Damageable>();
        m_Damageable.onDamageMessageReceivers.Add(this);

    }
    private void OnDisable()
    {
        m_Damageable.onDamageMessageReceivers.Remove(this);
    }

    public void AddHitNonLethalPlayer(float a, GameObject player)
    {
        hits.Add(new HitData(HitType.NONLETHAL, player.transform.position, player.transform.forward, Time.time));
    }
    public void AddHitNonLethalPlayer(HitDataMono mono)
    {
        hits.Add(mono.data);
    }

    public void OnReceiveMessage(MessageType type, object sender, object msg)
    {
        switch (type)
        {
            case MessageType.DAMAGED:
                hits.Add(new HitData(HitType.NONLETHAL, ((MonoBehaviour)sender).transform.position, ((DamageMessage)msg).direction, Time.time));
                break;
            case MessageType.DEAD:
                hits.Add(new HitData(HitType.LETHAL, ((MonoBehaviour)sender).transform.position, ((DamageMessage)msg).direction, Time.time));
                break;
            default:
                break;
        }
    }
       
}

