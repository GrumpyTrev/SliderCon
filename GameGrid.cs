// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game state
// Filename:    GameGrid.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The GameGrid class maintains a representation of the occupied board positions.
//
// Description:  The GameGrid is used to determine valid moves and to reflect a valid move back to the TilePosition list 
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
using System.Collections.Generic;
using System.Text;

using Android.Util;

namespace SliderCon
{
	/// <summary>
	/// The GameGrid class maintains a representation of the occupied board positions.
	/// </summary>
	public class GameGrid
	{
		//
		// Public methods
		//

		/// <summary>
		/// Public constructor specifying the Board and Tiles used to occupy the grid
		/// </summary>
		public GameGrid( Board theBoard, List< Tile > theTiles )
		{
			gameBoard = theBoard;
			tiles = theTiles;

			// Create a grid capable of holding the game board and occupy it with the game border and the tiles
			grid = new int[ gameBoard.WidthProperty, gameBoard.HeightProperty ];

			ApplyBoard();

			ApplyTiles();

//			DisplayGrid();
		}

		/// <summary>
		/// Called when a tile in the grid has been moved. Update the grid
		/// </summary>
		public void TileMoved()
		{
			ApplyBoard();

			ApplyTiles();

//			DisplayGrid();
		}

		/// <summary>
		/// Checks whether a tile can be positions at the specified location
		/// </summary>
		/// <returns><c>true</c>, if move is possible, <c>false</c> otherwise.</returns>
		/// <param name="movedTile">Moved tile.</param>
		/// <param name="newX">New x.</param>
		/// <param name="newY">New y.</param>
		public bool CheckMove( Tile movedTile, int newX, int newY )
		{
			return movedTile.CheckMove( grid, newX, newY );
		}

		//
		// Private methods
		//

		private void ApplyBoard()
		{
			// Clear the grid to zeroes
			grid.Initialize();
			for ( int yIndex = 0; yIndex < gameBoard.HeightProperty; ++yIndex )
			{
				for ( int xIndex = 0; xIndex < gameBoard.WidthProperty; ++xIndex )
				{
					grid[ xIndex, yIndex ] = 0;
				}
			}

			// Apply the border of the board
			gameBoard.ApplyBorderToGrid( grid );
		}
	
		private void ApplyTiles()
		{
			foreach ( Tile tileToApply in tiles )
			{
				tileToApply.ApplyTileToGrid( grid );
			}
		}

/*
		private void DisplayGrid()
		{
			for ( int yIndex = 0; yIndex < gameBoard.HeightProperty; ++yIndex )
			{
				StringBuilder displayLine = new StringBuilder();

				for ( int xIndex = 0; xIndex < gameBoard.WidthProperty; ++xIndex )
				{
					if ( grid[ xIndex, yIndex ] == -1 )
					{
						displayLine.Append( 'X' );
					}
					else if ( grid[ xIndex, yIndex ] == 0 )
					{
						displayLine.Append( ' ' );
					}
					else
					{
						displayLine.AppendFormat( "{0}", grid[ xIndex, yIndex ] );
					}
				}

				Log.Debug( LogTag, displayLine.ToString() );

			}
		}
*/

		//
		// Private data
		//

		/// <summary>
		/// Array holding the board and current tile positions
		/// </summary>
		private readonly int[ , ] grid = null;

		/// <summary>
		/// The game board.
		/// </summary>
		private readonly Board gameBoard = null;

		/// <summary>
		/// The game tiles
		/// </summary>
		private readonly List< Tile > tiles = null;
	}
}

