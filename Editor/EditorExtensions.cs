using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;

namespace Nebukam.Editor
{
    public static partial class EditorExtensions
    {

        public static float lY = 0f;
        public static float Y = 0f;
        public static float X = 0f;
        public static float W = 0f;

        public static bool inLayout = false;

        public static Rect CR(float height = -1)
        {
            if (height == -1) { height = EditorGUIUtility.singleLineHeight; }
            return new Rect(X,Y,W, height);
        }

        public static Rect R(float height = -1)
        {
            if (height == -1) { height = EditorGUIUtility.singleLineHeight; }

            Rect r = new Rect(X, Y, W, height);
            lY = Y;
            Y += height;

            if(inLayout)
                r = GUILayoutUtility.GetRect(W, height, GUIStyle.none);

            return r;
        }

        public static Rect RW(float height = -1, float width = -1)
        {
            Rect r = R(height);
            
            if(width >= 0f)
                r.width = width;

            return r;
        }

        public static void SetR(Rect r)
        {
            X = r.x;
            lY = Y = r.y;
            W = r.width;
        }

        public static void ToggleLayoutMode(bool toggle)
        {
            inLayout = toggle;
        }

        #region enum

        public static int EnumPopup<T>(ref T e, string label = "")
            where T : Enum
        {
            T r;
            if (label != "")
                r = (T)EditorGUI.EnumPopup(R(), label, e);
            else
                r = (T)EditorGUI.EnumPopup(R(), e);

            if (r.Equals(e)) { return 0; }

            e = r;
            return 1;

        }

        public static int EnumGrid<T>(ref T e, string label = "")
        {
            T[] eList = (T[])Enum.GetValues(typeof(T));
            string[] list = new string[eList.Length];
            int sel = 0, i = 0;
            foreach (T ee in eList)
            {
                list[i] = ee.ToString();
                if (ee.Equals(e)) { sel = i; }
                i++;
            }

            GUILayout.BeginHorizontal();

            if (label != "")
            {
                GUILayout.Label(label);
            }

            //GUILayout.FlexibleSpace();

            //int selection = GUILayout.SelectionGrid(sel, list, eList.Length);
            i = 0;
            int selection = -1;
            foreach (string S in list)
            {
                if (GUILayout.Toggle((sel == i), S, EditorStyles.toolbarButton)) { selection = i; }
                i++;
            }


            //if (label != "")
            GUILayout.EndHorizontal();

            if (selection == -1 || selection == sel) { return 0; }

            e = eList[selection];
            return 1;
        }

        #endregion

        #region bool

        public static int Checkbox(ref bool value, string label)
        {
            bool input = EditorGUI.Toggle(R(), label, value);
            if (input == value) { return 0; }
            value = input;
            return 1;
        }

        #endregion

        #region string

        public static int TextInput(ref string value, string label = "", float width = -1f)
        {
            string input;
            if (label != "")
                input = EditorGUI.TextField(RW(-1f,width), label, value);
            else
                input = EditorGUI.TextField(RW(-1f, width), value);

            if (input == value) { return 0; }
            value = input;
            return 1;
        }

        public static int PathInput(ref string value, string label = "", bool folder = false)
        {

            GUILayout.BeginHorizontal();

            int result = TextInput(ref value, label);

            if (folder)
            {
                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    string input = EditorUtility.OpenFolderPanel("Pick a folder...", "...", "");
                    value = input;
                }
            }
            else if (result == 0)
            {
                GUILayout.EndHorizontal();
                return 0;
            }

            value = value.Replace("\\", "/");

            if (folder && value.Length > 0 && value.Substring(value.Length - 1) != "/")
                value += "/";

            GUILayout.EndHorizontal();

            return 1;

        }

        #endregion

        #region int

        public static int Slider(ref int value, int min, int max, string label = "")
        {
            int input;
            if (label != "")
                input = EditorGUI.IntSlider(R(), label, value, min, max);
            else
                input = EditorGUI.IntSlider(R(), value, min, max);

            if (input == value) { return 0; }
            value = input;
            return 1;
        }

        public static int MinMaxSlider(ref int2 values, int2 range, string label = "")
        {
            float
                rx = values.x,
                ry = values.y;

            MiniLabel(label);

            Rect _r = R();

            rx = EditorGUI.IntField(new Rect(_r.x, _r.y, 30, _r.height), (int)rx);
            EditorGUI.MinMaxSlider(new Rect(_r.x + 30, _r.y, _r.width - 60, _r.height), ref rx, ref ry, range.x, range.y);
            ry = EditorGUI.IntField(new Rect(_r.x + _r.width-30, _r.y, 30, _r.height), (int)ry);

            if (values.x != (int)rx || values.y != (int)ry)
            {
                values.x = (int)rx;
                values.y = (int)ry;
                return 1;
            }

            return 0;

        }

        public static int StartSizeSlider(ref int2 values, int2 range, int minSize = 1, string label = "")
        {
            float
                rx = values.x,
                ry = rx + values.y;

            MiniLabel(label);

            Rect _r = R();

            rx = EditorGUI.IntField(new Rect(_r.x, _r.y, 30, _r.height), (int)rx);
            EditorGUI.MinMaxSlider(new Rect(_r.x + 30, _r.y, _r.width - 60, _r.height), ref rx, ref ry, range.x, range.y);
            ry = (int)ry - (int)rx;

            ry = EditorGUI.IntField(new Rect(_r.x + _r.width - 30, _r.y, 30, _r.height), (int)ry);


            if (values.x != (int)rx || values.y != (int)ry)
            {
                values.x = (int)rx;
                values.y = (int)ry;
                return 1;
            }

            return 0;

        }

        #endregion

        #region float

        public static int MinMaxSlider(ref float2 values, float2 range, string label = "")
        {
            float
                rx = values.x,
                ry = values.y;


            MiniLabel(label);

            Rect _r = R();

            rx = EditorGUI.FloatField(new Rect(_r.x, _r.y, 30, _r.height), rx);
            EditorGUI.MinMaxSlider(new Rect(_r.x + 30, _r.y, _r.width - 60, _r.height), ref rx, ref ry, range.x, range.y);
            ry = EditorGUI.FloatField(new Rect(_r.x + _r.width - 30, _r.y, 30, _r.height), ry);

            if (values.x != rx || values.y != ry)
            {
                values.x = rx;
                values.y = ry;
                return 1;
            }

            return 0;

        }

        public static int StartSizeSlider(ref float2 values, float2 range, string label = "")
        {
            float
                rx = values.x,
                ry = rx + values.y;

            MiniLabel(label);

            Rect _r = R();

            rx = EditorGUI.FloatField(new Rect(_r.x, _r.y, 30, _r.height), rx);
            EditorGUI.MinMaxSlider(new Rect(_r.x + 30, _r.y, _r.width - 60, _r.height), ref rx, ref ry, range.x, range.y);
            ry = ry - rx;

            ry = EditorGUI.FloatField(new Rect(_r.x + _r.width - 30, _r.y, 30, _r.height), ry);


            if (values.x != rx || values.y != ry)
            {
                values.x = rx;
                values.y = ry;
                return 1;
            }

            return 0;

        }

        public static bool Label(string label = "")
        {
            if(label == "") { return false; }
            EditorGUI.LabelField(R(), label);
            return true;
        }

        public static bool MiniLabel(string label = "")
        {
            if (label == "") { return false; }
            EditorGUI.LabelField(R(), label, EditorStyles.miniLabel);
            return true;
        }

        public static int FloatField(ref float value, string label = "")
        {

            float input;

            if (label != "")
                input = EditorGUI.FloatField(R(), "Scale", value);
            else
                input = EditorGUI.FloatField(R(), value);

            if (value == input) { return 0; }

            value = input;
            return 1;

        }

        #endregion

        #region Colors

        public static int ColorField(ref Color col, string label = "")
        {
            Color newCol;
            if (label != "")
                newCol = EditorGUI.ColorField(R(),label, col);
            else
                newCol = EditorGUI.ColorField(R(), col);

            if (newCol == col) { return 0; }

            col = newCol;
            return 1;

        }

        public static int InlineColorField(ref Color col, float _xOffset = 0f)
        {
            Rect _r = new Rect(W * _xOffset + 10f, lY - (inLayout ? EditorGUIUtility.singleLineHeight : 0f), 30, EditorGUIUtility.singleLineHeight);
            Color newCol;
            newCol = EditorGUI.ColorField(_r, GUIContent.none, col, false, true, false);

            if (newCol == col) { return 0; }

            col = newCol;
            return 1;

        }

        #endregion

        #region Objects

        public static int ObjectField<T>( ref T obj, string label = "", float width = -1f, bool allowAllObjects = false )
            where T : UnityEngine.Object
        {
            T result = EditorGUI.ObjectField(RW(-1f, width), "", obj, typeof(T), allowAllObjects) as T;

            if(result == obj) { return 0; }

            obj = result;
            return 1;

        }

        #endregion

        #region Misc

        public static bool Foldout(bool open, string label)
        {
            return EditorGUI.Foldout(R(), open, label, true);
        }

        public static void Line(float w = 1f, int i_height = 1)
        {

            R(2f);
            Rect rect = R(i_height);

            rect.x += (rect.width * (1f - w)) * 0.5f;
            rect.width = rect.width * w;

            EditorGUI.DrawRect(rect, new Color(0f, 0f, 0f, 0.5f));
            R(2f);
        }

        public static void Space(float size)
        {
            R(size);
        }

        #endregion

    }
}
