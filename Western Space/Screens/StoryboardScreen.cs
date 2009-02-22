using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Media;
using WesternSpace.Utility;
using WesternSpace.Input;

namespace WesternSpace.Screens
{
    class StoryboardScreen : Screen
    {
        /// <summary>
        /// The story boards that this cutscene will need to cycle through
        /// </summary>
        private LinkedList<StoryboardInformation> storyboards;

        /// <summary>
        /// The current scene that is being displayed on the scene
        /// </summary>
        private LinkedListNode<StoryboardInformation> currentScene;

        internal LinkedListNode<StoryboardInformation> CurrentScene
        {
            get { return currentScene; }
            set { currentScene = value; }
        }

        /// <summary>
        /// The timer that is used to signal that a new character is to be displayed on the screen
        /// </summary>
        private Timer characterTimer;

        /// <summary>
        /// The current text to display to the screen
        /// </summary>
        private string currentStoryboardText;

        /// <summary>
        /// The current character index the storyboard text is at
        /// </summary>
        private int currentStoryBoardTextIndex;

        /// <summary>
        /// The song that this cutscene needs to play
        /// </summary>
        private Song storyboardSong;

        /// <summary>
        /// The sprite batch used to draw the cutscene to the screen
        /// </summary>
        private SpriteBatch batch;

        /// <summary>
        /// The font used to draw the cutscene text
        /// </summary>
        private SpriteFont font;

        /// <summary>
        /// The transition state data used for fading the images
        /// </summary>
        private StoryboardTransition transition;

        private string nextScreen;

        private bool fadeEffectDone;

        public StoryboardScreen(string screenName, string nextScreen, string storyboardXmlAssetPath)
            : base(ScreenManager.Instance, screenName)
        {
            this.nextScreen = nextScreen;
            storyboards = new LinkedList<StoryboardInformation>();

            CreateStoryboardFromXML(storyboardXmlAssetPath);

            characterTimer.TimeHasElapsed += new EventHandler<EventArgs>(characterTimer_TimeHasElapsed);

            currentStoryboardText = String.Empty;
            currentStoryBoardTextIndex = 0;

            batch = new SpriteBatch(ScreenManager.Instance.GraphicsDevice);

            fadeEffectDone = true;
        }

        public override void Initialize()
        {
            base.Initialize();

            currentScene = storyboards.First;

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(storyboardSong);

            characterTimer.StartTimer();
        }

        public override void Update(GameTime gameTime)
        {
            if (fadeEffectDone)
            {
                if (currentScene.Value.IsTransitionAutomatic && !currentScene.Value.IsTimerStarted)
                {
                    currentScene.Value.StartTimer();
                }

                base.Update(gameTime);

                ScreenManager.Instance.UseSpriteBatchService = false;

                if (transition != null)
                {
                    transition.Update();

                    if (transition.IsTransitionComplete)
                    {
                        transition = null;
                        characterTimer.ResumeTimer();
                    }
                }

                if (((InputMonitor.Instance.WasJustPressed(InputMonitor.JUMP) || InputMonitor.Instance.WasJustPressed(InputMonitor.PAUSE)) && transition == null) || 
                    (currentScene.Value.IsTransitionAutomatic && currentScene.Value.IsReadyToTransition && transition == null))
                {
                    if (currentStoryBoardTextIndex < currentScene.Value.SceneText.Length)
                    {
                        // display the rest of the text
                        currentStoryboardText = currentScene.Value.SceneText;
                        currentStoryBoardTextIndex = currentScene.Value.SceneText.Length;
                        characterTimer.ResetTimer();
                        characterTimer.PauseTimer();

                        if (currentScene.Value.IsTransitionAutomatic && currentScene.Value.IsTimerStarted)
                        {
                            currentScene.Value.StopTimer();
                        }
                    }
                    else if (currentStoryboardText == currentScene.Value.SceneText && currentScene != storyboards.Last)
                    {
                        // transition to next story board
                        this.transition = new StoryboardTransition(this, 0.01f, 0.01f);
                        
                        fadeEffectDone = false;

                        characterTimer.ResetTimer();
                        characterTimer.PauseTimer();

                        currentStoryboardText = String.Empty;
                        currentStoryBoardTextIndex = 0;

                        if (currentScene.Value.IsTransitionAutomatic && currentScene.Value.IsTimerStarted)
                        {
                            currentScene.Value.StopTimer();
                        }
                    }
                    else if (currentStoryboardText == currentScene.Value.SceneText && currentScene == storyboards.Last)
                    {
                        if (currentScene.Value.IsTransitionAutomatic && currentScene.Value.IsTimerStarted)
                        {
                            currentScene.Value.StopTimer();
                        }

                        // we have reached our last story board, transition to the next screen
                        ScreenTransition sts = new ScreenTransition(this.Name, this.nextScreen, 0.01f, 0.01f, false, true);
                        ScreenManager.Instance.Transition(sts);
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            batch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);

            if (transition != null)
            {
                transition.BeginTransition();
            }

            batch.Draw(currentScene.Value.SceneTexture, new Vector2(0, 0), Color.White);
            batch.DrawString(font, currentStoryboardText, currentScene.Value.SceneTextPosition, Color.Red);

            if (transition != null)
            {
                this.TransitionComplete();
                transition.EndTransition();
            }

            batch.End();
        }

        public override void TransitionComplete()
        {
            base.TransitionComplete();
            
            this.fadeEffectDone = true;
        }

        private void CreateStoryboardFromXML(string assetPath)
        {
            XDocument xdoc = ScreenManager.Instance.Content.Load<XDocument>(assetPath);

            storyboardSong = ScreenManager.Instance.Content.Load<Song>(xdoc.Root.Attribute("Song").Value);
            font = ScreenManager.Instance.Content.Load<SpriteFont>(xdoc.Root.Attribute("Font").Value);

            int delayBetweenEachCharacter = Int32.Parse(xdoc.Root.Attribute("DelayBetweenEachCharacter").Value);
            characterTimer = new Timer(this, delayBetweenEachCharacter);

            IEnumerable<XElement> boards = xdoc.Descendants("Storyboard");

            foreach (XElement board in boards)
            {
                Texture2D sceneTexture = ScreenManager.Instance.Content.Load<Texture2D>(board.Attribute("SceneTexture").Value);
                string sceneText = board.Attribute("SceneText").Value;
                bool isTransitionAutomatic = Boolean.Parse(board.Attribute("IsTransitionAutomatic").Value);
                int automaticTransitionDelay = Int32.Parse(board.Attribute("AutomaticTransitionDelay").Value);

                float textPositionX = Single.Parse(board.Attribute("SceneTextPositionX").Value);
                float textPositionY = Single.Parse(board.Attribute("SceneTextPositionY").Value);
                Vector2 textPosition = new Vector2(textPositionX, textPositionY);

                StoryboardInformation si = new StoryboardInformation(this, sceneTexture, sceneText, textPosition, isTransitionAutomatic, automaticTransitionDelay);

                this.storyboards.AddLast(si);
            }
        }

        private void characterTimer_TimeHasElapsed(object sender, EventArgs e)
        {
            if (currentStoryBoardTextIndex <= (currentScene.Value.SceneText.Length - 1))
            {
                currentStoryBoardTextIndex++;
                currentStoryboardText =  currentScene.Value.SceneText.Substring(0, currentStoryBoardTextIndex);
            }

            characterTimer.ResetTimer();
        }
    }
}
