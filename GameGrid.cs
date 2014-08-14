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
	public class GameGrid : ICheckTileMovement
	{
		//
		// Public types
		//

		/// <summary>
		/// Delegate to call when the game has been successfully completed
		/// </summary>
		public delegate void GameCompletionDelegate();

		//
		// Public methods
		//

		/// <summary>
		/// Public constructor specifying the Board and Tiles used to occupy the grid
		/// </summary>
		public GameGrid( Board theBoard, List< Tile > theTiles, Completion theCompletion )
		{
			gameBoard = theBoard;
			tiles = theTiles;
			completionCheck = theCompletion;

			// Create a grid capable of holding the game board and occupy it with the game border and the tiles
			grid = new int[ gameBoard.WidthProperty, gameBoard.HeightProperty ];

			ApplyBoard();

			ApplyTiles();

			DisplayGrid();
		}

		/// <summary>
		/// Called when a tile is first selected for movement
		/// Save the starting gird position and the tile reference
		/// </summary>
		/// <param name="theSelectedTile">The selected tile.</param>
		public void TileSelected( Tile theSelectedTile )
		{
			// Save the current grid position as the last checked
			lastCheckedX = theSelectedTile.GridXProperty;
			lastCheckedY = theSelectedTile.GridYProperty;

			selectedTile = theSelectedTile;
		}

		/// <summary>
		/// Checks the tile movement.
		/// Check all the positions from the last checked to the new position, until either an invalid move is detected 
		/// </summary>
		/// <returns><c>true</c>, if tile movement was checked, <c>false</c> otherwise.</returns>
		/// <param name="xNewGrid">X new grid.</param>
		/// <param name="yNewGrid">Y new grid.</param>
		/// <param name="xBias">If set to <c>true</c> x bias.</param>
		/// <param name="xInvalid">X valid.</param>
		/// <param name="yInvalid">Y valid.</param>
		public bool CheckTileMovement( int xNewGrid, int yNewGrid, bool xBias, ref bool xInvalid, ref bool yInvalid )
		{
			// Assume its a valid move
			bool validMove = true;
			xInvalid = false;
			yInvalid = false;

			// Only validate the move if not previously checked
			if ( ( yNewGrid != lastCheckedX ) || ( xNewGrid != lastCheckedX ) )
			{
				// Keep local track of which movement is valid
				bool xValid = false;
				bool yValid = false;

				// Test is complete only when all positions have been tested
				bool testComplete = false;

				// Start checking all positions from the last checked to the new grid position
				while ( ( testComplete == false ) && ( validMove == true ) )
				{
					// Determine how far the test position is from the new grid position
					int dX = xNewGrid - lastCheckedX;
					int dY = yNewGrid - lastCheckedY;

					// If the new grid position has been reached then get out of this loop
					if ( ( dX == 0 ) && ( dY == 0 ) )
					{
						testComplete = true;
					}
					else
					{
						// Test the X and Y directions as required
						xValid = false;
						yValid = false;

						// If further away in the X direction then test that direction first
						if ( Math.Abs( dX ) > Math.Abs( dY ) )
						{
							xValid = CheckMove( selectedTile, lastCheckedX + Math.Sign( dX ), lastCheckedY );
							if ( xValid == true )
							{
								lastCheckedX += Math.Sign( dX );
							}
							else
							{
								// Move is not valid - stop testing at this point
								validMove = false;
							}
						}
						// If further away in the Y direction then test that direction first
						else if ( Math.Abs( dY ) > Math.Abs( dX ) )
						{
							yValid = CheckMove( selectedTile, lastCheckedX, lastCheckedY + Math.Sign( dY ) );
							if ( yValid == true )
							{
								lastCheckedY += Math.Sign( dY );
							}
							else
							{
								// Move is not valid - stop testing at this point
								validMove = false;
							}
						}
						// If equally far away in X and Y then attempt to test both.
						else if ( Math.Abs( dX ) == Math.Abs( dY ) )
						{
							// First test the individual X and Y moves
							xValid = CheckMove( selectedTile, lastCheckedX + Math.Sign( dX ), lastCheckedY );
							yValid = CheckMove( selectedTile, lastCheckedX, lastCheckedY + Math.Sign( dY ) );

							// If both individual moves are OK then try together
							if ( ( xValid == true ) && ( yValid == true ) )
							{
								if ( CheckMove( selectedTile, lastCheckedX + Math.Sign( dX ), lastCheckedY + Math.Sign( dY ) ) == false )
								{
									// Both individual moves are OK but the diagonal move is not.
									// Choose one of the individual moves to keep based on the overall event movement
									if ( xBias == true )
									{
										// Just keep the X
										lastCheckedX += Math.Sign( dX );
									}
									else
									{
										// Just keep the Y
										lastCheckedY += Math.Sign( dY );
									}

									// Stop at this point
									validMove = false;
								}
								else
								{
									// Diagonal move is valid - keep it
									lastCheckedX += Math.Sign( dX );
									lastCheckedY += Math.Sign( dY );
								}
							}
							else
							{
								// Use whichever (if any) of the individual moves
								if ( xValid == true )
								{
									lastCheckedX += Math.Sign( dX );
								}

								if ( yValid == true )
								{
									lastCheckedY += Math.Sign( dY );
								}

								// Stop at this point
								validMove = false;
							}
						}
					}
				}

				// If the move is not valid then tell the view which direction caused the problem
				if ( ( lastCheckedX != xNewGrid ) && ( xValid == false ) )
				{
					xInvalid = true;
				}

				if ( ( lastCheckedY != yNewGrid ) && ( yValid == false ) )
				{
					yInvalid = true;
				}

				// The move is assumed to be valid if at least one or the directions was valid
				validMove = ( ( lastCheckedX == xNewGrid ) || ( lastCheckedY == yNewGrid ) );

				Log.Debug( LogTag, string.Format( "Last valid check {0}, {1}", lastCheckedX, lastCheckedY ) );
			}

			return validMove;
		}

		/// <summary>
		/// Called when a selected tile has been moved
		/// Update the grid and check for completion
		/// </summary>
		public void TileMoved()
		{
			ApplyBoard();

			ApplyTiles();

			DisplayGrid();

			if ( completionDelegate != null )
			{
				if ( completionCheck.CheckCompletion( tiles ) == true )
				{
					completionDelegate();
				}
			}
		}

		/// <summary>
		/// Gets the last checked X property.
		/// </summary>
		/// <value>The last checked X property.</value>
		public int LastCheckedXProperty
		{
			get
			{
				return lastCheckedX;
			}
		}

		/// <summary>
		/// Gets the last checked Y property.
		/// </summary>
		/// <value>The last checked Y property.</value>
		public int LastCheckedYProperty
		{
			get
			{
				return lastCheckedY;
			}
		}

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

		/// <summary>
		/// The game board.
		/// </summary>
		/// <value>The board property.</value>
		public Board BoardProperty
		{
			get
			{
				return gameBoard;
			}
		}

		public GameCompletionDelegate OnGameCompletion
		{
			set
			{
				completionDelegate = value;
			}
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
			gameBoard.ExclusionProperty.Apply( grid, -1 );
		}
	
		private void ApplyTiles()
		{
			foreach ( Tile tileToApply in tiles )
			{
				if ( tileToApply.ExclusionProperty != null )
				{
					tileToApply.ExclusionProperty.Apply( grid, tileToApply.GridNumberProperty, tileToApply.HeightProperty, tileToApply.WidthProperty,
						tileToApply.GridXProperty, tileToApply.GridYProperty );
				}
			}
		}

		/// <summary>
		/// Checks whether a tile can be positions at the specified location
		/// </summary>
		/// <returns><c>true</c>, if move is possible, <c>false</c> otherwise.</returns>
		/// <param name="movedTile">Moved tile.</param>
		/// <param name="newX">New x.</param>
		/// <param name="newY">New y.</param>
		private bool CheckMove( Tile movedTile, int newX, int newY )
		{
			bool validMove = false;

			if ( movedTile.ExclusionProperty != null )
			{
				validMove = movedTile.ExclusionProperty.CheckApplication( grid, movedTile.GridNumberProperty, newX, newY );
			}

			return validMove;
		}

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

		/// <summary>
		/// The Completion instance to be used when testing for game completion
		/// </summary>
		private readonly Completion completionCheck = null;

		private int lastCheckedX = -1;
		private int lastCheckedY = -1;

		private Tile selectedTile = null;

		/// <summary>
		/// Delegate to call when the game has been completed
		/// </summary>
		private GameCompletionDelegate completionDelegate = null;

		/// <summary>
		/// The log tag for this class
		/// </summary>
		private readonly string LogTag = "GameGrid";
	}
}

