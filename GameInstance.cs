// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game definition
// Filename:    GameInstance.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The GameInstance class contains the positions of all the tiles in a game.
//
// Description:  The GameInstance class can be used to define all the available games of a given type, in which case the tile positions are
//				 thier starting positions. It can also be used to hold the current positions of tiles in an active game.
//				 At runtime the tile identity references are replaced with references to the actual Tile instances. These instances are
//				 not serialised.
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
	/// The GameInstance class contains the positions of all the tiles in a game.
	/// </summary>
	public class GameInstance
	{
		/// <summary>
		/// Public constructor required for serialisation
		/// </summary>
		public GameInstance()
		{
		}

		/// <summary>
		/// Resolve the tile identities held in the TileTemplates collection against the Tile collection held in the Game
		/// </summary>
		/// <param name="loadedGame">Loaded game.</param>
		public bool Initialise( Game loadedGame )
		{
			bool initialisedOk = true;

			List< TileTemplate >.Enumerator enumerator = TileTemplatesProperty.GetEnumerator();
			while ( ( initialisedOk == true ) && ( enumerator.MoveNext() == true ) )
			{
				TileTemplate template = enumerator.Current;

				Tile masterTile = loadedGame.GetTileByName( template.IdentityProperty );
				if ( masterTile == null ) 
				{
					Log.Debug( LogTag, string.Format( "Cannot find tile {0}", template.IdentityProperty ) );
					initialisedOk = false;
				}
				else
				{
					// Use the template to create a tile from the master held by the Game
					tiles.Add( template.CreateTile( loadedGame.TileDictionaryProperty ) );
				}
			}

			return initialisedOk;
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public GameInstance Clone()
		{
			GameInstance clonedInstance = new GameInstance();

			// Copy the simple members
			clonedInstance.NameProperty = NameProperty;
			clonedInstance.TileTemplatesProperty = TileTemplatesProperty;
			clonedInstance.FullNameProperty = FullNameProperty;

			return clonedInstance;
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

		[XmlElement( "TileTemplate" ) ]
		public List< TileTemplate > TileTemplatesProperty
		{
			get;
			set;
		}

		[XmlIgnoreAttribute]
		/// <summary>
		/// Gets the tiles property.
		/// </summary>
		/// <value>The tiles property.</value>
		public List< Tile > TilesProperty
		{
			get
			{
				return tiles;
			}
		}

		[XmlAttribute( "fullName" ) ]
		/// <summary>
		/// Gets or sets the instance's full name property.
		/// </summary>
		/// <value>The full name of the instance</value>
		public string FullNameProperty
		{
			get;
			set;
		}

		/// <summary>
		/// The tiles. associated with this instance created from the templates
		/// </summary>
		private List< Tile > tiles = new List< Tile >();

		/// <summary>
		/// The log tag for this class
		/// </summary>
		private static readonly string LogTag = "GameInstance";
	}
}

