// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Game definition
// Filename:    Completion.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The CompletionRecord class contains a collection of CompletionItem instances, each of which records the best move count for a 
//				 completed game instance.
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
using System.Collections.Generic;

namespace SliderCon
{
	/// <summary>
	/// The CompletionRecord class contains a collection of CompletionItem instances, each of which records the best move count for a 
	/// completed game instance.
	/// </summary>
	public class CompletionRecord
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public CompletionRecord()
		{
		}

		/// <summary>
		/// Adds a CompletionItem to the collection.
		/// </summary>
		/// <param name="item">Item.</param>
		public void AddCompletionItem( CompletionItem item )
		{
			// Check if there is already an entry in the collection
			CompletionItem existingItem = CompletionItemsProperty.Find( searchItem => searchItem.InstanceNameProperty == item.InstanceNameProperty );

			if ( existingItem != null )
			{
				// Update the move count if the supplied count is less than the existing count
				if ( item.MoveCountProperty < existingItem.MoveCountProperty )
				{
					existingItem.MoveCountProperty = item.MoveCountProperty;
				}	
			}
			else
			{
				// Add the item to the collection
				CompletionItemsProperty.Add( item );
			}
		}

		public int GetMoveCountForInstance( string instanceName )
		{
			int moveCount = 0;

			CompletionItem existingItem = CompletionItemsProperty.Find( searchItem => searchItem.InstanceNameProperty == instanceName );

			if ( existingItem != null )
			{
				moveCount = existingItem.MoveCountProperty;
			}

			return moveCount;
		}

		[XmlElement( "CompletionItem" ) ]
		/// <summary>
		/// Gets or sets the completion items property.
		/// </summary>
		/// <value>The completion items property.</value>
		public List< CompletionItem > CompletionItemsProperty
		{
			get;
			set;
		}
	}

	/// <summary>
	/// The CompletionItem class records the best move count for a completed game instance.
	/// </summary>
	public class CompletionItem
	{
		/// <summary>
		/// Default constructor required for serialisation
		/// </summary>
		public CompletionItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SliderCon.CompletionItem"/> class.
		/// </summary>
		/// <param name="instanceName">Instance name.</param>
		/// <param name="moveCount">Move count.</param>
		public CompletionItem( string instanceName, int moveCount )
		{
			InstanceNameProperty = instanceName;
			MoveCountProperty = moveCount;
		}

		[XmlAttribute( "instanceName" ) ]
		/// <summary>
		/// Gets or sets the instance name property.
		/// </summary>
		/// <value>The instance name property.</value>
		public string InstanceNameProperty
		{
			get;
			set;
		}

		[XmlAttribute( "moveCount" ) ]
		/// <summary>
		/// Gets or sets the move count property.
		/// </summary>
		/// <value>The move count property.</value>
		public int MoveCountProperty
		{
			get;
			set;
		}
	}
}

