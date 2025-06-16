using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class KiHocDropdownLoader : MonoBehaviour
{
    public TMP_Dropdown kiHocDropdown;

    void Start()
    {
        List<string> danhSachKiHoc = new List<string>()
        {
            "Kì I",
            "Kì II",
            "Kì III",
            "Kì IV",
            "Kì V",
            "Kì Học hè"
        };

        kiHocDropdown.ClearOptions();
        kiHocDropdown.AddOptions(danhSachKiHoc);
    }
}
