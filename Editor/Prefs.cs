using UnityEngine;
using UnityEditor;

namespace Nebukam.Editor
{

    public static class Prefs
    {

        internal static string Id(Object obj, string id) { return obj.GetType().FullName + "_"+id; }
        internal const string ID = "__o_";
        #region Get

        public static bool Get(string id, bool defaultValue)
        {
            if (EditorPrefs.HasKey(id))
                return EditorPrefs.GetBool(id); 

            Set(id, defaultValue);
            return defaultValue;
        }

        public static string Get(string id, string defaultValue)
        {
            if (EditorPrefs.HasKey(id)) 
                return EditorPrefs.GetString(id);

            Set(id, defaultValue);
            return defaultValue;
        }

        public static int Get(string id, int defaultValue)
        {
            if (EditorPrefs.HasKey(id)) 
                return EditorPrefs.GetInt(id); 

            Set(id, defaultValue);
            return defaultValue;
        }

        public static float Get(string id, float defaultValue)
        {
            if (EditorPrefs.HasKey(id)) 
                return EditorPrefs.GetFloat(id);

            Set(id, defaultValue);
            return defaultValue;
        }


        public static T Get<T>(string id, T defaultValue)
            where T : Object
        {

            id = ID + id;

            if (EditorPrefs.HasKey(id))
                return AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(EditorPrefs.GetString(id))) as T;

            string guid;
            long localid;

            if (defaultValue != null
                && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(defaultValue, out guid, out localid))
            {
                EditorPrefs.SetString(id, guid);
                return defaultValue;
            }

            return defaultValue;
        }

        #endregion

        #region Set

        public static bool Set(string id, bool value)
        {
            EditorPrefs.SetBool(id, value);
            return value;
        }

        public static string Set(string id, string value)
        {
            EditorPrefs.SetString(id, value);
            return value;
        }

        public static int Set(string id, int value)
        {
            EditorPrefs.SetInt(id, value);
            return value;
        }

        public static float Set(string id, float value)
        {
            EditorPrefs.SetFloat(id, value);
            return value;
        }

        public static T Set<T>(string id, T value)
            where T : Object
        {
            string guid;
            long localid;

            if (value != null
                && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(value, out guid, out localid))
            {
                EditorPrefs.SetString(ID + id, guid);
                return value;
            }

            return value;
        }

        #endregion

        #region Update

        public static bool Update(string id, ref bool value)
        {
            bool stored = Get(id, value);
            if(stored == value) { return false; }
            Set(id, value);
            return true;
        }

        public static bool Update(string id, ref string value)
        {
            string stored = Get(id, value);
            if (stored == value) { return false; }
            Set(id, value);
            return true;
        }

        public static bool Update(string id, ref int value)
        {
            int stored = Get(id, value);
            if (stored == value) { return false; }
            Set(id, value);
            return true;
        }

        public static bool Update(string id, ref float value)
        {
            float stored = Get(id, value);
            if (stored == value) { return false; }
            Set(id, value);
            return true;
        }

        public static bool Update<T>(string id, ref T value)
            where T : Object
        {
            Object stored = Get(id, value);
            if (stored == value) { return false; }
            Set(id, value);
            return true;
        }

        #endregion

    }

}
