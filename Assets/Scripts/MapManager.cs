using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public class MapManager : MonoBehaviour
{
    #region mapComponents
    [SerializeField] private GameObject startTile;
    [SerializeField] private GameObject finishTile;
    [SerializeField] private GameObject normalTile;
    [SerializeField] private GameObject shopTile;
    [SerializeField] private GameObject blankTile;
    [SerializeField] private GameObject horizontal;
    [SerializeField] private GameObject vertical;
    [SerializeField] private GameObject forwardslash;
    [SerializeField] private GameObject backslash;
    
    #endregion
    
    // Start is called before the first frame update
    void Start() {

        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

        int x = UnityEngine.Random.Range(-5, 5);
        int y = UnityEngine.Random.Range(-5, 5);

        spawnTile(startTile, x, y);

        // x = UnityEngine.Random.Range(0,10);
        // y = UnityEngine.Random.Range(0,10);

        // Instantiate(finishTile, new Vector3(x, y, 0.0f), quaternion.identity);
    }
    
    void spawnTile(GameObject prefabTile, int x, int y) {
        Instantiate(prefabTile, new Vector3(x, y, 0.0f), quaternion.identity);
    }
}
