using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPrefabExample : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        //注意 此处采用异步加载
        var  _str=await LoadPrefab.Instance.LoadAbPrefab("Cube");
        Instantiate( _str);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
