// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game state
// Filename:    TileMove.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The TileMove class represents a tile movement.
//
// Description:  The tile movement consists of a tile name, from grid position and to grid position. 
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

namespace SliderCon
{
	/// <summary>
	/// The TileMove class represents a tile movement.
	/// </summary>
	public class TileMove
	{
		/// <summary>
		/// Default constructor required for serialisation
		/// </summary>
		public TileMove()
		{
		}

		public TileMove( string tileIdentity, int fromX, int fromY, int toX, int toY)
		{
			IdentityProperty = tileIdentity;
			FromXProperty = fromX;
			FromYProperty = fromY;
			ToXProperty = toX;
			ToYProperty = toY;
		}

		[XmlAttribute( "identity" ) ]
		/// <summary>
		/// Gets or sets the name of the tile.
		/// </summary>
		/// <value>The name of the tile.</value>
		public string IdentityProperty
		{
			get;
			set;
		}

		[XmlAttribute( "fromX" ) ]
		/// <summary>
		/// Gets or sets the from X grid position.
		/// </summary>
		/// <value>The from X grid position</value>
		public int FromXProperty
		{
			get;
			set;
		}

		[XmlAttribute( "fromY" ) ]
		/// <summary>
		/// Gets or sets the from Y grid position.
		/// </summary>
		/// <value>The from Y grid position</value>
		public int FromYProperty
		{
			get;
			set;
		}

		[XmlAttribute( "toX" ) ]
		/// <summary>
		/// Gets or sets the to X grid position.
		/// </summary>
		/// <value>The to X grid position</value>
		public int ToXProperty
		{
			get;
			set;
		}

		[XmlAttribute( "toY" ) ]
		/// <summary>
		/// Gets or sets the to Y grid position.
		/// </summary>
		/// <value>The to Y grid position</value>
		public int ToYProperty
		{
			get;
			set;
		}
	}
}

