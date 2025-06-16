using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NamHocDropdownLoader : MonoBehaviour
{
    public TMP_Dropdown namHocDropdown;

    void Start()
    {
        namHocDropdown.ClearOptions(); // Xóa các Option A, B, C có sẵn

        List<string> namHocOptions = new List<string>
        {
            "Năm học 2014-2015",
            "Năm học 2015-2016",
            "Năm học 2016-2017",
            "Năm học 2017-2018",
            "Năm học 2018-2019",
            "Năm học 2019-2020",
            "Năm học 2020-2021",
            "Năm học 2021-2022",
            "Năm học 2022-2023",
            "Năm học 2023-2024",
            "Năm học 2024-2025",
            "Năm học 2025-2026",
        };

        namHocDropdown.AddOptions(namHocOptions); // Thêm các năm học mới
    }
}
