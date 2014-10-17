// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game definition
// Filename:    Completion.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The Completion class contains the positions that one or more tiles must be in for the game to complete.
//
// Description:  As purpose.
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

namespace SliderCon
{
	/// <summary>
	/// The Completion class contains the positions that one or more tiles must be in for the game to complete.
	/// </summary>
	public class Completion
	{
		/// <summary>
		/// Public constructor required for serialisation
		/// </summary>
		public Completion()
		{
		}

		[XmlElement( "TileTemplate" ) ]
		/// <summary>
		/// Gets or sets the tile templates property.
		/// </summary>
		/// <value>The tile templates property.</value>
		public List< TileTemplate > TileTemplatesProperty
		{
			get;
			set;
		}

		/// <summary>
		/// Checks whether or not all the tiles in the completion are in their correct positions
		/// </summary>
		/// <returns><c>true</c>, if completion was checked, <c>false</c> otherwise.</returns>
		/// <param name="theTiles">The tiles.</param>
		public bool CheckCompletion( List< Tile > theTiles )
		{
			bool gameCompleted = true;

			// Check whether all the tiles are in the completion positions
			// This is a bit clunky  and will not scale to large lists
			foreach ( TileTemplate completionTile in TileTemplatesProperty )
			{
				if ( gameCompleted == true )
				{
					Tile testTile = theTiles.Find( theTile => theTile.IdentityProperty == completionTile.IdentityProperty );
					if ( testTile != null )
					{
						gameCompleted = ( ( testTile.GridXProperty == completionTile.XProperty ) && ( testTile.GridYProperty == completionTile.YProperty ) );
					}
					else
					{
						gameCompleted = false;
					}
				}
			}

			return gameCompleted;
		}

		//
		// Private methods
		//
	}
}

