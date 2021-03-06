﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Used to subdivide polynomials in triangular faces. 
/// This is usefull for some applications in generating 3D meshes.
/// </summary>
public class Triangulator {

    public static int[] BasePoint(Vector3[] points) {

        int nbPoints = points.Length;
        int nboFaces = nbPoints - 2;

        if(nboFaces < 0)
            return new int[0];

        int[] toReturn = new int[nboFaces * 3];

        for(int i = 0; i < nboFaces; i++) {
            toReturn[3 * i] = 0;
            toReturn[(3 * i) + 1] = i + 1;
            toReturn[(3 * i) + 2] = i + 2;
        }

        return toReturn;
    }

    public static int[] ProjectBasic(Vector3[] points) {
        throw new System.Exception("Not implemented");
    }

    /* 
     * Basic 2D triangulation algorithm, 
     * could be used for stronger 3D algorithm by using plane projections
     * 
     */

    private List<Vector2> m_points = new List<Vector2>();

    public Triangulator(Vector2[] points) {
        m_points = new List<Vector2>(points);
    }

    public int[] Triangulate() {
        List<int> indices = new List<int>();

        int n = m_points.Count;
        if(n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if(Area() > 0) {
            for(int v = 0; v < n; v++)
                V[v] = v;
        }
        else {
            for(int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for(int m = 0, v = nv - 1; nv > 2;) {
            if((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if(nv <= u)
                u = 0;
            v = u + 1;
            if(nv <= v)
                v = 0;
            int w = v + 1;
            if(nv <= w)
                w = 0;

            if(Snip(u, v, w, nv, V)) {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for(s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private float Area() {
        int n = m_points.Count;
        float A = 0.0f;
        for(int p = n - 1, q = 0; q < n; p = q++) {
            Vector2 pval = m_points[p];
            Vector2 qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] V) {
        int p;
        Vector2 A = m_points[V[u]];
        Vector2 B = m_points[V[v]];
        Vector2 C = m_points[V[w]];
        if(Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for(p = 0; p < n; p++) {
            if((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = m_points[V[p]];
            if(InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P) {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x;
        ay = C.y - B.y;
        bx = A.x - C.x;
        by = A.y - C.y;
        cx = B.x - A.x;
        cy = B.y - A.y;
        apx = P.x - A.x;
        apy = P.y - A.y;
        bpx = P.x - B.x;
        bpy = P.y - B.y;
        cpx = P.x - C.x;
        cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }

    public static bool VertexInTriangle(Vector3 vertex, Vector3 v1, Vector3 v2, Vector3 v3, float maxDelta) {
        Vector3[] triangle = new Vector3[] { v1, v2, v3 };
        float deltaSqr = maxDelta * maxDelta;

        //check if near points themselves
        foreach(Vector3 v in triangle) {
            if((vertex - v).sqrMagnitude <= deltaSqr)
                return true;
        }

        //check if near edges
        for(int i = 0; i < 3; i++) {
            Vector3 start = triangle[i];
            Vector3 end = triangle[(i + 1) % 3];
            Vector3 dir = (end - start).normalized;
            bool afterStart = Vector3.Dot(vertex - start, dir) > 0f;
            bool beforeEnd = Vector3.Dot(vertex - end, dir) < 0f;
            float lineD = Vector3.Cross(dir, vertex - start).magnitude;
            if(afterStart && beforeEnd && lineD <= maxDelta)
                return true;
        }

        //check if near plane
        bool inTriangle = SameSide(vertex, v1, v2, v3) && SameSide(vertex, v2, v1, v3) && SameSide(vertex, v3, v1, v2);
        Plane plane = new Plane(v1, v2, v3);
        float d = Mathf.Abs(plane.GetDistanceToPoint(vertex));
        return inTriangle && d <= maxDelta;
    }

    private static bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b) {
        Vector3 cp1 = Vector3.Cross(b - a, p1 - a);
        Vector3 cp2 = Vector3.Cross(b - a, p2 - a);
        return Vector3.Dot(cp1, cp2) >= 0;
    }
}