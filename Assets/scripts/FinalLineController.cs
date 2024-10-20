using System.Collections;
using UnityEngine;

public class FinalLineController : MonoBehaviour
{
    public static FinalLineController Instance {  get; private set; }
    
    [SerializeField] GameObject finalLine;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public void SetFinalLine(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        finalLine.transform.eulerAngles = new Vector3(x1, y1, z1);
        finalLine.transform.localPosition = new Vector3(x2, y2, z2);
        finalLine.SetActive(true);
    }
}
