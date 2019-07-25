using System.Collections.Generic;
using UnityEngine;

namespace GUC {

    public static class Rand {

        /// <summary>
        /// Returns gaussian distributed random number
        /// </summary>
        public static float Gaussian(float mean, float std) {
            return Gaussian() * std + mean;
        }

        /// <summary>
        /// Returns randomly generated number from the 
        /// standard normal distribution.
        /// </summary>
        public static float Gaussian() {
            //box muller algorithm
            float r = -2f * Mathf.Log(Random.value);
            float th = 2f * Mathf.PI * Random.value;
            return Mathf.Sqrt(r) * Mathf.Sin(th);
        }

        /// <summary>
        /// Returns random entry from given array
        /// </summary>
        public static T GetRandom<T>(T[] array) {
            if(array == null || array.Length == 0)
                return default(T);
            return array[Random.Range(0, array.Length)];
        }


        /// <summary>
        /// Plays one clip of the given array at random.
        /// Playing is done via OneShot, allowing multiple sounds
        /// to overlap.
        /// </summary>
        public static void PlayOneShotRandom(this AudioSource sound, AudioClip[] clips) {
            sound.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }

        /// <summary>
        /// Returns a random vector that is a given distance away from the source vector,
        /// while still lying inside the unit square with padding around the sides.
        /// </summary>
        /// <param name="source">Source point from which to take the step</param>
        /// <param name="distance">Distance of the step</param>
        /// <param name="padding">Width of the exclusion region around the square perimeter</param>
        public static Vector2 TakeRandomStepInPaddedRect(Vector2 source, float distance, float padding, int retries = 25) {
            Vector2 toReturn;
            do {
                Vector2 step = distance * Random.insideUnitCircle.normalized;
                toReturn = source + step;
                retries--;
            } while(!Rects.InPaddedRect(toReturn, padding) && retries > 0);

            if(retries == 0) {
                float x = Random.Range(padding, 1f - padding);
                float y = Random.Range(padding, 1f - padding);
                toReturn = new Vector2(x, y);
            }
            return toReturn;
        }

        /// <summary>
        /// return an element of the given list at random.
        /// </summary>
        public static T GetRandom<T>(List<T> list) {
            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }

    }

}
