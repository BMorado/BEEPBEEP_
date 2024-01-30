using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Room up;

    [SerializeField] private Room down;
    
    [SerializeField] private Room right;
    
    [SerializeField] private Room left;
    
    [SerializeField] private Room topRight;
    
    [SerializeField] private Room topLeft;
   
    [SerializeField] private Room bottomRight;
    
    [SerializeField] private Room bottomLeft;

    public void Awake() {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
