using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace YooAsset.Editor
{
    public class DefaultIgnoreRule
    {
        /// <summary>
        /// 忽略的文件类型
        /// </summary>
        public readonly static HashSet<string> IgnoreFileExtensions = new HashSet<string>() { "", ".so", ".dll", ".cs", ".js", ".boo", ".meta", ".cginc", ".hlsl" };
    }

    /// <summary>
    /// 适配常规的资源构建管线
    /// </summary>
    public class NormalIgnoreRule : IIgnoreRule
    {
        /// <summary>
        /// 查询是否为忽略文件
        /// </summary>
        public bool IsIgnore(AssetInfo assetInfo)
        {
            if (assetInfo.AssetPath.StartsWith("Assets/") == false && assetInfo.AssetPath.StartsWith("Packages/") == false)
            {
                UnityEngine.Debug.LogError($"Invalid asset path : {assetInfo.AssetPath}");
                return true;
            }

            // 忽略文件夹
            if (AssetDatabase.IsValidFolder(assetInfo.AssetPath))
                return true;

            // 忽略编辑器图标资源
            if (assetInfo.AssetPath.Contains("/Gizmos/"))
                return true;

            // 忽略编辑器专属资源
            if (assetInfo.AssetPath.Contains("/Editor/") || assetInfo.AssetPath.Contains("/Editor Resources/"))
                return true;

            // 忽略编辑器下的类型资源
            if (assetInfo.AssetType == typeof(LightingDataAsset))
                return true;
            if (assetInfo.AssetType == typeof(LightmapParameters))
                return true;

            // 忽略Unity引擎无法识别的文件
            if (assetInfo.AssetType == typeof(UnityEditor.DefaultAsset))
            {
                UnityEngine.Debug.LogWarning($"Cannot pack default asset : {assetInfo.AssetPath}");
                return true;
            }

            if (assetInfo.AssetType == typeof(TMP_FontAsset) || assetInfo.AssetType == typeof(Font))
            {
#if WEIXINMINIGAME
                //小游戏忽略引用字体
                var font = AssetDatabase.LoadAssetAtPath<Object>(assetInfo.AssetPath);
                font.hideFlags = HideFlags.DontSave;//不参与打包
                return true;
#else   
                var font = AssetDatabase.LoadAssetAtPath<Object>(assetInfo.AssetPath);
                font.hideFlags = HideFlags.None;//还原
#endif
            }

            return DefaultIgnoreRule.IgnoreFileExtensions.Contains(assetInfo.FileExtension);
        }
    }
    
    /// <summary>
    /// 适配原生文件构建管线
    /// </summary>
    public class RawFileIgnoreRule : IIgnoreRule
    {
        /// <summary>
        /// 查询是否为忽略文件
        /// </summary>
        public bool IsIgnore(AssetInfo assetInfo)
        {
            if (assetInfo.AssetPath.StartsWith("Assets/") == false && assetInfo.AssetPath.StartsWith("Packages/") == false)
            {
                UnityEngine.Debug.LogError($"Invalid asset path : {assetInfo.AssetPath}");
                return true;
            }

            // 忽略文件夹
            if (AssetDatabase.IsValidFolder(assetInfo.AssetPath))
                return true;

            // 忽略编辑器下的类型资源
            if (assetInfo.AssetType == typeof(LightingDataAsset))
                return true;
            if (assetInfo.AssetType == typeof(LightmapParameters))
                return true;

            return DefaultIgnoreRule.IgnoreFileExtensions.Contains(assetInfo.FileExtension);
        }
    }
}