using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenuGroup : MonoBehaviour
{
    public GameObject groupToToggle;

    public void ToggleGroup()
    {
        if (groupToToggle != null)
        {
            groupToToggle.SetActive(!groupToToggle.activeSelf);
        }
    }
}
