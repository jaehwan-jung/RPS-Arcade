#region File Description
//-----------------------------------------------------------------------------//
// LoginActivity.cs															   //
//																			   //
// Class																	   //
// Activity for the login screen.							 				   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) 2014 Jae-Hwan Jung. All rights reserved.					   //
//-----------------------------------------------------------------------------//
using Android.Content.PM;
using Android.Util;

#endregion
#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Google.Ads;

#endregion
namespace RPSArcadeAndroid
{
	[Activity (
		Icon = "@drawable/rPSLogo",
		Label = "RPS Arcade", 
		Theme = "@android:style/Theme.NoTitleBar", 
		NoHistory = true,
		MainLauncher = true,
		ScreenOrientation = ScreenOrientation.Portrait,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation |
		Android.Content.PM.ConfigChanges.KeyboardHidden |
		Android.Content.PM.ConfigChanges.Keyboard)
		]			
	public sealed class LoginActivity : Activity
	{
		#region Fields

		private Button loginButton;
		private EditText idField;
		private EditText passwordField;
		private ProgressDialog progressDialog;
		private AdView adView;
		private ImageView logoImageView;

		#endregion

		#region OnCreate

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LoginLayout);
			InitializeWidgets ();
			InitializeAd ();
		}

		private void InitializeWidgets ()
		{
			loginButton = FindViewById<Button> (Resource.Id.loginButton);
			loginButton.Click += OnLoginClicked;
			idField = FindViewById<EditText> (Resource.Id.userIdTextField);
			idField.FocusChange += OnFocusChanged;
			passwordField = FindViewById<EditText> (Resource.Id.passwordTextField);
			passwordField.FocusChange += OnFocusChanged;
			logoImageView = FindViewById<ImageView> (Resource.Id.logoImageView);
			logoImageView.SetImageResource (Resource.Drawable.RPSLogo);
		}

		private void OnFocusChanged (object sender, View.FocusChangeEventArgs e)
		{
			var textview = sender as TextView;			
			if (textview != null && textview.IsFocused)
				textview.Text = string.Empty;			
		}

		private void OnLoginClicked (object sender, EventArgs e)
		{
			string errorMessage = string.Empty;
			var id = idField.Text;
			var isIdValid = IsIdValid (id);
			if (!isIdValid)
				errorMessage += string.Format ("Invalid Id. Use only letters. No Space. Max 12 Chars.{0}", System.Environment.NewLine);
				
			var pw = passwordField.Text;
			var isPasswordValid = IsPasswordValid (pw);
			if (!isPasswordValid)
				errorMessage += string.Format ("Invalid Password. Max 12 Characters."); 
			
			if (isIdValid && isPasswordValid)
				Login (id, pw);
			else
				NotifyViaToast (errorMessage);
		}

		private bool IsIdValid (string txt)
		{
			return !string.IsNullOrEmpty (txt) && !txt.Contains (" ") && txt.Length <= 12 && txt.All (x => char.IsLetter (x));
		}

		private bool IsPasswordValid (string txt)
		{
			return !string.IsNullOrEmpty (txt) && txt.Length <= 12;
		}

		private async void Login (string id, string pw)
		{
			DisplayProgressBar ();
			DisableLogin ();
			Log.Info ("Login", string.Format ("Entering CheckifUser..."));
			var exist = await CheckIfUserAlreadyExists (id);
			Log.Info ("AzureDB", "After CheckIfUserAlreadyExists - " + exist);
			if (exist)
				TryLogin (id, pw);
			else
				AskForNewAccountCreation (id, pw);
			
			EnableLogin ();
			DismissProgressBar ();
		}

		private void DisplayProgressBar ()
		{
			progressDialog = ProgressDialog.Show (this, "Please wait...", "Checking account info...", true);			
		}

		private void DisableLogin ()
		{
			loginButton.Enabled = false;
		}

		private async Task<bool> CheckIfUserAlreadyExists (string id)
		{
			return await AzureDB.DoesUserExist (id);
		}

		private async void TryLogin (string id, string pw)
		{
			Log.Info ("Login", string.Format ("Trying to log in: {0}, {1}", id, pw));
			var suceed = await AzureDB.Authenticate (id, pw);
			if (suceed) {
				UserAccount.Id = id;
				GoToGameScreen ();
			} else
				NotifyViaToast ("Incorrect Id/Password.");
		}

		private void EnableLogin ()
		{
			loginButton.Enabled = true;
		}

		private void DismissProgressBar ()
		{
			progressDialog.Dismiss ();
		}

		private void AskForNewAccountCreation (string id, string pw)
		{
			var msg = string.Format ("User Doesn't Exist. Would you like to create a new one with the following id/password?\n{0}/{1}", id, pw);
			var builder = new AlertDialog.Builder (this).SetMessage (msg);
			builder.SetPositiveButton ("Yes", async (sender, args) => {
				await AzureDB.InsertUserAsync (id, pw);
				NotifyViaToast ("User created. Please login.");
			});
			builder.SetNegativeButton ("No", (sender, args) => {
			});
			builder.SetTitle ("New Account Creation");
			builder.Show ();
		}

		private void GoToGameScreen ()
		{
			var intent = new Intent (this, typeof(GameActivity)); 
			StartActivity (intent);
		}

		private void InitializeAd ()
		{
			adView = FindViewById<AdView> (Resource.Id.ad);
			
			AdRequest adRequest = new AdRequest ();
			#if DEBUG
			adRequest.SetTesting (true);
			adRequest.AddTestDevice (AdRequest.TestEmulator);
			// If you're trying to show ads on device, use this.
			// The device ID to test will be shown on adb log.
			// adRequest.AddTestDevice (some_device_id);
			#endif
			adView.LoadAd (adRequest);
		}

		#endregion

		#region OnDestroy

		protected override void OnDestroy ()
		{
			if (adView != null) {
				adView.RemoveAllViews ();
				adView.Destroy ();
			}
			base.OnDestroy ();
		}

		#endregion

		#region Methods

		public void NotifyViaToast (string msg)
		{
			RunOnUiThread (() => {
				var toast = Toast.MakeText (this, msg, ToastLength.Long);
				toast.SetGravity (GravityFlags.CenterVertical, 0, 0);
				toast.Show ();
			});
		}

		#endregion
	}
}

