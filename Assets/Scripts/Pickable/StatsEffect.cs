/// <summary>
/// Struct to describe how something changes stats for an entity.
/// </summary>
[System.Serializable]
public struct StatsEffect
{
    public int health;
    public int defence;
    public int speed;
    public int luck;
    public int dodge;

    public StatsEffect(int health, int defence, int speed, int luck, int dodge)
    {
        this.health = health;
        this.defence = defence;
        this.speed = speed;
        this.luck = luck;
        this.dodge = dodge;
    }

    public void Add(StatsEffect effect)
    {
        health += effect.health;
        defence += effect.defence;
        speed += effect.speed;
        luck += effect.luck;
        dodge += effect.dodge;
    }
}
