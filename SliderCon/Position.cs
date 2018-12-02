// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game definition
// Filename:    Position.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The Position class specifies a grid position.
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
using System.Xml.Serialization;

namespace SliderCon
{
	/// <summary>
	/// The Position class specifies a grid position.
	/// </summary>
	public class Position
	{
		/// <summary>
		/// Public constructor required for serialisation
		/// </summary>
		public Position()
		{
		}

		[XmlAttribute( "x" ) ]
		/// <summary>
		/// Gets or sets the x position property.
		/// </summary>
		/// <value>The x position property.</value>
		public int XProperty
		{
			get;
			set;
		}

		[XmlAttribute( "y" ) ]
		/// <summary>
		/// Gets or sets the x position property.
		/// </summary>
		/// <value>The x position property.</value>
		public int YProperty
		{
			get;
			set;
		}
	}
}

