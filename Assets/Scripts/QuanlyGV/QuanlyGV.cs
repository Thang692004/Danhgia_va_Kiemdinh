using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

public class QuanlyGV : MonoBehaviour
{
    public string scriptUrl = "https://script.google.com/macros/s/AKfycbyt9im-_FEtBE6phxykMbZVM7UZZ8idUa2JUnurwe767SNM1CxxXUl80z8DD__TLQZE6Q/exec?sheet=ThongTinGiaoVien";

    [Header("UI")]
    public GameObject contentPanel;
    public GameObject rowPrefab;
    public Transform headerRow;
    public Transform valueCell;
    public List<RectTransform> columnImages = new List<RectTransform>();
    public List<string> columns = new List<string> { "ID", "Tên", "Ngày sinh", "Email", "Số điện thoại", "Bằng cấp", "Khoa", "Hệ số GV", "Salary" };
    public PanelSwitcher panel;
    public GameObject trangthongtincanhan;
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

                foreach (var row in list) //Vòng lặp các hàng
                {
                    GameObject rowObject = Instantiate(rowPrefab, contentPanel.transform); //SinhSinh
                    rowObject.transform.localScale = Vector3.one;
                    rowObject.tag = "CloneRow";

                    for (int i = 0; i < columns.Count; i++)
                    {
                        string nameColumn = columns[i];

                        if (row.TryGetValue(nameColumn, out string value)) //Kiểm tra trong Database có đúng cột không 
                        {
                            // Parse ngày sinh nếu cột là "Ngày sinh"
                            if (nameColumn == "Ngày sinh")
                            {
                                if (System.DateTime.TryParse(value, out System.DateTime date))
                                {
                                    value = date.ToString("dd/MM/yyyy");
                                }
                                else
                                {
                                    Debug.LogWarning("⚠️ Không parse được ngày sinh: " + value);
                                }
                            }

                            GameObject cell = Instantiate(columnImages[i].gameObject, rowObject.transform);
                            cell.transform.localScale = Vector3.one;
                            if (nameColumn == "ID" || nameColumn == "Tên")
                            {
                                cell.GetComponent<Button>().enabled = true;
                                Button btn = cell.GetComponent<Button>();
                                if (btn != null)
                                {
                                    string valueSave = value;
                                    btn.onClick.AddListener(() =>
                                    {
                                        DataGV.selectedValue = valueSave;
                                        DataGV.selectedColumn = nameColumn; // lưu lại toàn bộ row
                                        panel.ShowPanel(trangthongtincanhan);
                                    }
                                    );
                                }
                            }

                            TextMeshProUGUI text = cell.GetComponentInChildren<TextMeshProUGUI>();
                            if (text != null)
                            {
                                text.text = value;
                            }
                            else
                            {
                                Debug.LogWarning("❗ Không tìm thấy TextMeshPro trong cell.");
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
    public void ReloadData()
    {
        // Dọn dẹp các row clone cũ (không xóa header)
        ClearRows();

        // Gọi lại GetData để load mới
        StartCoroutine(GetData());
    }

    void ClearRows()
    {
        foreach (Transform child in contentPanel.transform)
        {
            if (child.CompareTag("CloneRow")) Destroy(child.gameObject); // không xóa header row
            else continue;
            
        }
    }


}

