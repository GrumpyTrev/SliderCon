// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Activity Support
// Filename:    ActivityBase.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The ActivityBase class is used to provide asynchronouse initialisation functionality
//
// Description:  As purpose
//
//
//
// File History
// ------------
//
// %version:  1 %
//
// (c) Copyright 2014 Trevor Simmonds.
// This software is protected by copyright, the design of any 
// article recorded in the software is protected by design 
// right and the information contained in the software is 
// confidential. This software may not be copied, any design 
// may not be reproduced and the information contained in the 
// software may not be used or disclosed except with the
// prior written permission of and in a manner permitted by
// the proprietors Trevor Simmonds (c) 2014
//
//    Copyright Holders:
//       Trevor Simmonds,
//       t.simmonds@virgin.net
//
using System;
using Android.App;
using Android.Util;
using Android.Content;

namespace SliderCon
{
	/// <summary>
	/// The ActivityBase class is used to provide asynchronouse initialisation functionality
	/// </summary>
	public abstract class ActivityBase : Activity
	{
		//
		// Protected methods
		//

		/// <summary>
		/// Raises the resume event.
		/// Display a 'loading..' message.
		/// Set up a delegate to call when the application data has been initialised.
		/// Start the application initialisation process.
		/// </summary>
		protected override void OnResume()
		{
			base.OnResume();

			Log.Debug( LogTag, "ActivityBase.OnResume" );

			if ( ApplicationData.InstanceProperty.IsInitialisedProperty == false )
			{
				Log.Debug( LogTag, "ActivityBase.App is NOT initialized" );

				// Show the loading overlay on the UI thread
				progress = ProgressDialog.Show( this, "Loading", "Please Wait...", true ); 

				// When the app has initialized, hide the progress bar and call Finished Initialzing
				initialisedHandler = ( initialisedOk ) =>
				{
					// call finished initializing so that any derived activities have a chance to do work
					RunOnUiThread( () =>
					{
						if ( initialisedOk == true )
						{
							FinishedInitialising();

							// Hide the progress bar
							if ( progress != null )
							{
								progress.Dismiss();
							}
						}
						else
						{
							new AlertDialog.Builder(this)
								.SetPositiveButton("OK", (sender, args) =>
								{
									this.Finish();
								})
								.SetMessage("An error happened!")
								.SetTitle("Error")
								.Show();
						}
					} );
				};

				// Register delegate
				ApplicationData.InstanceProperty.initialisationDelegate += initialisedHandler;

				// Start the initialisation
				ApplicationData.InstanceProperty.Initialise( ( Context )this );
			}
			else
			{
				Log.Debug( LogTag, "ActivityBase.App is initialized" );
			}
		}

		/// <summary>
		/// Called as part of the activity lifecycle when an activity is going into
		/// the background, but has not (yet) been killed.
		/// </summary>
		protected override void OnPause()
		{
			base.OnPause();

			// In the case of rotation before the app is fully intialized, we have
			// to remove our intialized event handler, and dismiss the progres. otherwise
			// we'll get multiple Initialized handler calls and a window leak.
			if ( initialisedHandler != null )
			{
				ApplicationData.InstanceProperty.initialisationDelegate -= initialisedHandler;
			}

			// Hide the progress bar
			if ( progress != null )
			{
				progress.Dismiss();
			}

			ApplicationData.InstanceProperty.SavePersistenData();
		}

		/// <summary>
		/// Override this method to perform tasks after the app class has fully initialized
		/// </summary>
		protected abstract void FinishedInitialising();

		//
		// Private data
		//

		/// <summary>
		/// The log tag for this class
		/// </summary>
		private readonly string LogTag = "ActivityBase";

		/// <summary>
		/// The ProgressDialog instance used to indicate progress to the user.
		/// </summary>
		private ProgressDialog progress = null;

		/// <summary>
		/// Handler used to process the initialised event
		/// </summary>
		private ApplicationData.InitialisationCompleteDelegate initialisedHandler = null;
	}
}



