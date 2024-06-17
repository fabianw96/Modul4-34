using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour, IProjectile
{
    public float speed;
    protected Vector3 direction;

    public void Launch(Vector3 _direction)
    {
        direction = _direction;
        //Implement Launch Logic
    }


    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnHit();
        // Implement collision logic
    }
    public abstract void OnHit();
}
