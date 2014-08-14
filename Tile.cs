// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game definition
// Filename:    Tile.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The Tile contains the definition of a tile used for a game.
//
// Description:  The class specifies the dimensions of the tile (in cells) and the image used to display it.  
//				 It also specifies which cells of the tile are not used.
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

using Android.Util;
using Android.Graphics;

namespace SliderCon
{
	/// <summary>
	/// The Tile contains the definition of a tile used for a game.
	/// </summary>
	public class Tile
	{
		/// <summary>
		/// Transformations that can be applied to a tile when cloning a tile for placement on a particular game
		/// </summary>
		public enum TileTransformation
		{
			/// <summary>Flip the tile along its horizontal axis</summary>
			Horizontal,
			/// <summary>Flip the tile along its vertical axis</summary>
			Vertical,
			/// <summary>Rotate the tile 90 degress anticlockwise</summary>
			Rotate
		}

		/// <summary>
		/// How is the movement of this tile constrained
		/// </summary>
		public enum MovementConstraintType
		{
			/// <summary>Tile can only move horizontally</summary>
			[XmlEnum( Name = "horizontal" )]
			Horizontal,
			/// <summary>Tile can only move vertically</summary>
			[XmlEnum( Name = "vertical" )]
			Vertical,
			/// <summary>No constraint</summary>
			[XmlEnum( Name = "none" )]
			None
		}

		/// <summary>
		/// Public constructor required for serialisation
		/// </summary>
		public Tile()
		{
			MovementConstraintProperty = Tile.MovementConstraintType.None;
		}

		/// <summary>
		/// Initialise this instance.
		/// Read the tile's image from a file
		/// </summary>
		/// <param name="gameDirectory">Game directory.</param>
		/// <param name="tileNumber">Tile number used omn the game grid.</param>
		public bool Initialise( string gameDirectory, int tileNumber, Tile.MovementConstraintType defaultMovementConstraint )
		{
			bool initialisedOk = false;

			// Read in the image for the tile
			if ( ImageNameProperty.Length > 0 )
			{
				image = ApplicationData.LoadImage( System.IO.Path.Combine( gameDirectory, ImageNameProperty ) );
				if ( image != null )
				{
					initialisedOk = true;
					gridNumber = tileNumber;
				}
			}
			else
			{
				Log.Debug( LogTag, string.Format( "No image defined for tile {0} in directory {1}", IdentityProperty, gameDirectory ) );
			}

			if ( initialisedOk == true )
			{
				// Apply the default movement constraint from the game unless one has been specifed for this tile
				if ( MovementConstraintProperty == MovementConstraintType.None )
				{
					MovementConstraintProperty = defaultMovementConstraint;
				}

				// If no Exclusion instance has been deserialised then create an empty one
				if ( ExclusionProperty == null )
				{
					ExclusionProperty = new Exclusion();
				}
			}

			return initialisedOk;
		}

		/// <summary>
		/// Applies the specified transformations to a clone of this tile and return it.
		/// </summary>
		/// <param name="transformations">Transformations.</param>
		public Tile Transform( TileTransformation[] transformations )
		{
			Tile transformedTile = new Tile();

			transformedTile.IdentityProperty = IdentityProperty;
			transformedTile.gridNumber = GridNumberProperty;

			// TO DO
			// The following properties need to have the transformations applied to them
			transformedTile.WidthProperty = WidthProperty;
			transformedTile.HeightProperty = HeightProperty;
			transformedTile.ExclusionProperty = ExclusionProperty;
			transformedTile.image = ImageProperty;
			transformedTile.MovementConstraintProperty = MovementConstraintProperty;

			return transformedTile;
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


		[XmlAttribute( "width" ) ]
		/// <summary>
		/// Gets or sets the width property.
		/// </summary>
		/// <value>The width property.</value>
		public int WidthProperty
		{
			get;
			set;
		}

		[XmlAttribute( "height" ) ]
		/// <summary>
		/// Gets or sets the height property.
		/// </summary>
		/// <value>The height property.</value>
		public int HeightProperty
		{
			get;
			set;
		}

		[XmlAttribute( "image" ) ]
		/// <summary>
		/// Gets or sets the image name property.
		/// </summary>
		/// <value>The image name property.</value>
		public string ImageNameProperty
		{
			get;
			set;
		}

		[XmlElement( "Exclusion" ) ]
		/// <summary>
		/// Gets or sets the Exclusion property.
		/// This specifies which grid positions are not part of the tile and thus allows for non-rectiniar tiles
		/// </summary>
		/// <value>The Exclusion property.</value>
		public Exclusion ExclusionProperty
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

		/// <summary>
		/// Gets the image property.
		/// </summary>
		/// <value>The image property.</value>
		public Bitmap ImageProperty
		{
			get
			{
				return image;
			}
		}

		/// <summary>
		/// Gets the grid number property.
		/// </summary>
		/// <value>The grid number property.</value>
		public int GridNumberProperty
		{
			get
			{
				return gridNumber;
			}
		}

		/// <summary>
		/// Gets or sets the x grid property.
		/// </summary>
		/// <value>The x grid property.</value>
		public int GridXProperty
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the y grid property.
		/// </summary>
		/// <value>The y grid property.</value>
		public int GridYProperty
		{
			get;
			set;
		}

		//
		// Private data
		//

		/// <summary>
		/// The image for the tile.
		/// </summary>
		private Bitmap image = null;

		/// <summary>
		/// The number used for this tile when identifying it on the game grid
		/// </summary>
		private int gridNumber = 0;

		/// <summary>
		/// The log tag for this class
		/// </summary>
		private readonly string LogTag = "Tile";
	}
}

