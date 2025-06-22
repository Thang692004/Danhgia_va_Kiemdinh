using UnityEngine;

public class GiangVienFormManager : MonoBehaviour
{
    public GameObject trangThemMoi;  // Panel nhập giảng viên mới
    public GameObject trangChinh;    // Trang danh sách (dữ liệu gốc)

    public void MoFormNhap()
    {
        if (trangThemMoi != null) trangThemMoi.SetActive(true);
        if (trangChinh != null) trangChinh.SetActive(false);
    }

    public void DongFormNhap()
    {
        if (trangThemMoi != null) trangThemMoi.SetActive(false);
        if (trangChinh != null) trangChinh.SetActive(true);
    }
}
