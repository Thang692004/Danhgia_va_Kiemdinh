using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DinhMuc : MonoBehaviour
{
    [Header("UI References")]
    public GameObject contentPanel;
    public GameObject rowPrefab;
    public List<RectTransform> columnImages;
    public GameObject themDinhMuc; // Panel nhập dữ liệu
    public TMP_InputField year, cost;
    public Button okButton, saveButton;

    private GameObject editingRow = null;

    void Start()
    {
        themDinhMuc.SetActive(false);
        okButton.onClick.AddListener(OnOkClicked);
        saveButton.onClick.AddListener(OnSaveClicked);
    }

    public void AddDinhMuc() // Gọi khi ấn nút Add
    {
        year.text = "";
        cost.text = "";
        themDinhMuc.SetActive(true);
        okButton.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(false);
        editingRow = null;
    }

    void OnOkClicked()
    {
        GameObject rowObject = Instantiate(rowPrefab, contentPanel.transform);
        rowObject.transform.localScale = Vector3.one;
        rowObject.tag = "CloneRow";

        for (int i = 0; i < columnImages.Count; i++)
        {
            GameObject cell = Instantiate(columnImages[i].gameObject, rowObject.transform);
            cell.transform.localScale = Vector3.one;

            TextMeshProUGUI text = cell.GetComponentInChildren<TextMeshProUGUI>();

            if (i == 0)
            {
                text.text = year.text;
                Button btn = cell.GetComponentInChildren<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() => OnEditRowClicked(rowObject));
                }
            }
            else if (i == 1)
            {
                text.text = cost.text;
            }
        }

        themDinhMuc.SetActive(false);
    }

    void OnEditRowClicked(GameObject row)
    {
        editingRow = row;

        TMP_Text yearText = row.transform.GetChild(0).GetComponentInChildren<TMP_Text>();
        TMP_Text costText = row.transform.GetChild(1).GetComponentInChildren<TMP_Text>();

        year.text = yearText.text;
        cost.text = costText.text;

        themDinhMuc.SetActive(true);
        okButton.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(true);
    }

    void OnSaveClicked()
    {
        if (editingRow == null) return;

        for (int i = 0; i < editingRow.transform.childCount; i++)
        {
            TMP_Text text = editingRow.transform.GetChild(i).GetComponentInChildren<TMP_Text>();

            if (i == 0)
            {
                text.text = year.text;
            }
            else if (i == 1)
            {
                text.text = cost.text;
            }
        }

        themDinhMuc.SetActive(false);
        editingRow = null;
    }

    public void OnDeleteClick()
    {
        if (editingRow == null) return;
        Destroy(editingRow);
        themDinhMuc.SetActive(false);
        editingRow = null;
    }

    public void Cancel()
    {
        themDinhMuc.SetActive(false);
    }
}
