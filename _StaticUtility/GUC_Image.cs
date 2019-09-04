using UnityEngine;

namespace GUC {

    public static class Image {

        /// <summary>
        /// Convert a pair of pixel indices to a pair of normalized coordinates 
        /// of the center of the corresponding pixel (constrained between 0 and 1).
        /// </summary>
        public static Vector2 PixelToCenterNormalized(Vector2 input, Texture2D texture) {
            Vector2 texScale = GetDimensions(texture);
            return input.Shift(.5f).InverseScale(texScale);
        }

        /// <summary>
        /// Convert a pair of normalized coordinates to pixel scale coordinates.
        /// Cast the coordinates from the output of this function to integers 
        /// if you want to retrieve the pixel in which the given normalized point lies
        /// </summary>
        public static Vector2 NormalizedToPixel(Vector2 input, Texture2D texture) {
            Vector2 texScale = GetDimensions(texture);
            return input.NewScale(texScale);
        }

        public static Rect NormalizedToPixel(Rect input, Texture2D texture) {
            Vector2 texScale = GetDimensions(texture);
            Vector2 position = NormalizedToPixel(input.position, texture);
            Vector2 size = input.size.NewScale(texScale);
            return new Rect(position, size);
        }

        /// <summary>
        /// Returns Vector containg width and height of the given texture
        /// </summary>
        public static Vector2 GetDimensions(this Texture texture) {
            return new Vector2(texture.width, texture.height);
        }

        /// <summary>
        /// same as GetPixelBilinear but takes 
        /// a vector of the coordinates as input.
        /// </summary>
        public static Color GetPixelBilinear(this Texture2D texture, Vector2 coordinates) {
            return texture.GetPixelBilinear(coordinates.x, coordinates.y);
        }

        /// <summary>
        /// Get a pixel based on pixel scale input coordinates provided 
        /// in a vector. Works the same as the standard GetPixel if the 
        /// vector is constructed from integer values.
        /// </summary>
        public static Color GetPixel(this Texture2D texture, Vector2 coordinates) {
            return texture.GetPixel((int)coordinates.x, (int)coordinates.y);
        }

        /// <summary>
        /// Same as SetPixel but takes a vector as input coordinates.
        /// Conversion is done via default trunctation, making this method 
        /// compatible with a standard, pixel scale coordinate grid.
        /// </summary>
        public static void SetPixel(this Texture2D texture, Vector2 coordinates, Color value) {
            texture.SetPixel((int)coordinates.x, (int)coordinates.y, value);
        }

        /// <summary>
        /// Create a new blank texture of the same format as the given input texture
        /// </summary>
        public static Texture2D GenerateBlankTexture(Texture template) {
            return GenerateBlankTexture(template.width, template.height);
        }

        /// <summary>
        /// Generate a new blank texture with width and height equal to size
        /// </summary>
        public static Texture2D GenerateBlankTexture(int size) {
            return GenerateBlankTexture(size, size);
        }

        /// <summary>
        /// Generate a new blank texture with standardized formats, 
        /// making sure copy and blit will work without problems.
        /// </summary>
        public static Texture2D GenerateBlankTexture(int width, int height) {
            Texture2D toReturn = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Clear(toReturn);
            toReturn.wrapMode = TextureWrapMode.Clamp;
            toReturn.filterMode = FilterMode.Point;
            return toReturn;
        }

        /// <summary>
        /// Set all pixels of the given texture to the given color
        /// </summary>
        public static void FillColor(Texture2D tex, Color color) {
            int nPixels = tex.width * tex.height;
            Color[] pixels = new Color[nPixels];
            for(int i = 0; i < nPixels; i++)
                pixels[i] = color;
            tex.SetPixels(pixels);
            tex.Apply();
        }

        /// <summary>
        /// Set all pixels of the given texture to transparant black.
        /// </summary>
        public static void Clear(Texture2D texture) {
            texture.SetPixels(new Color[texture.width * texture.height]);
            texture.Apply();
        }



        /// <summary>
        /// Returns a 2D array of Vector4 values that encode the average 
        /// r,g,b,a values of the given texture over a square grid.
        /// This is useful for doing pixel perfect texture downscaling.
        /// </summary>
        /// <param name="tex">input texture</param>
        /// <param name="newWidth">Width of the output</param>
        /// <param name="newHeight">Height of the output</param>
        public static Vector4[,] AveragePixelValues(Texture2D tex, int newWidth, int newHeight) {

            float ratioX = ((float)tex.width) / newWidth;
            float ratioY = ((float)tex.height) / newHeight;
            Color[] texColors = tex.GetPixels();
            Vector4[,] toReturn = new Vector4[newWidth, newHeight];

            for(int y = 0; y < newHeight; y++) {
                int yw = y * newWidth;

                for(int x = 0; x < newWidth; x++) {
                    Rect exactBounds = new Rect(x * ratioX, y * ratioY, ratioX, ratioY);
                    Rect bounds = Rects.BoundingRect(exactBounds);

                    //average all colors from original texture that overlap with the new pixel
                    Vector4 sum = Vector4.zero;
                    float norm = 0f;

                    for(int i = (int)bounds.xMin; i < bounds.xMax; i++) {
                        float xFrac = GetFrac(i, (int)bounds.xMin, (int)bounds.xMax,
                            exactBounds.xMin, exactBounds.xMax);

                        for(int j = (int)bounds.yMin; j < bounds.yMax; j++) {
                            float yFrac = GetFrac(j, (int)bounds.yMin, (int)bounds.yMax,
                                exactBounds.yMin, exactBounds.yMax);

                            float frac = xFrac * yFrac;
                            Color c = texColors[i + (j * tex.width)];
                            sum = AddColor(sum, frac, c);
                            norm += frac;
                        }
                    }

                    toReturn[x, y] = sum / norm;
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Returns a value corresponding to the fraction of area 
        /// of a certain pixel that overlaps with a square sampling grid.
        /// This function handles one dimension, to get the actual fraction 
        /// you must multiply results in x and y directions.
        /// </summary>
        /// <param name="pix">pixel coordinate of the pixel from the source texture in question</param>
        /// <param name="min">lowest pixel coordinate that (partially) overlaps with the sampling square</param>
        /// <param name="max">highest pixel coordinate that (partially) overlaps with the sampling square</param>
        /// <param name="exactMin">The exact pixel coordinate value of the start of the sampling square</param>
        /// <param name="exactMax">The exact pixel coordinate value of the end of the sampling square</param>
        /// <returns></returns>
        private static float GetFrac(int pix, int min, int max, float exactMin, float exactMax) {
            float toReturn = 1f;
            if(pix == min)
                toReturn -= exactMin - min;
            if(pix == max)
                toReturn -= max - exactMax;
            return toReturn;
        }

        /// <summary>
        /// Add r,g,b,a values of a color to the given sum vector.
        /// The added values are scaled by frac.
        /// </summary>
        /// <returns></returns>
        private static Vector4 AddColor(Vector4 sum, float frac, Color c) {
            return new Vector4(
                sum.x + frac * c.r,
                sum.y + frac * c.g,
                sum.z + frac * c.b,
                sum.w + frac * c.a);
        }

        /// <summary>
        /// Add rgb values of toAdd to those of sum.
        /// </summary>
        public static void AddTextures(Texture2D sum, Texture2D toAdd) {
            Color[] colors = sum.GetPixels();
            Color[] aColors = toAdd.GetPixels();
            for(int i = 0; i < colors.Length; i++)
                colors[i] += aColors[i];
            sum.SetPixels(colors);
            sum.Apply();
        }

    }

}
