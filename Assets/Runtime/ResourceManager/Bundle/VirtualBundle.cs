
namespace YooAsset
{
    internal class VirtualBundle
    {
        private readonly IFileSystem _fileSystem;
        private readonly PackageBundle _packageBundle;
        
        internal VirtualBundle(IFileSystem fileSystem, PackageBundle packageBundle)
        {
            _fileSystem = fileSystem;
            _packageBundle = packageBundle;
        }
    }
}