using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPanel : MonoBehaviour
{
    [SerializeField] private GameObject mapPanel;
    public void OpenMap() {
        mapPanel.transform.Translate(new Vector3(800,0,0));
    }
}
