// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Initialisation and game state
// Filename:    History.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The History class contains the current and historical state of the games.
//
// Description:  The class contains the name of the current GameState and all the historical GameState instances.

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
	/// The History class contains the current and historical state of the games.
	/// </summary>
	public class History
	{
		/// <summary>
		/// Public constructor required for serialisation
		/// </summary>
		public History()
		{
		}

		[XmlElement( "CurrentGame" ) ]
		public string CurrentGameProperty
		{
			get;
			set;
		}

		[XmlElement( "GameInstance" ) ]
		public GameInstance CurrentInstanceProperty
		{
			get;
			set;
		}
	}
}

