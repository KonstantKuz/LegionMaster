// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("EHVLqVbCQARKmg/QAku79ZmLGOP4Ev6X0YlNX+oYUna/wv4IIYZi7J8iMKTqQM+sRegA9kULnEIS1ksE4pPgF+pYBDKeKW8fn8wXfehlIlJbA3S4GNccRIcge3EdNJPE97FjH1wvYwI2YWjH9LPM6tmZLiGfTiCdmtoCF4AwKJsoZw9qj1lEWEzzmKQ+hzSzpB/ALZjsh5mvsbg69XyXF4YFCwQ0hgUOBoYFBQT3Va6Snuick6lJKSL+FHYTy+eYmCTLyS3/5A7xeICio+/4RKwVej3wvl1HYwgJIjSGBSY0CQINLoJMgvMJBQUFAQQHI8OQ+jx3f2tYnilYcK6rAY6C0/Ajq8vhQne0qWLr1ozBUQ5syS5kAWvfOce+MkkYPwYHBQQF");
        private static int[] order = new int[] { 13,5,6,7,11,8,11,7,8,11,12,13,13,13,14 };
        private static int key = 4;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
