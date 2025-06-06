using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space_1
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ChangeMaterialColor : MonoBehaviour
    {
        public enum ColorType { Red, Green, Blue, Yellow }
        public ColorType targetColor = ColorType.Red;

        void Start()
        {
            var meshFilter = GetComponent<MeshFilter>();
            var mesh = meshFilter.mesh;

            var colors = new Color[mesh.vertexCount];
            Color vertexColor = Color.red;

            switch (targetColor)
            {
                case ColorType.Red:
                    vertexColor = new Color(1, 0, 0); break;
                case ColorType.Green:
                    vertexColor = new Color(0, 1, 0); break;
                case ColorType.Blue:
                    vertexColor = new Color(0, 0, 1); break;
                case ColorType.Yellow:
                    vertexColor = new Color(1, 1, 0); break;
            }

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = vertexColor;
            }

            mesh.colors = colors;
        }
    }
}

