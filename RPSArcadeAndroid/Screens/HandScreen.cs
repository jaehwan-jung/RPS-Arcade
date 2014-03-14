#region File Description
//-----------------------------------------------------------------------------//
// HandScreen.cs															   //
//																			   //
// Class																	   //
// Represents the screen for all computer hands and the collision area     	   //
// The user must choose a hand only when a hand is in the collision area       //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion
namespace RPSArcadeAndroid
{
	public sealed class HandScreen: GameScreen
	{
		#region Constants

		public const string COLLISION_BAR_BACKGROUND_XNB = "Textures/collisionBar";
		public const string COLLISION_BAR_XNB = "Textures/collisionBarInner";
		public const int COLLISION_BAR_BACKGROUND_ORIGIN_X = 10;
		public const int COLLISION_BAR_BACKGROUND_ORIGIN_Y = 690;
		public const int COLLISION_BAR_BACKGROUND_WIDTH = 460;
		public const int COLLISION_BAR_BACKGROUND_HEIGHT = 20;
		public const int COLLISION_BAR_WIDTH = 100;
		public const int COLLISION_BAR_HEIGHT = 20;
		public const int HAND_HEIGHT = 100;
		public const int HAND_WIDTH = 100;
		public const int INITIAL_NEWHAND_RELEASE_INTERVAL = 1;

		#endregion

		#region Fields

		private Color actionBarColor = Color.LightBlue;
		private Random random = new Random ();

		#endregion

		#region Properties

		public Rectangle CollisionArea { get; set; }

		public Vector2 CollisionBarOrigin { get; set; }

		public Texture2D CollisionBarBackgroundTexture { get; set; }

		public Texture2D CollisionBarTexture { get; set; }

		public IList<DifficultyController> Levels { get; set; }

		public IList<Hand> Hands { get; set; }

		public HandFactory HandFactory { get; private set; }

		public TimeSpan LastReleaseTotalGameTime { get; set; }

		public TimeSpan NewHandReleaseInterval { get; set; }

		public Vector2 Size { get; set; }

		#endregion

		#region Initialization

		public HandScreen (Game game, ScreenManager screenManager)
			: base (game, screenManager)
		{			
			InitializeHandReleaseStatus ();
			InitializeGameComponents ();
			LoadContent ();
			SetupLevels ();
		}

		private void InitializeHandReleaseStatus ()
		{					
			LastReleaseTotalGameTime = TimeSpan.Zero;
			NewHandReleaseInterval = TimeSpan.FromSeconds (INITIAL_NEWHAND_RELEASE_INTERVAL);
		}

		private void InitializeGameComponents ()
		{			
			Hands = new List<Hand> ();
			CollisionArea = new Rectangle (COLLISION_BAR_BACKGROUND_ORIGIN_X, COLLISION_BAR_BACKGROUND_ORIGIN_Y, COLLISION_BAR_BACKGROUND_WIDTH, COLLISION_BAR_BACKGROUND_HEIGHT);
			CollisionBarOrigin = new Vector2 (COLLISION_BAR_BACKGROUND_ORIGIN_X, COLLISION_BAR_BACKGROUND_ORIGIN_Y);
			Size = new Vector2 (HAND_WIDTH, HAND_HEIGHT);
		}

		private void LoadContent ()
		{
			HandFactory = new HandFactory (Game, ContentManager);
			CollisionBarBackgroundTexture = LoadTexture2D (COLLISION_BAR_BACKGROUND_XNB);
			CollisionBarTexture = LoadTexture2D (COLLISION_BAR_XNB);
		}

		private void SetupLevels ()
		{
			Levels = new List<DifficultyController> ();
			Levels.Add (ControllerFactory.Create (LevelTypes.Speed));
			Levels.Add (ControllerFactory.Create (LevelTypes.HandType));
		}

		#endregion

		#region Update and Draw

		public override void Update (GameTime gameTime)
		{
			ClearDestroyedHand ();
			ReleaseNewHands (gameTime);
			UpdateHands (gameTime);		
			base.Update (gameTime);
		}

		private void ClearDestroyedHand ()
		{
			for (int i = Hands.Count - 1; i >= 0; i--)
				RemoveIfNotValid (Hands [i]);
		}

		private void RemoveIfNotValid (Hand hand)
		{
			if (!hand.IsScheduledToDraw)
				Hands.Remove (hand);
		}

		private void ReleaseNewHands (GameTime gameTime)
		{			
			if (LastReleaseTotalGameTime == null || LastReleaseTotalGameTime.Equals (TimeSpan.Zero))
				LastReleaseTotalGameTime = gameTime.TotalGameTime;
			
			var elapsedTime = gameTime.TotalGameTime.Subtract (LastReleaseTotalGameTime);
			if (elapsedTime >= NewHandReleaseInterval) {
				var randomOrigin = GetRandomStartPosition ();
				var randomHand = GetRandomHand (randomOrigin);
				ReleaseHand (randomHand);
				UpdateReleaseTime (gameTime);
			}
		}

		private Vector2 GetRandomStartPosition ()
		{
			float y = 0f;			
			float x = (float)GetRandomInt (10, 470 - (int)Size.X);
			return new Vector2 (x, y);
		}

		private Hand GetRandomHand (Vector2 origin)
		{
			var rectangle = new Rectangle ((int)origin.X, (int)origin.Y, (int)Size.X, (int)Size.Y);
			var hand = HandFactory.CreateComputerHand (rectangle);
			hand.Type = (HandType)GetRandomInt (0, 2);	
			return hand;
		}

		private void ReleaseHand (Hand hand)
		{
			Hands.Add (hand);
		}

		private void UpdateReleaseTime (GameTime gameTime)
		{
			LastReleaseTotalGameTime = gameTime.TotalGameTime;
		}

		private void UpdateHands (GameTime gameTime)
		{			
			for (int i = 0, handsCount = Hands.Count; i < handsCount; i++) {
				var hand = Hands [i];
				UpdateHandState (hand);
				ApplyLevel (hand, gameTime);
				hand.Update (gameTime);
			}
		}

		private void UpdateHandState (Hand hand)
		{
			var isInCollisionArea = IsInCollisionArea (hand.Rectangle);
			switch (hand.State) {
			case HandState.Active:
				if (isInCollisionArea)
					hand.State = HandState.Reactive;
				break;
			case HandState.Reactive:
				if (!isInCollisionArea)
					hand.State = HandState.Expired;
				break;
			default:
				break;
			}
		}

		private bool IsInCollisionArea (Rectangle rectangle)
		{
			return CollisionArea.Intersects (rectangle);
		}

		private void ApplyLevel (Hand hand, GameTime gameTime)
		{			
			for (int i = 0, CurrentGameLevelCount = Levels.Count; i < CurrentGameLevelCount; i++)
				Levels [i].ApplyLevel (this, hand, gameTime);
		}

		public override void Draw (GameTime gameTime)
		{
			for (int i = 0, handsCount = Hands.Count; i < handsCount; i++)
				Hands [i].Draw (gameTime);
				
			DrawCollisionArea ();	
			base.Draw (gameTime);
		}

		private void DrawCollisionArea ()
		{
			DrawCollisionBarBackground ();
			DrawCollisionBar ();
		}

		private void DrawCollisionBarBackground ()
		{
			Resolution.DrawAtRectangle (CollisionBarBackgroundTexture, CollisionArea, 1f);
		}

		private void DrawCollisionBar ()
		{
			if (Hands.Count > 0) {
				var validHands = Hands.Where (xxx => xxx.State == HandState.Active || xxx.State == HandState.Reactive);
				var bottomHand = validHands.Aggregate ((agg, listItem) => agg.Origin.Y > listItem.Origin.Y ? agg : listItem);
				var xPosition = bottomHand.Origin.X;
				var barArea = new Rectangle ((int)xPosition, COLLISION_BAR_BACKGROUND_ORIGIN_Y, COLLISION_BAR_WIDTH, COLLISION_BAR_HEIGHT);
				Resolution.DrawAtRectangle (CollisionBarTexture, barArea, 1f, actionBarColor);
			}
		}

		public void ChangeActionBarColor (Color color)
		{
			actionBarColor = color;
		}

		#endregion

		#region Methods

		public MatchResult GetMatchResult (Hand tappedUserHandn, GameTime gameTime)
		{
			var bottomHand = Hands.FirstOrDefault (x => x.State == HandState.Reactive);
			
			if (bottomHand == null)
				return new MatchResult (){ Result = ResultType.WrongTiming };
			
			if (IsInCollisionArea (bottomHand)) { 				
				var matchResult = GetMatchResult (tappedUserHandn, bottomHand);
				if (matchResult.Result == ResultType.Victory)
					StartHandAnimation (bottomHand, gameTime);
				
				bottomHand.State = HandState.Destroyed;
				bottomHand.Freeze ();
				return matchResult;
			} else {
				return new MatchResult (){ Result = ResultType.WrongTiming };
			}
			
		}

		private void StartHandAnimation (Hand hand, GameTime gameTime)
		{
			hand.StartAnimation (TimeSpan.FromSeconds (0.5), gameTime);
		}

		private MatchResult GetMatchResult (Hand userHand, Hand hand)
		{
			if (userHand.Type == hand.Type)
				return new MatchResult (){ Result = ResultType.Draw };
			
			bool didUserWin = DidUserWin (userHand, hand);
			var matchResult = GetMatchResult (didUserWin);			
			matchResult.TapTiming = GetTapTiming (hand);
			
			return matchResult;
		}

		private bool DidUserWin (Hand userHand, Hand hand)
		{
			switch (userHand.Type) {
			case HandType.Rock:
			case HandType.RockPressed:
				return hand.Type == HandType.Scissors;
			case HandType.Paper:
			case HandType.PaperPressed:
				return  hand.Type == HandType.Rock;
			case HandType.Scissors:
			case HandType.ScissorsPressed:
				return  hand.Type == HandType.Paper;
			default:
				throw new InvalidOperationException ("Not Recognized Hand Type");	
			}
		}

		private MatchResult GetMatchResult (bool didUserWin)
		{
			var matchResult = new MatchResult ();	
			
			if (didUserWin)
				matchResult.Result = ResultType.Victory;
			else
				matchResult.Result = ResultType.Defeat;
			
			return matchResult;
		}

		private Timing GetTapTiming (Hand hand)
		{
			var handCenterLine = hand.Origin.Y + hand.Size.Y / 2f;
			var collisionAreaCenterLine = CollisionArea.Top + CollisionArea.Height / 2f;					
			var delta = Math.Abs (collisionAreaCenterLine - handCenterLine);
			var threshold = CollisionArea.Height / 2f;
			
			if (delta <= threshold)
				return Timing.Great;
			else if (delta <= threshold * 2)
				return Timing.Good;
			else if (delta <= hand.Size.Y / 2f + CollisionArea.Height / 2f)
				return Timing.SoSo;
			else
				return Timing.None;
		}

		private bool IsInCollisionArea (Hand hand)
		{
			return hand.State == HandState.Reactive;
		}

		#endregion

		#region Helper Methods

		private int GetRandomInt (int start, int end)
		{
			return random.Next (start, end + 1);
		}

		#endregion
	}
}

