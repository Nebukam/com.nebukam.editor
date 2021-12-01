using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;
using static Nebukam.Editor.EditorDrawer;

namespace Nebukam.Editor
{
    public static partial class EditorGLDrawer
    {

        private static Dictionary<Shader, Material> m_mats = new Dictionary<Shader, Material>();
        private static Material m_currentGLMat = null;
        public static Rect GLArea;

        public static void SetGLShader(Shader shader)
        {
            if (!m_mats.TryGetValue(shader, out m_currentGLMat) || m_currentGLMat == null)
            {
                m_currentGLMat = new Material(shader);
                m_mats[shader] = m_currentGLMat;
            }
        }

        public static void SetGLShader(string shader)
        {
            Shader s = Shader.Find(shader);
            if (s == null) { s = Shader.Find("Hidden/Internal-Colored"); }
            SetGLShader(s);
        }

        private static bool __GLACTIVE = false;

        public static bool BeginGL(float height = -1)
        {

            GLArea = __GetRect(height);

            if (Event.current.type != EventType.Repaint) { return false; }

            GUI.BeginClip(GLArea);

            if (m_currentGLMat == null)
                SetGLShader("Hidden/Internal-Colored");

            GL.Flush();
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);

            m_currentGLMat.SetPass(0);

            return true;

        }

        public static void GLCol(Color col)
        {
            GL.Color(col);
        }

        public static void GLCol(Color col, float alpha)
        {
            col.a = alpha;
            GL.Color(col);
        }

        public static void GLFill(Color col)
        {
            GL.Begin(GL.QUADS);
            GL.Color(col);
            GLRect(new Rect(0, 0, GLArea.width, GLArea.height));
            GL.End();
        }

        public static void GLRect(Rect rect, bool close = false)
        {
            GL.Vertex3(rect.x, rect.y, 0);
            GL.Vertex3(rect.x + rect.width, rect.y, 0);
            GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0);
            GL.Vertex3(rect.x, rect.y + rect.height, 0);

            if (close)
                GL.Vertex3(rect.x, rect.y, 0);

        }

        public static void EndGL()
        {
            GL.End();
            GL.PopMatrix();
            GUI.EndClip();
        }

    }
}
