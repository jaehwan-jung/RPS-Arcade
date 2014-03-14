#region File Description
//-----------------------------------------------------------------------------//
// HandFactory.cs															   //
//																			   //
// Class																	   //
// Represents the factory to produce hands of the Rock-Paper-Scissors game.    //
// Graphics based on 480x800 resolution.							           //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion
namespace RPSArcadeAndroid
{
	public sealed class HandFactory
	{
		#region Constants

		public const string ROCK_XNB = "Textures/animatedRock";
		public const string PAPER_XNB = "Textures/animatedPaper";
		public const string SCISSORS_XNB = "Textures/animatedScissors";
		public const string USER_ROCK_XNB = "Textures/userRock";
		public const string USER_PAPER_XNB = "Textures/userPaper";
		public const string USER_SCISSORS_XNB = "Textures/userScissors";
		public const string USER_ROCK_PRESSED_XNB = "Textures/userRockPressed";
		public const string USER_PAPER_PRESSED_XNB = "Textures/userPaperPressed";
		public const string USER_SCISSORS_PRESSED_XNB = "Textures/userScissorsPressed";

		#endregion

		#region Fields

		private ContentManager contentManager;
		private Game game;
		private Texture2D rock;
		private Texture2D paper;
		private Texture2D scissors;
		private Texture2D userRock;
		private Texture2D userPaper;
		private Texture2D userScissors;
		private Texture2D userRockPressed;
		private Texture2D userPaperPressed;
		private Texture2D userScissorsPressed;

		#endregion

		#region Factory Method

		public Hand CreateComputerHand (Rectangle rectangle)
		{					
			return new Hand (CreateAnimations (PlayerType.Computer), game, rectangle);		
		}

		public Hand CreateUserHand (Rectangle rectangle)
		{					
			return new Hand (CreateAnimations (PlayerType.User), game, rectangle);		
		}

		private IDictionary<HandType, Animation> CreateAnimations (PlayerType playerType)
		{
			switch (playerType) {
			case PlayerType.Computer:
				return CreateAnimationsForComputer ();
			case PlayerType.User:
				return CreateAnimationsForUser ();
			default:
				throw new Exception (string.Format ("Unrecognized PlayerType: {0}", playerType));
			}
		}

		private IDictionary<HandType, Animation> CreateAnimationsForComputer ()
		{
			var matrix = new FrameMatrix (){ Rows = 3, Columns = 3 };	
			var handAnimations = new Dictionary<HandType, Animation> ();
			var animation = new Animation (LoadTexture2D (ROCK_XNB), matrix);
			animation.ExpandMultiplier = 3;
			handAnimations.Add (HandType.Rock, animation);
			animation = new Animation (LoadTexture2D (PAPER_XNB), matrix);
			animation.ExpandMultiplier = 3;
			handAnimations.Add (HandType.Paper, animation);
			animation = new Animation (LoadTexture2D (SCISSORS_XNB), matrix);
			animation.ExpandMultiplier = 3;
			handAnimations.Add (HandType.Scissors, animation);
			return handAnimations;
		}

		private IDictionary<HandType, Animation> CreateAnimationsForUser ()
		{
			var matrix = new FrameMatrix (){ Rows = 1, Columns = 1 };
			var userHandAnimations = new Dictionary<HandType, Animation> ();
			userHandAnimations.Add (HandType.Rock, new Animation (LoadTexture2D (USER_ROCK_XNB), matrix));			
			userHandAnimations.Add (HandType.Paper, new Animation (LoadTexture2D (USER_PAPER_XNB), matrix));			
			userHandAnimations.Add (HandType.Scissors, new Animation (LoadTexture2D (USER_SCISSORS_XNB), matrix));
			userHandAnimations.Add (HandType.PaperPressed, new Animation (LoadTexture2D (USER_PAPER_PRESSED_XNB), matrix));
			userHandAnimations.Add (HandType.RockPressed, new Animation (LoadTexture2D (USER_ROCK_PRESSED_XNB), matrix));
			userHandAnimations.Add (HandType.ScissorsPressed, new Animation (LoadTexture2D (USER_SCISSORS_PRESSED_XNB), matrix));
			return userHandAnimations;
		}

		#endregion

		#region Initialization

		public HandFactory (Game game, ContentManager contentManager)
		{	
			this.game = game;
			this.contentManager = contentManager;
			LoadTextures ();			
		}

		private void LoadTextures ()
		{
			rock = LoadTexture2D (ROCK_XNB);
			paper = LoadTexture2D (PAPER_XNB);
			scissors = LoadTexture2D (SCISSORS_XNB);
			userRock = LoadTexture2D (USER_ROCK_XNB);
			userPaper = LoadTexture2D (USER_PAPER_XNB);
			userScissors = LoadTexture2D (USER_SCISSORS_XNB);
			userRockPressed = LoadTexture2D (USER_ROCK_PRESSED_XNB);
			userPaperPressed = LoadTexture2D (USER_PAPER_PRESSED_XNB);
			userScissorsPressed = LoadTexture2D (USER_SCISSORS_PRESSED_XNB);
		}

		#endregion

		#region Helper Methods

		private Texture2D LoadTexture2D (string assetName)
		{			
			return contentManager.Load<Texture2D> (assetName);
		}

		#endregion
	}
}

