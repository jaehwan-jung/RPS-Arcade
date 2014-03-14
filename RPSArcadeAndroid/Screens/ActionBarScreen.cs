#region File Description
//-----------------------------------------------------------------------------//
// ActionBarScreen.cs														   //
//																			   //
// Class																	   //
// Represents the action bar where the user chooses (taps) his hand	           //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

#endregion
namespace RPSArcadeAndroid
{
	public sealed class ActionBarScreen: GameScreen
	{
		#region Constants

		public const string ACTIONBAR_BACKGROUND_XNB = "Textures/userActionBar";
		public const float ACTIONBAR_CENTER_X = 240f;
		public const float ACTIONBAR_CENTER_Y = 755f;
		public const float ACTIONBAR_HEIGHT = 80f;
		public const float ACTIONBAR_WIDTH = 460f;
		public const float HAND_HEIGHT = 80f;
		public const float HAND_WIDTH = 80f;
		public const float USER_HAND_ONE_X_POSITION = 80f;
		public const float USER_HAND_TWO_X_POSITION = 240f;
		public const float USER_HAND_THREE_X_POSITION = 400f;
		public const float USER_HAND_Y_POSITION = 755f;

		#endregion

		#region Properties

		public Texture2D ActionbarBackgroundTexture { get; set; }

		public Vector2 ActionbarCenter { get; set; }

		public Vector2 ActionbarSize  { get; set; }

		public HandFactory HandFactory  { get; private set; }

		public IList<Hand> Hands { get; set; }

		public Vector2 HandSize  { get; set; }

		#endregion

		#region Initialization

		public ActionBarScreen (Game game, ScreenManager screenManager)
			: base (game, screenManager)
		{
			Hands = new List<Hand> ();
			LoadTextures ();			
			LoadHands ();
		}

		private void LoadTextures ()
		{
			ActionbarBackgroundTexture = LoadTexture2D (ACTIONBAR_BACKGROUND_XNB);			
			ActionbarCenter = new Vector2 (ACTIONBAR_CENTER_X, ACTIONBAR_CENTER_Y);
			ActionbarSize = new Vector2 (ACTIONBAR_WIDTH, ACTIONBAR_HEIGHT);
		}

		private void LoadHands ()
		{
			HandSize = new Vector2 (HAND_WIDTH, HAND_HEIGHT);
			HandFactory = new HandFactory (Game, ContentManager);
			var origin = new Vector2 (USER_HAND_ONE_X_POSITION - HAND_WIDTH / 2, USER_HAND_Y_POSITION - HAND_HEIGHT / 2);
			Hands.Add (CreateHand (origin, HandType.Rock));
			origin = new Vector2 (USER_HAND_TWO_X_POSITION - HAND_WIDTH / 2, USER_HAND_Y_POSITION - HAND_HEIGHT / 2);
			Hands.Add (CreateHand (origin, HandType.Paper));
			origin = new Vector2 (USER_HAND_THREE_X_POSITION - HAND_WIDTH / 2, USER_HAND_Y_POSITION - HAND_HEIGHT / 2);
			Hands.Add (CreateHand (origin, HandType.Scissors));
			
			
		}

		private Hand CreateHand (Vector2 origin, HandType type)
		{
			var rectangle = new Rectangle ((int)origin.X, (int)origin.Y, (int)HAND_WIDTH, (int)HAND_HEIGHT);
			var hand = HandFactory.CreateUserHand (rectangle);
			hand.Type = type;
			return hand;
		}

		#endregion

		#region Update and Draw

		public override void Update (GameTime gameTime)
		{
			UpdateHands (gameTime);	
			base.Update (gameTime);
		}

		private void UpdateHands (GameTime gameTime)
		{
			for (int i = 0, handsCount = Hands.Count; i < handsCount; i++)
				Hands [i].Update (gameTime);
		}

		public override void Draw (GameTime gameTime)
		{
			DrawActionbarBackground ();
			DrawHands (gameTime);
			base.Draw (gameTime);
		}

		private void DrawActionbarBackground ()
		{						
			Resolution.DrawAtCenter (ActionbarBackgroundTexture, ActionbarCenter, ActionbarSize);			
		}

		private void DrawHands (GameTime gameTime)
		{
			for (int i = 0, handsCount = Hands.Count; i < handsCount; i++)
				Hands [i].Draw (gameTime);			
		}

		#endregion

		#region Non-Static Methods

		public Hand GetTappedUserHand (Vector2 virtualTappedPosition)
		{
			for (int i = 0, handsCount = Hands.Count; i < handsCount; i++) {
				var hand = Hands [i];
				if (hand.Rectangle.Contains (virtualTappedPosition))
					return hand;
			}
			
			return Hand.Dummy;
		}

		#endregion
	}
}

