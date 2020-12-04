using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PatternGun", menuName = "Weapons/Pattern Gun")]
public class PatternGun : Weapon
{
    static PatternGun()
    {
        List<Vector2> patternList = new List<Vector2>();

        Vector2 startVector = new Vector2(0.0f, 1.0f);
        for (int i = 0; i < 360; i+= 10)
        {
            patternList.Add(Rotate(startVector, i).normalized);
        }

        patterns = patternList.ToArray();
    }

    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private float aliveTime;

    static Vector2[] patterns;
    int atPattern = 0;

    private void Awake()
    {
        if (patterns != null)
            return;

    }

    public override void Fire(GameObject shooter, Vector3 origin, Vector2 direction)
    {
        if (++atPattern == patterns.Length)
            atPattern = 0;

        direction = patterns[atPattern];

        Instantiate(WeaponDict.Instance.bulletPrefab, origin, Quaternion.identity).Set(shooter, direction * speed, damage, aliveTime);
    }

    public static Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}
