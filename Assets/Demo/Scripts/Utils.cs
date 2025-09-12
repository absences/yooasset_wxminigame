using UnityEngine;

namespace MyGame
{
    public static class Utils
    {
        public static Texture2D CreateTexture(Gradient gradient)
        {
            // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
            var texture = new Texture2D(1, 10, TextureFormat.ARGB32, false);



            for (var i = 0; i < texture.height; i++)
            {
                texture.SetPixel(0,i,gradient.Evaluate((i+0f)/(texture.height-1))
                );
            }

            texture.wrapMode = TextureWrapMode.Clamp;
//            // set the pixel values
//            texture.SetPixel(0, 0, new Color(1.0f, 1.0f, 1.0f, 0.5f));
//            texture.SetPixel(1, 0, Color.clear);
//            texture.SetPixel(0, 1, Color.white);
//            texture.SetPixel(1, 1, Color.black);

            // Apply all SetPixel calls
            texture.Apply();

            return texture;
        }
    }
}