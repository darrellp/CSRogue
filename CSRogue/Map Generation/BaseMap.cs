using System;
using System.Collections.Generic;
using System.Linq;
using CSRogue.Item_Handling;

namespace CSRogue.Map_Generation
{
    public class BaseMap : IMap
    {
        private IMapLocationData[][] _mapLocationData;
        private int _height;
        private int _width;
	    private Func<IMapLocationData> _dataCreator;

		public BaseMap(int height, int width, Func<IMapLocationData> dataCreator = null )
        {
            _height = height;
            _width = width;
            _mapLocationData = new IMapLocationData[_width][];
			_dataCreator = dataCreator ?? (() => new MapLocationData());
            for (var iCol = 0; iCol < _width; iCol++)
            {
                _mapLocationData[iCol] = new IMapLocationData[_height];
                for (var iRow = 0; iRow < _height; iRow++)
                {
                    _mapLocationData[iCol][iRow] = _dataCreator();
                }
            }
        }

        public BaseMap(string mapString, IItemFactory factory, Func<IMapLocationData> dataCreator = null)
		{
            var excavator = new FileExcavator(mapString, factory, dataCreator);
            excavator.Excavate(this);
        }

        IMapLocationData IMap.this[int iCol, int iRow]
        {
            get { return _mapLocationData[iCol][iRow]; }
            set { _mapLocationData[iCol][iRow] = value; }
        }

        IMapLocationData IMap.this[MapCoordinates loc]
        {
            get { return _mapLocationData[loc.Column][loc.Row]; }
            set { _mapLocationData[loc.Column][loc.Row] = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	
        /// Indexer to get item collections from a spot on the map.  Indexing order is column then row!! 
        /// </summary>
        ///
        /// <value>	The indexed item collection. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IMapLocationData this[int iCol, int iRow]
        {
            get
            {
                return _mapLocationData[iCol][iRow];
            }
            set
            {
                _mapLocationData[iCol][iRow] = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	
        /// Indexer to get item collections from a spot on the map using MapCoordinates 
        /// </summary>
        ///
        /// <value>	The indexed item collection. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IMapLocationData this[MapCoordinates location]
        {
            get
            {
                return _mapLocationData[location.Column][location.Row];
            }
            set
            {
                _mapLocationData[location.Column][location.Row] = value;
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                if (value == Height)
                {
                    return;
                }
                for (var iCol = 0; iCol < Width; iCol++)
                {
                    if (value > Height)
                    {
                        var columnData = new IMapLocationData[value];
                        _mapLocationData[iCol].CopyTo(columnData, 0);
                        _mapLocationData[iCol] = columnData;
                    }
                    else
                    {
                        _mapLocationData[iCol] = _mapLocationData[iCol].Take(value).ToArray();
                    }
                }
                _height = value;
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                if (value == Width)
                {
                    return;
                }
                var newData = new IMapLocationData[value][];
                for (var iCol = 0; iCol < value; iCol++)
                {
                    if (iCol < Width)
                    {
                        newData[iCol] = _mapLocationData[iCol];
                    }
                    else
                    {
                        newData[iCol] = new IMapLocationData[_height];
                    }
                }
                _mapLocationData = newData;
                _width = value; 
            }
        }
    }
}
