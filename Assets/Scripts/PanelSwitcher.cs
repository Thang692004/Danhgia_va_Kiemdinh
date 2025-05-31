using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    [Header("Danh sách các Panel")]
    public List<GameObject> allPanels; // Kéo tất cả các panel vào đây

    [Header("Panel mặc định hiển thị")]
    public GameObject defaultPanel; // Kéo panel bạn muốn hiển thị khi bắt đầu

    void Start()
    {
        // Đảm bảo chỉ panel mặc định được hiển thị khi trò chơi/ứng dụng bắt đầu
        ShowPanel(defaultPanel);
    }

    public void ShowPanel(GameObject panelToShow)
    {
        // Duyệt qua tất cả các panel trong danh sách
        foreach (GameObject panel in allPanels)
        {
            // Nếu đây là panel chúng ta muốn hiển thị, bật nó lên
            if (panel == panelToShow)
            {
                panel.SetActive(true);
            }
            // Ngược lại, tắt nó đi
            else
            {
                panel.SetActive(false);
            }
        }
    }
}