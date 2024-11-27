using System;
using System.IO;
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

        [MenuItem("Tools/ScriptableObject Creator")]
        public static void ShowWindow()
        {
            GetWindow<SOCreatorWindow>("ScriptableObject Creator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create SM ScriptableObject", EditorStyles.boldLabel);

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
        }

        private void CreateScriptableObjectFile()
        {
            // 创建一个新的 ScriptableObject 实例
            // Debug.Log(typeof(ItemGroupNode).FullName);
            // return; 
            ScriptableObject newAsset = CreateInstance(Type.GetType(nameSpace+"." + className+", "+assemblyName));

            // 确保在 Assets 文件夹中创建资源
            string path = LoopCheack(assetName);
            AssetDatabase.CreateAsset(newAsset, path);
            AssetDatabase.SaveAssets();

            // 选择并显示新创建的资源
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newAsset;

            Debug.Log($"ScriptableObject '{assetName}' created with type '{className}' at {path}");
        }
        private string LoopCheack(string name, int index = 0)
        {
            string newpath = $"{createDirectory}/{name}_{index}.asset";
            if (File.Exists(newpath))
            {
                return LoopCheack(name, index + 1);
            }
            return newpath;
        }
    }
}

