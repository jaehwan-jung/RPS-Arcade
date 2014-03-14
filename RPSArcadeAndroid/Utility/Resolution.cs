#region File Description
//-----------------------------------------------------------------------------//
// Resolution.cs															   //
//																			   //
// Statc Class																   //
// Represents a utility for drawing, scaling, etc (related to graphics)        //
// Draw everything using this utility to automatically scale according to the  //
// real resolution of the device.											   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion
namespace RPSArcadeAndroid
{
	public static class Resolution
	{
		#region Constants

		public const float VIRTUAL_SCREEN_WIDTH = 480f;
		public const float VIRTUAL_SCREEN_HEIGHT = 800f;

		#endregion

		#region Fields

		private static Matrix Scale;
		private static Vector3 ScalingFactor;

		#endregion

		#region Properties

		public static int ActualScreenHeight { get; private set; }

		public static int ActualScreenWidth { get; private set; }

		public static GraphicsDevice GraphicsDevice { get; private set; }

		public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }

		public static bool HasBegun { get; private set; }

		public static bool HasEnded { get; private set; }

		public static bool IsInitialized {
			get {
				return SpriteBatch != null && GraphicsDeviceManager != null && GraphicsDevice != null;
			}
		}

		public static SpriteBatch SpriteBatch { get; set; }

		public static Vector2 VirtualResolution { get; set; }

		#endregion

		#region Initialization

		public static void Initialize (GraphicsDeviceManager graphicsDeviceManager, SpriteBatch spriteBatch)
		{						
			VirtualResolution = new Vector2 (VIRTUAL_SCREEN_WIDTH, VIRTUAL_SCREEN_HEIGHT);
			HasBegun = false;
			HasEnded = true;
			GraphicsDeviceManager = graphicsDeviceManager;
			GraphicsDevice = graphicsDeviceManager.GraphicsDevice;
			GraphicsDeviceManager.SupportedOrientations = DisplayOrientation.Portrait;
			SpriteBatch = spriteBatch;
			ReadActualScreenSize (graphicsDeviceManager);
			ComputeScale ();	
		}

		private static void ReadActualScreenSize (GraphicsDeviceManager graphicsDeviceManager)
		{
			ActualScreenHeight = graphicsDeviceManager.GraphicsDevice.Viewport.Height;
			ActualScreenWidth = graphicsDeviceManager.GraphicsDevice.Viewport.Width;
		}

		private static void ComputeScale ()
		{
			var widthScale = (float)ActualScreenWidth / VirtualResolution.X; 
			var heightScale = (float)ActualScreenHeight / VirtualResolution.Y;			
			ScalingFactor = new Vector3 (widthScale, heightScale, 1);
			Matrix.CreateScale (ref ScalingFactor, out Scale);
		}

		#endregion

		#region Begin

		public static void BeginDraw ()
		{
			PreBeginDrawCheck ();					
			SpriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Scale);
		}

		private static void PreBeginDrawCheck ()
		{
			if (HasEnded == false)
				throw new Exception ("SpriteBatch need to be ended before begin.");
			
			HasBegun = true;
			HasEnded = false;
		}

		#endregion

		#region End

		public static void EndDraw ()
		{
			PreEndDrawCheck ();
			SpriteBatch.End ();			
		}

		private static void PreEndDrawCheck ()
		{
			if (HasBegun == false)
				throw new Exception ("SpriteBatch need to be began before end.");
			
			HasBegun = false;
			HasEnded = true;
		}

		#endregion

		#region Draw

		public static void DrawStringAtCenter (SpriteFont font, string text, Vector2 centerPoint, Color color)
		{
			ThrowExceptionIfNotInitialized ();
			var length = font.MeasureString (text);
			var origin = new Vector2 (centerPoint.X - length.X / 2, centerPoint.Y - length.Y / 2);
			DrawString (font, text, origin, color);
		}

		public static void DrawStringAtOrigin (SpriteFont font, string text, Vector2 origin, Color color)
		{
			ThrowExceptionIfNotInitialized ();
			DrawString (font, text, origin, color);
		}

		private static void DrawString (SpriteFont font, string text, Vector2 position, Color color)
		{
			CheckToContinueDrawing ();
			SpriteBatch.DrawString (font, text, position, color);
		}

		public static void DrawFullScreen (Texture2D texture)
		{
			ThrowExceptionIfNotInitialized ();
			var dest = new Rectangle (0, 0, (int)VirtualResolution.X, (int)VirtualResolution.Y);
			var src = new Rectangle (0, 0, texture.Width, texture.Height);
			DrawTexture (texture, dest, src, Color.White);
		}

		public static void DrawAtRectangle (Texture2D texture, Rectangle rectangle, float transparency)
		{
			ThrowExceptionIfNotInitialized ();
			var src = new Rectangle (0, 0, texture.Width, texture.Height);
			DrawTexture (texture, rectangle, src, Color.White * transparency);
		}

		public static void DrawAtRectangle (Texture2D texture, Rectangle rectangle, float transparency, Color color)
		{
			ThrowExceptionIfNotInitialized ();
			var src = new Rectangle (0, 0, texture.Width, texture.Height);
			DrawTexture (texture, rectangle, src, color * transparency);
		}

		public static void DrawAtRectangle (Texture2D texture, Rectangle dest, Rectangle src, Color color)
		{
			ThrowExceptionIfNotInitialized ();
			DrawTexture (texture, dest, src, color);
		}

		public static void DrawFromScreenOrigin (Texture2D texture, Vector2 size)
		{
			ThrowExceptionIfNotInitialized ();
			var dest = new Rectangle (0, 0, (int)size.X, (int)size.Y);
			var src = new Rectangle (0, 0, texture.Width, texture.Height);
			DrawTexture (texture, dest, src, Color.White);
		}

		public static void DrawAtOrigin (Texture2D texture, Vector2 origin, Vector2 size)
		{
			ThrowExceptionIfNotInitialized ();
			var dest = new Rectangle ((int)origin.X, (int)origin.Y, (int)size.X, (int)size.Y);
			var src = new Rectangle (0, 0, texture.Width, texture.Height);
			DrawTexture (texture, dest, src, Color.White);
		}

		public static void DrawAtOrigin (Texture2D texture, Vector2 origin, Vector2 size, Color color)
		{
			ThrowExceptionIfNotInitialized ();
			var dest = new Rectangle ((int)origin.X, (int)origin.Y, (int)size.X, (int)size.Y);
			var src = new Rectangle (0, 0, texture.Width, texture.Height);
			DrawTexture (texture, dest, src, color);
		}

		public static void DrawAtCenter (Texture2D texture, Vector2 centerPoint, Vector2 size)
		{
			ThrowExceptionIfNotInitialized ();
			var origin = GetOriginFromCenter (centerPoint, size);
			var dest = new Rectangle ((int)origin.X, (int)origin.Y, (int)size.X, (int)size.Y);
			var src = new Rectangle (0, 0, texture.Width, texture.Height);
			DrawTexture (texture, dest, src, Color.White);
		}

		private static Vector2 GetOriginFromCenter (Vector2 centerPoint, Vector2 size)
		{
			int originX = (int)(centerPoint.X - size.X / 2);
			int originY = (int)(centerPoint.Y - size.Y / 2);
			return new Vector2 (originX, originY);
		}

		private static void DrawTexture (Texture2D texture, Rectangle dest, Rectangle src, Color color)
		{			
			CheckToContinueDrawing ();
			SpriteBatch.Draw (texture, dest, src, color);
		}

		private static void CheckToContinueDrawing ()
		{			
			if (HasBegun != true || HasEnded != false)
				throw new Exception ("Begin (and not End) SpriteBatch to draw.");
		}

		private static void ThrowExceptionIfNotInitialized ()
		{
			if (!IsInitialized)
				throw new NotInitializedException ("Resolution class is not initialized.");
		}

		#endregion

		#region Methods

		public static void ClearScreen ()
		{
			GraphicsDevice.Clear (Color.Yellow);
		}

		public static Vector2 GetVirtualFromActual (Vector2 actualVector)
		{
			var x = actualVector.X / ScalingFactor.X;
			var y = actualVector.Y / ScalingFactor.Y;
			return new Vector2 (x, y);
		}

		#endregion
	}
}

