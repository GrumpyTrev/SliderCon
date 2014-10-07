// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game state
// Filename:    MoveHistory.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The MoveHistory class maintains a bounded history of all the moves made for the current game instance.
//
// Description:  The history is made up of a collection of TileTemplate instances. 
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
	/// The MoveHistory class maintains a bounded history of all the moves made for the current game instance.
	/// A Queue cannot be easily used as it is not serialisable.
	/// For now use a List
	/// </summary>
	public class MoveHistory
	{
		public MoveHistory()
		{
		}

		public void AddMove( TileMove theMove )
		{	
			bool duplicateProcessed = false;

			// Check whether or not this move undoes the previous move
			if ( MoveCountProperty > 0 )
			{
				TileMove previousMove = TileMovesProperty[ MoveCountProperty - 1 ];
				if ( ( previousMove.IdentityProperty == theMove.IdentityProperty ) &&
					( previousMove.FromXProperty == theMove.ToXProperty ) &&
					( previousMove.FromYProperty == theMove.ToYProperty ) )
				{
					TileMovesProperty.RemoveAt( MoveCountProperty - 1 );
					duplicateProcessed = true;
				}
			}

			if ( duplicateProcessed == false )
			{
				TileMovesProperty.Add( theMove );
				if ( MoveCountProperty > MaxQueueCount )
				{
					TileMovesProperty.RemoveAt( 0 );
				}
			}
		}

		/// <summary>
		/// Clear the move list
		/// </summary>
		public void Reset()
		{
			TileMovesProperty.Clear();
		}

		public TileMove RemoveMove()
		{
			TileMove removedTile = null;

			if ( MoveCountProperty > 0 )
			{
				removedTile = TileMovesProperty[ MoveCountProperty - 1 ];
				TileMovesProperty.RemoveAt( MoveCountProperty - 1 );
			}

			return removedTile;
		}

		[ XmlIgnore ]
		public int MoveCountProperty
		{
			get
			{
				return TileMovesProperty.Count;
			}
		}

		[XmlElement( "TileMove" ) ]
		public List< TileMove > TileMovesProperty
		{
			get
			{
				return tileMoves;
			}

			set
			{
				tileMoves = value;
			}
		}

		//
		// Private data
		//

		private List< TileMove > tileMoves = null;

		private const int MaxQueueCount = 9999;
	}
}

