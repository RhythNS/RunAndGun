using FMOD.Studio;
using FMODUnity;
using Mirror;
using UnityEngine;

public class FMODTest : NetworkBehaviour
{
    [BankRef]
    public string[] bankNames;

    [EventRef]
    public string eventName;
    public string parameterName;
    EventInstance eventInstance;
    PARAMETER_ID eventParameter;

    [Range(0.0f, 1.0f)]
    public float paraValue;

    public override void OnStartClient()
    {
        foreach (string bankName in bankNames)
        {
            RuntimeManager.LoadBank(bankName);
        }
        eventInstance = RuntimeManager.CreateInstance(eventName);
        eventInstance.start();

        RuntimeManager.AttachInstanceToGameObject(eventInstance, transform, GetComponent<Rigidbody2D>());

        eventInstance.getDescription(out EventDescription eventDescription);
        eventDescription.getParameterDescriptionByName(parameterName, out PARAMETER_DESCRIPTION parameterDesc);
        eventParameter = parameterDesc.id;
    }

    private void Update()
    {
        eventInstance.setParameterByID(eventParameter, paraValue);
    }

    private void OnDestroy()
    {
        eventInstance.release();
    }
}
