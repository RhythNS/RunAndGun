using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class DungeonButton : NetworkBehaviour
{
    [System.Serializable]
    [System.Flags]
    public enum TriggerEntities
    {
        Players = (1 << 0),
        Enemies = (1 << 1),
        Bullets = (1 << 2),
    }

    [SerializeField] [BitMask(typeof(TriggerEntities))] private TriggerEntities entities;

    public bool CanChangeState => locked == false && onButton.Count == 0 && !(pressed && stayPressed);

    public bool Locked { get => locked; set => locked = value; }
    [SerializeField] [SyncVar] private bool locked;

    public bool Pressed => pressed;
    [SerializeField] [SyncVar(hook = nameof(PressedChanged))] private bool pressed = false;
    [SerializeField] [SyncVar] private bool stayPressed = false;

    public event BoolChanged OnPressedChanged;

    [SerializeField] private List<GameObject> onButton = new List<GameObject>();

    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Sprite unpressedSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [HideInInspector] public DungeonButtonGroup group;

    [Server]
    public void SetPressed(bool newPressed, bool invokePressEvents = true)
    {
        if (pressed == newPressed)
            return;

        pressed = newPressed;

        if (invokePressEvents == false)
            return;

        if (group != null)
            group.OnButtonChanged(this, newPressed);
        OnPressedChanged?.Invoke(newPressed);
    }

    [Server]
    public void SetStayPressed(bool newStay)
    {
        stayPressed = newStay;
    }

    private void PressedChanged(bool _, bool newPressed)
    {
        spriteRenderer.sprite = newPressed ? pressedSprite : unpressedSprite;
    }

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnStartClient()
    {
        PressedChanged(Pressed, Pressed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isServer == false)
            return;

        if (ShouldColliderTrigger(other, out bool addToList) == false)
            return;

        if (CanChangeState == true)
            SetPressed(!Pressed);

        if (addToList)
            onButton.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isServer == false)
            return;

        if (onButton.Contains(other.gameObject) == false)
            return;

        onButton.Remove(other.gameObject);
    }

    private bool ShouldColliderTrigger(Collider2D other, out bool addToList)
    {
        addToList = false;
        if (other.TryGetComponent(out Entity entity))
        {
            if ((entity.EntityType == EntityType.Enemy && !entities.HasFlag(TriggerEntities.Enemies)) ||
                (entity.EntityType == EntityType.Player && !entities.HasFlag(TriggerEntities.Players)))
                return false;
            addToList = true;
        }
        else if (other.TryGetComponent(out Bullet bullet))
        {
            if (!entities.HasFlag(TriggerEntities.Bullets))
                return false;
        }
        else
        {
            return false;
        }

        return true;
    }
}
