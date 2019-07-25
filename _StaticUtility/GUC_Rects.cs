using UnityEngine;
using UnityEngine.EventSystems;

namespace GUC {

    public static class Rects {

        /// <summary>
        /// Returns the screen space Rect of the given Transform (assumed RectTransform),
        /// taking into account scaling and anchorposition.
        /// </summary>
        public static Rect GetScreenRect(this Transform transform) {
            return GetScreenRect(transform.GetComponent<RectTransform>());
        }

        /// <summary>
        /// Returns the screen space Rect of the given RectTransform,
        /// taking into account scaling, anchorposition and pivotting.
        /// </summary>
        public static Rect GetScreenRect(this RectTransform transform) {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
            rect.x -= (transform.pivot.x * size.x);
            rect.y -= ((1.0f - transform.pivot.y) * size.y);
            return rect;
        }

        /// <summary>
        /// Returns screen space Rect that spans given transform (assumed RectTransform),
        /// and all of its (direct) children.
        /// </summary>
        public static Rect GetSpanningRect(this Transform transform) {
            Rect toReturn = GetScreenRect(transform);
            for(int i = 0; i < transform.childCount; i++) {
                Rect childRect = GetScreenRect(transform.GetChild(i));
                float minX = Mathf.Min(toReturn.xMin, childRect.xMin);
                float maxX = Mathf.Max(toReturn.xMax, childRect.xMax);
                float minY = Mathf.Min(toReturn.yMin, childRect.yMin);
                float maxY = Mathf.Max(toReturn.yMax, childRect.yMax);
                toReturn = new Rect(minX, minY, maxX - minX, maxY - minY);
            }
            return toReturn;
        }

        /// <summary>
        /// Returns a Rect which is the input scaled by scale.
        /// </summary>
        public static Rect Scale(this Rect input, Vector2 scale) {
            Vector2 newPos = input.position.NewScale(scale);
            Vector2 newSize = input.size.NewScale(scale);
            return new Rect(newPos, newSize);
        }

        /// <summary>
        /// Creates a rect based on the center point and size
        /// </summary>
        /// <param name="center">center point of the rect</param>
        /// <param name="size">total width and height of the rect</param>
        public static Rect CenterRect(Vector2 center, Vector2 size) {
            return new Rect(center - .5f * size, size);
        }

        /// <summary>
        /// Creates a rect based on the center point and size
        /// </summary>
        /// <param name="x">Center x coordinate</param>
        /// <param name="y">Center y coordinate</param>
        /// <param name="width">total width of the rect</param>
        /// <param name="height">total height of the rect</param>
        /// <returns></returns>
        public static Rect CenterRect(float x, float y, float width, float height) {
            return new Rect(x - .5f * width, y - .5f * height, width, height);
        }

        /// <summary>
        /// Get smallest rect with integer bounds that encompasses the given rect.
        /// </summary>
        public static Rect BoundingRect(Rect input) {
            int xStart = Mathf.FloorToInt(input.xMin);
            int xEnd = Mathf.CeilToInt(input.xMax);
            int yStart = Mathf.FloorToInt(input.yMin);
            int yEnd = Mathf.CeilToInt(input.yMax);
            Vector2 position = new Vector2(xStart, yStart);
            Vector2 size = new Vector2(xEnd - xStart, yEnd - yStart);
            return new Rect(position, size);
        }

        /// <summary>
        /// Get smallest rect with integer bounds that encompasses the given rect,
        /// but is also bound by the dimensions of the given texture
        /// </summary>
        public static Rect BoundingRect(Rect input, Texture2D texture) {
            int xStart = Mathf.Max(0, Mathf.FloorToInt(input.xMin));
            int xEnd = Mathf.Min(texture.width, Mathf.CeilToInt(input.xMax));
            int yStart = Mathf.Max(0, Mathf.FloorToInt(input.yMin));
            int yEnd = Mathf.Min(texture.height, Mathf.CeilToInt(input.yMax));
            Vector2 position = new Vector2(xStart, yStart);
            Vector2 size = new Vector2(xEnd - xStart, yEnd - yStart);
            return new Rect(position, size);
        }




        /// <summary>
        /// Get screen pixel coorinates of the given pointer event. 
        /// EventData must come from the pointer e.g. click, drag
        /// </summary>
        public static Vector2 GetScreenPoint(BaseEventData e) {
            return ((PointerEventData)e).position;
        }

        /// <summary>
        /// Transforms screen pixel coordinates to localposition coordinates 
        /// of the given rect transform.
        /// </summary>
        public static Vector2 GetRelativePosition(Vector2 screenPoint, RectTransform panel) {
            Vector2 toReturn;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    panel, screenPoint, null, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms screen pixel coordinates to normalized coordinates
        /// in the given panel (ranging from 0 to 1 in both axes).
        /// </summary>
        public static Vector2 GetNormalizedPosition(Vector2 screenPoint, RectTransform panel) {
            return GetRelativePosition(screenPoint, panel).InverseScale(panel.rect.size);
        }

        /// <summary>
        /// Get normalized position of the center of the given (Rect)Transform, 
        /// relative to its parent
        /// </summary>
        public static Vector2 GetNormalizedPosition(Transform input) {
            RectTransform parent = input.parent.GetComponent<RectTransform>();
            return GetNormalizedPosition(input.GetComponent<RectTransform>(), parent);
        }

        /// <summary>
        /// Get normalized position of the center of the given transform, relative to 
        /// the given panel
        /// </summary>
        public static Vector2 GetNormalizedPosition(RectTransform input, RectTransform parent) {
            Vector2 parentPivotOffset = parent.pivot.NewScale(parent.rect.size);
            Vector2 pivotOffset = input.pivot.Shift(-.5f).NewScale(input.rect.size);
            Vector2 shift = (Vector2)input.localPosition - pivotOffset + parentPivotOffset;
            return shift.InverseScale(parent.rect.size);
        }

        /// <summary>
        /// Set normalized position of the center of the given (Rect)Transform 
        /// relative to its parent.
        /// </summary>
        public static void SetNormalizedPosition(Transform input, Vector2 position) {
            SetNormalizedPosition(input.GetComponent<RectTransform>(), position);
        }

        public static void SetNormalizedX(Transform input, float x) {
            Vector2 position = GetNormalizedPosition(input);
            position.x = x;
            SetNormalizedPosition(input, position);
        }

        public static void SetNormalizedY(Transform input, float y) {
            Vector2 position = GetNormalizedPosition(input);
            position.y = y;
            SetNormalizedPosition(input, position);
        }

        /// <summary>
        /// Set normalized position of the center of the given RectTransform 
        /// relative to its parent.
        /// </summary>
        private static void SetNormalizedPosition(RectTransform input, Vector2 position) {
            RectTransform parent = input.parent.GetComponent<RectTransform>();
            Vector2 parentPivotOffset = parent.pivot.NewScale(parent.rect.size);
            Vector2 pivotOffset = input.pivot.Shift(-.5f).NewScale(input.rect.size);
            Vector2 shift = position.NewScale(parent.rect.size);
            input.localPosition = shift + pivotOffset - parentPivotOffset;
        }


        /// <summary>
        /// Convert local coordinates relative to source transform 
        /// to local coordinates relative to target transform.
        /// This is done via normalized scale: e.g. top right corner
        /// of source maps to top right corner of target etc.
        /// </summary>
        public static Vector2 TransformRelative(Vector2 position, RectTransform source, RectTransform target) {
            Vector2 normalizedPosition = position.InverseScale(source.rect.size);
            return normalizedPosition.NewScale(target.rect.size);
        }

        /// <summary>
        /// True if given vector lies in the unit square, ranging from 0 to 1.
        /// </summary>
        public static bool InRect01(Vector2 input) {
            return input.x >= 0f
                && input.x <= 1f
                && input.y >= 0f
                && input.y <= 1f;
        }

        /// <summary>
        /// True if given vector lies in the unit square, with at 
        /// least a distance of 'padding' from each of its sides.
        /// </summary>
        public static bool InPaddedRect(Vector2 input, float padding) {
            return input.x >= padding
                && input.x <= 1f - padding
                && input.y >= padding
                && input.y <= 1f - padding;
        }

        public static Rect GetRect(GameObject g) {
            return g.GetComponent<RectTransform>().rect;
        }

        public static float RectWidth(GameObject g) {
            return GetRect(g).width;
        }

        public static float RectHeight(GameObject g) {
            return GetRect(g).height;
        }

        public static Vector2 RectSize(GameObject g) {
            return GetRect(g).size;
        }

        public static Rect GetRect(Component c) {
            return c.GetComponent<RectTransform>().rect;
        }

        public static float RectWidth(Component c) {
            return GetRect(c).width;
        }

        public static float RectHeight(Component c) {
            return GetRect(c).height;
        }

        public static Vector2 RectSize(Component c) {
            return GetRect(c).size;
        }

    }

}
