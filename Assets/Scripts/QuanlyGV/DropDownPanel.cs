using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;

public class DropDownPanel : MonoBehaviour
{
    [Header("URL")]
    public string scriptURL = "https://script.google.com/macros/s/AKfycbyt9im-_FEtBE6phxykMbZVM7UZZ8idUa2JUnurwe767SNM1CxxXUl80z8DD__TLQZE6Q/exec";

    [Header("UI")]
    public TMP_Dropdown yearDropdown;
    public TMP_InputField ky1Input, ky2Input;
    public TMP_InputField luong1Input, luong2Input;

    private List<string> yearList = new List<string>();
    private Dictionary<string, List<Dictionary<string, object>>> fullDataByYear = new Dictionary<string, List<Dictionary<string, object>>>();

    void OnEnable()
    {
        StartCoroutine(GetAllTietDayData());
    }

    public class ResponseWrapper
    {
        public string status;
        public List<Dictionary<string, object>> data;
    }

    IEnumerator GetAllTietDayData()
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "getAll");
        form.AddField("sheet", "TietDay");

        UnityWebRequest www = UnityWebRequest.Post(scriptURL, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Lỗi lấy dữ liệu: " + www.error);
            yield break;
        }

        try
        {
            var rows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(www.downloadHandler.text);

            fullDataByYear.Clear();
            yearList.Clear();

            foreach (var row in rows)
            {
                if (!row.ContainsKey("Năm")) continue;

                string year = row["Năm"].ToString();

                if (!fullDataByYear.ContainsKey(year))
                {
                    fullDataByYear[year] = new List<Dictionary<string, object>>();
                    yearList.Add(year);
                }
                fullDataByYear[year].Add(row);
            }

            yearDropdown.ClearOptions();
            yearList.Sort();
            yearDropdown.AddOptions(yearList);

            yearDropdown.onValueChanged.RemoveAllListeners();
            yearDropdown.onValueChanged.AddListener(delegate { OnYearChanged(); });

            OnYearChanged(); // Gọi lần đầu
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❗ JSON Parse lỗi (năm): " + ex.Message);
        }
    }


    void OnYearChanged()
    {
        string selectedYear = yearDropdown.options[yearDropdown.value].text;
        string selectedCol = DataGV.selectedColumn;
        string selectedVal = DataGV.selectedValue;

        if (fullDataByYear.ContainsKey(selectedYear))
        {
            foreach (var row in fullDataByYear[selectedYear])
            {
                if (row.ContainsKey(selectedCol) && row[selectedCol].ToString() == selectedVal)
                {
                    ky1Input.text = row.ContainsKey("Kỳ I (TietDay)") ? row["Kỳ I (TietDay)"].ToString() : "";
                    ky2Input.text = row.ContainsKey("Kỳ II (TietDay)") ? row["Kỳ II (TietDay)"].ToString() : "";

                    luong1Input.text = row.ContainsKey("Kỳ I (Luong)") ? row["Kỳ I (Luong)"].ToString() : "";
                    luong2Input.text = row.ContainsKey("Kỳ II (Luong)") ? row["Kỳ II (Luong)"].ToString() : "";

                    return;
                }
            }
        }

        // Không tìm thấy
        ky1Input.text = "";
        ky2Input.text = "";
        luong1Input.text = "";
        luong2Input.text = "";
    }

}
