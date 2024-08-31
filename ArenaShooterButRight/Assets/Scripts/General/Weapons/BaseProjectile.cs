using General.Player;
using System;
using UnityEngine;

namespace General.Weapons
{
    public class BaseProjectile : MonoBehaviour
    {
        protected float damage;
        protected float projectileSpeed;
        protected float range;
        protected Vector3 startPosition;
        protected Vector3 direction = Vector3.forward;
        protected Vector3 distanceTravelled;

        protected virtual void Start()
        {
            startPosition = transform.position;
            range = 50f;
        }

        protected virtual void Update()
        {
            LaunchProjectile();
            //distanceTravelled += direction * projectileSpeed * Time.deltaTime;
            //Debug.Log(distanceTravelled.z);
            //if (HasReachedMaxRange())
            //{
            //    Destroy(gameObject);
            //}
        }

        protected virtual void LaunchProjectile()
        {
            transform.Translate(direction * projectileSpeed * Time.deltaTime);
        }
        protected bool HasReachedMaxRange()
        {
            if (distanceTravelled.z >= range) 
            {
                Debug.Log("range reached");
                return true;
            }
            return false;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            Destroy(gameObject);
        }
    }
}
