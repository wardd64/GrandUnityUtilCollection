using UnityEngine;

namespace GUC {

    public static class Meshs {

        /// <summary>
        /// Returns a centered rectangle mesh with the given x, y extents.
        /// </summary>
        public static Mesh MakeQuad(Vector3 extents) {
            float width = extents.x;
            float height = extents.y;
            return MakeQuad(width, height);
        }

        /// <summary>
        /// Returns a centered rectangle mesh (a quad) with the given width and height.
        /// </summary>
        public static Mesh MakeQuad(float width, float height) {
            Mesh mesh = new Mesh();
            mesh.name = "StretchedQuad";

            Vector3[] vertices = new Vector3[4];
            float x = width / 2f;
            float y = height / 2f;
            vertices[0] = new Vector3(-x, -y, 0);
            vertices[1] = new Vector3(x, -y, 0);
            vertices[2] = new Vector3(-x, y, 0);
            vertices[3] = new Vector3(x, y, 0);

            mesh.vertices = vertices;
            int[] tri = new int[] { 0, 2, 1, 2, 3, 1 };
            mesh.triangles = tri;

            Vector3[] normals = new Vector3[4];
            for(int i = 0; i < 4; i++)
                normals[i] = -Vector3.forward;
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);
            mesh.uv = uv;

            return mesh;
        }

    }

}
