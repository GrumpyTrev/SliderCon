// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game definition
// Filename:    Board.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The Board class contains the definition of the board used for a game.
//
// Description:  The class specifies the dimensions of the board (in cells) and the image used to display.  
//				 It also specifies which cells of the board are not in the playing area.
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
using System.IO;

using Android.Graphics;
using Android.Util;

namespace SliderCon
{
	/// <summary>
	/// The Board class contains the definition of the board used for a game.
	/// </summary>
	public class Board
	{
		/// <summary>
		/// Public constructor required for serialisation
		/// </summary>
		public Board()
		{
		}

		/// <summary>
		/// Initialise this instance.
		/// </summary>
		public bool Initialise( string gameDirectory )
		{
			bool initialisedOk = false;

			// Read in the image for the board
			if ( ImageNameProperty.Length > 0 )
			{
				image = ApplicationData.LoadImage( System.IO.Path.Combine( gameDirectory, ImageNameProperty ) );
				if ( image != null )
				{
					initialisedOk = true;
				}

				// If no Exclusion instance has been deserialised then create an empty one
				if ( ExclusionProperty == null )
				{
					ExclusionProperty = new Exclusion();
				}
			}
			else
			{
				Log.Debug( LogTag, string.Format( "No image defined for board in directory {0}", gameDirectory ) );
			}

			return initialisedOk;
		}

		/// <summary>
		/// Apply the border specified in the ExclusionProperty to the supplied grid
		/// </summary>
		/// <param name="grid">The grid to apply the border to.</param>
		public void ApplyBorderToGrid( int [ , ] grid )
		{
			ExclusionProperty.Apply( grid, -1 );
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
		/// </summary>
		/// <value>The Exclusion property.</value>
		public Exclusion ExclusionProperty
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

		//
		// Private data
		//

		/// <summary>
		/// The image for the board.
		/// </summary>
		private Bitmap image = null;

		/// <summary>
		/// The log tag for this class
		/// </summary>
		private readonly string LogTag = "Board";
	}
}

