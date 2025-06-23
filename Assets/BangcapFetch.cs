// Attach this script to a Unity GameObject (e.g. Manager or Controller)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // If using TextMeshPro Dropdown

[System.Serializable]
public class BangCapItem {
    public string bangCap;
    public string vietTat;
}

[System.Serializable]
public class BangCapResponse {
    public string status;
    public List<BangCapItem> data;
}

public class BangCapFetcher : MonoBehaviour
{
    [Header("Dropdown để hiển thị tên bằng cấp")]
    public TMP_Dropdown dropdown; // Kéo Dropdown từ Unity vào đây

    [Header("Cấu hình API")]
    public string baseUrl = "https://script.google.com/macros/s/AKfycby1CItno0zdXLCVDxex8WeFDOVVXWTZPoaSlCelh9UJ5q0AvI2xa8AyM50pEX4rbcBIRA/exec?sheet=BangHeSo&action=getBangCapList";

    void Start()
    {
        StartCoroutine(GetBangCapList());
    }

    IEnumerator GetBangCapList()
    {
        string url = baseUrl + "?sheet=BangHeSo&action=getBangCapList";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Lỗi khi lấy dữ liệu: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            BangCapResponse response = JsonUtility.FromJson<BangCapResponse>(json);

            if (response.status == "success")
            {
                // Xóa lựa chọn cũ
                dropdown.ClearOptions();

                List<string> options = new List<string>();
                foreach (BangCapItem item in response.data)
                {
                    options.Add(item.bangCap + " (" + item.vietTat + ")");
                }

                dropdown.AddOptions(options);
            }
            else
            {
                Debug.LogError("Phản hồi lỗi: " + json);
            }
        }
    }
}