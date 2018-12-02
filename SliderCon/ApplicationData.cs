// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Activity Support
// Filename:    ApplicationData.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The ApplicationData class is used to provide a Singleton class for Application wide data objects.
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
using System.Collections;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

using Android.Util;
using Android.Content;
using Android.Graphics;

namespace SliderCon
{
	/// <summary>
	/// Singleton class for Application wide objects. 
	/// </summary>
	public class ApplicationData
	{
		//
		// Public types
		//

		/// <summary>
		/// Definition of delegate type to call when initialisation is complete
		/// </summary>
		public delegate void InitialisationCompleteDelegate( bool initialisedOk );

		//
		// Public methods
		//

		/// <summary>
		/// Start the asynchronous initialisation task
		/// </summary>
		/// <param name="applicationContext">Application context.</param>
		public void Initialise( Context applicationContext )
		{
			// Any work here is likely to be blocking (static constructors run on whatever thread that first 
			// access its instance members, which in our case is an activity doing an initialization check),
			// so we want to do it on a background thread
			new Task( () =>
			{ 
				// Copy games files from the installation to the external storage
				bool initialisedOk = GameUpdater.UpdateFiles( GameFileDirectory, ApplicationDirectory, applicationContext.Assets,
					applicationContext.GetSharedPreferences( "SliderCon", FileCreationMode.Private ) );

				// Create a list of all the available games
				if ( initialisedOk == true )
				{
					initialisedOk = LoadAvailableGamesList();
				}

				// Load the game history
				if ( initialisedOk == true )
				{
					initialisedOk = LoadGameHistory();
				}

				// Initialise the selected game and instance for playing
				if ( initialisedOk == true )
				{
					initialisedOk = InitialiseInstanceForPlaying();
				}

				Log.Debug( LogTag, string.Format( "ApplicationData initialised, setting Initialised {0}", initialisedOk ) );

				// Set our initialisation flag so we know that we're all setup
				IsInitialisedProperty = true;

				// Raise our intialised event
				initialisationDelegate( initialisedOk );

			} ).Start();
		}

		/// <summary>
		/// Saves the game history.
		/// </summary>
		/// <returns><c>true</c>, if game history was saved, <c>false</c> otherwise.</returns>
		public bool SavePersistenData()
		{
			bool savedOk = true;

			string historyFileName = System.IO.Path.Combine( Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, ApplicationDirectory, 
				HistoryFile );

			try 
			{
				// Serialise the history class into the History file
				XmlSerializer serializer = new XmlSerializer( typeof( History ) );
				using ( Stream writer = File.Open( historyFileName, FileMode.Create ) )
				{
					serializer.Serialize( writer, gameHistory ); 
				}
			}
			catch ( Exception anyException )
			{
				Log.Debug( LogTag, anyException.Message );
				savedOk = false;
			}

			return savedOk;
		}

		/// <summary>
		/// Changes to new games instance.
		/// </summary>
		/// <param name="newGame">New game.</param>
		/// <param name="newInstance">New instance.</param>
		public void ChangeToNewInstance( string newGame, GameInstance newInstance )
		{
			// Save the name of the selected game
			gameHistory.CurrentGameProperty = newGame;

			// Clone the instance
			gameHistory.CurrentInstanceProperty = newInstance.Clone();

			// Initialise the instance and associated player
			InitialiseInstanceForPlaying();
		}

		/// <summary>
		/// Loads the image (bitmap) from the speciifed file.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="fileName">File name.</param>
		public static Bitmap LoadImage( string fileName )
		{
			Bitmap image = null;

			if ( File.Exists( fileName ) == true )
			{
				try 
				{
					BitmapFactory.Options options = new BitmapFactory.Options();
					options.InPreferredConfig = Bitmap.Config.Argb8888;

					image = BitmapFactory.DecodeFile( fileName, options );
				}
				catch ( Exception anyException )
				{
					Log.Debug( LogTag, anyException.Message );
				}
			}
			else
			{
				Log.Debug( LogTag, string.Format( "Cannot load image {0}", fileName ) );
			}

			return image;
		}

		/// <summary>
		/// Gets the named game.
		/// </summary>
		/// <returns>The named game.</returns>
		/// <param name="gameName">Game name.</param>
		public Game GetNamedGame( string gameName )
		{
			Game namedGame = null;
			if ( games.ContainsKey( gameName ) == true )
			{
				namedGame = games[ gameName ];
			}

			return namedGame;
		}

		/// <summary>
		/// Gets the move count item for current instance.
		/// </summary>
		/// <returns>The move count for current instance.</returns>
		public string GetMoveCountForCurrentInstance()
		{
			return GetMoveCountForInstance( gameHistory.CurrentInstanceProperty.FullNameProperty );
		}

		/// <summary>
		/// Gets the completion item for the speciifed instance.
		/// </summary>
		/// <returns>The completion item for current instance.</returns>
		public string GetMoveCountForInstance( string instanceName )
		{
			string itemCount = "";

			int count = gameHistory.CompletionRecordProperty.GetMoveCountForInstance( instanceName );

			if ( count > 0 )
			{
				itemCount = count.ToString();
			}

			return itemCount;
		}

		/// <summary>
		/// Adds a completion item for the current instance.
		/// </summary>
		public void AddCompletionItem()
		{
			// Add a completion item to the completion record
			CompletionItem item = new CompletionItem( gameHistory.CurrentInstanceProperty.FullNameProperty,
				gameHistory.MoveHistoryProperty.MoveCountProperty );
			gameHistory.CompletionRecordProperty.AddCompletionItem( item );
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is initialised property.
		/// </summary>
		/// <value><c>true</c> if this instance is initialised property; otherwise, <c>false</c>.</value>
		public bool IsInitialisedProperty
		{
			get;
			set;
		}

		/// <summary>
		/// The single instance of the ApplicationData class
		/// </summary>
		public static ApplicationData InstanceProperty
		{
			get
			{ 
				if ( instance == null )
				{
					instance = new ApplicationData();
				}

				return instance;
			}
		}

		/// <summary>
		/// Gets the game names property.
		/// </summary>
		/// <value>The game names property.</value>
		public string[] GameNamesProperty
		{
			get
			{
				return gameNames;
			}
		}

		/// <summary>
		/// Gets the loaded game property.
		/// </summary>
		/// <value>The loaded game property.</value>
		public Game SelectedGameProperty
		{
			get
			{
				return selectedGame;
			}
		}

		/// <summary>
		/// Gets the GamePlayer property.
		/// </summary>
		/// <value>The GamePlayer property.</value>
		public GamePlayer GamePlayerProperty
		{
			get
			{
				return player;
			}
		}

		/// <summary>
		/// Gets the instance full name property.
		/// </summary>
		/// <value>The instance full name property.</value>
		public string InstanceFullNameProperty
		{
			get
			{
				string fullName = "";
				if ( gameHistory.CurrentInstanceProperty != null )
				{
					fullName = gameHistory.CurrentInstanceProperty.FullNameProperty;
				}

				return fullName;
			}
		}

		//
		// Public data
		//

		/// <summary>
		/// Delegate to call when the initialisation has finished
		/// </summary>
		public InitialisationCompleteDelegate initialisationDelegate = null;

		//
		// Protected methods
		//

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected ApplicationData()
		{
		}

		//
		// Private methods
		//

		/// <summary>
		/// Loads the game from the associated game file.
		/// </summary>
		/// <returns>The Game instance if game was loaded and initialised OK, null otherwise.</returns>
		/// <param name="gameName">Game name.</param>
		private Game LoadGame( string gameName )
		{
			Game gameLoaded = null;

			// Check if the associated game file exists
			string gameDirectory = System.IO.Path.Combine( Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, ApplicationDirectory,
				GameFileDirectory, gameName );

			string gameFileName = System.IO.Path.Combine( gameDirectory, gameName + ".xml" );

			if ( File.Exists( gameFileName ) == true )
			{
				try 
				{
					// Deserialise the history file into a History class
					XmlSerializer deserializer = new XmlSerializer( typeof( Game ) );
					using ( Stream reader = File.Open( gameFileName, FileMode.Open ) )
					{
						gameLoaded = ( Game )deserializer.Deserialize( reader );
					}

					// Initialise the loaded game
					if ( gameLoaded.Initialise( gameDirectory ) == false )
					{
						// Initialisation failed - return null
						gameLoaded = null;
					}
				}
				catch ( Exception anyException )
				{
					Log.Debug( LogTag, anyException.Message );
				}
			}
			else
			{
				Log.Debug( LogTag, string.Format( "Cannot load game {0}", gameName ) );
			}

			return gameLoaded;
		}

		/// <summary>
		/// Loads the game history from the history file.
		/// </summary>
		/// <returns><c>true</c>, if game history was loaded, <c>false</c> otherwise.</returns>
		private bool LoadGameHistory()
		{
			bool loadedOk = true;

			// Check if a history file exists
			string historyFileName = System.IO.Path.Combine( Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, ApplicationDirectory, 
				HistoryFile );

			if ( File.Exists( historyFileName ) == true )
			{
				try 
				{
					// Deserialise the history file into a History class
					XmlSerializer deserializer = new XmlSerializer( typeof( History ) );
					using ( Stream reader = File.Open( historyFileName, FileMode.Open ) )
					{
						gameHistory = ( History )deserializer.Deserialize( reader ); 

						if ( gameHistory != null )
						{
							if ( gameHistory.CompletionRecordProperty == null )
							{
								gameHistory.CompletionRecordProperty = new CompletionRecord();
								gameHistory.CompletionRecordProperty.CompletionItemsProperty = new List< CompletionItem >();
							}
						}
					}
				}
				catch ( Exception anyException )
				{
					Log.Debug( LogTag, anyException.Message );
					loadedOk = false;
				}
			}

			// If the History instance has not been deserialised (or there was no file) then create a new one
			if ( gameHistory == null )
			{
				// Need to create an empty History with the first available game as the current game
				gameHistory = new History();
				gameHistory.CurrentGameProperty = GameNamesProperty[ 0 ];

				// Create an empty move history
				gameHistory.MoveHistoryProperty = new MoveHistory();
				gameHistory.MoveHistoryProperty.TileMovesProperty = new List<TileMove>();

				// Create an empty CompletionRecord
				gameHistory.CompletionRecordProperty = new CompletionRecord();
				gameHistory.CompletionRecordProperty.CompletionItemsProperty = new List< CompletionItem >();
			}

			return loadedOk;
		}

		/// <summary>
		/// Loads the available games list from external storage
		/// </summary>
		/// <returns><c>true</c>, if available games list was loaded, <c>false</c> otherwise.</returns>///
		private bool LoadAvailableGamesList()
		{
			bool gamesLoaded = true;

			string[] gamePaths = Directory.GetDirectories( System.IO.Path.Combine( Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, 
				ApplicationDirectory, GameFileDirectory ) );

			if ( gamePaths.Length > 0 )
			{
				// Load the Game instance associate with each game type and store locally
				// This can fail so loop round in a while
				int gameIndex = 0;
				while ( ( gamesLoaded == true ) && ( gameIndex < gamePaths.Length ) )
				{
					Game loadedGame = LoadGame( System.IO.Path.GetFileName( gamePaths[ gameIndex++ ] ) );
					if ( loadedGame != null )
					{
						games[ loadedGame.NameProperty ] = loadedGame;
					}
					else
					{
						gamesLoaded = false;
					}
				}
			}
			else
			{
				Log.Debug( LogTag, "No games found to load" );
				gamesLoaded = false;
			}

			// If successful copy the names of the games to a string array
			if ( gamesLoaded == true )
			{
				gameNames = new string[ gamePaths.Length ];
				games.Keys.CopyTo( gameNames, 0 );
				Array.Sort( gameNames );
			}

			return gamesLoaded;
		}

		/// <summary>
		/// Initialises the instance for playing.
		/// </summary>
		/// <returns><c>true</c>, if instance for playing was initialised, <c>false</c> otherwise.</returns>
		private bool InitialiseInstanceForPlaying()
		{
			bool initialisedOk = true;

			// Load the current game definition from the available games
			if ( initialisedOk == true )
			{
				if ( games.ContainsKey( gameHistory.CurrentGameProperty ) == true )
				{
					selectedGame = games[ gameHistory.CurrentGameProperty ];
				}
				else
				{
					Log.Debug( LogTag, "Selected Game [%0] does not exist", gameHistory.CurrentGameProperty );
					initialisedOk = false;
				}
			}

			// Initialise the current game instance
			if ( initialisedOk == true )
			{
				// If there is no GameInstance specified then clone the first instance in the current game.
				if ( gameHistory.CurrentInstanceProperty == null )
				{
					gameHistory.CurrentInstanceProperty = selectedGame.GameInstancesProperty[ 0 ].Clone();
				}

				// Initialise the current instance against the current game
				initialisedOk = gameHistory.CurrentInstanceProperty.Initialise( selectedGame );
			}

			// Create a GamePlayer for this game
			if ( initialisedOk == true )
			{
				// Reset the move history
				gameHistory.MoveHistoryProperty.Reset();

				// Must remove any delegates from the existing player
				if ( player != null )
				{
					player.BackButtonProperty = null;
				}

				// Determine which Completion to use, either the Game's or the instance's
				Completion gameCompletion = gameHistory.CurrentInstanceProperty.CompletionProperty;
				if ( gameCompletion == null )
				{
					gameCompletion = selectedGame.CompletionProperty;
				}

				player = new GamePlayer( selectedGame.BoardProperty, gameHistory.CurrentInstanceProperty.TilesProperty, gameCompletion, gameHistory.MoveHistoryProperty );
			}

			return initialisedOk;
		}

		//
		// Private data
		//

		/// <summary>
		/// The single instance of the App class
		/// </summary>
		private static ApplicationData instance = null;

		/// <summary>
		/// Hashtable of the available Game instances indexed by game name
		/// </summary>
		private Dictionary< string, Game > games = new Dictionary< string, Game >();

		/// <summary>
		/// Names of all the available games
		/// </summary>
		private string[] gameNames = null;

		/// <summary>
		/// The game history.
		/// </summary>
		private History gameHistory = null;

		/// <summary>
		/// The selected game.
		/// </summary>
		private Game selectedGame = null;

		/// <summary>
		/// The game player for the loaded game instance.
		/// </summary>
		private GamePlayer player = null;

		/// <summary>
		/// The log tag for this class
		/// </summary>
		private static readonly string LogTag = "ApplicationData";

		/// <summary>
		/// The game file directory.
		/// </summary>
		private readonly string GameFileDirectory = "GameFiles";

		/// <summary>
		/// The application directory.
		/// </summary>
		private readonly string ApplicationDirectory = "SliderCon";

		/// <summary>
		/// The history file name.
		/// </summary>
		private readonly string HistoryFile = "History.xml";
	}
}
