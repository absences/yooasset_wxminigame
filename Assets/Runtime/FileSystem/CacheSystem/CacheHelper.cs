using System;
using System.IO;

namespace YooAsset
{
    internal class CacheHelper
    {
        /// <summary>
        /// 获取默认的缓存根目录
        /// </summary>
        public static string GetDefaultCacheRoot()
        {
#if UNITY_EDITOR
            // 注意：为了方便调试查看，编辑器下把存储目录放到项目里。
            string projectPath = Path.GetDirectoryName(UnityEngine.Application.dataPath);
            projectPath = PathUtility.RegularPath(projectPath);
            return PathUtility.Combine(projectPath, YooAssetSettingsData.Setting.DefaultYooFolderName);
#elif UNITY_STANDALONE
            return PathUtility.Combine(UnityEngine.Application.dataPath, YooAssetSettingsData.Setting.DefaultYooFolderName);
#else
            return PathUtility.Combine(UnityEngine.Application.persistentDataPath, YooAssetSettingsData.Setting.DefaultYooFolderName);	
#endif
        }
    }
}