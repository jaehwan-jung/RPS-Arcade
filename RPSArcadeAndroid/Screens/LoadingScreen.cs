#region File Description
//-----------------------------------------------------------------------------//
// LoadingScreen.cs															   //
//																			   //
// Class																	   //
// Loading screen between games										           //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statementsusing System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Threading.Tasks;
using System.Threading;

#endregion
namespace RPSArcadeAndroid
{
	public class LoadingScreen: GameScreen
	{
		#region Constants

		public const string LOADING_STRING_FONT = "Fonts/motorWerkFont";
		public const string LOADING_STRING_SMALL_FONT = "Fonts/motorWerkFontSmall";
		public const string LOADING_STRING = "LOADING";
		public const float LOADING_CENTER_X = 240f;
		public const float LOADING_CENTER_Y = 300f;
		public const float EXTRA_LOADING_MESSAGE_Y = 350f;

		#endregion

		#region Fields

		private Rectangle backgroundRectangle;
		private Color loadingStringColor = Color.White;

		#endregion

		#region Properties

		public Texture2D BackgroundTexture { get; set; }

		public string ExtraLoadingMessage { get { return MyLogger.CurrentMessage; } }

		public Vector2 ExtraLoadingMessageCenter { get; set; }

		public SpriteFont Font { get; set; }

		public bool IsLoading { get; set; }

		public bool IsLoadingScreenOn { get; set; }

		public Vector2 LoadingCenter { get; set; }

		public SpriteFont SmallFont { get; set; }

		#endregion

		#region Initialization

		public LoadingScreen (Game game, ScreenManager screenManager)
			: base (game, screenManager)
		{
			LoadContent ();
			LoadingCenter = new Vector2 (LOADING_CENTER_X, LOADING_CENTER_Y);
			ExtraLoadingMessageCenter = new Vector2 (LOADING_CENTER_X, EXTRA_LOADING_MESSAGE_Y);
		}

		private void LoadContent ()
		{
			Font = LoadSpriteFont (LOADING_STRING_FONT);
			SmallFont = LoadSpriteFont (LOADING_STRING_SMALL_FONT);
			BackgroundTexture = new Texture2D (ScreenManager.GraphicsDevice, 1, 1);
			BackgroundTexture.SetData (new Color[] { Color.Black });
			backgroundRectangle = new Rectangle (0, 0, 480, 800);
		}

		#endregion

		#region Update and Draw

		public override void Update (GameTime gameTime)
		{
			if (IsLoading) {
				TouchPanel.EnabledGestures = GestureType.None;
				base.Update (gameTime);
			}
		}

		public override void Draw (GameTime gameTime)
		{			
			if (IsLoading) {
				IsLoadingScreenOn = true;
				BackgroundTexture = new Texture2D (ScreenManager.GraphicsDevice, 1, 1);
				BackgroundTexture.SetData (new Color[] { Color.Black });
				Resolution.DrawAtRectangle (BackgroundTexture, backgroundRectangle, 0.7f);
				Resolution.DrawStringAtCenter (Font, LOADING_STRING, LoadingCenter, loadingStringColor);
				if (ExtraLoadingMessage != null && !ExtraLoadingMessage.Equals (string.Empty))
					Resolution.DrawStringAtCenter (SmallFont, ExtraLoadingMessage, ExtraLoadingMessageCenter, loadingStringColor);	
				base.Draw (gameTime);
			} else {
				IsLoadingScreenOn = false;
			}
		}

		#endregion

		#region Methods

		public async Task ShowLoadingScreen (GameTime gameTime)
		{
			IsLoading = true;
			await Task.Factory.StartNew (() => {
				while (!IsLoadingScreenOn)
					Thread.Sleep (100);
			});
		}

		public void CloseLoadingScreen ()
		{
			IsLoading = false;
		}

		#endregion
	}
}

