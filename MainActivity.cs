// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game control
// Filename:    MainActivity.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The MainActivity class is the main activity of the SliderCon application 
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
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace SliderCon
{
	[Activity( Label = "SliderCon", MainLauncher = true )]
	/// <summary>
	/// Main activity.
	/// </summary>
	public class MainActivity : ActivityBase
	{
		//
		// Public methods
		//

		/// <summary>
		/// Initialize the contents of the Activity's standard options menu.
		/// </summary>
		/// <param name="menu">The options menu in which you place your items.</param>
		/// <returns>To be added.</returns>
		public override bool OnCreateOptionsMenu( IMenu menu )
		{
			// Inflate the menu items for use in the action bar
			MenuInflater.Inflate( Resource.Menu.MainActivityActions, menu );

			return base.OnCreateOptionsMenu( menu );
		}

		/// <summary>
		/// This hook is called whenever an item in your options menu is selected.
		/// </summary>
		/// <param name="item">The menu item that was selected.</param>
		/// <returns>To be added.</returns>
		public override bool OnOptionsItemSelected( IMenuItem item )
		{
			bool handled = false;
			switch ( item.ItemId )
			{
				case Resource.Id.action_select_game:
				{
					ShowSelectGameDialogue();
					handled = true;
					break;
				}

				default:
				{
					handled = base.OnOptionsItemSelected( item );
					break;
				}
			}

			return handled;
		}

		//
		// Protected methods
		//

		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate( Bundle bundle )
		{
			base.OnCreate( bundle );

			// Set our view from the "main" layout resource
			SetContentView( Resource.Layout.Main );
		}

		/// <summary>
		/// Raises the resume event.
		/// </summary>
		protected override void OnResume()
		{
			base.OnResume();

			// If the application has been initialised then call the FinishedInitialising method
			if ( ApplicationData.InstanceProperty.IsInitialisedProperty == true )
			{
				FinishedInitialising();
			}

		}

		/// <summary>
		/// Override this method to perform tasks after the app class has fully initialized.
		/// This method is called on the UI thread
		/// </summary>
		protected override void FinishedInitialising()
		{
			// Initialise the BoardView with the loaded game.
			// This initialisation includes scaling and displaying the board and then displaying each tile in the current game instance.
			// The identity of the tiles are used to select the actual tile from the game type. The instance of the tile may be rotated. This rotation 
			// needs to be applied to the size of the tile, its usage mask and the bitmap.  The bitmap also needs to be scaled the same as the game bitmap.
			BoardView view = FindViewById<BoardView>( Resource.Id.gameboard );

			// Initialise the view but only render it if it has a valid size
			GameGrid theGrid = ApplicationData.InstanceProperty.GameGridProperty;
			view.Initialise( theGrid.BoardProperty.ImageProperty, theGrid.TilesProperty, theGrid, theGrid.BoardProperty.WidthProperty, 
				( ( view.Width > 0 ) && ( view.Height > 0 ) ) );

			theGrid.OnGameCompletion = new GameGrid.GameCompletionDelegate( GameCompleted );

			// Get the game name text field
			TextView gameName = FindViewById<TextView>( Resource.Id.gameName );

			gameName.Text = ApplicationData.InstanceProperty.HistoryProperty.CurrentGameProperty;
		}

		//
		// Private methods
		// 

		/// <summary>
		/// Shows the select game dialogue.
		/// This lets the user select the type of game.  
		/// </summary>
		private void ShowSelectGameDialogue()
		{
			AlertDialog.Builder dialogueBuilder = new AlertDialog.Builder( this );

			dialogueBuilder.SetTitle( Resource.String.action_select_game );    
			dialogueBuilder.SetItems( ApplicationData.InstanceProperty.GameNamesProperty, ( sender, args ) => 
			{
				string selectedGame = ApplicationData.InstanceProperty.GameNamesProperty[ args.Which ];

				ApplicationData.InstanceProperty.ChangeToNewGame( selectedGame );
				ShowSelectGameInstanceDialogue();
			} );

/*
			dialogueBuilder.SetPositiveButton( Resource.String.alert_dialog_ok, ( sender, args ) =>
			{
				ApplicationData.InstanceProperty.ChangeToNewGame( selectedGame );

				if ( ApplicationData.InstanceProperty.LoadedGameProperty.GameInstancesProperty.Count > 1 )
				{
					ShowSelectGameInstanceDialogue();
				}
				else
				{
					ApplicationData.InstanceProperty.ChangeToNewInstance( ApplicationData.InstanceProperty.LoadedGameProperty.GameInstancesProperty[ 0 ] );
					FinishedInitialising();
				}
			} );

			dialogueBuilder.SetNegativeButton( Resource.String.alert_dialog_cancel, ( sender, args ) =>
			{
			} );
*/
			dialogueBuilder.Create().Show();
		}

		/// <summary>
		/// Shows the select game instance dialogue.
		/// This lets the user select the instance of a game type.  
		/// </summary>
		private void ShowSelectGameInstanceDialogue()
		{
			AlertDialog.Builder dialogueBuilder = new AlertDialog.Builder( this );

			dialogueBuilder.SetTitle( Resource.String.action_select_game_instance );    
			dialogueBuilder.SetItems( ApplicationData.InstanceProperty.SelectedGameProperty.GameInstanceNamesProperty, ( sender, args ) => 
			{
				GameInstance selectedInstance = ApplicationData.InstanceProperty.SelectedGameProperty.GameInstancesProperty[ args.Which ];
				ApplicationData.InstanceProperty.ChangeToNewInstance( selectedInstance );
				FinishedInitialising();
			} );

			/*
			dialogueBuilder.SetPositiveButton( Resource.String.alert_dialog_ok, ( sender, args ) =>
			{
				ApplicationData.InstanceProperty.ChangeToNewInstance( selectedInstance );
				FinishedInitialising();
			} );

			dialogueBuilder.SetNegativeButton( Resource.String.alert_dialog_cancel, ( sender, args ) =>
			{
			} );
*/
			dialogueBuilder.Create().Show();
		}

		private void GameCompleted()
		{
			// For now just display an alert dialogue
			AlertDialog.Builder dialogueBuilder = new AlertDialog.Builder( this );
			dialogueBuilder.SetMessage( Resource.String.alert_completion_what_next );
			dialogueBuilder.Create().Show();
		}
	}
}


