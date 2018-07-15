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
        }

        public override void Initialize()
        {
            base.Initialize();

            currentScene = storyboards.First;

           // MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(storyboardSong);

            characterTimer.StartTimer();
        }

        public override void Update(GameTime gameTime)
        {
            // start the timer on the scene if its an automatic transition and its the first storyboard
            if (currentScene.Value.IsTransitionAutomatic && !currentScene.Value.IsTimerStarted)
            {
                currentScene.Value.StartTimer();
            }

            base.Update(gameTime);

            // tell the screen manager not to use the sprite batch service
            // we need to use SpriteBatchMode.Immediate to use HLSL
            ScreenManager.Instance.UseSpriteBatchService = false;


            if (transition != null) // if we currently have a transition happening
            {
                transition.Update(); // update the transition

                if (transition.IsTransitionComplete)
                {
                    // reset the transition and start the text timer
                    transition = null;
                    characterTimer.ResumeTimer();

                    // reset timer on the scene if its an automatic transition...coming from a previous transition
                    if (currentScene.Value.IsTransitionAutomatic && currentScene.Value.IsTimerStarted)
                    {
                        currentScene.Value.StopTimer();
                        currentScene.Value.StartTimer();
                    }
                }
            }

            // if there is input or it is an automatic transition and there is no transition happening
            if (((InputMonitor.Instance.WasJustPressed(InputMonitor.JUMP) || InputMonitor.Instance.WasJustPressed(InputMonitor.PAUSE)) && transition == null) ||
                (currentScene.Value.IsTransitionAutomatic && currentScene.Value.IsReadyToTransition && transition == null))
            {
                // user inputs in the middle of the text
                if (currentStoryBoardTextIndex < currentScene.Value.SceneText.Length)
                {
                    // display the rest of the text
                    currentStoryboardText = currentScene.Value.SceneText;
                    currentStoryBoardTextIndex = currentScene.Value.SceneText.Length;
                    characterTimer.ResetTimer();
                    characterTimer.PauseTimer();
                }
                // if we have another storyboard and the text is done on the current storyboard
                else if (currentStoryboardText == currentScene.Value.SceneText && currentScene != storyboards.Last)
                {
                    // transition to next story board
                    this.transition = new StoryboardTransition(this, 0.01f, 0.01f);

                    //fadeEffectDone = false;

                    // reset the timer
                    characterTimer.ResetTimer();
                    characterTimer.PauseTimer();

                    // reset the text being displayed
                    currentStoryboardText = String.Empty;
                    currentStoryBoardTextIndex = 0;

                    // if we have an automatic transition stop the timer
                    if (currentScene.Value.IsTransitionAutomatic && currentScene.Value.IsTimerStarted)
                    {
                        currentScene.Value.StopTimer();
                    }
                }
                // we are at the last storyboard
                else if (currentStoryboardText == currentScene.Value.SceneText && currentScene == storyboards.Last)
                {
                    // if we have an automatic transition
                    if (currentScene.Value.IsTransitionAutomatic && currentScene.Value.IsTimerStarted)
                    {
                        // stop the timer
                        currentScene.Value.StopTimer();
                    }

                    // we have reached our last story board, transition to the next screen
                    ScreenTransition sts = new ScreenTransition(this.Name, this.nextScreen, 0.01f, 0.01f, false, true);
                    ScreenManager.Instance.Transition(sts);
                    characterTimer.RemoveTimer();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (ScreenManager.Instance.UseSpriteBatchService == false)
            {
                base.Draw(gameTime);

                //batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                if (transition != null)
                {
                    transition.BeginTransition();
                    Effect tEffect = transition.GetEffect();
                    batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, tEffect);

                }
                else
                {
                    batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                }

                batch.Draw(currentScene.Value.SceneTexture, new Vector2(0, 0), Color.White);
                batch.DrawString(font, currentStoryboardText, currentScene.Value.SceneTextPosition, Color.Red);

                if (transition != null)
                {
                    transition.EndTransition();
                }

                batch.End();
            }
        }

        private void CreateStoryboardFromXML(string assetPath)
        {
         
            XDocument xdoc = ScreenManager.Instance.Content.Load<XDocument>(assetPath);
            System.Console.Write("{0}\n", xdoc.Root.Attribute("Song").Value);

            //storyboardSong = ScreenManager.Instance.Content.Load<Song>(xdoc.Root.Attribute("Song").Value);
            //String songFilename = @"Content\\" + xdoc.Root.Attribute("Song").Value + ".ogg";
            //storyboardSong = Song.FromUri(songFilename, new Uri(songFilename, UriKind.Relative));

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
            if (currentStoryBoardTextIndex < currentScene.Value.SceneText.Length)
            {
                currentStoryBoardTextIndex++;
                currentStoryboardText = currentScene.Value.SceneText.Substring(0, currentStoryBoardTextIndex);
            }

            characterTimer.ResetTimer();
        }
    }
}
