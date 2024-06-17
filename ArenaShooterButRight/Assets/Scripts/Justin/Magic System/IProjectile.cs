using UnityEngine;

public interface IProjectile
{
    void Launch(Vector3 _direction);
    void OnHit();
}
