using UnityEngine;

/// <summary>
/// Sets an trigger for an animation on death.
/// </summary>
public class SetAnimatorDeath : MonoBehaviour, IDieable
{
    public void Die()
    {
        GetComponent<Animator>().SetTrigger("Died");
    }
}
