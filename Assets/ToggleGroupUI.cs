using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGroupUI : MonoBehaviour
{
    public GameObject groupChildren;

    public void Toggle()
    {
        if (groupChildren != null)
        {
            groupChildren.SetActive(!groupChildren.activeSelf);
        }
    }
}

