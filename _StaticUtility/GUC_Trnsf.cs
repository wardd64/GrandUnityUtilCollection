using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GUC {

    public static class Trnsf {

        /// <summary>
        /// Returns first instance of the given component in the chain of 
        /// parents of this transform. Search does not include the transform itself.
        /// </summary>
        public static T GetComponentInParentExclusive<T>(this Transform source) {
            T toReturn = default;
            bool foundComp = false;
            while((source = source.parent) != null && !foundComp) {
                toReturn = source.GetComponent<T>();
                source = source.parent;
                foundComp = !EqualityComparer<T>.Default.Equals(toReturn, default);
            }
            return toReturn;
        }

        /// <summary>
        /// Returns all objects of a given type currently loaded, regardless of 
        /// whether the objects they are attached to are active or not.
        /// </summary>
        public static List<T> GetAllObjectsOfType<T>() {
            int sceneCount = SceneManager.sceneCount;
            List<GameObject> roots = new List<GameObject>();

            for(int i = 0; i < sceneCount; i++) {
                GameObject[] sceneRoots = SceneManager.GetSceneAt(i).GetRootGameObjects();
                roots.AddRange(sceneRoots);
            }

            List<T> toReturn = new List<T>();

            for(int i = 0; i < roots.Count; i++) {
                List<T> nextList = new List<T>();
                roots[i].GetComponentsInChildren(true, nextList);
                toReturn.AddRange(nextList);
            }

            return toReturn;
        }

        /// <summary>
        /// Returns true if any parent of the given transform, all 
        /// the way up to the root is the given parent.
        /// Also returns true if the child and parent objects are equal.
        /// </summary>
        public static bool IsParentInHierarchy(this Transform child, Transform parent) {
            if(parent == null)
                return false;
            Transform check = child;

            while(check != null) {
                if(check == parent)
                    return true;
                check = check.parent;
            }

            return false;
        }

        /// <summary>
        /// Copies values of the other component to the first one and returns it
        /// </summary>
        public static T GetCopyOf<T>(T t, T other) where T : Component {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.Default |
                BindingFlags.DeclaredOnly;

            PropertyInfo[] pinfos = typeof(T).GetProperties(flags);
            foreach(var pinfo in pinfos) {
                if(pinfo.CanWrite) {
                    try {
                        pinfo.SetValue(t, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }

            FieldInfo[] finfos = typeof(T).GetFields(flags);
            foreach(var finfo in finfos) {
                finfo.SetValue(t, finfo.GetValue(other));
            }
            return t as T;
        }

        /// <summary>
        /// Copies the given component over to the given gameobject and returns it
        /// </summary>
        public static T AddComponent<T>(GameObject go, T toAdd) where T : Component {
            T t = go.AddComponent<T>();
            return GetCopyOf(t, toAdd);
        }


        /// <summary>
        /// Returns a path string that can be used to find the same transform 
        /// in a scene with the same hierarchy, when it has unloaded and loaded again.
        /// </summary>
        public static string GetIndexPath(this Transform t) {
            if(t == null)
                return "";
            StringBuilder path = new StringBuilder();

            //write sibling positions
            while(t.parent != null) {
                path.Insert(0, "/" + t.GetSiblingIndex());
                t = t.parent;
            }

            //find and write root position
            int firstIndex = 0;
            GameObject[] roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            for(int i = 0; i < roots.Length; i++) {
                if(roots[i].transform == t)
                    firstIndex = i;
            }
            path.Insert(0, firstIndex);

            return path.ToString();
        }

        /// <summary>
        /// Returns Transform at given path. 
        /// Complementary to GetTransformPath(transform);
        /// </summary>
        public static Transform FindFromIndexPath(string path) {
            if(string.IsNullOrEmpty(path))
                return null;

            //get indices
            string[] indices = path.Split('/');

            //get correct root
            GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
            Transform t = roots[int.Parse(indices[0])].transform;

            //work way down hierarchy
            for(int i = 1; i < indices.Length; i++)
                t = t.GetChild(int.Parse(indices[i]));
            return t;
        }

        /// <summary>
        /// Resets the local transform parameters to their default values.
        /// </summary>
        public static void ResetLocal(this Transform transform) {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }


        /// <summary>
        /// Sets given object and all of its children to be static or not.
        /// </summary>
        public static void SetStaticRecursively(this GameObject g, bool value) {
            for(int i = 0; i < g.transform.childCount; i++)
                SetStaticRecursively(g.transform.GetChild(i).gameObject, value);
            g.isStatic = value;
        }

        /// <summary>
        /// Sets given object and all of its children to the given layer.
        /// </summary>
        public static void SetLayerRecursively(this GameObject g, int layer) {
            for(int i = 0; i < g.transform.childCount; i++)
                SetLayerRecursively(g.transform.GetChild(i).gameObject, layer);
            g.layer = layer;
        }

        /// <summary>
        /// Sets all of the children of the given transform
        /// to the default local transform.
        /// </summary>
        public static void ResetChildrenRecursively(this Transform t) {
            for(int i = 0; i < t.childCount; i++)
                t.GetChild(i).ResetRecursively();
        }

        /// <summary>
        /// Sets given transform and all of its children to default 
        /// local transform.
        /// </summary>
        public static void ResetRecursively(this Transform t) {
            for(int i = 0; i < t.childCount; i++)
                ResetRecursively(t.GetChild(i));
            t.ResetLocal();
        }

        /// <summary>
        /// Makes local scale uniform.
        /// The uniform scale value will be the 'odd one out' 
        /// if appropriate. In this way, you can use this method to keep 
        /// the scale of the transform uniform while a user edits 
        /// the components of the scale in the editor.
        /// </summary>
        public static void EqualizeScale(this Transform t) {
            Vector3 scale = t.localScale.EqualizeScale();
            t.localScale = scale;
        }

        /// <summary>
        /// Return true if the local transform variables are all
        /// their default values
        /// </summary>
        public static bool IsIdentity(this Transform t) {
            if(t.localPosition != Vector3.zero)
                return false;
            if(t.localRotation != Quaternion.identity)
                return false;
            return t.localScale == Vector3.one;
        }

    }

}
