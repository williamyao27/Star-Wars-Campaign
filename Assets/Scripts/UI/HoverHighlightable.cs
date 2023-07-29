using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides self-highlighting behaviors for other UI elements to inherit. Note that such elements require a "Highlight" object in their prefab.
/// </summary>
public class HoverHighlightable : MonoBehaviour
{
    [SerializeField] private GameObject hoverHighlight;  

    public virtual void OnMouseEnter()
    {
        hoverHighlight.SetActive(true);
    }

    public virtual void OnMouseExit()
    {
        hoverHighlight.SetActive(false);
    }
}
