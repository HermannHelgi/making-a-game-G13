using UnityEngine;

[CreateAssetMenu()]
public class ItemAnimationsObject : ScriptableObject
{
    [Header("How the character will hold these items.")]
    [SerializeField] private AnimationClip m_holdingLeft = null;
    public AnimationClip HoldingLeft { get { return m_holdingLeft; } }

    [SerializeField] private AnimationClip m_holdingRight = null;
    public AnimationClip HoldingRight { get { return m_holdingRight; } }

    [Header("If Crouching poses are null, will use standing poses.")]
    [SerializeField] private AnimationClip m_crouchingHoldingLeft = null;
    public AnimationClip CrouchingHoldingLeft
    {
        get
        {
            if (m_crouchingHoldingLeft != null) { return m_crouchingHoldingLeft; }
            else { return m_holdingLeft; }
        }
    }

    [SerializeField] private AnimationClip m_crouchingHoldingRight = null;
    public AnimationClip CrouchingHoldingRight
    {
        get
        {
            if (m_crouchingHoldingRight != null) { return m_crouchingHoldingRight; }
            else { return m_holdingRight; }
        }
    }

    [Space(20)]
    [Header("Interaction animations with these items.")]
    [Header("Only set start animation if you don't need the animation to loop.")]
    [Space(10)]
    [Header("Standing")]
    [SerializeField] private AnimationClip m_interactionLeftStart = null;
    public AnimationClip InteractionLeftStart { get { return m_interactionLeftStart; } }
    [SerializeField] private AnimationClip m_interactionLeftLoop = null;
    public AnimationClip InteractionLeftLoop { get { return m_interactionLeftLoop; } }
    [SerializeField] private AnimationClip m_interactionLeftEnd = null;
    public AnimationClip InteractionLeftEnd { get { return m_interactionLeftEnd; } }
    [SerializeField] private float m_interactionLeftLoopTime = 1;
    public float InteractionLeftLoopTime { get { return m_interactionLeftLoopTime; } }

    [Space(10)]
    [SerializeField] private AnimationClip m_interactionRightStart = null;
    public AnimationClip InteractionRightStart { get { return m_interactionRightStart; } }
    [SerializeField] private AnimationClip m_interactionRightLoop = null;
    public AnimationClip InteractionRightLoop { get { return m_interactionRightLoop; } }
    [SerializeField] private AnimationClip m_interactionRightEnd = null;
    public AnimationClip InteractionRightEnd { get { return m_interactionRightEnd; } }
    [SerializeField] private float m_interactionRightLoopTime = 1;
    public float InteractionRightLoopTime { get { return m_interactionRightLoopTime; } }

    [Space(20)]
    [Header("Crouching")]
    [SerializeField] private bool m_useStandingInteractionAnimations = false;
    [SerializeField] private AnimationClip m_crouchingInteractionLeftStart = null;
    public AnimationClip CrouchingInteractionLeftStart
    {
        get
        {
            if (!m_useStandingInteractionAnimations) { return m_crouchingInteractionLeftStart; }
            else { return m_interactionLeftStart; }
        }
    }
    [SerializeField] private AnimationClip m_crouchingInteractionLeftLoop = null;
    public AnimationClip CrouchingInteractionLeftLoop
    {
        get
        {
            if (!m_useStandingInteractionAnimations) { return m_crouchingInteractionLeftLoop; }
            else { return m_interactionLeftLoop; }
        }
    }
    [SerializeField] private AnimationClip m_crouchingInteractionLeftEnd = null;
    public AnimationClip CrouchingInteractionLeftEnd
    {
        get
        {
            if (!m_useStandingInteractionAnimations) { return m_crouchingInteractionLeftEnd; }
            else { return m_interactionLeftEnd; }
        }
    }
    [SerializeField] private float m_crouchingInteractionLeftLoopTime = 1;
    public float CrouchingInteractionLeftLoopTime { get { return m_crouchingInteractionLeftLoopTime; } }

    [Space(10)]
    [SerializeField] private AnimationClip m_crouchingInteractionRightStart = null;
    public AnimationClip CrouchingInteractionRightStart
    {
        get
        {
            if (!m_useStandingInteractionAnimations) { return m_crouchingInteractionRightStart; }
            else { return m_interactionRightStart; }
        }
    }
    [SerializeField] private AnimationClip m_crouchingInteractionRightLoop = null;
    public AnimationClip CrouchingInteractionRightLoop
    {
        get
        {
            if (!m_useStandingInteractionAnimations) { return m_crouchingInteractionRightLoop; }
            else { return m_interactionRightLoop; }
        }
    }
    [SerializeField] private AnimationClip m_crouchingInteractionRightEnd = null;
    public AnimationClip CrouchingInteractionRightEnd
    {
        get
        {
            if (!m_useStandingInteractionAnimations) { return m_crouchingInteractionRightEnd; }
            else { return m_interactionRightEnd; }
        }
    }
    [SerializeField] private float m_crouchingInteractionRightLoopTime = 1;
    public float CrouchingInteractionRightLoopTime { get { return m_crouchingInteractionRightLoopTime; } }

    [Space(20)]
    [Header("Equipment animations with these items.")]
    [SerializeField] private AnimationClip m_equipLeft = null;
    public AnimationClip EquipLeft { get { return m_equipLeft; } }

    [SerializeField] private AnimationClip m_equipRight = null;
    public AnimationClip EquipRight { get { return m_equipRight; } }

    [SerializeField] private AnimationClip m_unEquipLeft = null;
    public AnimationClip UnEquipLeft { get { return m_unEquipLeft; } }

    [SerializeField] private AnimationClip m_unEquipRight = null;
    public AnimationClip UnEquipRight { get { return m_unEquipRight; } }

    [Space(20)]
    [Header("Does this item use the weapon movement animations?")]
    [SerializeField] private bool m_isWeapon = false;
    public bool IsWeapon { get { return m_isWeapon; } }
}