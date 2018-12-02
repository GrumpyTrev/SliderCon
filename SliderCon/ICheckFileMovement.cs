// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game control
// Filename:    ICheckTileMovement.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The ICheckTileMovement interface is used to check the validity of tile movements 
//
// Description:  As purpose
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

namespace SliderCon
{
	/// <summary>
	/// The ICheckTileMovement interface is used to check the validity of tile movements 
	/// </summary>
	public interface ICheckTileMovement
	{
		void TileSelected( Tile selectedTile );

		bool CheckTileMovement( int xNewGrid, int yNewGrid, bool xBias, ref bool xValid, ref bool yValid );

		void TileMoved( TileMove move );

		int LastCheckedXProperty
		{
			get;
		}

		int LastCheckedYProperty
		{
			get;
		}
	}
}

