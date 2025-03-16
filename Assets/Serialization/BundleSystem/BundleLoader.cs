using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class BundleLoader : MonoBehaviour
{
    public static readonly string ResourceMapPath = $"Assets/Resources/ResourcesMap.txt";
        
    [MenuItem("Utils/ResourceManagement/GenerateResourceDirectoryMap")]
    public static void GenerateResourceDirectoryMap()
    {
        var dir = Path.GetDirectoryName(ResourceMapPath);
        if (!Directory.Exists(dir))
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(dir);
        }
        File.WriteAllText(ResourceMapPath,JsonConvert.SerializeObject(GetMap(),Formatting.Indented));
        AssetDatabase.ImportAsset(ResourceMapPath,ImportAssetOptions.ForceUpdate);
    }
    
    public static ByResourcesResourceProvider.ResourcesMap GetMap()
    {
        var allObjects = Resources.LoadAll("");
        var result = new ByResourcesResourceProvider.ResourcesMap();
        foreach (var obj in allObjects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            if (AssetDatabase.IsSubAsset(obj) || path.Contains($"{Path.AltDirectorySeparatorChar}Editor{Path.AltDirectorySeparatorChar}") ||
                !path.Contains($"Assets{Path.AltDirectorySeparatorChar}Resources{Path.AltDirectorySeparatorChar}") || Directory.Exists(path))
            {
                continue;
            }
            var name = Path.GetFileNameWithoutExtension(path);
            if (result.TryGetValue(name, out var assetPath))
            {
                throw new ArgumentException($"Asset with name {name}, has different variants in resources folders: " +
                                            $"{string.Join(",",new[]{assetPath.Path,path})}");
            }
            var extraInfo = new Dictionary<string, string>();
            ResourceExtraInfoManager.Process(obj, extraInfo);
            var resInfo = new ByResourcesResourceProvider.ResourcesMap.ResInfo(name,TypeUtils.GetTypeForResourceInfo(obj),
                extraInfo,path.Split($"{Path.AltDirectorySeparatorChar}Resources{Path.AltDirectorySeparatorChar}")[1].Split('.')[0]);
            result.Add(name,resInfo);
        }
        Debug.Log("ResMap generated");
        return result;
    }
}

public static class TypeUtils
{
    public static string GetTypeForResourceInfo(UnityEngine.Object o)
    {
        switch (o)
        {
            case SceneAsset:
                return typeof(UnitySceneType).FullName;
            default: return o.GetType().FullName;
        }
    }
}

public static class UnitySceneType{
}

public static class ResourceExtraInfoManager
{
    public interface IResourceExtraInfoProvider
    {
        public bool CanProvideFor(object o);
        public void Provide(object o, Dictionary<string, string> data);
    }

    private static readonly List<IResourceExtraInfoProvider> InfoProviders = new();
    public static void Register(IResourceExtraInfoProvider infoProvider) => InfoProviders.Add(infoProvider);
        
    public static void Process(object o, Dictionary<string, string> data)
    {
        foreach (var i in InfoProviders)
        {
            if (i.CanProvideFor(o))
            {
                i.Provide(o,data);
            }
        }
    }
}

public class ByResourcesResourceProvider
{
    public class ResourcesMap : Dictionary<string, ResourcesMap.ResInfo>
    {
        public class ResInfo:BaseResourceInfo
        {
            public readonly string Path;

            [JsonConstructor]
            public ResInfo(string assetName, string assetType, Dictionary<string, string> extraInfo, string path) : base(assetName, assetType, extraInfo)
            {
                Path = path;
            }
        }
    }
}

public class BaseResourceInfo
{
    public readonly string AssetName;
    public readonly Type AssetType;
    public readonly IReadOnlyDictionary<string, string> ExtraInfo;

    [JsonConstructor]
    public BaseResourceInfo(
        string assetName, 
        string assetType, 
        Dictionary<string, string> extraInfo)
    {
        AssetName = assetName;
        AssetType = GetTypeSmart(assetType,assetType);
        ExtraInfo = extraInfo;
    }
    public static Type GetTypeSmart(string fullTypeName, string typeName)
    {
        Type type = Type.GetType(fullTypeName);
        if (type != null) return type;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }
        Debug.LogError($"Cant find type {typeName}");
        return null;
    }
}
