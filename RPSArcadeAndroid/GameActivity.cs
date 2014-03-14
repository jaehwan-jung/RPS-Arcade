#region File Description
//-----------------------------------------------------------------------------//
// GameActivity.cs															   //
//																			   //
// Class																	   //
// Activity for the main game screens						 				   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) 2014 Jae-Hwan Jung. All rights reserved.					   //
//-----------------------------------------------------------------------------//


#endregion
#region Using Statements
using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Google.Ads;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

#endregion
namespace RPSArcadeAndroid
{
	[Activity (Label = "RPSArcadeAndroid", 		
		Icon = "@drawable/rPSLogo",
		AlwaysRetainTaskState = true,
		ScreenOrientation = ScreenOrientation.Portrait,
		LaunchMode = Android.Content.PM.LaunchMode.SingleInstance,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation |
		Android.Content.PM.ConfigChanges.KeyboardHidden |
		Android.Content.PM.ConfigChanges.Keyboard)]
	public class GameActivity : AndroidGameActivity
	{
		#region Fields

		private static AdView adView;

		#endregion

		static InterstitialAd interstitialAd;

		public static float AdBannerHeight {
			get{ return adView.Height; }
		}

		#region OnCreate

		protected override void OnCreate (Bundle bundle)
		{
			try {
				base.OnCreate (bundle);
				RPSArcadeGame.Activity = this;
				var game = new RPSArcadeGame ();
				FrameLayout fl = new FrameLayout (this);
				LinearLayout ll = new LinearLayout (this);
				ll.Orientation = Orientation.Vertical;
				ll.SetGravity (GravityFlags.Top);
				fl.AddView (game.Window);
				adView = new AdView (this, AdSize.Banner, "ca-app-pub-3805641000844271/9009097745");
				ll.AddView (adView);
				fl.AddView (ll);
				SetContentView (fl);
				AdRequest adRequest = new AdRequest ();
				#if DEBUG
			adRequest.SetTesting (true);
			adRequest.AddTestDevice (AdRequest.TestEmulator);
			// If you're trying to show ads on device, use this.
			// The device ID to test will be shown on adb log.
			// adRequest.AddTestDevice (some_device_id);
				#endif
				adView.LoadAd (adRequest);			
				game.Run ();
			
				interstitialAd = new InterstitialAd (this, "ca-app-pub-3805641000844271/7877114945");
				AdRequest adRequest2 = new AdRequest ();
				#if DEBUG
			adRequest2.SetTesting (true);
			adRequest2.AddTestDevice (AdRequest.TestEmulator);
			// If you're trying to show ads on device, use this.
			// The device ID to test will be shown on adb log.
			// adRequest.AddTestDevice (some_device_id);
				#endif
				interstitialAd.LoadAd (adRequest2);
			} catch (Exception e) {
				NotifyViaToast (e.Message);
			}
		}

		#endregion

		public override void OnBackPressed ()
		{			
			MediaPlayer.Pause ();
			if (GameControl.IsPlaying)
				GameControl.Pause ();
			base.OnBackPressed ();
		}

		protected override void OnStop ()
		{
			MediaPlayer.Pause ();
			if (GameControl.IsPlaying)
				GameControl.Pause ();
			base.OnStop ();
		}

		public static void ShowFullAd ()
		{
			if (interstitialAd != null && interstitialAd.IsReady)
				interstitialAd.Show ();
		}

		public static void RefreshFullAd ()
		{
			if (interstitialAd != null) {
				AdRequest adRequest2 = new AdRequest ();
				#if DEBUG
				adRequest2.SetTesting (true);
				adRequest2.AddTestDevice (AdRequest.TestEmulator);
				// If you're trying to show ads on device, use this.
				// The device ID to test will be shown on adb log.
				// adRequest.AddTestDevice (some_device_id);
				#endif
				interstitialAd.LoadAd (adRequest2);
			}
		}

		protected override void OnDestroy ()
		{
			if (adView != null) {
				adView.RemoveAllViews ();
				adView.Destroy ();
			}
			if (interstitialAd != null) {
				interstitialAd.Dispose ();
			}
			base.OnDestroy ();
		}

		public void NotifyViaToast (string msg)
		{
			RunOnUiThread (() => {
				var toast = Toast.MakeText (this, msg, ToastLength.Long);
				toast.SetGravity (GravityFlags.CenterVertical, 0, 0);
				toast.Show ();
			});
		}
	}
}


