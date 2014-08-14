// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Activity Support
// Filename:    App.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The App class is used to provide a Singleton class for Application wide data objects.
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
				bool initialisedOk = GameUpdater.UpdateFiles( GameFileDirectory, ApplicationDirectory, applicationContext.Assets );

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

				// Load the current game definition
				if ( initialisedOk == true )
				{
					initialisedOk = LoadGame( gameHistory.CurrentGameProperty );
				}

				// Initialise the current game instance
				if ( initialisedOk == true )
				{
					// If there is no GameInstance in the history then clone the first instance in the current game, store in the history
					// and initialise it against the current game
					if ( gameHistory.CurrentInstanceProperty == null )
					{
						gameHistory.CurrentInstanceProperty = loadedGame.GameInstancesProperty[ 0 ].Clone();
					}

					initialisedOk = gameHistory.CurrentInstanceProperty.Initialise( loadedGame );
				}

				// Create a GameGrid for this game
				if ( initialisedOk == true )
				{
					grid = new GameGrid( LoadedGameProperty.BoardProperty, gameHistory.CurrentInstanceProperty.TilesProperty, LoadedGameProperty.CompletionProperty );
				}

				Log.Debug( LogTag, string.Format( "App initialized, setting Initialized {0}", initialisedOk ) );

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

		public void ChangeToNewGame( string newGame )
		{
			// Save the name of the selected game and load it and its instance
			HistoryProperty.CurrentGameProperty = newGame;
			LoadGame( newGame );
		}

		public void ChangeToNewInstance( GameInstance newInstance )
		{
			HistoryProperty.CurrentInstanceProperty = newInstance.Clone();
			HistoryProperty.CurrentInstanceProperty.Initialise( LoadedGameProperty );
			grid = new GameGrid( LoadedGameProperty.BoardProperty, HistoryProperty.CurrentInstanceProperty.TilesProperty, LoadedGameProperty.CompletionProperty );
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
		/// Gets or sets a value indicating whether this instance is initialised property.
		/// </summary>
		/// <value><c>true</c> if this instance is initialised property; otherwise, <c>false</c>.</value>
		public bool IsInitialisedProperty
		{
			get;
			set;
		}

		/// <summary>
		/// The single instance of the App class
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
		/// The game history.
		/// </summary>
		public History HistoryProperty
		{
			get
			{
				return gameHistory;
			}
		}

		/// <summary>
		/// Gets the games property.
		/// </summary>
		/// <value>The games property.</value>
		public string[] GamesProperty
		{
			get
			{
				return gameDirectories;
			}
		}

		/// <summary>
		/// Gets the loaded game property.
		/// </summary>
		/// <value>The loaded game property.</value>
		public Game LoadedGameProperty
		{
			get
			{
				return loadedGame;
			}
		}

		/// <summary>
		/// Gets the game grid property.
		/// </summary>
		/// <value>The game grid property.</value>
		public GameGrid GameGridProperty
		{
			get
			{
				return grid;
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
		/// Loads the game.
		/// </summary>
		/// <returns><c>true</c>, if game was loaded and initialised OK, <c>false</c> otherwise.</returns>
		/// <param name="gameName">Game name.</param>
		private bool LoadGame( string gameName )
		{
			bool gameLoaded = true;

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
						loadedGame = ( Game )deserializer.Deserialize( reader );
					}

					// Initialise the loaded game
					gameLoaded = loadedGame.Initialise( gameDirectory );
				}
				catch ( Exception anyException )
				{
					Log.Debug( LogTag, anyException.Message );
					gameLoaded = false;
				}
			}
			else
			{
				Log.Debug( LogTag, string.Format( "Cannot load game {0}", gameName ) );
				gameLoaded = false;
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
					}
				}
				catch ( Exception anyException )
				{
					Log.Debug( LogTag, anyException.Message );
					loadedOk = false;
				}
			}
			else
			{
				// Need to create an empty History with the first available game as the current game
				gameHistory = new History();
				gameHistory.CurrentGameProperty = gameDirectories[ 0 ];
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
				gameDirectories = new string[ gamePaths.Length ];
				int index = 0;

				foreach ( string gamePath in gamePaths )
				{
					// Only need the actual directory name
					gameDirectories[ index++ ] = System.IO.Path.GetFileNameWithoutExtension( gamePath );
				}
			}
			else
			{
				Log.Debug( LogTag, "No games found to load" );
				gamesLoaded = false;
			}

			return gamesLoaded;
		}

		//
		// Private data
		//

		/// <summary>
		/// The single instance of the App class
		/// </summary>
		private static ApplicationData instance = null;

		/// <summary>
		/// List of the names of all the available games directories
		/// </summary>
		private string[] gameDirectories = null;

		/// <summary>
		/// The game history.
		/// </summary>
		private History gameHistory = null;

		/// <summary>
		/// The loaded game.
		/// </summary>
		private Game loadedGame = null;

		/// <summary>
		/// The game grid for the loaded game instance.
		/// </summary>
		private GameGrid grid = null;

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



