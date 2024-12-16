﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YooAsset
{
    /// <summary>
    /// 缓存文件系统
    /// 说明：正在进行的下载器会在ResourcePackage销毁的时候执行Abort操作！
    /// </summary>
    internal class DefaultCacheFileSystem : IFileSystem, ICacheSystem
    {
        protected readonly Dictionary<string, CacheWrapper> _wrappers = new Dictionary<string, CacheWrapper>(10000);
        protected readonly Dictionary<string, Stream> _loadedStream = new Dictionary<string, Stream>(10000);
        protected readonly Dictionary<string, string> _dataFilePaths = new Dictionary<string, string>(10000);
        protected readonly Dictionary<string, string> _infoFilePaths = new Dictionary<string, string>(10000);
        protected readonly Dictionary<string, string> _tempFilePaths = new Dictionary<string, string>(10000);
        protected readonly Dictionary<string, DefaultDownloadFileOperation> _downloaders = new Dictionary<string, DefaultDownloadFileOperation>(1000);
        protected readonly List<string> _removeList = new List<string>(1000);

        protected string _packageRoot;
        protected string _cacheFileRoot;
        protected string _tempFileRoot;
        protected string _manifestFileRoot;

        /// <summary>
        /// 包裹名称
        /// </summary>
        public string PackageName { private set; get; }

        /// <summary>
        /// 文件根目录
        /// </summary>
        public string FileRoot
        {
            get
            {
                return _packageRoot;
            }
        }

        /// <summary>
        /// 文件数量
        /// </summary>
        public int FileCount
        {
            get
            {
                return _wrappers.Count;
            }
        }

        #region 自定义参数
        /// <summary>
        /// 自定义参数：远程服务接口
        /// </summary>
        public IRemoteServices RemoteServices { private set; get; } = null;

        /// <summary>
        /// 自定义参数：初始化的时候缓存文件校验级别
        /// </summary>
        public EFileVerifyLevel FileVerifyLevel { private set; get; } = EFileVerifyLevel.Middle;

        /// <summary>
        /// 自定义参数：数据文件追加文件格式
        /// </summary>
        public bool AppendFileExtension { private set; get; } = false;

        /// <summary>
        /// 自定义参数：原生文件构建管线
        /// </summary>
        public bool RawFileBuildPipeline { private set; get; } = false;

        /// <summary>
        /// 自定义参数：启用断点续传的最小尺寸
        /// </summary>
        public long ResumeDownloadMinimumSize { private set; get; } = long.MaxValue;

        /// <summary>
        /// 自定义参数：断点续传下载器关注的错误码
        /// </summary>
        public List<long> ResumeDownloadResponseCodes { private set; get; } = null;

        /// <summary>
        ///  自定义参数：解密方法类
        /// </summary>
        public IDecryptionServices DecryptionServices { private set; get; }
        #endregion


        public DefaultCacheFileSystem()
        {
        }
        public virtual FSInitializeFileSystemOperation InitializeFileSystemAsync()
        {
            var operation = new DCFSInitializeOperation(this);
            OperationSystem.StartOperation(PackageName, operation);
            return operation;
        }
        public virtual FSLoadPackageManifestOperation LoadPackageManifestAsync(string packageVersion, int timeout)
        {
            var operation = new DCFSLoadPackageManifestOperation(this, packageVersion, timeout);
            OperationSystem.StartOperation(PackageName, operation);
            return operation;
        }
        public virtual FSRequestPackageVersionOperation RequestPackageVersionAsync(bool appendTimeTicks, int timeout)
        {
            var operation = new DCFSRequestPackageVersionOperation(this, appendTimeTicks, timeout);
            OperationSystem.StartOperation(PackageName, operation);
            return operation;
        }
        public virtual FSClearCacheBundleFilesOperation ClearCacheBundleFilesAsync(PackageManifest manifest, string clearMode, object clearParam)
        {
            if(clearMode == EFileClearMode.ClearAllBundleFiles.ToString())
            {
                var operation = new ClearAllCacheFilesOperation(this);
                OperationSystem.StartOperation(PackageName, operation);
                return operation;
            }
            else if(clearMode == EFileClearMode.ClearUnusedBundleFiles.ToString())
            {
                var operation = new ClearUnusedCacheFilesOperation(this, manifest);
                OperationSystem.StartOperation(PackageName, operation);
                return operation;
            }
            else if(clearMode == EFileClearMode.ClearBundleFilesByTags.ToString())
            {
                var operation = new ClearCacheFilesByTagsOperaiton(this, manifest, clearParam);
                OperationSystem.StartOperation(PackageName, operation);
                return operation;
            }
            else
            {
                string error = $"Invalid clear mode : {clearMode}";
                var operation = new FSClearCacheBundleFilesCompleteOperation(error);
                OperationSystem.StartOperation(PackageName, operation);
                return operation;
            }
        }
        public virtual FSDownloadFileOperation DownloadFileAsync(PackageBundle bundle, DownloadParam param)
        {
            // 查询旧的下载器
            if (_downloaders.TryGetValue(bundle.BundleGUID, out var oldDownloader))
            {
                oldDownloader.Reference();
                return oldDownloader;
            }

            // 创建新的下载器
            {
                if (string.IsNullOrEmpty(param.ImportFilePath))
                {
                    param.MainURL = RemoteServices.GetRemoteMainURL(bundle.FileName);
                    param.FallbackURL = RemoteServices.GetRemoteFallbackURL(bundle.FileName);
                }
                else
                {
                    // 注意：把本地文件路径指定为远端下载地址
                    param.MainURL = DownloadSystemHelper.ConvertToWWWPath(param.ImportFilePath);
                    param.FallbackURL = param.MainURL;
                }

                if (bundle.FileSize >= ResumeDownloadMinimumSize)
                {
                    var newDownloader = new DownloadResumeFileOperation(this, this, bundle, param, ResumeDownloadResponseCodes);
                    newDownloader.Reference();
                    _downloaders.Add(bundle.BundleGUID, newDownloader);
                    OperationSystem.StartOperation(PackageName, newDownloader);
                    return newDownloader;
                }
                else
                {
                    var newDownloader = new DownloadNormalFileOperation(this, this, bundle, param);
                    newDownloader.Reference();
                    _downloaders.Add(bundle.BundleGUID, newDownloader);
                    OperationSystem.StartOperation(PackageName, newDownloader);
                    return newDownloader;
                }
            }
        }
        public virtual FSLoadBundleOperation LoadBundleFile(PackageBundle bundle)
        {
            if (RawFileBuildPipeline)
            {
                var operation = new DCFSLoadRawBundleOperation(this, bundle);
                OperationSystem.StartOperation(PackageName, operation);
                return operation;
            }
            else
            {
                var operation = new DCFSLoadAssetBundleOperation(this, bundle);
                OperationSystem.StartOperation(PackageName, operation);
                return operation;
            }
        }
        public virtual void UnloadBundleFile(PackageBundle bundle, object result)
        {
            AssetBundle assetBundle = result as AssetBundle;
            if (assetBundle == null)
                return;

            if (assetBundle != null)
                assetBundle.Unload(true);

            if (_loadedStream.TryGetValue(bundle.BundleGUID, out Stream managedStream))
            {
                if (managedStream != null)
                {
                    managedStream.Close();
                    managedStream.Dispose();
                }
                _loadedStream.Remove(bundle.BundleGUID);
            }
        }

        public virtual void SetParameter(string name, object value)
        {
            if (name == FileSystemParametersDefine.REMOTE_SERVICES)
            {
                RemoteServices = (IRemoteServices)value;
            }
            else if (name == FileSystemParametersDefine.FILE_VERIFY_LEVEL)
            {
                FileVerifyLevel = (EFileVerifyLevel)value;
            }
            else if (name == FileSystemParametersDefine.APPEND_FILE_EXTENSION)
            {
                AppendFileExtension = (bool)value;
            }
            else if (name == FileSystemParametersDefine.RAW_FILE_BUILD_PIPELINE)
            {
                RawFileBuildPipeline = (bool)value;
            }
            else if (name == FileSystemParametersDefine.RESUME_DOWNLOAD_MINMUM_SIZE)
            {
                ResumeDownloadMinimumSize = (long)value;
            }
            else if (name == FileSystemParametersDefine.RESUME_DOWNLOAD_RESPONSE_CODES)
            {
                ResumeDownloadResponseCodes = (List<long>)value;
            }
            else if (name == FileSystemParametersDefine.DECRYPTION_SERVICES)
            {
                DecryptionServices = (IDecryptionServices)value;
            }
            else
            {
                YooLogger.Warning($"Invalid parameter : {name}");
            }
        }
        public virtual void OnCreate(string packageName, string rootDirectory)
        {
            PackageName = packageName;

            if (string.IsNullOrEmpty(rootDirectory))
                rootDirectory = CacheHelper.GetDefaultCacheRoot();

            _packageRoot = PathUtility.Combine(rootDirectory, packageName);
            _cacheFileRoot = PathUtility.Combine(_packageRoot, DefaultCacheFileSystemDefine.SaveFilesFolderName);
            _tempFileRoot = PathUtility.Combine(_packageRoot, DefaultCacheFileSystemDefine.TempFilesFolderName);
            _manifestFileRoot = PathUtility.Combine(_packageRoot, DefaultCacheFileSystemDefine.ManifestFilesFolderName);
        }
        public virtual void OnUpdate()
        {
            _removeList.Clear();

            foreach (var valuePair in _downloaders)
            {
                var downloader = valuePair.Value;

                // 注意：主动终止引用计数为零的下载任务
                if (downloader.RefCount <= 0)
                {
                    _removeList.Add(valuePair.Key);
                    downloader.SetAbort();
                    continue;
                }

                if (downloader.IsDone)
                    _removeList.Add(valuePair.Key);
            }

            foreach (var key in _removeList)
            {
                _downloaders.Remove(key);
            }
        }

        public virtual bool Belong(PackageBundle bundle)
        {
            // 注意：缓存文件系统保底加载！
            return true;
        }
        public virtual bool Exists(PackageBundle bundle)
        {
            return _wrappers.ContainsKey(bundle.BundleGUID);
        }
        public virtual bool NeedDownload(PackageBundle bundle)
        {
            if (Belong(bundle) == false)
                return false;

            return Exists(bundle) == false;
        }
        public virtual bool NeedUnpack(PackageBundle bundle)
        {
            return false;
        }
        public virtual bool NeedImport(PackageBundle bundle)
        {
            if (Belong(bundle) == false)
                return false;

            return Exists(bundle) == false;
        }

        public virtual byte[] ReadFileData(PackageBundle bundle)
        {
            if (Exists(bundle) == false)
                return null;

            if (bundle.Encrypted)
            {
                if (DecryptionServices == null)
                {
                    YooLogger.Error($"The {nameof(IDecryptionServices)} is null !");
                    return null;
                }

                string filePath = GetCacheFileLoadPath(bundle);
                var fileInfo = new DecryptFileInfo()
                {
                    BundleName = bundle.BundleName,
                    FileLoadCRC = bundle.UnityCRC,
                    FileLoadPath = filePath,
                };
                return DecryptionServices.ReadFileData(fileInfo);
            }
            else
            {
                string filePath = GetCacheFileLoadPath(bundle);
                return FileUtility.ReadAllBytes(filePath);
            }
        }
        public virtual string ReadFileText(PackageBundle bundle)
        {
            if (Exists(bundle) == false)
                return null;

            if (bundle.Encrypted)
            {
                if (DecryptionServices == null)
                {
                    YooLogger.Error($"The {nameof(IDecryptionServices)} is null !");
                    return null;
                }

                string filePath = GetCacheFileLoadPath(bundle);
                var fileInfo = new DecryptFileInfo()
                {
                    BundleName = bundle.BundleName,
                    FileLoadCRC = bundle.UnityCRC,
                    FileLoadPath = filePath,
                };
                return DecryptionServices.ReadFileText(fileInfo);
            }
            else
            {
                string filePath = GetCacheFileLoadPath(bundle);
                return FileUtility.ReadAllText(filePath);
            }
        }

        #region 缓存系统
        public string GetCacheFileRoot()
        {
            return _cacheFileRoot;
        }
        public string GetTempFilePath(PackageBundle bundle)
        {
            if (_tempFilePaths.TryGetValue(bundle.BundleGUID, out string filePath) == false)
            {
                filePath = PathUtility.Combine(_tempFileRoot, bundle.BundleGUID);
                _tempFilePaths.Add(bundle.BundleGUID, filePath);
            }
            return filePath;
        }
        public string GetDataFilePath(PackageBundle bundle)
        {
            if (_dataFilePaths.TryGetValue(bundle.BundleGUID, out string filePath) == false)
            {
                string folderName = bundle.FileHash.Substring(0, 2);
                filePath = PathUtility.Combine(_cacheFileRoot, folderName, bundle.BundleGUID, DefaultCacheFileSystemDefine.SaveBundleDataFileName);
                if (AppendFileExtension)
                    filePath += bundle.FileExtension;
                _dataFilePaths.Add(bundle.BundleGUID, filePath);
            }
            return filePath;
        }
        public string GetInfoFilePath(PackageBundle bundle)
        {
            if (_infoFilePaths.TryGetValue(bundle.BundleGUID, out string filePath) == false)
            {
                string folderName = bundle.FileHash.Substring(0, 2);
                filePath = PathUtility.Combine(_cacheFileRoot, folderName, bundle.BundleGUID, DefaultCacheFileSystemDefine.SaveBundleInfoFileName);
                _infoFilePaths.Add(bundle.BundleGUID, filePath);
            }
            return filePath;
        }
        public List<string> GetAllCachedBundleGUIDs()
        {
            return _wrappers.Keys.ToList();
        }

        public bool IsRecordFile(string bundleGUID)
        {
            return _wrappers.ContainsKey(bundleGUID);
        }
        public bool RecordFile(string bundleGUID, CacheWrapper wrapper)
        {
            if (_wrappers.ContainsKey(bundleGUID))
            {
                YooLogger.Error($"{nameof(DefaultCacheFileSystem)} has element : {bundleGUID}");
                return false;
            }

            _wrappers.Add(bundleGUID, wrapper);
            return true;
        }

        public EFileVerifyResult VerifyCacheFile(PackageBundle bundle)
        {
            if (_wrappers.TryGetValue(bundle.BundleGUID, out CacheWrapper wrapper) == false)
                return EFileVerifyResult.CacheNotFound;

            EFileVerifyResult result = FileVerifyHelper.FileVerify(wrapper.DataFilePath, wrapper.DataFileSize, wrapper.DataFileCRC, EFileVerifyLevel.High);
            return result;
        }
        public bool WriteCacheFile(PackageBundle bundle, string copyPath)
        {
            if (_wrappers.ContainsKey(bundle.BundleGUID))
            {
                throw new Exception("Should never get here !");
            }

            string infoFilePath = GetInfoFilePath(bundle);
            string dataFilePath = GetDataFilePath(bundle);

            try
            {
                if (File.Exists(infoFilePath))
                    File.Delete(infoFilePath);
                if (File.Exists(dataFilePath))
                    File.Delete(dataFilePath);

                FileUtility.CreateFileDirectory(dataFilePath);

                // 拷贝数据文件
                FileInfo fileInfo = new FileInfo(copyPath);
                fileInfo.CopyTo(dataFilePath);

                // 写入文件信息
                WriteInfoFile(infoFilePath, bundle.FileCRC, bundle.FileSize);
            }
            catch (Exception e)
            {
                YooLogger.Error($"Failed to write cache file ! {e.Message}");
                return false;
            }

            var wrapper = new CacheWrapper(infoFilePath, dataFilePath, bundle.FileCRC, bundle.FileSize);
            return RecordFile(bundle.BundleGUID, wrapper);
        }
        public bool DeleteCacheFile(string bundleGUID)
        {
            if (_wrappers.TryGetValue(bundleGUID, out CacheWrapper wrapper))
            {
                try
                {
                    string dataFilePath = wrapper.DataFilePath;
                    FileInfo fileInfo = new FileInfo(dataFilePath);
                    if (fileInfo.Exists)
                        fileInfo.Directory.Delete(true);
                    _wrappers.Remove(bundleGUID);
                    return true;
                }
                catch (Exception e)
                {
                    YooLogger.Error($"Failed to delete cache file ! {e.Message}");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private readonly BufferWriter _sharedBuffer = new BufferWriter(1024);
        public void WriteInfoFile(string filePath, string dataFileCRC, long dataFileSize)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                _sharedBuffer.Clear();
                _sharedBuffer.WriteUTF8(dataFileCRC);
                _sharedBuffer.WriteInt64(dataFileSize);
                _sharedBuffer.WriteToStream(fs);
                fs.Flush();
            }
        }
        public void ReadInfoFile(string filePath, out string dataFileCRC, out long dataFileSize)
        {
            byte[] binaryData = FileUtility.ReadAllBytes(filePath);
            BufferReader buffer = new BufferReader(binaryData);
            dataFileCRC = buffer.ReadUTF8();
            dataFileSize = buffer.ReadInt64();
        }
        #endregion

        #region 内部方法
        public string GetCacheFileLoadPath(PackageBundle bundle)
        {
            return GetDataFilePath(bundle);
        }
        public string GetCachePackageHashFilePath(string packageVersion)
        {
            string fileName = YooAssetSettingsData.GetPackageHashFileName(PackageName, packageVersion);
            return PathUtility.Combine(_manifestFileRoot, fileName);
        }
        public string GetCachePackageManifestFilePath(string packageVersion)
        {
            string fileName = YooAssetSettingsData.GetManifestBinaryFileName(PackageName, packageVersion);
            return PathUtility.Combine(_manifestFileRoot, fileName);
        }
        public string GetSandboxAppFootPrintFilePath()
        {
            return PathUtility.Combine(_manifestFileRoot, DefaultCacheFileSystemDefine.AppFootPrintFileName);
        }

        /// <summary>
        /// 删除所有清单文件
        /// </summary>
        public void DeleteAllManifestFiles()
        {
            if (Directory.Exists(_manifestFileRoot))
            {
                Directory.Delete(_manifestFileRoot, true);
            }
        }

        /// <summary>
        /// 加载加密资源文件
        /// </summary>
        public AssetBundle LoadEncryptedAssetBundle(PackageBundle bundle)
        {
            string filePath = GetCacheFileLoadPath(bundle);
            var fileInfo = new DecryptFileInfo()
            {
                BundleName = bundle.BundleName,
                FileLoadCRC = bundle.UnityCRC,
                FileLoadPath = filePath,
            };

            var assetBundle = DecryptionServices.LoadAssetBundle(fileInfo, out var managedStream);
            _loadedStream.Add(bundle.BundleGUID, managedStream);
            return assetBundle;
        }

        /// <summary>
        /// 加载加密资源文件
        /// </summary>
        public AssetBundleCreateRequest LoadEncryptedAssetBundleAsync(PackageBundle bundle)
        {
            string filePath = GetCacheFileLoadPath(bundle);
            var fileInfo = new DecryptFileInfo()
            {
                BundleName = bundle.BundleName,
                FileLoadCRC = bundle.UnityCRC,
                FileLoadPath = filePath,
            };

            var createRequest = DecryptionServices.LoadAssetBundleAsync(fileInfo, out var managedStream);
            _loadedStream.Add(bundle.BundleGUID, managedStream);
            return createRequest;
        }
        #endregion
    }
}