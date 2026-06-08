

using System;

namespace Sim.Dispositio.Shared
{
    [Serializable]
    public sealed class FolderReference
    {
        public string FolderId;

#if UNITY_EDITOR
        public string EditorPath => UnityEditor.AssetDatabase.GUIDToAssetPath(FolderId);
#endif
    }
}
