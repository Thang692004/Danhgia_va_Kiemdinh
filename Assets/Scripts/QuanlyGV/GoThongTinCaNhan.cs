using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

public class GoThongTinCaNhan : MonoBehaviour
{
    public string scriptUrl = "https://script.google.com/macros/s/AKfycbyt9im-_FEtBE6phxykMbZVM7UZZ8idUa2JUnurwe767SNM1CxxXUl80z8DD__TLQZE6Q/exec?sheet=ThongTinGiaoVien";

    [Header("UI")]
    public List<TMP_InputField> input;

    [Header("Cột dữ liệu")]
    public List<string> columns = new List<string>
    {
        "ID", "Tên", "Ngày sinh", "Email", "Số điện thoại", "Bằng cấp", "Khoa", "Hệ số GV", "Salary"
    };

    public static int rowSave;

    void OnEnable() // Gọi lại mỗi khi Panel bật lên
    {
        StartCoroutine(GetData());
    }

    IEnumerator GetData()
    {
        UnityWebRequest www = UnityWebRequest.Get(scriptUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ API Error: " + www.error);
        }
        else
        {
            string jsonText = www.downloadHandler.text;

            try
            {
                var list = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonText);

                // Lấy giá trị người dùng click từ class DataGV
                string columnClicked = DataGV.selectedColumn;
                string valueClicked = DataGV.selectedValue;

                foreach (var row in list)
                {
                    if (row.TryGetValue(columnClicked, out string value) && value == valueClicked)
                    {
                        rowSave = list.IndexOf(row); // lưu lại vị trí dòng đang được chọn

                        for (int i = 0; i < columns.Count && i < input.Count; i++)
                        {
                            string col = columns[i];

                            if (row.TryGetValue(col, out string val))
                            {
                                if (col == "Ngày sinh" && System.DateTime.TryParse(val, out System.DateTime date))
                                {
                                    val = date.ToString("dd/MM/yyyy");
                                }

                                input[i].text = val;
                            }
                            else
                            {
                                input[i].text = "";
                            }
                        }

                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("❗ JSON Parse Error: " + ex.Message);
            }
        }
    }
}
