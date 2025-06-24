using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

public class HeSoGiaoVien : MonoBehaviour
{
    public string scriptUrl = "https://script.google.com/macros/s/AKfycby1CItno0zdXLCVDxex8WeFDOVVXWTZPoaSlCelh9UJ5q0AvI2xa8AyM50pEX4rbcBIRA/exec?sheet=ThongTinGiaoVien";

    [Header("UI")]
    public GameObject contentPanel;
    public GameObject rowPrefab;
    public List<RectTransform> columnImages = new List<RectTransform>();
    public List<string> columns = new List<string> { "Tên", "Bằng cấp", "Hệ số GV" };

    void Start()
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

                foreach (var row in list)
                {
                    GameObject rowObject = Instantiate(rowPrefab, contentPanel.transform);
                    rowObject.transform.localScale = Vector3.one;
                    rowObject.tag = "CloneRow";

                    for (int i = 0; i < columns.Count; i++)
                    {
                        string nameColumn = columns[i];

                        if (row.TryGetValue(nameColumn, out string value))
                        {
                            GameObject cell = Instantiate(columnImages[i].gameObject, rowObject.transform);
                            cell.transform.localScale = Vector3.one;

                            TextMeshProUGUI text = cell.GetComponentInChildren<TextMeshProUGUI>();
                            if (text != null)
                            {
                                text.text = value;
                            }
                            else
                            {
                                Debug.LogWarning("❗ Không tìm thấy TMP trong cell.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"⚠️ Không có dữ liệu cho cột: {nameColumn}");
                        }
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
