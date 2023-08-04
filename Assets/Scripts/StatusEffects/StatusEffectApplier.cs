using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
  
/// <summary>
/// This struct is a data container that describes a distinct attempt to apply a Status Effect to a unit.
/// </summary>
[Serializable]
public struct StatusEffectApplier
{
    public string name;
    public int duration;
    public int chance;
    public bool resistible;
}