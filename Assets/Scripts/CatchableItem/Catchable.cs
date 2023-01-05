using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICatchable
{
    public bool IsCatching { get; set; }

    public void Catch(GameObject tool);

    public void Release();
}
