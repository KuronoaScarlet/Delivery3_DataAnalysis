using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    [RequireComponent(typeof(Collider))]
    public class DeathVolume : MonoBehaviour
    {
        public new AudioSource audio;


        void OnTriggerEnter(Collider other)
        {
            var d = other.GetComponent<Damageable>();
            if (d == null)
                return;

            var msg = new Damageable.DamageMessage()
            {
                amount = d.maxHitPoints,
                damager = this,
                direction = Vector3.up,
                stopCamera = false
            };

            d.ApplyDamage(msg);

            var pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.Die(new Damageable.DamageMessage());
            }
            if (audio != null)
            {
                audio.transform.position = other.transform.position;
                if (!audio.isPlaying)
                    audio.Play();
            }
        }

        void Reset()
        {
            if (LayerMask.LayerToName(gameObject.layer) == "Default")
                gameObject.layer = LayerMask.NameToLayer("Environment");
            var c = GetComponent<Collider>();
            if (c != null)
                c.isTrigger = true;
        }

    }
}
