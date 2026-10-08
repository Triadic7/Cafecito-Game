using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// The object thats displayed on hover.
    /// </summary>
    [SerializeField] private GameObject hoverableObject;

    [SerializeField] UnityEvent onHover;

    protected event Action<GameObject> OnHoverObject;

    private bool isHovered = false;

    /// <summary>
    /// Sets the hoverable gameobject. 
    /// </summary>
    /// <param name="gameObject">The gameobject.</param>
    public void SetHoverable(GameObject gameObject)
    {
        this.hoverableObject = gameObject;
    }

    /// <summary>
    /// Sets the hoverable to true.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovered)
        {
            isHovered = true;
            this.hoverableObject.SetActive(true);
            onHover?.Invoke();
            OnHoverObject?.Invoke(hoverableObject);
            Debug.Log($"Entered hoverable {gameObject.name}");
        }
    }

    /// <summary>
    /// Sets the hoverable to false.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        this.hoverableObject.SetActive(false);
        Debug.Log($"Exited hoverable {gameObject.name}");
    }
}
