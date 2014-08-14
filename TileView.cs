// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game redering
// Filename:    TileView.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The TileView class displays a tile on the board.
//
// Description:  As purpose for now
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

using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace SliderCon
{
	/// <summary>
	/// The BoardView class displays the current state of a game
	/// </summary>
	public class TileView : ImageView
	{
		/// <summary>
		/// Public constructor
		/// </summary>
		public TileView( Context tileContext, Tile associatedTile ) : base( tileContext )
		{
			wrappedTile = associatedTile;
		}

		/// <summary>
		/// Gets the tile property.
		/// </summary>
		/// <value>The tile property.</value>
		public Tile TileProperty
		{
			get
			{
				return wrappedTile;
			}
		}

		//
		// Private data
		//

		private readonly Tile wrappedTile = null;
	}
}

