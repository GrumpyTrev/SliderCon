// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game state
// Filename:    GamePlayer.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The GamePlayer class represents a GameInstance being played on a BoardView.
//
// Description:   
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

using Android.Util;
using Android.Widget;
using Android.Graphics;

namespace SliderCon
{
	/// <summary>
	/// The GamePlayer class represents a GameInstance being played on a BoardView.
	/// </summary>
	public class GamePlayer
	{
		//
		// Public types
		//

		/// <summary>
		/// Delegate to call when the game has been successfully completed
		/// </summary>
		public delegate void GameCompletionDelegate();

		/// <summary>
		/// Delegate to call when the back button has been pressed
		/// </summary>
		public delegate void BackMoveDelegate( TileMove theMove );

		//
		// Public methods
		//

		/// <summary>
		/// Public constructor specifying the Board and Tiles used to occupy the grid
		/// </summary>
		public GamePlayer( Board theBoard, List< Tile > theTiles, Completion theCompletion, MoveHistory theHistory )
		{
			gameBoard = theBoard;
			tiles = theTiles;
			completionCheck = theCompletion;
			history = theHistory;

			// Create a GameGrid to perform move checking
			grid = new GameGrid( theBoard, tiles );
		}

		/// <summary>
		/// Called when a tile is first selected for movement
		/// Save the starting gird position and the tile reference
		/// </summary>
		/// <param name="theSelectedTile">The selected tile.</param>
		public void TileSelected( Tile theSelectedTile )
		{
			// Save the selected tile
			selectedTile = theSelectedTile;
		}

		/// <summary>
		/// Checks the tile movement.
		/// Check all the positions from the last checked to the new position, until either an invalid move is detected or all such positions have been checked 
		/// </summary>
		/// <returns><c>true</c>, if tile movement was checked, <c>false</c> otherwise.</returns>
		/// <param name="currentGridPositions">List of grid positions to use as the starting point of a number of checks</param>
		/// <param name="xNewGrid">X new grid.</param>
		/// <param name="yNewGrid">Y new grid.</param>
		/// <param name="xBias">If set to true if on a diagonal move the X should be tried before the Y x bias.</param>
		/// <param name="xInvalid">Returns true if the reason for a failed move is an invalid X position.</param>
		/// <param name="yInvalid">Returns true if the reason for a failed move is an invalid Y position.</param>
		public bool CheckTileMovement( List< Point > currentGridPositions, int xNewGrid, int yNewGrid, bool xBias, ref bool xInvalid, ref bool yInvalid )
		{
			// Assume its a valid move
			bool validMove = true;

			int xMove = xNewGrid - currentGridPositions[ 0 ].X;
			int yMove = yNewGrid - currentGridPositions[ 0 ].Y;
			List< Point >.Enumerator enumerator = currentGridPositions.GetEnumerator();
			while ( ( validMove == true )  &&  ( enumerator.MoveNext() == true ) )
			{
				Point checkPosition = enumerator.Current;
				validMove = CheckTileMovement( checkPosition, xMove + checkPosition.X, yMove + checkPosition.Y, xBias, ref xInvalid, ref yInvalid );
			}

			return validMove;
		}

		/// <summary>
		/// Checks the tile movement.
		/// Check all the positions from the last checked to the new position, until either an invalid move is detected or all such positions have been checked 
		/// </summary>
		/// <returns><c>true</c>, if tile movement was checked, <c>false</c> otherwise.</returns>
		/// <param name="xNewGrid">X new grid.</param>
		/// <param name="yNewGrid">Y new grid.</param>
		/// <param name="xBias">If set to true if on a diagonal move the X should be tried before the Y x bias.</param>
		/// <param name="xInvalid">Returns true if the reason for a failed move is an invalid X position.</param>
		/// <param name="yInvalid">Returns true if the reason for a failed move is an invalid Y position.</param>
		public bool CheckTileMovement( Point currentGridPosition, int xNewGrid, int yNewGrid, bool xBias, ref bool xInvalid, ref bool yInvalid )
		{
			// Assume its a valid move
			bool validMove = true;
			xInvalid = false;
			yInvalid = false;

			// Keep local track of which movement is valid
			bool xValid = false;
			bool yValid = false;

				// Use the current grid position as the starting point
			int checkX = currentGridPosition.X;
			int checkY = currentGridPosition.Y;

			// Test is complete only when all positions have been tested
			bool testComplete = false;

			// Start checking all positions from the last checked to the new grid position
			while ( ( testComplete == false ) && ( validMove == true ) )
			{
				// Determine how far the test position is from the new grid position
				int dX = xNewGrid - checkX;
				int dY = yNewGrid - checkY;

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
						xValid = grid.CheckMove( selectedTile, checkX + Math.Sign( dX ), checkY );
						if ( xValid == true )
						{
							checkX += Math.Sign( dX );
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
						yValid = grid.CheckMove( selectedTile, checkX, checkY + Math.Sign( dY ) );
						if ( yValid == true )
						{
							checkY += Math.Sign( dY );
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
						xValid = grid.CheckMove( selectedTile, checkX + Math.Sign( dX ), checkY );
						yValid = grid.CheckMove( selectedTile, checkX, checkY + Math.Sign( dY ) );

						// If both individual moves are OK then try together
						if ( ( xValid == true ) && ( yValid == true ) )
						{
							if ( grid.CheckMove( selectedTile, checkX + Math.Sign( dX ), checkY + Math.Sign( dY ) ) == false )
							{
								// Both individual moves are OK but the diagonal move is not.
								// Choose one of the individual moves to keep based on the overall event movement
								if ( xBias == true )
								{
									// Just keep the X
									checkX += Math.Sign( dX );
									yValid = false;
								}
								else
								{
									// Just keep the Y
									checkY += Math.Sign( dY );
									xValid = false;
								}

								// Stop at this point
								validMove = false;
							}
							else
							{
								// Diagonal move is valid - keep it
								checkX += Math.Sign( dX );
								checkY += Math.Sign( dY );
							}
						}
						else
						{
							// Use whichever (if any) of the individual moves
							if ( xValid == true )
							{
								checkX += Math.Sign( dX );
							}

							if ( yValid == true )
							{
								checkY += Math.Sign( dY );
							}

							// Stop at this point
							validMove = false;
						}
					}
				}
			}

			// If the move is not valid then tell the view which direction caused the problem
			if ( ( checkX != xNewGrid ) && ( xValid == false ) )
			{
				xInvalid = true;
			}

			if ( ( checkY != yNewGrid ) && ( yValid == false ) )
			{
				yInvalid = true;
			}

			// The move is assumed to be valid if at least one or the directions was valid
			validMove = ( ( checkX == xNewGrid ) || ( checkY == yNewGrid ) );

			Log.Debug( LogTag, string.Format( "Last valid check {0}, {1}", checkX, checkY ) );

			return validMove;
		}

		/// <summary>
		/// Called when a selected tile has been moved
		/// Update the grid and check for completion
		/// </summary>
		public void TileMoved( TileMove move )
		{
			// Position the selected tile's grid position
			if ( ( selectedTile != null ) && ( selectedTile.IdentityProperty == move.IdentityProperty ) )
			{
				selectedTile.GridXProperty = move.ToXProperty;
				selectedTile.GridYProperty = move.ToYProperty;
			}

			history.AddMove( move );

			if ( backButton != null )
			{
				backButton.Enabled =  ( history.MoveCountProperty > 0 );
			}

			if ( moveCount != null )
			{
				moveCount.Text = history.MoveCountProperty.ToString();
			}

			// Update the grid
			grid.TileMoved();

			if ( completionDelegate != null )
			{
				if ( completionCheck.CheckCompletion( tiles ) == true )
				{
					completionDelegate();
				}
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

		public BackMoveDelegate OnBackMove
		{
			set
			{
				undoDelegate = value;
			}
		}

		public TextView MoveCountProperty
		{
			set
			{
				moveCount = value;
				moveCount.Text = history.MoveCountProperty.ToString();
			}
		}

		public Button BackButtonProperty
		{
			set
			{
				if ( ( value == null ) && ( backButton != null ) )
				{
					backButton.Click -= HandleBackButton;
				}
				else if ( value != null )
				{
					backButton = value;

					backButton.Click += HandleBackButton; 

					backButton.Enabled =  ( history.MoveCountProperty > 0 );
				}
			}
		}

		private void HandleBackButton( object sender, EventArgs e )
		{
			TileMove lastMove = history.RemoveMove();
			if ( lastMove != null )
			{
				if ( moveCount != null )
				{
					moveCount.Text = history.MoveCountProperty.ToString();
				}

				backButton.Enabled =  ( history.MoveCountProperty > 0 );

				// Find the tile associated with this move
				Tile associatedtile = TilesProperty.Find( theTile => theTile.IdentityProperty == lastMove.IdentityProperty );
				if ( associatedtile != null )
				{
					associatedtile.GridXProperty = lastMove.FromXProperty;
					associatedtile.GridYProperty = lastMove.FromYProperty;
				}

				if ( undoDelegate != null )
				{
					undoDelegate( lastMove );
				}
			}
		}

		//
		// Private data
		//

		/// <summary>
		/// The selected tile.
		/// </summary>
		private Tile selectedTile = null;

		/// <summary>
		/// The game board.
		/// </summary>
		private readonly Board gameBoard = null;

		/// <summary>
		/// The game tiles
		/// </summary>
		private readonly List< Tile > tiles = null;

		/// <summary>
		/// The move history instance
		/// </summary>
		private readonly MoveHistory history = null;

		/// <summary>
		/// The Completion instance to be used when testing for game completion
		/// </summary>
		private readonly Completion completionCheck = null;

		/// <summary>
		/// The GameGrid used to check move validity.
		/// </summary>
		private readonly GameGrid grid = null;

		/// <summary>
		/// Delegate to call when the game has been completed
		/// </summary>
		private GameCompletionDelegate completionDelegate = null;

		private BackMoveDelegate undoDelegate = null;

		private TextView moveCount = null;

		private Button backButton = null;

		/// <summary>
		/// The log tag for this class
		/// </summary>
		private readonly string LogTag = "GamePlayer";
	}
}

