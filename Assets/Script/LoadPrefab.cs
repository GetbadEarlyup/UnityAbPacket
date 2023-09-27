using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class LoadPrefab : Singleton<LoadPrefab>
{
    //加载的路径地址
    string AbPath = Application.streamingAssetsPath+ "/AbNum.json";
    System.Collections.Generic.List<DataAbNum> _dataAbNum = null;


    public async Task<GameObject> LoadAbPrefab(string name)
    {
        string readJson = File.ReadAllText(AbPath);
        _dataAbNum = JsonConvert.DeserializeObject<System.Collections.Generic.List<DataAbNum>>(readJson);

        if (_dataAbNum != null)
        {
            for (int i = 0; i < _dataAbNum.Count; i++)
            {
                if (_dataAbNum[i].Name == name)
                {
                    //加载
                    Task<GameObject> taskGame = LoadAsync(_dataAbNum[i]);
                    return await taskGame;
                }
            }
            Debug.LogError("加载对象失败！");
            return null;
        }
        else
        {
            Debug.Log("加载对象为空！");
            return null;
        }

       
    }

   
    private async Task<GameObject> LoadAsync(DataAbNum _str)
    {
        if ( _str != null )
        {
            GameObject prefabObj = await Addressables.LoadAssetAsync<GameObject>(_str.LoadPath).Task;

            if (prefabObj != null)
            {
                return prefabObj;
            }
            else
            {
                Debug.LogError("加载对象为空！");
                return null;
            }
        }
        else 
        {
            Debug.LogError("加载对象为空！");
            return null; 
        }
    }

}
public class DataAbNum
{
    public string Name;
    public string LoadPath;
    public string DirectoryName;
}


