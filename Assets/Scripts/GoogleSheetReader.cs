using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

public class GoogleSheetReader : MonoBehaviour
{
    public string scriptUrl = "https://script.google.com/macros/s/AKfycbzpfHVm73-20Bowrtws3WOxpNRcg_1JtMqp4xo5Vd4lyjJ6RjzDk8LGzvHFZ9CleQ0eTg/exec";

    [Header("UI")]
    public GameObject contentPanel;      // Content trong ScrollView
    public GameObject rowPrefab;         // Prefab mỗi hàng (có Horizontal Layout Group)
    public GameObject cellPrefab;        // Prefab mỗi ô (TextMeshProUGUI)

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
                int rowIndex = 10;
                var list = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonText);

                foreach (var row in list)
                {
                    // Tạo 1 hàng
                  GameObject rowObject = Instantiate(rowPrefab, contentPanel.transform);
                  RectTransform rt = rowObject.GetComponent<RectTransform>();

                  // Set vị trí (localPosition) và kích thước (sizeDelta)
                    rt.anchoredPosition = new Vector2(0, -rowIndex * 120); // ví dụ cách 120 mỗi dòng
                    rt.sizeDelta = new Vector2(1800, 200); // Width: 800, Height: 100

                    foreach (var pair in row)
                    {
                        GameObject cell = Instantiate(cellPrefab, rowObject.transform);
                        RectTransform ce = cell.GetComponent<RectTransform>();

                        ce.sizeDelta = new Vector2(200, 200);   
                        // Tìm component TextMeshProUGUI trong con của cell
                        TextMeshProUGUI text = cell.GetComponentInChildren<TextMeshProUGUI>();
                        text.text = pair.Value;
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
