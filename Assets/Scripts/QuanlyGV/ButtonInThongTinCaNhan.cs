using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class ButtonInThongTinCaNhan : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField[] inputFields;
    public Button btnChinhSua;
    public Button btnUpdate;
    public Button btnAdd;
    public Button btnDelete;

    [Header("Cột dữ liệu")]
    public List<string> columns = new List<string>
    {
        "ID", "Tên", "Ngày sinh", "Email", "Số điện thoại", "Bằng cấp", "Khoa", "Hệ số GV"
    };

    [Header("Dữ liệu thêm ")]
    public TMP_InputField[] addValues;

    private string scriptUrl => GameObject.FindObjectOfType<GoThongTinCaNhan>().scriptUrl;
    private int rowSave => GoThongTinCaNhan.rowSave;

    private bool isEditable = false;

    void Start()
    {
        SetInputsEditable(false);
        btnChinhSua.onClick.AddListener(OnEditButtonClicked);
        btnUpdate.onClick.AddListener(OnUpdateButtonClick);
        btnAdd.onClick.AddListener(OnAddButtonClick);
        btnDelete.onClick.AddListener(OnDeleteButtonClick);
    }

    void OnEditButtonClicked()
    {
        isEditable = !isEditable;
        SetInputsEditable(isEditable);
        btnChinhSua.GetComponentInChildren<TMP_Text>().text = isEditable ? "Khoá lại" : "Chỉnh sửa";
    }

    void SetInputsEditable(bool canEdit)
    {
        foreach (var input in inputFields)
        {
            input.interactable = canEdit;
        }
    }

    void OnUpdateButtonClick()
    {
        StartCoroutine(UpdateData());
    }

    IEnumerator UpdateData()
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "update");
        form.AddField("sheet", "ThongTinGiaoVien");
        form.AddField("row", (rowSave + 2).ToString());

        for (int i = 0; i < columns.Count && i < inputFields.Length; i++)
        {
            form.AddField(columns[i], inputFields[i].text);
        }

        UnityWebRequest www = UnityWebRequest.Post(scriptUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Update Error: " + www.error);
        }
        else
        {
            Debug.Log("✅ Cập nhật thành công: " + www.downloadHandler.text);
        }
    }

    void OnAddButtonClick()
    {
        StartCoroutine(AddData());
    }

    IEnumerator AddData()
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "add");
        form.AddField("sheet", "ThongTinGiaoVien");

        for (int i = 0; i < columns.Count && i < addValues.Length; i++)
        {
            form.AddField(columns[i], addValues[i].text);
        }

        UnityWebRequest www = UnityWebRequest.Post(scriptUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Add Error: " + www.error);
        }
        else
        {
            Debug.Log("✅ Thêm mới thành công: " + www.downloadHandler.text);
        }
    }

    void OnDeleteButtonClick()
    {
        StartCoroutine(DeleteData());
    }

    IEnumerator DeleteData()
    {
        string id = inputFields[0].text.Trim(); // Giả sử ID là ô đầu tiên

        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("⚠️ Vui lòng nhập ID để xoá.");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("action", "delete");
        form.AddField("sheet", "ThongTinGiaoVien");
        form.AddField("ID", id);

        UnityWebRequest www = UnityWebRequest.Post(scriptUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Delete Error: " + www.error);
        }
        else
        {
            Debug.Log("✅ Xoá thành công: " + www.downloadHandler.text);
            ClearInputs();
        }
    }

    void ClearInputs()
    {
        foreach (var input in inputFields)
        {
            input.text = "";
        }
    }
}
