using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

public class CreatTex : EditorWindow
{

    string AbPath;
    private int select = 0;
    private string[] names = {"P1","P2"};
    string[] guids = null;
    public UnityEngine.Object obj;
    string guid;
    long file;
   

    [MenuItem("Tool/Show")]
    public static void Init()
    {
        EditorWindow.GetWindow<CreatTex>();
        
    }

    void OnGUI()
    {
        select = GUILayout.Toolbar(select, names);

        if (select == 0)
        {
            GUILayout.BeginVertical();
            //提示信息
            GUILayout.Label("点击确认会自动创建文件夹");
            GUILayout.Label("请将所有预制件放入CreatAB文件夹中");
            GUILayout.Label("如果加载完请删除此文件夹，再导入新对象");
            GUILayout.Label("并记得更新Json文件");
            GUILayout.Label("注意！：");
            GUILayout.Label("生成的Ab数据文件无需移动位置,\n插件系统会自动将Ab包复制到流目录中");
            GUILayout.Label("打成Ab包对象的名称不可重复");
            GUILayout.Label("使用时名称不要携带括号、空格，\n名称全英，并采用驼峰命名法命名");
            GUILayout.Label("每次重新生成时会导致Json重新写入，\n请手动保存下数据文件");
            GUILayout.Label("到P2进行一键打包操作！");
            if (GUILayout.Button("确认"))
            {
                //生成创建文件夹
                AbPath = Application.dataPath + "/CreatAB";

                //判断文件夹路径是否存在
                if (!System.IO.Directory.Exists(AbPath))
                {
                    //创建
                    System.IO.Directory.CreateDirectory(AbPath);
                }
                else
                {
                    Debug.Log("已存在！");
                }
               
                //刷新
                AssetDatabase.Refresh();

            }

            GUILayout.EndVertical();
        }
        else if (select == 1)
        {
            if (GUILayout.Button("生成打包数据"))
            {
                AbPath = Application.dataPath + "/CreatAB";
                //校验文件夹是否存在
                if (!System.IO.Directory.Exists(AbPath))
                {
                    Debug.LogError("文件夹不存在，请先创建文件夹！");
                    return;
                }
                else
                {
                    //便利文件夹下所有prefab对象
                    //打成AB包文件
                    List<FileInfo> _list= GetAllprefab(AbPath);
                    List<DataAbNum> _dataAbnum= new List<DataAbNum>();
                    //创建csv格式
                    DataTable dt = new DataTable("Lession1");
                    dt.Columns.Add("Id");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("prefabPath");
                    dt.Columns.Add("filePath");


                    //便利所有对象拿到

                    for (int i = 0; i < _list.Count; i++)
                    {
                        
                        //创建写入
                        DataAbNum _dataAb=new DataAbNum();
                        _dataAb.Name = Regex.Split(FormatAssetPath(_list[i].Name), ".prefab")[0];

                        var strPath = "Assets" + Regex.Split( FormatAssetPath(_list[i].FullName),"/Assets")[1];
                        _dataAb.LoadPath = strPath;
                        _dataAb.DirectoryName= FormatAssetPath(_list[i].DirectoryName);

                        _dataAbnum.Add(_dataAb);
                    }

                    if (!System.IO.Directory.Exists(Application.streamingAssetsPath))
                    {
                        //创建
                        System.IO.Directory.CreateDirectory(Application.streamingAssetsPath);
                    }

                    //调用写入数据保存json文件
                    string str = JsonConvert.SerializeObject(_dataAbnum);
                    System.IO.File.WriteAllText(Application.streamingAssetsPath+"/AbNum.json", str);

                    //刷新
                    AssetDatabase.Refresh();
                }
            }
        }
       
    }

    /// <summary>
    /// 格式化路径成Asset的标准格式
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string FormatAssetPath(string filePath)
    {
        var newFilePath1 = filePath.Replace("\\", "/");
        var newFilePath2 = newFilePath1.Replace("//", "/").Trim();
        newFilePath2 = newFilePath2.Replace("///", "/").Trim();
        newFilePath2 = newFilePath2.Replace("\\\\", "/").Trim();
        return newFilePath2;
    }

  

    public List<FileInfo> GetAllprefab(string paths)
    {
        string path = string.Format("{0}", paths);
        List<FileInfo> _list=new List<FileInfo>();
        if (System.IO.Directory.Exists(path))
        {
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*.prefab");
            for (int i = 0; i < files.Length; i++)
            {
                //忽略关联文件
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }

                Debug.Log(files[i].Name);
                Debug.Log(files[i].FullName);
                Debug.Log(files[i].DirectoryName);
                _list.Add(files[i]);
            }
        }

        return _list;

    }

}


