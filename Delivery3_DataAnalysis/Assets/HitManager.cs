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
[System.Serializable]
public class HitData
{
    public HitType hitType;
    public string damager;
    public Vector3 postion;
    public Vector3 forward;
    public float time;

    public HitData() { }
    public HitData(HitType h, string d, Vector3 pos, Vector3 fw, float t)
    {
        hitType = h;
        damager = d;
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
        m_Damageable = GameObject.Find("Ellen").GetComponent<Damageable>();
        m_Damageable.onDamageMessageReceivers.Add(this);

    }
    private void OnDisable()
    {
        m_Damageable.onDamageMessageReceivers.Remove(this);
    }
    public void OnReceiveMessage(MessageType type, object sender, object msg)
    {
        switch (type)
        {
            case MessageType.DAMAGED:
                hits.Add(new HitData(HitType.NONLETHAL, ((DamageMessage)msg).damager.gameObject.transform.root.name ,((MonoBehaviour)sender).transform.position, ((DamageMessage)msg).direction, Time.time));
                break;
            case MessageType.DEAD:
                hits.Add(new HitData(HitType.LETHAL, ((DamageMessage)msg).damager.gameObject.transform.root.name, ((MonoBehaviour)sender).transform.position, ((DamageMessage)msg).direction, Time.time));
                break;
            default:
                break;
        }
    }
       
}

