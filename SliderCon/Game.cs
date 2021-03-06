﻿// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game definition
// Filename:    Game.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The Game class contains the definition of a game type.
//
// Description:  The class contains a Board object, plus one or more Tiles, movement constrains, end game condition and one or more GameInstances
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
using System.Xml.Serialization;
using System.Collections.Generic;

using Android.Util;

namespace SliderCon
{
	/// <summary>
	/// The Game class contains the definition of a game type.
	/// </summary>
	public class Game : GameContainer
	{
		/// <summary>
		/// Public constructor required for serialisation
		/// </summary>
		public Game()
		{
			MovementConstraintProperty = Tile.MovementConstraintType.None;
		}

		/// <summary>
		/// Initialise this instance.
		/// </summary>
		public bool Initialise( string gameDirectory )
		{
			bool initialisedOk = false;

			// Initialise the Board instance
			if ( BoardProperty != null )
			{
				initialisedOk = BoardProperty.Initialise( gameDirectory );	
			}
			else
			{
				Log.Debug( LogTag, string.Format( "No board defined for game in directory {0}", gameDirectory ) );
			}

			// Initialise the Tiles in the List and then form a dictionary
			if ( initialisedOk == true )
			{
				if ( TilesProperty != null )
				{
					List< Tile >.Enumerator enumerator = TilesProperty.GetEnumerator();
					int tileNumber = 1;
					while ( ( initialisedOk == true ) && ( enumerator.MoveNext() == true ) )
					{
						Tile currentTile = enumerator.Current;
						initialisedOk = currentTile.Initialise( gameDirectory, tileNumber++, MovementConstraintProperty );

						if ( initialisedOk == true )
						{
							if ( tileDictionary.ContainsKey( currentTile.IdentityProperty ) == false )
							{
								tileDictionary[ currentTile.IdentityProperty ] = currentTile;
							}
							else
							{
								Log.Debug( LogTag, string.Format( "Duplicate tile {0} game in directory {1}", currentTile.IdentityProperty, 
									gameDirectory ) );
								initialisedOk = false;
							}
						}
					}
				}
				else
				{
					Log.Debug( LogTag, string.Format( "No tiles defined for game in directory {0}", gameDirectory ) );
					initialisedOk = false;
				}
			}

			if ( initialisedOk == true )
			{
				// Collect the names of all the instances in the configured container
				CollectionNameProperty = NameProperty;
				initialisedOk = GatherInstances( "", gameInstances );
			}

			return initialisedOk;
		}

		/// <summary>
		/// Gets the tile with the specified name
		/// </summary>
		/// <returns>The tile or null if no such tile</returns>
		/// <param name="name">Name.</param>
		public Tile GetTileByName( string name )
		{
			Tile tileFound = null;

			if ( tileDictionary.ContainsKey( name ) == true )
			{
				tileFound = tileDictionary[ name ];
			}

			return tileFound;
		}

		/// <summary>
		/// Gets the game instance identified by the instance name.
		/// </summary>
		/// <returns>The game instance.</returns>
		/// <param name="instanceName">Instance name.</param>
		public GameInstance GetGameInstance( string instanceName )
		{
			GameInstance retrievedInstance = null;

			if ( gameInstances.ContainsKey( instanceName ) == true )
			{
				retrievedInstance = gameInstances[ instanceName ];
			}

			return retrievedInstance;
		}

		[XmlAttribute( "name" ) ]
		/// <summary>
		/// Gets or sets the name property.
		/// </summary>
		/// <value>The name property.</value>
		public string NameProperty
		{
			get;
			set;
		}

		[XmlAttribute( "movementConstraint" ) ]
		/// <summary>
		/// Gets or sets the name property.
		/// </summary>
		/// <value>The name property.</value>
		public Tile.MovementConstraintType MovementConstraintProperty
		{
			get;
			set;
		}

		[XmlElement( "Board" ) ]
		/// <summary>
		/// Gets or sets the board property.
		/// </summary>
		/// <value>The board property.</value>
		public Board BoardProperty
		{
			get;
			set;
		}

		[XmlElement( "Tile" ) ]
		/// <summary>
		/// Gets or sets the tiles property.
		/// </summary>
		/// <value>The tiles property.</value>
		public List< Tile > TilesProperty
		{
			get;
			set;
		}

		[XmlElement( "Completion" ) ]
		/// <summary>
		/// Gets or sets the Completion property.
		/// </summary>
		/// <value>The board property.</value>
		public Completion CompletionProperty
		{
			get;
			set;
		}

		[XmlIgnoreAttribute]
		/// <summary>
		/// Accesses the tile dictionary.
		/// </summary>
		/// <value>The tile dictionary.</value>
		public Dictionary< string, Tile > TileDictionaryProperty
		{
			get
			{
				return tileDictionary;
			}
		}

		//
		// Private methods
		//

		//
		// Private data
		//

		/// <summary>
		/// The tile dictionary.
		/// </summary>
		private Dictionary< string, Tile > tileDictionary = new Dictionary< string, Tile >();

		/// <summary>
		/// Dictionary of GameInstance instances indexed by their full names
		/// </summary>
		private Dictionary< string, GameInstance > gameInstances = new Dictionary<string, GameInstance>();

		/// <summary>
		/// The log tag for this class
		/// </summary>
		private readonly string LogTag = "Game";
	}
}

