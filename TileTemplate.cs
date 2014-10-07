// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game definition
// Filename:    TileTemplate.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The TileTemplate class specifies the position and orientation of a tile.
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
	/// The TileTemplate class specifies the position and orientation of a tile
	/// </summary>
	public class TileTemplate
	{
		/// <summary>
		/// Public constructor required for serialisation
		/// </summary>
		public TileTemplate()
		{
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public TileTemplate Clone()
		{
			TileTemplate clonedTilePosition = new TileTemplate();

			clonedTilePosition.IdentityProperty = IdentityProperty;
			clonedTilePosition.XProperty = XProperty;
			clonedTilePosition.YProperty = YProperty;

			return clonedTilePosition;
		}

		/// <summary>
		/// Creates the tile from the template
		/// </summary>
		/// <returns>The tile.</returns>
		/// <param name="tileDictionary">Dictionary of tile name to tile</param>
		public Tile CreateTile( Dictionary< string, Tile > tileDictionary )
		{
			Tile createdTile = null;

			if ( tileDictionary.ContainsKey( IdentityProperty ) == true )
			{
				createdTile = tileDictionary[ IdentityProperty ].Transform( new Tile.TileTransformation[ 0 ] );
				createdTile.GridXProperty = XProperty;
				createdTile.GridYProperty = YProperty;

				// Add a reference back to this template
				createdTile.AssociatedTemplateProperty = this;
			}

			return createdTile;
		}

		[XmlAttribute( "identity" ) ]
		/// <summary>
		/// Gets or sets the identity property.
		/// </summary>
		/// <value>The width property.</value>
		public string IdentityProperty
		{
			get;
			set;
		}

		[XmlAttribute( "x" ) ]
		/// <summary>
		/// Gets or sets the x position property.
		/// </summary>
		/// <value>The x position property.</value>
		public int XProperty
		{
			get;
			set;
		}

		[XmlAttribute( "y" ) ]
		/// <summary>
		/// Gets or sets the x position property.
		/// </summary>
		/// <value>The x position property.</value>
		public int YProperty
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the tile property.
		/// </summary>
		/// <value>The tile property.</value>
		public Tile TileProperty
		{
			get;
			set;
		}

	}
}

