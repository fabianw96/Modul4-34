using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    public float speed;
    protected Vector3 direction;
    [SerializeField] private VisualEffect vfx;

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
        //OnHit();
        // Implement collision logic
    }
    //public void OnHit();
}
