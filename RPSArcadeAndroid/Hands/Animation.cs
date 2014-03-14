#region File Description
//-----------------------------------------------------------------------------//
// Animation.cs																   //
//																			   //
// Class																	   //
// Represents animation of  a texture              							   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//


#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion
namespace RPSArcadeAndroid
{
	public class Animation
	{
		#region Properties

		public TimeSpan AnimationLength { get; protected set; }

		public int CurrentFrame { get; protected set; }

		public int ExpandMultiplier { get; set; }

		public bool IsAnimating { get; protected set; }

		public TimeSpan LastUpdatedTime { get; protected set; }

		public TimeSpan LengthPerFrame { get; set; }

		public FrameMatrix Matrix { get; protected set; }

		public Texture2D Texture { get; protected set; }

		#endregion

		#region Initialization

		public Animation (Texture2D texture, FrameMatrix matrix)
		{
			Texture = texture;
			Matrix = matrix;
			ExpandMultiplier = 1;
		}

		#endregion

		#region Update and Draw

		public void Update (GameTime gameTime)
		{
			if (LastUpdatedTime.Equals (TimeSpan.Zero) || LengthPerFrame.Equals (TimeSpan.Zero))
				return;
			
			var isItTimeToIncrementFrame = gameTime.TotalGameTime.Subtract (LastUpdatedTime) >= LengthPerFrame;
			if (IsAnimating && isItTimeToIncrementFrame)
				UpdateFrame (gameTime);
		}

		void UpdateFrame (GameTime gameTime)
		{
			LastUpdatedTime = gameTime.TotalGameTime;
			CurrentFrame++;
			if (CurrentFrame >= Matrix.TotalFrames)
				Reset ();
		}

		public void Draw (Rectangle rectangle)
		{				
			var srcRectangle = GetRectangleForCurrentFrame ();				
			var multiplier = 1 + (ExpandMultiplier - 1) * CurrentFrame / (float)Matrix.TotalFrames;
			var width = rectangle.Width * multiplier;
			var height = rectangle.Height * multiplier;
			int x = rectangle.X;
			int y = rectangle.Y;
			
			if (width != rectangle.Width)
				x = rectangle.X - (int)((width - rectangle.Width) / 2f);
			
			if (height != rectangle.Height)
				y = rectangle.Y - (int)((width - rectangle.Height) / 2f);
			
			var expandedRec = new Rectangle (x, y, (int)width, (int)height);
			Resolution.DrawAtRectangle (Texture, expandedRec, srcRectangle, Color.White);
		}

		private Rectangle GetRectangleForCurrentFrame ()
		{
			var width = Texture.Width / Matrix.Columns;
			var height = Texture.Height / Matrix.Rows;
			var row = CurrentFrame / Matrix.Columns;
			var column = CurrentFrame % Matrix.Columns;
			return new Rectangle (width * column, height * row, width, height);
		}

		#endregion

		#region Methods

		public void Reset ()
		{
			IsAnimating = false;
			CurrentFrame = 0;
		}

		public void StartAnimation (TimeSpan animationLength, GameTime gameTime)
		{
			if (IsAnimating == false) {			
				IsAnimating = true;
				LengthPerFrame = TimeSpan.FromSeconds (animationLength.TotalSeconds / Matrix.TotalFrames);
				LastUpdatedTime = gameTime.TotalGameTime;
			}
		}

		#endregion
	}
}

