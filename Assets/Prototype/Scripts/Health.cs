using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int startingHealth;
    private bool dead = false;

    public int CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = startingHealth;
    }

    public void Damage(int damage)
    {
        if (dead == true)
            return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, startingHealth);
        if (CurrentHealth == 0)
        {
            GetComponent<IDieable>().Die();
            dead = true;
        }
    }

}
