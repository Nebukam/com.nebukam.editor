#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


namespace Nebukam.Editor
{

    public static class Load
    {

        public static string projectPath;
        public static string packagePath;
        public static string settingPath;

        private static Dictionary<string, Texture2D> m_texCache = new Dictionary<string, Texture2D>();

        static Load()
        {
            projectPath = Path.Combine(Application.dataPath, "../");
            settingPath = Path.Combine(Application.dataPath, "../ProjectSettings");
            packagePath = Path.Combine(Application.dataPath, "../Packages");
        }

        public static Texture2D ImageGUID(string guid)
        {
            return LoadOrGetCacheGUID(guid);
        }

        public static Texture2D Image(string absolutePath)
        {
            return LoadOrGetCache(absolutePath);
        }

        public static Texture2D Image(string basePath, string relativePath)
        {
            return LoadOrGetCache(Path.Combine(basePath, relativePath));
        }

        private static Texture2D LoadOrGetCache(string path)
        {
            Texture2D result;

            if (m_texCache.TryGetValue(path, out result) && result != null)
                return result;

            if (File.Exists(path))
            {
                byte[] fileData;
                fileData = File.ReadAllBytes(path);
                result = new Texture2D(2, 2);
                result.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            else
            {
                Debug.LogError("Path does not exists : "+ path);
            }

            m_texCache[path] = result;
            return result;
        }

        private static Texture2D LoadOrGetCacheGUID(string guid)
        {
            Texture2D result;

            if (m_texCache.TryGetValue(guid, out result) && result != null)
                return result;

            result = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid));

            m_texCache[guid] = result;
            return result;
        }

    }

}
#endif