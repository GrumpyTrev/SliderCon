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

		public static Activity ActivityProperty
		{
			get
			{
				return activityInstance;
			}
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

			resetButton = FindViewById<Button>( Resource.Id.resetButton);
			resetButton.Click += delegate 
			{
				ApplicationData.InstanceProperty.ChangeToNewInstance( ApplicationData.InstanceProperty.HistoryProperty.CurrentGameProperty,
					ApplicationData.InstanceProperty.SelectedGameProperty.GetGameInstance( 
						ApplicationData.InstanceProperty.HistoryProperty.CurrentInstanceProperty.FullNameProperty ) );
				FinishedInitialising();
			};

			activityInstance = this;
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
			// Display the game name
			FindViewById<TextView>( Resource.Id.gameName ).Text = ApplicationData.InstanceProperty.HistoryProperty.CurrentInstanceProperty.FullNameProperty;

			// Display the minimum move count associated with the instance
			FindViewById< TextView >( Resource.Id.minCount ).Text = ApplicationData.InstanceProperty.GetCompletionItemForCurrentInstance();

			// Initialise the BoardView with the loaded game.
			// This initialisation includes scaling and displaying the board and then displaying each tile in the current game instance.
			// The identity of the tiles are used to select the actual tile from the game type. The instance of the tile may be rotated. This rotation 
			// needs to be applied to the size of the tile, its usage mask and the bitmap.  The bitmap also needs to be scaled the same as the game bitmap.
			BoardView view = FindViewById<BoardView>( Resource.Id.gameboard );

			// Initialise the view but only render it if it has a valid size
			GamePlayer thePlayer = ApplicationData.InstanceProperty.GamePlayerProperty;
			view.Initialise( thePlayer.BoardProperty.ImageProperty, thePlayer.TilesProperty, thePlayer, thePlayer.BoardProperty.WidthProperty, 
				( ( view.Width > 0 ) && ( view.Height > 0 ) ) );

			// Tell the GamePlayer what delegate to call when the game has completed
			thePlayer.OnGameCompletion = new GamePlayer.GameCompletionDelegate( GameCompleted );

			// Tell the GamePlayer what delegate to call when the back button has been pressed
			thePlayer.OnBackMove = new GamePlayer.BackMoveDelegate( view.BackMove );

			// Give the GamePlayer a reference to the move count field
			thePlayer.MoveCountProperty = FindViewById<TextView>( Resource.Id.movesCount );

			// Give the GamePlayer a reference to the Back button
			thePlayer.BackButtonProperty = FindViewById<Button>( Resource.Id.backButton );
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
				// Determine which game has been selected and display the game instance selection dialogue
				string selectedGameName = ( string )( ( AlertDialog )sender ).ListView.Adapter.GetItem( args.Which );
				Game selectedGame = ApplicationData.InstanceProperty.GetNamedGame( selectedGameName );

				ShowSelectGameInstanceDialogue( selectedGame, selectedGame );
			} );

			dialogueBuilder.Create().Show();
		}

		/// <summary>
		/// The DialogCancelListener class is used to detect when a dialogue has been cancelled and to call a specified delegate
		/// </summary>
		class DialogCancelListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
		{
			/// <summary>
			/// Constructor specifying the cancel action
			/// </summary>
			/// <param name="cleanup">The action to perform when the dialogue is cancelled</param>
			public DialogCancelListener( Action cancelAction )
			{
				actionToPerformOnCancel = cancelAction;
			}

			/// <summary>
			/// This method will be invoked when the dialog is canceled.
			/// </summary>
			/// <param name="dialog">The dialog that was canceled will be passed into the method.</param>
			public void OnCancel ( IDialogInterface dialog )
			{
				actionToPerformOnCancel();
			}

			/// <summary>
			/// The action to perform on cancel.
			/// </summary>
			private readonly Action actionToPerformOnCancel = null;
		}

		/// <summary>
		/// Shows the select game instance dialogue.
		/// This lets the user select the instance of a game type.  
		/// </summary>
		private void ShowSelectGameInstanceDialogue( Game selectedGame, GameContainer container )
		{
			// Get the list of instances and mark those that have already been completed.
			string[] itemText = new string[ container.ItemsProperty.Count ];
			for ( int itemIndex = 0; itemIndex < container.ItemsProperty.Count; itemIndex++ )
			{
				GameInstance item = container.GetIndexedItem( itemIndex ) as GameInstance; 
				if ( item != null )
				{
					if ( ApplicationData.InstanceProperty.HistoryProperty.CompletionRecordProperty.GetMoveCountForInstance( item.FullNameProperty ) != 0 )
					{
						itemText[ itemIndex ] = item.NameProperty + " [c]";
					}
					else
					{
						itemText[ itemIndex ] = item.NameProperty;
					}
				}
				else
				{
					itemText[ itemIndex ] = container.ItemsProperty[ itemIndex ];
				}
			}

			AlertDialog.Builder dialogueBuilder = new AlertDialog.Builder( this );
			dialogueBuilder.SetTitle( Resource.String.action_select_game_instance );    
			dialogueBuilder.SetItems( itemText, ( sender, args ) => 
			{
				object selectedItem = container.GetIndexedItem( args.Which );
				if ( ( selectedItem is GameContainer ) == true )
				{
					ShowSelectGameInstanceDialogue( selectedGame, ( GameContainer )selectedItem );
				}
				else if ( ( selectedItem is GameInstance ) == true )
				{
					ApplicationData.InstanceProperty.ChangeToNewInstance( selectedGame.NameProperty, ( GameInstance )selectedItem );
					FinishedInitialising();
				}
			} );
			dialogueBuilder.SetOnCancelListener( new DialogCancelListener( () =>
			{
				if ( container.ParentContainerProperty != null )
				{
					ShowSelectGameInstanceDialogue( selectedGame, container.ParentContainerProperty );
				}

			} ) );

			AlertDialog theDialogue = dialogueBuilder.Create();

			theDialogue.Show();
		}
		
		private void GameCompleted()
		{
			// Add a completion item to the completion record
			ApplicationData.InstanceProperty.AddCompletionItem();

			// Display the minimum move count associated with the instance
			FindViewById< TextView >( Resource.Id.minCount ).Text = ApplicationData.InstanceProperty.GetCompletionItemForCurrentInstance();

			// For now just display an alert dialogue
			AlertDialog.Builder dialogueBuilder = new AlertDialog.Builder( this );
			dialogueBuilder.SetMessage( Resource.String.alert_completion_what_next );
			dialogueBuilder.Create().Show();
		}

		//
		//
		//

		private Button resetButton = null;

		private static Activity activityInstance = null;
	}
}


