using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

/// <summary>
/// A horizontal-drag touch control feeding a single float (-1..1) to the bound control.
/// Unlike Unity's built-in OnScreenStick (full 2D circular drag, driving a Vector2), this is
/// constrained to the X axis and is meant to sit on a control that covers the whole touch
/// area it should respond to - typically the full screen, with a separate small stick graphic
/// (assigned to <see cref="knobVisual"/>) acting as a visual indicator rather than the actual
/// touch target, so a drag can start anywhere, not just precisely on the visible knob.
/// </summary>
[AddComponentMenu("Input/On-Screen Horizontal Stick")]
public class OnScreenHorizontalStick : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] [Min(0)] private float movementRange = 100f;

    [InputControl(layout = "Axis")]
    [SerializeField] private string m_ControlPath;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    [Tooltip("Optional. Slides left/right within movementRange to visualize the current drag. Purely cosmetic - this component itself doesn't move.")]
    public RectTransform knobVisual;

    private RectTransform canvasRectTransform;
    private Vector2 knobRestPos;
    private Vector2 pointerDownLocalPos;

    // Deliberately not overriding OnEnable: OnScreenControl.OnEnable() (protected virtual) is what
    // calls SetupInputControl() to resolve controlPath into an actual InputControl. Declaring a
    // non-override OnEnable here would hide it instead of running alongside it, since Unity invokes
    // whichever OnEnable is most-derived, silently breaking SendValueToControl.
    private void Start()
    {
        if (transform.parent != null)
            canvasRectTransform = transform.parent.GetComponentInParent<RectTransform>();

        if (knobVisual != null)
            knobRestPos = knobVisual.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canvasRectTransform == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out pointerDownLocalPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvasRectTransform == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out var localPos);

        float deltaX = Mathf.Clamp(localPos.x - pointerDownLocalPos.x, -movementRange, movementRange);

        if (knobVisual != null)
            knobVisual.anchoredPosition = new Vector2(knobRestPos.x + deltaX, knobRestPos.y);

        SendValueToControl(deltaX / movementRange);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (knobVisual != null)
            knobVisual.anchoredPosition = knobRestPos;

        SendValueToControl(0f);
    }
}
