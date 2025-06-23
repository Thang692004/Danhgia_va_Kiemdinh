using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

public class QuanLyKhoa : MonoBehaviour
{
    public string scriptUrl = "https://script.google.com/macros/s/AKfycby1CItno0zdXLCVDxex8WeFDOVVXWTZPoaSlCelh9UJ5q0AvI2xa8AyM50pEX4rbcBIRA/exec";

    [Header("UI hiển thị")]
    public GameObject contentPanel;
    public GameObject rowPrefab;
    public List<RectTransform> columnImages = new List<RectTransform>();
    public List<string> columns = new List<string> { "Tên khoa", "Tên viết Tắt" };

    [Header("UI thêm mới")]
    public GameObject themmoi;
    public TMP_InputField inputTenKhoa;
    public TextMeshProUGUI textVietTat;
    public Button btnThem;
    public Button btnCancel;

    void Start()
    {
        ReloadData();

        inputTenKhoa.onValueChanged.AddListener(delegate { AutoFillVietTat(); });
        btnThem.onClick.AddListener(GuiKhoaLenGoogleSheet);
        btnCancel.onClick.AddListener(() => themmoi.SetActive(false));
    }

    // --- Hiển thị dữ liệu từ Google Sheets ---
    IEnumerator GetData()
    {
        UnityWebRequest www = UnityWebRequest.Get($"{scriptUrl}?sheet=BangHeSo");
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

    public void AddKhoa()
    {
        themmoi.SetActive(true);
        inputTenKhoa.text = "";
        textVietTat.text = "";
        inputTenKhoa.Select();
        inputTenKhoa.ActivateInputField();
    }

    void AutoFillVietTat()
    {
        string tenKhoa = inputTenKhoa.text.Trim();
        textVietTat.text = LayVietTat(tenKhoa);
    }

    string LayVietTat(string input)
    {
        string[] parts = input.Split(' ');
        string result = "";
        foreach (var p in parts)
        {
            if (!string.IsNullOrWhiteSpace(p))
            {
                result += char.ToUpper(p[0]);
            }
        }
        return result;
    }

    void GuiKhoaLenGoogleSheet()
    {
        string tenKhoa = inputTenKhoa.text.Trim();
        string vietTat = textVietTat.text.Trim();

        if (string.IsNullOrEmpty(tenKhoa) || string.IsNullOrEmpty(vietTat))
        {
            Debug.LogWarning("⚠️ Chưa nhập đủ thông tin.");
            return;
        }

        StartCoroutine(PostKhoa(tenKhoa, vietTat));
    }

    IEnumerator PostKhoa(string tenKhoa, string vietTat)
    {
        WWWForm form = new WWWForm();
        form.AddField("sheet", "BangHeSo");
        form.AddField("action", "add");
        form.AddField("Tên khoa", tenKhoa);
        form.AddField("Tên viết tắt", vietTat);

        UnityWebRequest www = UnityWebRequest.Post(scriptUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Lỗi gửi: " + www.error);
        }
        else
        {
            Debug.Log("✅ Gửi thành công!");
            themmoi.SetActive(false);
            ReloadData();
        }
    }
}
