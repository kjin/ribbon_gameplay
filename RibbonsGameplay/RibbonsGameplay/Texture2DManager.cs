using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RibbonsGameplay
{
    public class Texture2DManager
    {
        #region TextureAsset class (private)
        class TextureAsset
        {
            string assetName;
            Texture2D texture;

            public TextureAsset(string assetName) { this.assetName = assetName; }

            public void LoadContent(ContentManager content) { texture = content.Load<Texture2D>(assetName); }

            public Texture2D Texture { get { return texture; } }
            public string AssetName { get { return assetName; } }
        }
        #endregion

        List<TextureAsset> textures;
        //after this flag is set, no more assets may be added.
        bool contentLoaded = false;

        public Texture2DManager(params string[] assetNames)
        {
            textures = new List<TextureAsset>();
            foreach (string str in assetNames)
                textures.Add(new TextureAsset(str));
        }

        public void AddTexture(params string[] assetNames)
        {
            if (!contentLoaded)
                foreach (string s in assetNames)
                    textures.Add(new TextureAsset(s));
        }

        public void LoadContent(ContentManager content)
        {
            foreach (TextureAsset t in textures)
                t.LoadContent(content);
            contentLoaded = true;
        }

        public Texture2D GetTexture(string assetName)
        {
            for (int i = 0; i < textures.Count; i++)
            {
                Console.WriteLine(assetName);
                if (assetName == textures[i].AssetName)
                    return textures[i].Texture;
            }
            throw new ArgumentException();
        }
    }
}
