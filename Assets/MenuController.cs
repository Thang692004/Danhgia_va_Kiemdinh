using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject panelBangCap;
    public GameObject panelKhoa;
    public GameObject panelGiaoVien;
    public GameObject panelThongKe;

    private void Start()
    {
        ShowBangCap(); // Panel đầu tiên được hiển thị mặc định
    }

    public void ShowBangCap()
    {
        SetActivePanel(panelBangCap);
    }

    public void ShowKhoa()
    {
        SetActivePanel(panelKhoa);
    }

    public void ShowGiaoVien()
    {
        SetActivePanel(panelGiaoVien);
    }

    public void ShowThongKe()
    {
        SetActivePanel(panelThongKe);
    }

    private void SetActivePanel(GameObject panelToShow)
    {
        panelBangCap.SetActive(false);
        panelKhoa.SetActive(false);
        panelGiaoVien.SetActive(false);
        panelThongKe.SetActive(false);

        panelToShow.SetActive(true);
    }
}
