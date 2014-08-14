// 
// File Details 
// -------------- 
//
// Project:     SliderCon
// Task:        Initialisation
// Filename:    GameUpdater.cs
// Created by:  T. Simmonds
//
//
// File Description
// ------------------
//
// Purpose:      The GameUpdater class copies Game definition files from the AssetManager to external storage.
//
// Description:  Copying is only carried out if the AssetManager contains a different version of files to those already in external storage
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
using System.IO;
using Android.Content.Res;
using Android.Util;

namespace SliderCon
{
	/// <summary>
	/// The GameUpdater class copies Game definition files from the AssetManager to external storage.
	/// </summary>
	class GameUpdater
	{
		/// <summary>
		/// Copy all Game files in the AssetManager to external storage
		/// </summary>
		/// <returns><c>true</c>, if files was updated, <c>false</c> otherwise.</returns>
		/// <param name="gamesDirectoryName">Name of the directory containing all the Game files</param>
		/// <param name="externalDirectoryName">Name of the directory on external storage to copy the files to</param>
		public static bool UpdateFiles( string gamesDirectoryName, string externalDirectoryName, AssetManager assets )
		{
			bool updatedOk = false;

			try 
			{
				// Directory to store the games in
				string externalGamesDirectoryName = Path.Combine( Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, externalDirectoryName, gamesDirectoryName );
				Directory.CreateDirectory( externalGamesDirectoryName );

				// Get the names of all the game folders in the GameFiles directory
				string[] games = assets.List( gamesDirectoryName );

				// Copy each game
				foreach ( string gameName in games )
				{
					// Create a directory for the game
					string externalGameDirectoryName = Path.Combine( externalGamesDirectoryName, gameName );
					Directory.CreateDirectory( externalGameDirectoryName );

					// Get the files for this game
					string assetGameDirectoryName = Path.Combine( gamesDirectoryName, gameName );
					string[] files = assets.List( Path.Combine( assetGameDirectoryName ) );

					foreach ( string fileName in files )
					{
						using ( Stream assetStream = assets.Open( Path.Combine( assetGameDirectoryName, fileName ) ) )
						using ( Stream copyStream = File.Create( Path.Combine( externalGameDirectoryName, fileName ) ) )
						{
							assetStream.CopyTo( copyStream );
						}
					}
				}

				updatedOk = true;
			} 
			catch ( IOException fileException ) 
			{
				Log.Debug( LogTag, fileException.Message );
			}
			catch ( Exception anyException )
			{
				Log.Debug( LogTag, anyException.Message );
			}

			return updatedOk;
		}

		//
		// Private data
		//

		/// <summary>
		/// The log tag for this class
		/// </summary>
		private static readonly string LogTag = "GameUpdater";
	}
}

