using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WesternSpace.Screens
{
    public class StoryboardInformation
    {
        private Texture2D sceneTexture;

        public Texture2D SceneTexture
        {
            get { return sceneTexture; }
        }

        private string sceneText;

        public string SceneText
        {
            get { return sceneText; }
            set { sceneText = value; }
        }

        private Vector2 sceneTextPosition;

        public Vector2 SceneTextPosition
        {
            get { return sceneTextPosition; }
        }

        public StoryboardInformation(Texture2D texture, string text, Vector2 textPosition)
        {
            this.sceneTexture = texture;
            this.sceneText = text;
            this.sceneTextPosition = textPosition;
        }
    }
}
