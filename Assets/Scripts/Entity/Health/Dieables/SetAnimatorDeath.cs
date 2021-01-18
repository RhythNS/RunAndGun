using UnityEngine;

public class SetAnimatorDeath : MonoBehaviour, IDieable
{
    public void Die()
    {
        GetComponent<Animator>().SetTrigger("Died");
    }
}
