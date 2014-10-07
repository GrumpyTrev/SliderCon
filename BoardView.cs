// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game redering
// Filename:    BoardView.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The BoardView class displays the current state of a game.
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
using System.Collections.Generic;
using System.Text;

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
	public class BoardView : RelativeLayout, View.IOnTouchListener
	{
		/// <summary>
		/// Public constructor
		/// </summary>
		public BoardView( Context viewContext, IAttributeSet attributes ) : base( viewContext, attributes )
		{
		}

		/// <summary>
		/// Initialise the view.
		/// Save the passed in variables and render the view if required
		/// </summary>
		/// <param name="theBoardBitmap">The Bitmap used for the board</param>
		/// <param name="theTilesCollection">The tiles to render on the board</param>
		/// <param name="checker">A ICheckTileMovement instance used to check tile movements</param>
		/// <param name="width">The width of the board in terms of grid positions</param>
		/// <param name="render">If set to <c>true</c> render.</param>
		public void Initialise( Bitmap theBoardBitmap, List< Tile > theTilesCollection, GamePlayer thePlayer, int width, bool render )
		{
			// Save the board image, the tile collection and the movement checker
			boardBitmap = theBoardBitmap;
			tilesCollection = theTilesCollection;
			movementChecker = thePlayer;
			boardWidth = width;

			if ( render == true )
			{
				MainActivity.ActivityProperty.RunOnUiThread( () =>
				{
					// Initialise the board background
					InitialiseBoardBackground();

					// Load the tiles
					LoadTiles();
				} );
			}
		}

		/// <summary>
		/// Called when a move is to be undone.
		/// The tile has already been updated. Update the associated TileView position
		/// </summary>
		/// <param name="theMove">The move.</param>
		public void BackMove( TileMove theMove )
		{
			// Need to find the TileView with a Tile with the correct indentity
			bool tileFound = false;

			int childCount = 0;
			TileView childView = null;
			while ( ( tileFound == false ) && ( childCount < ChildCount ) )
			{
				childView = ( TileView )GetChildAt( childCount++ );
				if ( childView.TileProperty.IdentityProperty == theMove.IdentityProperty )
				{
					tileFound = true;
				}
			}

			if ( tileFound == true )
			{
				childView.SetX( ViewXFromGrid( childView.TileProperty.GridXProperty ) );
				childView.SetY( ViewYFromGrid( childView.TileProperty.GridYProperty ) );
			}
			else
			{
				Log.Debug( LogTag, string.Format( "Could not find tile {0}", theMove.IdentityProperty ) );
			}
		}


		//
		// Private methods
		//

		/// <summary>
		/// Initialises the board background.
		/// Scale the bitmap and set as the background.
		/// Save the applied aspect change for applying to the tiles
		/// </summary>
		private void InitialiseBoardBackground()
		{
			// Work out how much the board bitmap needs to be scaled
			float aspectFactor = Math.Min( ( float )Height / ( float )boardBitmap.Height, ( float )Width / ( float )boardBitmap.Width );

			// Work out the scaled height and width of the board's bitmap
			int preferredWidth = ( int ) ( ( float )boardBitmap.Height * aspectFactor );
			int preferredHeight = ( int ) ( ( float )boardBitmap.Height * aspectFactor );

			// Scale the bitmap, centre it and set it as the background
			Bitmap scaledBitmap = Bitmap.CreateScaledBitmap( boardBitmap, preferredWidth, preferredHeight, false );

			BitmapDrawable backgroundDrawable = new BitmapDrawable( Resources, scaledBitmap );
			backgroundDrawable.Gravity = GravityFlags.Center;

			Background = backgroundDrawable;

			// The board's offset from the top and LHS of the view can now be worked out and saved
			xOffset = ( Width - preferredWidth ) / 2;
			yOffset = ( Height - preferredHeight ) / 2;

			// Assuming a square pixel, the number of pixels per grid postion can also be determined
			pixelsPerGridPosition = preferredWidth / boardWidth;
		}

		/// <summary>
		/// Loads the tiles from current instance.
		/// </summary>
		private void LoadTiles()
		{
			// Remove any existing tiles
			RemoveViews( 0, ChildCount );

			// Wrap each TilePosition in the current instance in a TileView.
			// Position and size the TileView based on the grid scale factor and fill with the Tile's bitmap 

			List< Tile >.Enumerator tilesEnumerator = tilesCollection.GetEnumerator();
			while ( tilesEnumerator.MoveNext() == true )
			{
				Tile instanceTile = tilesEnumerator.Current;

				// Determine size and position of this tile
				RelativeLayout.LayoutParams parameters = new RelativeLayout.LayoutParams( instanceTile.WidthProperty * pixelsPerGridPosition,
					instanceTile.HeightProperty * pixelsPerGridPosition );

				parameters.TopMargin = ViewYFromGrid( instanceTile.GridYProperty );
				parameters.LeftMargin = ViewXFromGrid( instanceTile.GridXProperty );

				TileView tileWrapper = new TileView( Context, instanceTile );
				tileWrapper.SetImageBitmap( instanceTile.ImageProperty );
				tileWrapper.SetOnTouchListener( this );

				AddView( tileWrapper, parameters);
			}
		}

		/// <summary>
		/// Called in response to touch events
		/// </summary>
		/// <param name="touchedView">Touched view.</param>
		/// <param name="viewEvent">View event.</param>
		public bool OnTouch( View touchedView, MotionEvent viewEvent )
		{
			bool touched = true;

			if ( viewEvent.Action == MotionEventActions.Down )
			{
				ProcessDownEvent( touchedView, viewEvent );
			}
			else if ( viewEvent.Action == MotionEventActions.Up )
			{
				ProcessUpEvent();
			}
			else if ( viewEvent.Action == MotionEventActions.Move )
			{
				// Check that a tile is being moved, and that the movement still refers to this tile
				if ( movedTile != null )
				{
					Rect tileBounds = new Rect();
					movedTile.GetGlobalVisibleRect( tileBounds );
					if ( tileBounds.Contains( ( int )viewEvent.RawX, ( int )viewEvent.RawY ) == true )
					{
						// Determine how far has been moved since last time. 
						float dxEvent = viewEvent.RawX - lastDraggedPosition.X;
						float dyEvent = viewEvent.RawY - lastDraggedPosition.Y;

						Log.Debug( LogTag, string.Format( "Event from {0}, {1} by {2}, {3} to {4}, {5}", lastDraggedPosition.X, lastDraggedPosition.Y,
							dxEvent, dyEvent, viewEvent.RawX, viewEvent.RawY ) );

						// Apply any movement constraint
						if ( movedTile.TileProperty.MovementConstraintProperty == Tile.MovementConstraintType.Horizontal )
						{
							dyEvent = 0;
						}
						else if ( movedTile.TileProperty.MovementConstraintProperty == Tile.MovementConstraintType.Vertical )
						{
							dxEvent = 0;
						}

						// Determine the new pixel position of the tile being moved
						float dxTile = movedTile.GetX() + dxEvent;
						float dyTile = movedTile.GetY() + dyEvent;

						Log.Debug( LogTag, string.Format( "Tile from {0}, {1} to {2}, {3}", movedTile.GetX(), movedTile.GetY(), dxTile, dyTile ) );

						// In working out the new grid positions to test the direction of movement has to be taken into account
						int xNewGrid = 0;
						if ( dxEvent >= 0 )
						{
							// Round up
							xNewGrid = GridXFromView( dxTile );
						}
						else
						{
							// Round down
							xNewGrid = ( int )( dxTile - xOffset ) / pixelsPerGridPosition;
						}

						int yNewGrid = 0;
						if ( dyEvent >= 0 )
						{
							// Round up
							yNewGrid = GridYFromView( dyTile);
						}
						else
						{
							// Round down
							yNewGrid = ( int )( dyTile - yOffset ) / pixelsPerGridPosition;
						}

						Log.Debug( LogTag, string.Format( "Last checked grid {0}, {1} new grid {2}, {3} original grid {4}, {5}", movementChecker.LastCheckedXProperty,
							movementChecker.LastCheckedYProperty, xNewGrid, yNewGrid, movedTile.TileProperty.GridXProperty, movedTile.TileProperty.GridYProperty ) );

						// Validate the move
						bool xInvalid = false;
						bool yInvalid = false;
						bool validMove = movementChecker.CheckTileMovement( xNewGrid, yNewGrid, Math.Abs( dxEvent ) > Math.Abs( dyEvent ), ref xInvalid, ref yInvalid );

						// If the move was not valid then adjust the delta for any failed direction.
						if ( xInvalid == true )
						{
							dxTile = ViewXFromGrid( movementChecker.LastCheckedXProperty );
							Log.Debug( LogTag, string.Format( "New X {0} is not last checked {1} setting X to {2}", xNewGrid, movementChecker.LastCheckedXProperty, dxTile ) );
						}

						if ( yInvalid == true )
						{
							dyTile = ViewYFromGrid( movementChecker.LastCheckedYProperty );
							Log.Debug( LogTag, string.Format( "New Y {0} is not last checked {1} setting Y to {2}", yNewGrid, movementChecker.LastCheckedYProperty, dyTile ) );
						}

						// If either of the new grid positions are valid then move the tile
						if ( validMove == true )
						{
							Log.Debug( LogTag, string.Format( "Valid move - tile now at {0}, {1} new grid {2}, {3}", dxTile, dyTile, GridXFromView( dxTile ), GridYFromView( dyTile ) ) );

							movedTile.SetX( dxTile );
							movedTile.SetY( dyTile );

							lastDraggedPosition = new PointF( viewEvent.RawX, viewEvent.RawY );
						}
					}
					else
					{
						// No longer over this tile - treat as an Up event
						ProcessUpEvent();
					}
				}
			}

			return touched;
		}

		/// <summary>
		/// Processes up event.
		/// If the selected tile has been moved then move it to its new posiiton on the view and redraw the view
		/// </summary>
		private void ProcessUpEvent()
		{
			// Check that a tile is being moved
			if ( movedTile != null )
			{
				// Has the selected tile been moved
				if ( ( movedTile.GetX() != ViewXFromGrid( movedTile.TileProperty.GridXProperty ) ) ||
					( movedTile.GetY() != ViewYFromGrid( movedTile.TileProperty.GridYProperty ) ) )
				{
					// Work out for each direction the nearest grid division
					int newX = ( int )Math.Round( ( movedTile.GetX() - xOffset ) / pixelsPerGridPosition );
					int newY = ( int )Math.Round( ( movedTile.GetY() - yOffset ) / pixelsPerGridPosition );

					// Tile has moved, has it moved to a new grid position
					if ( ( newX != movedTile.TileProperty.GridXProperty ) || ( newY != movedTile.TileProperty.GridYProperty ) )
					{
						Log.Debug( LogTag, string.Format( "Tile has moved from {0}, {1} to {2}, {3}", movedTile.TileProperty.GridXProperty, movedTile.TileProperty.GridYProperty,
							newX, newY ) );

						// Create a TileMove record for this
						TileMove move = new TileMove( movedTile.TileProperty.IdentityProperty, movedTile.TileProperty.GridXProperty,
							movedTile.TileProperty.GridYProperty, newX, newY );

						// Update the grid used by the board
						movementChecker.TileMoved( move );
					}

					// Align the view position to the grid position
					movedTile.SetX( ViewXFromGrid( movedTile.TileProperty.GridXProperty ) );
					movedTile.SetY( ViewYFromGrid( movedTile.TileProperty.GridYProperty ) );
				}
			}

			movedTile = null;
			lastDraggedPosition = null;
		}

		/// <summary>
		/// Processes down event.
		/// Record the tile that has been selected and initialise the drag and checking variables
		/// </summary>
		/// <param name="touchedView">Touched view.</param>
		/// <param name="viewEvent">View event.</param>
		private void ProcessDownEvent( View touchedView, MotionEvent viewEvent )
		{
			// Make sure a tile has been touched
			TileView touchedTile = touchedView as TileView;
			if ( touchedTile != null )
			{
				// Save a reference to the tile being moved and the start drag position
				movedTile = touchedTile;
				lastDraggedPosition = new PointF( viewEvent.RawX, viewEvent.RawY );

				// Notify the movement checker of the tile being moved
				movementChecker.TileSelected( movedTile.TileProperty );

				Log.Debug( LogTag, string.Format( "Down: Raw event position {0}, {1} current tile position {2}, {3}, grid {4}, {5} ", viewEvent.RawX, viewEvent.RawY, 
					movedTile.GetX(), movedTile.GetY(), movedTile.TileProperty.GridXProperty, movedTile.TileProperty.GridYProperty ) );
			}
		}

		/// <summary>
		/// This is called during layout when the size of this view has changed.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="oldWidth">Old width.</param>
		/// <param name="oldHeight">Old height.</param>
		protected override void OnSizeChanged( int width, int height, int oldWidth, int oldHeight )
		{
			base.OnSizeChanged( width, height, oldWidth, oldHeight );

			// This can be called before the view has been sized or the application initialised so check first
			if ( ( Width > 0 ) && ( Height > 0 ) && ( movementChecker != null )  )
			{	
				MainActivity.ActivityProperty.RunOnUiThread( () =>
				{
					InitialiseBoardBackground();
					LoadTiles();
				} );
			}
		}

		/// <summary>
		/// Determine the pixel X coordinate from the grid X coordinate
		/// </summary>
		/// <returns>The X from grid.</returns>
		/// <param name="gridPosition">Grid position.</param>
		private int ViewXFromGrid( int gridPosition )
		{
			return ( gridPosition * pixelsPerGridPosition ) + xOffset;
		}

		/// <summary>
		/// Determine the grid X coordinate from the pixel X coordinate
		/// </summary>
		/// <returns>The X from view.</returns>
		/// <param name="viewPosition">View position.</param>
		private int GridXFromView( float viewPosition )
		{
			return ( int )Math.Ceiling( ( viewPosition - xOffset ) / pixelsPerGridPosition );
		}

		/// <summary>
		/// Determine the pixel Y coordinate from the grid Y coordinate
		/// </summary>
		/// <returns>The Y from grid.</returns>
		/// <param name="gridPosition">Grid position.</param>
		private int ViewYFromGrid( int gridPosition )
		{
			return ( gridPosition * pixelsPerGridPosition ) + yOffset;
		}

		/// <summary>
		/// Determine the grid Y coordinate from the pixel Y coordinate
		/// </summary>
		/// <returns>The Y from view.</returns>
		/// <param name="viewPosition">View position.</param>
		private int GridYFromView( float viewPosition )
		{
			return (int )Math.Ceiling( ( viewPosition - yOffset ) / pixelsPerGridPosition );
		}

		//
		// Private data
		//

		private int pixelsPerGridPosition = 0;

		/// <summary>
		/// The offset of the board from the LHS of the view.
		/// </summary>
		private int xOffset = 0;

		/// <summary>
		/// The offset of the board from the top of the view.
		/// </summary>
		private int yOffset = 0;

		/// <summary>
		/// The tile being moved.
		/// </summary>
		private TileView movedTile = null;

		/// <summary>
		/// The last dragged position.
		/// </summary>
		private PointF lastDraggedPosition = null;

		/// <summary>
		/// The movement checker.
		/// </summary>
		private GamePlayer movementChecker = null;

		/// <summary>
		/// The board bitmap.
		/// </summary>
		private Bitmap boardBitmap = null;

		/// <summary>
		/// The tiles collection.
		/// </summary>
		private List< Tile > tilesCollection = null;

		/// <summary>
		/// The width of the board.
		/// </summary>
		private int boardWidth = 0;

		/// <summary>
		/// The log tag for this class
		/// </summary>
		private static readonly string LogTag = "BoardView";
	}
}

