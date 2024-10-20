using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Field : MonoBehaviour
{
    public GameObject FieldGO { get;private set; }
    public bool IsClicked { get; set; }
    public Image Image { get; private set; }
    public int FieldId;
    private void Start()
    {
        FieldGO = this.gameObject;
        IsClicked = false;
        Image = GetComponent<Image>();
    }
}
