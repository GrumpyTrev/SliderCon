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

using Android.Util;

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

		[XmlAttribute( "collectionName" ) ]
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

		/// <summary>
		/// List of the names of the child containers and instances held by this container
		/// </summary>
		/// <value>names of the child containers and instances held by this container</value>
		public List< string > ItemsProperty
		{
			get
			{
				return items;
			}
		}

		public GameContainer ParentContainerProperty
		{
			get;
			set;
		}

		public object GetIndexedItem( int index )
		{
			object item = null;

			if ( ( index >= 0 ) && ( index < GameCollectionsProperty.Count ) )
			{
				item = GameCollectionsProperty[ index ];
			}
			else if ( ( index >= GameCollectionsProperty.Count ) && ( index < ( GameCollectionsProperty.Count + GameInstancesProperty.Count ) ) )
			{
				item = GameInstancesProperty[ index - GameCollectionsProperty.Count ];
			}

			return item;
		}

		//
		// Protected methods
		//

		/// <summary>
		/// Gathers the names of all the GameInstance objects held in the container and adds them to the collection.
		/// The full pathname of the instance is set as the hierarchy is traversed. Checks for unique pathnames are made.
		/// </summary>
		/// <returns><c>true</c>, if instances were gathered ok, <c>false</c> otherwise.</returns>
		/// <param name="parentPath">Parent path.</param>
		/// <param name="instances">Instances.</param>
		protected bool GatherInstances( string parentPath, Dictionary< string, GameInstance > instances )
		{
			bool gatheredOk = true;

			// Form the pathname from the parent's path
			if ( parentPath.Length > 0 )
			{
				pathname = parentPath + CollectionNameProperty; 
			}
			else
			{
				pathname = CollectionNameProperty;
			}

			pathname += '\\';

			// Gather instances from all of the child containers
			List< GameContainer >.Enumerator containerEnumerator = GameCollectionsProperty.GetEnumerator();
			while ( ( gatheredOk == true ) && ( containerEnumerator.MoveNext() == true ) )
			{
				gatheredOk = containerEnumerator.Current.GatherInstances( pathname, instances );

				// Add the short name of this container to the list of child items
				items.Add( containerEnumerator.Current.CollectionNameProperty + "..." );

				containerEnumerator.Current.ParentContainerProperty = this;
			}

			// Add all the GameInstances in this container
			List< GameInstance >.Enumerator instanceEnumerator = GameInstancesProperty.GetEnumerator();
			while ( ( gatheredOk == true ) && ( instanceEnumerator.MoveNext() == true ) )
			{
				GameInstance instance = instanceEnumerator.Current;
				instance.FullNameProperty = pathname + instance.NameProperty;

				if ( instances.ContainsKey( instance.FullNameProperty ) == false )
				{
					instances[ instance.FullNameProperty ] = instance;

					// Add the short name of this instance to the list of child items
					items.Add( instance.NameProperty );
				}
				else
				{
					Log.Debug( LogTag, string.Format( "Instance with full name [{0}] is not unique", instance.FullNameProperty ) );
					gatheredOk = false;
				}
			}

			return gatheredOk;
		}

		//
		// Private methods
		//

		//
		// Private data
		//

		/// <summary>
		/// The full pathname of this instance, including all the parent containers
		/// </summary>
		private string pathname = "";

		/// <summary>
		/// List of the names of the child containers and instances held by this container
		/// </summary>
		private List< string > items = new List<string>();

		/// <summary>
		/// The log tag for this class
		/// </summary>
		private readonly string LogTag = "GameContainer";

	}
}

