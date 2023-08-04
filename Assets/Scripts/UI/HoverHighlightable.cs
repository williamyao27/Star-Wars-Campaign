using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides self-highlighting behaviors for other UI elements to inherit. Note that such elements require a "Highlight" object in their prefab.
/// </summary>
public class HoverHighlightable : MonoBehaviour
{
    [SerializeField] private GameObject hoverHighlight;
    protected bool hoverEnabled = true;

    public virtual void OnMouseEnter()
    {
        hoverHighlight.SetActive(hoverEnabled);
    }

    public virtual void OnMouseExit()
    {
        hoverHighlight.SetActive(false);
    }
}
