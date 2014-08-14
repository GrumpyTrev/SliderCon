// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game definition
// Filename:    Exclusion.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The Exclusion class defines an area of the board or tile that is either not part of the playing area, or not part of the tile.
//
// Description:  The Exclusion is made up of an optional border width and height, and zero or more grid positions
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

using Android.Util;

namespace SliderCon
{
	/// <summary>
	/// The Exclusion class defines an area of the board or tile that is either not part of the playing area, or not part of the tile.
	/// </summary>
	public class Exclusion
	{
		/// <summary>
		/// Public constructor required for serialisation
		/// </summary>
		public Exclusion()
		{
		}

		/// <summary>
		/// Apply the positions specified by the border and the Position list to the supplied array
		/// </summary>
		/// <param name="applicationArea">Application area.</param>
		/// <param name="valueToApply">Value to apply.</param>
		public bool Apply( int [ , ] applicationArea, int valueToApply )
		{
			bool appliedOk = true;

			// Apply the border first
			if ( ( borderWidth > 0 ) && ( borderHeight > 0 ) )
			{
				// A border can only be applied if its dimensions are less than the application area
				if ( ( ( borderWidth * 2 ) < applicationArea.GetLength( 0 ) ) &&	
					( ( borderHeight * 2 ) < applicationArea.GetLength( 1 ) ) )
				{
					for ( int yIndex = 0; yIndex < applicationArea.GetLength( 1 ); ++yIndex )
					{
						for ( int xIndex = 0; xIndex < applicationArea.GetLength( 0 ); ++xIndex )
						{
							if ( ( yIndex < borderHeight ) || ( yIndex >= ( applicationArea.GetLength( 1 ) - borderHeight ) ) )
							{
								applicationArea[ xIndex, yIndex ] = valueToApply;
							}
							else if ( ( xIndex < borderWidth ) || ( xIndex >= ( applicationArea.GetLength( 0 ) - borderWidth ) ) )
							{
								applicationArea[ xIndex, yIndex ] = valueToApply;
							}
						}
					}
				}
				else
				{
					Log.Debug( LogTag, string.Format( "Border width {0} or height {1} too big for playing area (2}, {3}",
						borderWidth, borderHeight, applicationArea.GetLength( 0 ), applicationArea.GetLength( 1 ) ) );
					appliedOk = false;
				}
			}

			// Now apply the individual exclusions
			if ( appliedOk == true )
			{
				if ( ExcludedPositionsProperty != null )
				{
					List< Position >.Enumerator enumerator = ExcludedPositionsProperty.GetEnumerator();
					while ( ( appliedOk == true ) && ( enumerator.MoveNext() == true ) )
					{
						Position positionToApply = enumerator.Current;
						if ( ( positionToApply.XProperty < applicationArea.GetLength( 0 ) ) && ( positionToApply.YProperty < applicationArea.GetLength( 1 ) )  )
						{
							applicationArea[ positionToApply.XProperty, positionToApply.YProperty ] = valueToApply;
						}
						else
						{
							Log.Debug( LogTag, string.Format( "Position {0}, {1} invalid for playing area (2}, {3}",
								positionToApply.XProperty, positionToApply.YProperty, applicationArea.GetLength( 0 ), applicationArea.GetLength( 1 ) ) );
							appliedOk = false;
						}
					}
				}
			}

			return appliedOk;
		}

		/// <summary>
		/// Apply the specified value to the non-excluded parts of the applicationArea
		/// </summary>
		/// <param name="applicationArea">Application area.</param>
		/// <param name="valueToApply">Value to apply.</param>
		/// <param name="height">Height.</param>
		/// <param name="width">Width.</param>
		/// <param name="xOffset">X offset.</param>
		/// <param name="yOffset">Y offset.</param>
		public bool Apply( int [ , ] applicationArea, int valueToApply, int height, int width, int xOffset, int yOffset )
		{
			bool appliedOk = true;

			// Create an integer array to hold the tile's excluded positions
			if ( excludedArea == null )
			{
				excludedArea = new int[ width, height ];
				if ( ExcludedPositionsProperty != null )
				{
					List< Position >.Enumerator enumerator = ExcludedPositionsProperty.GetEnumerator();
					while ( ( appliedOk == true ) && ( enumerator.MoveNext() == true ) )
					{
						Position positionToApply = enumerator.Current;
						if ( ( positionToApply.XProperty < width ) && ( positionToApply.YProperty < height )  )
						{
							excludedArea[ positionToApply.XProperty, positionToApply.YProperty ] = -1;
						}
						else
						{
							Log.Debug( LogTag, string.Format( "Position {0}, {1} invalid for tile area (2}, {3}",
								positionToApply.XProperty, positionToApply.YProperty, width, height ) );
							appliedOk = false;
						}
					}
				}
			}

			if ( appliedOk == true )
			{
				// Now apply any non-excluded tile positions to the application area.
				for ( int yIndex = 0; yIndex < height; ++yIndex )
				{
					for ( int xIndex = 0; xIndex < width; ++xIndex )
					{
						// Check if this position is excluded
						if ( excludedArea[ xIndex, yIndex ] == 0 )
						{
							// Not excluded so apply
							if ( ( ( xIndex + xOffset ) < applicationArea.GetLength( 0 ) ) &&
								( ( yIndex + yOffset ) < applicationArea.GetLength( 1 ) ) )
							{
								applicationArea[ xIndex + xOffset, yIndex + yOffset ] = valueToApply;
							}
						}
					}
				}
			}

			return appliedOk;
		}

		public bool CheckApplication( int [ , ] applicationArea, int valueToCheck, int xOffset, int yOffset )
		{
			bool checkOk = false;

			if ( excludedArea != null )
			{
				checkOk = true;

				Log.Debug( LogTag, string.Format( "Checking tile {0} at offset {1}, {2}", valueToCheck, xOffset, yOffset ) );

				// Now apply any non-excluded tile positions to the application area.
				for ( int yIndex = 0; ( yIndex < excludedArea.GetLength( 1 ) ) && ( checkOk== true ) ; ++yIndex )
				{
					for ( int xIndex = 0; ( xIndex < excludedArea.GetLength( 0 ) ) && ( checkOk== true ) ; ++xIndex )
					{
						// Check if this position is excluded
						if ( excludedArea[ xIndex, yIndex ] == 0 )
						{
							int gameValue = applicationArea[ xIndex + xOffset, yIndex + yOffset ];
							Log.Debug( LogTag, string.Format( "Game value at {0} ({1}), {2} ({3}) is {4}", xIndex, xIndex + xOffset, yIndex, yIndex + yOffset, gameValue ) );

							if ( ( gameValue != valueToCheck ) && ( gameValue != 0 ) )
							{
								Log.Debug( LogTag, string.Format( "Check failed for tile {0}", valueToCheck ) );
								checkOk = false;
							}
						}
					}
				}

			}

			return checkOk;
		}

		[XmlAttribute( "width" ) ]
		/// <summary>
		/// Gets or sets the width property.
		/// </summary>
		/// <value>The name property.</value>
		public int BorderWidthProperty
		{
			get
			{
				return borderWidth;
			}

			set
			{
				borderWidth = value;
			}
		}

		[XmlAttribute( "height" ) ]
		/// <summary>
		/// Gets or sets the type property.
		/// </summary>
		/// <value>The type property.</value>
		public int BorderHeightProperty
		{
			get
			{
				return borderHeight;
			}

			set
			{
				borderHeight = value;
			}
		}

		[XmlElement( "Position" ) ]
		public List< Position > ExcludedPositionsProperty
		{
			get;
			set;
		}

		private int borderWidth = 0;

		private int borderHeight = 0;

		/// <summary>
		/// An integer array holding a tile's excluded area (marked as -1)
		/// </summary>
		private int [ , ] excludedArea = null;

		/// <summary>
		/// The log tag for this class
		/// </summary>
		private readonly string LogTag = "Exclusion";

	}
}

