using System;
using System.IO;
using Foundation;
using Newtonsoft.Json;
using ScriptableObjectBase;
using UnityEditor;
using UnityEngine;

namespace SOCreator
{
    public class SOCreatorWindow : EditorWindow
    {
        private static string className;
        private static string nameSpace = "";
        private static string assemblyName = "Assembly-CSharp";
        private string assetName;

        private static string createDirectory= "Assets";

        private static string Json;

        [MenuItem("Tools/ScriptableObject Creator")]
        public static void ShowWindow()
        {
            GetWindow<SOCreatorWindow>("ScriptableObject Creator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create ScriptableObject", EditorStyles.boldLabel);

            createDirectory = EditorGUILayout.TextField("Create Directory Path , Assets : ....", createDirectory);

            assemblyName = EditorGUILayout.TextField("Assembly Name", assemblyName);

            nameSpace = EditorGUILayout.TextField("Namespace", nameSpace);

            // 选择类型
            className = EditorGUILayout.TextField("Select Type", className);

            assetName = className;

            if (GUILayout.Button("Create ScriptableObject"))
            {
                CreateScriptableObjectFile();
            }

            Json = EditorGUILayout.TextField(" Json: ", Json);

            // if (GUILayout.Button("Create ScriptableObject By Json"))
            // {
            //     // Runner.AllConfig obj = Serializer.Deserialize<Runner.AllConfig>(Json);
            //     // if(obj!=null)
            //     // {
            //     //     CreateSOAsset(obj);
            //     //     AssetDatabase.AddObjectToAsset();
            //     // }
            // }
        }

        private string CreateSOAsset (ScriptableObject so , string name = null)
        {
            if(string.IsNullOrEmpty(name))
            {
                name = so.GetType().Name;
            }
            // 确保在 Assets 文件夹中创建资源
            string path = LoopCheack(name);
            AssetDatabase.CreateAsset(so, path);
            AssetDatabase.SaveAssets();

            // 选择并显示新创建的资源
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = so;

            return path;
        }

        private void CreateScriptableObjectFile()
        {
            // 创建一个新的 ScriptableObject 实例
            // Debug.Log(typeof(ItemGroupNode).FullName);
            // return; 
            ScriptableObject newAsset = CreateInstance(Type.GetType(nameSpace+"." + className+", "+assemblyName));

            string path = CreateSOAsset(newAsset);

            Debug.Log($"ScriptableObject '{assetName}' created with type '{className}' at {path}");
        }
        private string LoopCheack(string name, int index = 0)
        {
            string newpath = null;
            if(index == 0)
            {
                newpath = $"{createDirectory}/{name}.asset";
            }
            else{
                newpath = $"{createDirectory}/{name}_{index}.asset";
            }
            if (File.Exists(newpath))
            {
                return LoopCheack(name, index + 1);
            }
            return newpath;
        }
    }
}

