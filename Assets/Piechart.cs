using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PieChart : MonoBehaviour
{
    public GameObject pieSlicePrefab; // Prefab hình tròn
    public Transform chartParent;     // Object chứa các slice
    public List<float> values;        // Giá trị phần trăm (vd: 0.3, 0.5...)
    public List<Color> colors;        // Màu sắc tương ứng từng phần

    void Start()
    {
        DrawPieChart();
    }

    void DrawPieChart()
    {
        float zRotation = 0f;

        for (int i = 0; i < values.Count; i++)
        {
            GameObject slice = Instantiate(pieSlicePrefab, chartParent);
            Image img = slice.GetComponent<Image>();
            img.color = colors[i];
            img.fillAmount = values[i];

            slice.transform.localRotation = Quaternion.Euler(0, 0, -zRotation);
            zRotation += values[i] * 360f;
        }
    }
}
