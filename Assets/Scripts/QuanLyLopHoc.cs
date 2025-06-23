using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;

public class QuanLyLopHoc : MonoBehaviour
{
    public string scriptUrl = "https://script.google.com/macros/s/AKfycby1CItno0zdXLCVDxex8WeFDOVVXWTZPoaSlCelh9UJ5q0AvI2xa8AyM50pEX4rbcBIRA/exec?sheet=ThongTinMonHoc";

    [Header("UI")]
    public GameObject contentPanel;
    public GameObject rowPrefab;
    public List<RectTransform> columnImages = new List<RectTransform>(); // Prefab cho từng cột

    // Danh sách tên cột hiển thị đúng thứ tự
    private List<string> expectedColumns = new List<string> {
        "Tên môn học", "Mã môn", "Mã lớp học", "Số tín chỉ", "Giá tiền môn", "Số SV",
        "Hệ số lớp", "Hệ số học phần", "Số tiết học","Số tiết quy đổi", "Khoa giảng dạy",
        "Giáo viên đứng lớp", "Kỳ", "Năm", "Tiền dạy"
    };

    void Start()
    {
        Debug.Log("🚀 QuanLyLopHoc đã khởi động");
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
                Debug.Log("📥 JSON raw trả về:\n" + jsonText);
                var list = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonText);
                Debug.Log("✅ Tổng số dòng nhận được: " + list.Count);

                foreach (var row in list)
                {
                    GameObject rowObject = Instantiate(rowPrefab, contentPanel.transform);
                    rowObject.transform.localScale = Vector3.one;
                    rowObject.tag = "CloneRow";

                    for (int i = 0; i < expectedColumns.Count && i < columnImages.Count; i++)
                    {
                        string columnName = expectedColumns[i];
                        string value = row.ContainsKey(columnName) ? row[columnName] : "";

                        GameObject cell = Instantiate(columnImages[i].gameObject, rowObject.transform);
                        cell.transform.localScale = Vector3.one;

                        TextMeshProUGUI text = cell.GetComponentInChildren<TextMeshProUGUI>();
                        if (text != null)
                            text.text = value;
                        else
                            Debug.LogWarning($"⚠️ Không tìm thấy TMP trong cột {columnName}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("❗ JSON Parse Error: " + ex.Message);
                Debug.Log("❗ JSON Raw:\n" + www.downloadHandler.text);
            }
        }
    }

    public void ReloadData()
    {
        ClearRows();
        StartCoroutine(GetData());
    }

    void ClearRows()
    {
        foreach (Transform child in contentPanel.transform)
        {
            if (child.CompareTag("CloneRow"))
                Destroy(child.gameObject);
        }
    }
}
