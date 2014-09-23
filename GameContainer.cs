// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game definition
// Filename:    GameContainer.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The GameContainer class contains the definition of one of more game instances.
//
// Description:  The class can contain one or more GameInstance instances or one or more GameContainer instances modelling a hierarchy of game instances.
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
using System.Xml.Serialization;

namespace SliderCon
{
	/// <summary>
	/// The GameContainer class contains the definition of one of more game instances.
	/// </summary>
	public class GameContainer
	{
		public GameContainer()
		{
		}

		[XmlAttribute( "name" ) ]
		/// <summary>
		/// Gets or sets the name of the collection.
		/// </summary>
		/// <value>The collection name</value>
		public string CollectionNameProperty
		{
			get;
			set;
		}

		[XmlElement( "GameInstance" ) ]
		/// <summary>
		/// Gets or sets the game instances property.
		/// </summary>
		/// <value>The game instances property.</value>
		public List< GameInstance > GameInstancesProperty
		{
			get;
			set;
		}

		[XmlElement( "GameCollection" ) ]
		/// <summary>
		/// Gets or sets the game instances property.
		/// </summary>
		/// <value>The game instances property.</value>
		public List< GameContainer > GameCollectionsProperty
		{
			get;
			set;
		}
	}
}

