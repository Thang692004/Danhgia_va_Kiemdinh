using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class ThemLopHoc : MonoBehaviour
{
    public PanelSwitcher panel;
    public GameObject themHocPhan, trangThemMoi;

    [Header("Input")]
    public TMP_InputField subject, quantity, giaTien, tietHoc;
    public TMP_Dropdown tinChiDropdown;

    [Header("Prefab")]
    public List<Image> columns;
    public GameObject rowPrefab;
    public Transform contentParent;
    public List<string> columnNames = new List<string> {
        "Tên môn học", "Mã môn", "Mã lớp học", "Số tín chỉ", "Giá tiền môn", "Số SV", "Hệ số lớp",
        "Hệ số học phần", "Số tiết học", "Khoa giảng dạy", "Giáo viên đứng lớp", "Kỳ", "Năm"
    };
    // Những cột cần ép kiểu sang số (dựa vào tên trong columnNames)
    private readonly HashSet<string> numericColumns = new HashSet<string> {
    "Số SV", "Số tiết học", "Số tín chỉ", "Giá tiền môn", "Hệ số lớp", "Hệ số học phần"
    };

    public string scriptUrl = "https://script.google.com/macros/s/AKfycby1CItno0zdXLCVDxex8WeFDOVVXWTZPoaSlCelh9UJ5q0AvI2xa8AyM50pEX4rbcBIRA/exec"; // thay bằng URL thực tế

    public void AddClass()
    {
        panel.ShowPanel(themHocPhan);
        trangThemMoi.SetActive(true);

        // ✅ Reset dữ liệu nhập cũ
        subject.text = "";
        quantity.text = "";
        giaTien.text = "";
        tietHoc.text = "";
        tinChiDropdown.value = 0; // hoặc giá trị mặc định bạn muốn

        // ✅ Reset dropdown để hiển thị đúng text
        tinChiDropdown.RefreshShownValue();
    }

    public void Cancel()
    {
        trangThemMoi.SetActive(false); // Hủy thêm lớp học
    }

    public void TaoBangTruoc()
    {
        string monHoc = subject.text.Trim();
        string gia = giaTien.text.Trim();
        string tiet = tietHoc.text.Trim();
        int tinChi = tinChiDropdown.value;
        int soLuongLop;

        if (!int.TryParse(quantity.text, out soLuongLop) || string.IsNullOrEmpty(monHoc))
        {
            Debug.LogError("❌ Dữ liệu không hợp lệ.");
            return;
        }

        string vietTat = LayVietTat(monHoc);
        trangThemMoi.SetActive(false);

        foreach (Transform child in contentParent)
        {
            if (child.CompareTag("CloneRow"))
                Destroy(child.gameObject);
        }

        for (int i = 1; i <= soLuongLop; i++)
        {
            GameObject row = Instantiate(rowPrefab, contentParent);
            row.transform.localScale = Vector3.one;
            row.tag = "CloneRow";

            for (int j = 0; j < columns.Count; j++)
            {
                GameObject cell = Instantiate(columns[j].gameObject, row.transform);
                cell.transform.localScale = Vector3.one;

                if (j == 0) SetText(cell, monHoc);               // Tên môn học
                else if (j == 1) SetText(cell, vietTat);         // Viết tắt
                else if (j == 2) SetText(cell, vietTat + "_" + i); // Mã lớp
                else if (j == 3) SetText(cell, tinChiDropdown.options[tinChiDropdown.value].text); // Tín chỉ
                else if (j == 4) SetText(cell, gia);             // Giá tiền
                else if (j == 5) SetInput(cell, (val) =>         // Số SV
                {
                    if (int.TryParse(val, out int soSV))
                    {
                        TMP_Text hs = FindColumnText(row.transform, 6);
                        if (hs != null)
                            hs.text = LayGiaTriTuongUng(soSV);
                    }
                });
                else if (j == 6) SetText(cell, "");              // Hệ số lớp (auto)
                else if (j == 8) SetText(cell, tiet);            // Số tiết học
            }
        }
    }

    // --- Hàm phụ: Gán text ---
    private void SetText(GameObject cell, string value)
    {
        var text = cell.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
            text.text = value;
    }

    // --- Hàm phụ: Gán input SốSV và bắt sự kiện ---
    private void SetInput(GameObject cell, UnityEngine.Events.UnityAction<string> onChange)
    {
        var input = cell.GetComponentInChildren<TMP_InputField>();
        if (input != null)
        {
            input.text = "";
            input.onValueChanged.RemoveAllListeners();
            input.onValueChanged.AddListener(onChange);
        }
    }

    // --- Tìm TextMeshProUGUI ở cột ---
    private TMP_Text FindColumnText(Transform row, int colIndex)
    {
        if (colIndex < 0 || colIndex >= row.childCount) return null;
        return row.GetChild(colIndex).GetComponentInChildren<TextMeshProUGUI>();
    }


    private string LayVietTat(string input)
    {
        string[] parts = input.Split(' ');
        string result = "";
        foreach (var p in parts)
            if (!string.IsNullOrWhiteSpace(p)) result += char.ToUpper(p[0]);
        return result;
    }

    private string LayGiaTriTuongUng(int soSV)
    {
        if (soSV < 20) return "-0.3";
        if (soSV < 30) return "-0.2";
        if (soSV < 40) return "-0.1";
        if (soSV < 50) return "0.0";
        if (soSV < 60) return "0.1";
        if (soSV < 70) return "0.2";
        if (soSV < 80) return "0.3";
        if (soSV < 90) return "0.4";
        if (soSV < 100) return "0.5";
        return "0.6"; // từ 100 trở lên
    }



    public void GuiTatCaLenGoogleSheet()
    {
        foreach (Transform row in contentParent)
        {
            if (!row.CompareTag("CloneRow")) continue;

            List<string> values = new List<string>();

            for (int i = 0; i < row.childCount; i++)
            {
                Transform cell = row.GetChild(i);

                // Trường hợp 1: TMP_InputField
                TMP_InputField inputField = cell.GetComponentInChildren<TMP_InputField>();
                if (inputField != null)
                {
                    values.Add(inputField.text);
                    continue;
                }

                // Trường hợp 2: TMP_Dropdown
                TMP_Dropdown dropdown = cell.GetComponentInChildren<TMP_Dropdown>();
                if (dropdown != null)
                {
                    values.Add(dropdown.options[dropdown.value].text);
                    continue;
                }

                // Trường hợp 3: TextMeshProUGUI (nội dung hiển thị không chỉnh sửa)
                TextMeshProUGUI textField = cell.GetComponentInChildren<TextMeshProUGUI>();
                if (textField != null)
                {
                    values.Add(textField.text);
                    continue;
                }

                // Trường hợp còn lại: không có gì → đẩy chuỗi rỗng
                values.Add("");
            }

            StartCoroutine(PostRowToGoogleSheets(values));
        }

        trangThemMoi.SetActive(false); // Ẩn panel sau khi gửi
    }


    IEnumerator PostRowToGoogleSheets(List<string> values)
    {
        if (values.Count != columnNames.Count)
        {
            Debug.LogError("⚠️ Số cột không khớp.");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("sheet", "ThongTinMonHoc");
        form.AddField("action", "add");

        for (int i = 0; i < columnNames.Count; i++)
        {
            string colName = columnNames[i];
            string rawValue = values[i].Trim();

            // Nếu là cột số, cố gắng chuyển thành dạng số chuẩn
            if (numericColumns.Contains(colName))
            {
                if (float.TryParse(rawValue, out float f))
                    form.AddField(colName, f.ToString()); // Google Sheet tự hiểu là số
                else
                    form.AddField(colName, "0"); // hoặc bỏ qua / cảnh báo
            }
            else
            {
                form.AddField(colName, rawValue);
            }
        }

        using (UnityWebRequest www = UnityWebRequest.Post(scriptUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError("❌ Lỗi gửi: " + www.error);
            else
                Debug.Log("✅ Gửi thành công!");
        }
    }

}
