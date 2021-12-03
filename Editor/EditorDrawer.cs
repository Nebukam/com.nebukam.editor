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
    public static partial class EditorDrawer
    {

        public static float lY = 0f;
        public static float Y = 0f;
        public static float X = 0f;
        public static float W = 0f;

        public static bool inLayout = false;

        public static GUIStyle centeredLabel;

        public static GUIStyle miniButtonLeft;
        public static GUIStyle miniButtonMid;
        public static GUIStyle miniButtonRight;
        public static GUIStyle toolbarButton;
        public static GUIStyle miniLabel;
        public static GUIStyle miniDropdown;
        public static GUIStyle miniPopup;

        static EditorDrawer()
        {

            int s = 10;

            centeredLabel = new GUIStyle(GUI.skin.label);
            centeredLabel.fontSize = s;
            centeredLabel.alignment = TextAnchor.MiddleCenter;

            miniButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
            miniButtonLeft.fontSize = s;
            miniButtonMid = new GUIStyle(EditorStyles.miniButtonMid);
            miniButtonMid.fontSize = s;
            miniButtonRight = new GUIStyle(EditorStyles.miniButtonRight);
            miniButtonRight.fontSize = s;

            miniLabel = new GUIStyle(EditorStyles.miniLabel);
            miniLabel.alignment = TextAnchor.LowerLeft;

            toolbarButton = new GUIStyle(EditorStyles.toolbarButton);
            toolbarButton.fontSize = s;

            miniDropdown = new GUIStyle(EditorStyles.toolbarDropDown);
            miniDropdown.fontSize = s;

            miniPopup = new GUIStyle(EditorStyles.popup);
            miniPopup.fontSize = s;

        }

        #region rect

        /// <summary>
        /// Return current draw area
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static Rect __GetCurrentRect(float xOffset = 0f, float yOffset = 0f, float height = -1, float width = -1)
        {
            if (height == -1) { height = EditorGUIUtility.singleLineHeight; }
            return new Rect(X+xOffset,Y+yOffset, width < 0f ? W : width, height);
        }

        /// <summary>
        /// Creates & update the next draw area
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rect __GetRect(float height = -1)
        {
            if (height == -1) { height = EditorGUIUtility.singleLineHeight; }

            Rect r = new Rect(X, Y, W, height);
            lY = Y;
            Y += height;

            if (inLayout)
            GUILayoutUtility.GetRect(W, height, GUIStyle.none);

            return r;
        }

        /// <summary>
        /// Creates & set the next draw area from a given rect
        /// </summary>
        /// <param name="r"></param>
        public static void __SetRect(Rect r)
        {
            X = r.x;
            lY = Y = r.y;
            W = r.width;

           if (inLayout)
           GUILayoutUtility.GetRect(W, r.height, GUIStyle.none);

        }

        #endregion

        #region layout

        private static Rect onColStart;
        private static int colCount;
        private static int colIndex;
        private static float colWidth;
        private static float colSpacing;
        private static float colYMax;

        public static void __BeginCol(int count = 2, float spacing = 5f)
        {
            onColStart = __GetCurrentRect();
            colYMax = Y;
            colIndex = 0;
            colCount = count;
            colSpacing = spacing;
            colWidth = (onColStart.width - (colSpacing * (count - 1))) / count;
            __NextCol();
        }

        public static void __NextCol()
        {
            colYMax = math.max(colYMax, Y);
            __SetRect(new Rect(onColStart.x + ((colWidth + colSpacing) * colIndex), onColStart.y, colWidth, 0f));
            colIndex++;
        }

        public static void __EndCol()
        {
            __SetRect(new Rect(onColStart.x, math.max(colYMax, Y), onColStart.width, 0f));
        }

        public static void __RequireRectUpdate(bool toggle)
        {
            inLayout = toggle;
        }

        private static Rect onInLineStart;
        private static bool inlining = false;
        private static float inlineXOffset;
        private static float inlineYMax;
        public static float WLeft { get{ return onInLineStart.width - inlineXOffset; } }

        public static void __BeginInLine(float width)
        {
            onInLineStart = __GetCurrentRect();
            inlineYMax = Y;
            inlineXOffset = 0f;
            inlining = true;
            __NextInline(width);
        }

        public static void __NextInline(float width)
        {
            inlineYMax = math.max(inlineYMax, Y);
            __SetRect(new Rect(onInLineStart.x + inlineXOffset, onInLineStart.y, width, 0f));
            inlineXOffset += width;
        }

        public static void __EndInLine()
        {
            __SetRect(new Rect(onInLineStart.x, math.max(inlineYMax, Y), onInLineStart.width, 0f));
            inlining = false;
        }

        #endregion

        #region GUI

        private static Color __col_col;
        private static Color __col_bg;
        private static Color __col_cont;

        public static void HoldGUI()
        {
            __col_col = GUI.color;
            __col_bg = GUI.backgroundColor;
            __col_cont = GUI.contentColor;
        }

        public static void RestoreGUI()
        {
            GUI.color = __col_col;
            GUI.backgroundColor = __col_bg;
            GUI.contentColor = __col_cont;
        }

        #endregion

        #region enum

        public static int EnumPopup<T>(ref T e, string label = "")
            where T : Enum
        {
            T r;
            if (label != "")
                r = (T)EditorGUI.EnumPopup(__GetRect(), label, e, miniPopup);
            else
                r = (T)EditorGUI.EnumPopup(__GetRect(), e, miniPopup);

            if (r.Equals(e)) { return 0; }

            e = r;
            return 1;

        }

        public static int EnumPopupInlined<T>(ref T e, string label = "")
            where T : Enum
        {
            
            MiniLabel(label);

            T r = (T)EditorGUI.EnumPopup(__GetRect(), e, miniPopup);

            if (r.Equals(e)) { return 0; }

            e = r;
            return 1;

        }

        public static int EnumInlined<T>(ref T e, bool displayEnumValue = false, string label = "")
            where T : Enum
        {

            T[] enums = (T[])Enum.GetValues(typeof(T));
            string[] labels = new string[enums.Length];
            int currentSelection = 0, i = 0;
            string l;

            foreach (T curenum in enums)
            {
                if (displayEnumValue)
                    l = ""+ Convert.ToInt32(curenum);
                else
                    l = curenum.ToString();

                labels[i] = l;
                if (curenum.Equals(e)) { currentSelection = i; }
                i++;
            }

            MiniLabel(label);

            i = 0;
            int finalSelection = -1;
            
            Rect r = __GetRect();

            Color colTrue = Color.gray;
            Color colFalse = GUI.backgroundColor;
            colFalse.a = 0.25f;

            HoldGUI();

            r.width = r.width / labels.Length;
            GUIStyle style;
            foreach (string elab in labels)
            {
                bool selected = currentSelection == i;
                if (i == 0) { style = miniButtonLeft; }
                else if (i == labels.Length-1) { style = miniButtonMid; }
                else { style = miniButtonRight; }

                GUI.backgroundColor = selected ? colTrue : colFalse;

                if (GUI.Button(r, elab, style)) { finalSelection = i; }
                r.x += r.width;
                i++;
            }

            RestoreGUI();

            if (finalSelection == -1 || finalSelection == currentSelection) { return 0; }

            e = enums[finalSelection];

            return 1;

        }

        public static int EnumFlagsField<T>(ref T e, string label = "")
            where T : Enum
        {
            MiniLabel(label);
            T r = (T)EditorGUI.EnumFlagsField(__GetRect(), e);

            if (r.Equals(e)) { return 0; }

            e = r;
            return 1;
        }

        #endregion

        #region bool

        public static int Checkbox(ref bool value, string label)
        {
            bool input = EditorGUI.Toggle(__GetRect(), label, value);
            if (input == value) { return 0; }
            value = input;
            return 1;
        }

        #endregion

        #region string

        public static int TextInput(ref string value, string label = "")
        {
            string input;
            if (label != "")
                input = EditorGUI.TextField(__GetRect(), label, value);
            else
                input = EditorGUI.TextField(__GetRect(), value);

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
                input = EditorGUI.IntSlider(__GetRect(), label, value, min, max);
            else
                input = EditorGUI.IntSlider(__GetRect(), value, min, max);

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

            Rect _r = __GetRect();

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

            Rect _r = __GetRect();

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

        public static int IntField(ref int value, string label = "")
        {

            int input;

            MiniLabel(label);
            float b = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 20f;
            input = EditorGUI.IntField(__GetRect(), "·: :·", value);
            EditorGUIUtility.labelWidth = b;
            if (value == input) { return 0; }

            value = input;
            return 1;

        }

        public static int IntFieldInline(ref int value, string label = "")
        {

            int input;

            if (label != "")
                input = EditorGUI.IntField(__GetRect(), label, value);
            else
                input = EditorGUI.IntField(__GetRect(), value);

            if (value == input) { return 0; }

            value = input;
            return 1;

        }

        #endregion

        #region float

        public static int Slider(ref float value, float min, float max, string label = "")
        {
            
            Rect _r = __GetRect();

            float result = EditorGUI.Slider(_r, value, min, max);

            if (result == value) { return 0; }

            value = result;
            return 1;

        }

        public static int MinMaxSlider(ref float2 values, float2 range, string label = "")
        {
            float
                rx = values.x,
                ry = values.y;


            MiniLabel(label);

            Rect _r = __GetRect();

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

            Rect _r = __GetRect();

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

        public static int FloatFieldInline(ref float value, string label = "")
        {

            float input;

            if (label != "")
                input = EditorGUI.FloatField(__GetRect(), label, value);
            else
                input = EditorGUI.FloatField(__GetRect(), value);

            if (value == input) { return 0; }

            value = input;
            return 1;

        }

        public static int FloatField(ref float value, string label = "")
        {

            float input;

            MiniLabel(label);
            float b = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 20f;
            input = EditorGUI.FloatField(__GetRect(), "·: :·", value);
            EditorGUIUtility.labelWidth = b;
            if (value == input) { return 0; }

            value = input;
            return 1;

        }

        public static int FloatFieldClamped(ref float value, float min, float max, string label = "")
        {

            float input;

            MiniLabel(label);
            float b = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 20f;
            input = math.clamp(EditorGUI.FloatField(__GetRect(), "·: :·", value), min, max);
            EditorGUIUtility.labelWidth = b;
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
                newCol = EditorGUI.ColorField(__GetRect(),label, col);
            else
                newCol = EditorGUI.ColorField(__GetRect(), col);

            if (newCol == col) { return 0; }

            col = newCol;
            return 1;

        }

        public static int ColorFieldInlined(ref Color col)
        {

            Color newCol;
            newCol = EditorGUI.ColorField(__GetRect(), GUIContent.none, col, false, true, false);

            if (newCol == col) { return 0; }

            col = newCol;
            return 1;

        }

        #endregion

        #region Buttons

        public static bool Button(string label)
        {
            Rect r = __GetRect();
            return GUI.Button(r, label);
        }

        #endregion

        #region Objects

        public static int ObjectField<T>( ref T obj, string label = "", bool allowAllObjects = false )
            where T : UnityEngine.Object
        {
            T result = EditorGUI.ObjectField(__GetRect(), label, obj, typeof(T), allowAllObjects) as T;

            if(result == obj) { return 0; }

            obj = result;
            return 1;

        }

        #endregion

        #region Misc

        public static bool Label(string label = "")
        {
            if (label == "") { return false; }
            EditorGUI.LabelField(__GetRect(), label);
            return true;
        }

        public static bool MiniLabel(string label = "")
        {
            if (label == "") { return false; }
            Rect _r = __GetRect(EditorGUIUtility.singleLineHeight + 5f);
            _r.y -= 4f;
            EditorGUI.LabelField(_r, label, miniLabel);
            return true;
        }

        public static bool Foldout(bool open, string label)
        {
            return EditorGUI.Foldout(__GetRect(), open, label, true);
        }

        public static void Line(float w = 1f, int i_height = 1)
        {

            __GetRect(2f);
            Rect rect = __GetRect(i_height);

            rect.x += (rect.width * (1f - w)) * 0.5f;
            rect.width = rect.width * w;

            EditorGUI.DrawRect(rect, new Color(0f, 0f, 0f, 0.5f));
            __GetRect(2f);
        }

        public static void Space(float size)
        {
            __GetRect(size);
        }

        public static void Separator(float size)
        {
            Rect r = __GetRect(size);
            r.y += size * 0.5f - 1f;
            r.height = 1f;
            EditorGUI.DrawRect(r, new Color(0f, 0f, 0f, 0.5f));
        }

        public static void Separator(float size, Color col)
        {
            Rect r = __GetRect(size);
            r.y += size * 0.5f - 1f;
            r.height = 1f;
            EditorGUI.DrawRect(r, col);
        }

        #endregion

    }
}
