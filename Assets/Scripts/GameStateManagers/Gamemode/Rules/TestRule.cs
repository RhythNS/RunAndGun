using UnityEngine;

/// <summary>
/// Test class for testing the gamerule system.
/// </summary>
[CreateAssetMenu(menuName = "Gamemode/Rules/TestRule")]
public class TestRule : GameRule
{
    public override string Description => "This is a test";

    public override void Disable()
    {
        Debug.Log("Disabled the testrule");
    }

    public override void Enable()
    {
        Debug.Log("Enable the testrule");
    }
}
