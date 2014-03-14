#region File Description
//-----------------------------------------------------------------------------//
// Hand.cs																	   //
//																			   //
// Class																	   //
// Represents a single hand of the Rock-Paper-Scissors game.                   //
// Graphics based on 480x800 resolution.							           //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//


#endregion
#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion
namespace RPSArcadeAndroid
{
	public class Hand
	{
		#region Constants

		public int PRESSED_EFFECT_DURATION_IN_MS = 200;

		#endregion

		#region Fields

		private HandType handType;
		private TimeSpan pressedEffectLength;
		private TimeSpan pressedEffectStartTime;
		private bool isFrozen;
		private bool isPressedOn;

		#endregion

		#region Properties

		public IDictionary<HandType, Animation> Animations { get; protected set; }

		public Animation CurrentAnimation { get; protected set; }

		public static Hand Dummy { get { return new Hand (null, null, Rectangle.Empty) { Type = HandType.None }; } }

		public HandType Type {
			get { return handType; }
			set {
				handType = value;
				CurrentAnimation = GetAnimation (Type);
			}
		}

		public bool IsScheduledToDraw { 
			get { return State == HandState.Active || State == HandState.Reactive || CurrentAnimation.IsAnimating; }
		}

		public Game Game{ get; set; }

		public Vector2 Origin { get; set; }

		public Rectangle Rectangle { 
			get { return new Rectangle ((int)Origin.X, (int)Origin.Y, (int)Size.X, (int)Size.Y); }
		}

		public Vector2 Size{ get; set; }

		public HandState State{ get; set; }

		public Movement Movement{ get; set; }

		#endregion

		#region Initialization

		public Hand (IDictionary<HandType, Animation> animations, Game game, Rectangle rectangle)
		{	
			Animations = animations;
			Game = game;		
			Origin = GetOrigin (rectangle);
			Size = GetSize (rectangle);
			Movement = new Movement ();
			pressedEffectLength = TimeSpan.FromMilliseconds (PRESSED_EFFECT_DURATION_IN_MS);
		}

		#endregion

		#region Update and Draw

		public virtual void Update (GameTime gameTime)
		{			
			if (!IsScheduledToDraw)
				return;
			
			if (isPressedOn && IsPressedEffectDone (gameTime))
				TurnOffPressedEffect ();
			
			
			if (!isFrozen)
				UpdateCurrentPosition (gameTime);
			
			CurrentAnimation.Update (gameTime);
		}

		private bool IsPressedEffectDone (GameTime gameTime)
		{
			var elapsed = gameTime.TotalGameTime.Subtract (pressedEffectStartTime);
			var duration = pressedEffectLength;
			return  elapsed >= duration;
		}

		private void TurnOffPressedEffect ()
		{
			isPressedOn = false;
			
			switch (Type) {
			case HandType.Rock:
			case HandType.RockPressed:
				Type = HandType.Rock;
				break;
			case HandType.Paper:
			case HandType.PaperPressed:
				Type = HandType.Paper;
				break;
			case HandType.Scissors:
			case HandType.ScissorsPressed:
				Type = HandType.Scissors;
				break;
			default:
				throw new Exception (string.Format ("Unrecognized Hand Type: {0}", Type.ToString ()));
			}			
		}

		private void UpdateCurrentPosition (GameTime gameTime)
		{			
			var ydelta = Movement.VerticalSpeed * gameTime.ElapsedGameTime.TotalSeconds;
			ydelta = Movement.VerticalDirection == Direction.DOWN ? ydelta : -ydelta;
			var xdelta = Movement.HorizontalSpeed * gameTime.ElapsedGameTime.TotalSeconds;
			xdelta = Movement.VerticalDirection == Direction.RIGHT ? xdelta : -xdelta;
			Origin += new Vector2 ((float)xdelta, (float)ydelta); 
		}

		public virtual void Draw (GameTime gameTime)
		{
			if (IsScheduledToDraw)
				CurrentAnimation.Draw (GetCurrentRectangle ());
		}

		private Rectangle GetCurrentRectangle ()
		{
			return new Rectangle ((int)Origin.X, (int)Origin.Y, (int)Size.X, (int)Size.Y);	
		}

		#endregion

		#region Non-Static Methods

		public void UnFreeze ()
		{
			isFrozen = false;
		}

		public void Freeze ()
		{
			isFrozen = true;
		}

		public void StartAnimation (TimeSpan animationLength, GameTime gameTime)
		{
			CurrentAnimation.StartAnimation (animationLength, gameTime);
		}

		private Animation GetAnimation (HandType type)
		{
			switch (type) {
			case HandType.Rock:
				return Animations [HandType.Rock];
			case HandType.Paper:
				return Animations [HandType.Paper];
			case HandType.Scissors:
				return Animations [HandType.Scissors];
			case HandType.RockPressed:
				return Animations [HandType.RockPressed];
			case HandType.PaperPressed:
				return Animations [HandType.PaperPressed];
			case HandType.ScissorsPressed:
				return Animations [HandType.ScissorsPressed];
			case HandType.None:
				return null;
			default:
				throw new ArgumentException ("Unrecognized HandType.");
			}
		}

		public void ShowPressedEffect (GameTime gameTime)
		{			
			isPressedOn = true;
			pressedEffectStartTime = gameTime.TotalGameTime;
			switch (Type) {
			case HandType.Rock:
			case HandType.RockPressed:
				Type = HandType.RockPressed;
				break;
			case HandType.Paper:
			case HandType.PaperPressed:
				Type = HandType.PaperPressed;
				break;
			case HandType.Scissors:
			case HandType.ScissorsPressed:
				Type = HandType.ScissorsPressed;
				break;
			default:
				throw new Exception (string.Format ("Unrecognized Hand Type: {0}", Type.ToString ()));
			}			
		}

		#endregion

		#region Static Methods

		public static Vector2 GetOrigin (Rectangle rectangle)
		{
			return new Vector2 (rectangle.X, rectangle.Y);
		}

		public static Vector2 GetSize (Rectangle rectangle)
		{
			var x = rectangle.Width;
			var y = rectangle.Height;
			return new Vector2 (x, y);
		}

		public static bool IsDummy (Hand hand)
		{
			return hand.Type == HandType.None;
		}

		#endregion
	}
}

