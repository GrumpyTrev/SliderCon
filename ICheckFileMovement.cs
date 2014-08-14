using System;

namespace SliderCon
{
	public interface ICheckTileMovement
	{
		void TileSelected( Tile selectedTile );

		bool CheckTileMovement( int xNewGrid, int yNewGrid, bool xBias, ref bool xValid, ref bool yValid );

		void TileMoved();

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

