using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    public float ShootPerSecond => shootPerSecond;
    [SerializeField] private float shootPerSecond;
    

    public abstract void Fire(GameObject shooter, Vector3 origin, Vector2 direction);
}
