using System;
using System.Collections.Generic;
using System.Linq;
using CSRogue.Map_Generation;

namespace CSRogue.Utilities
{
	class BresenhamStepper
	{
		private Dir _dirStep;
		private MapCoordinates _incLong;
		private MapCoordinates _incShort;
		private int _tally;
		private int _smallValue;
		private int _largeValue;
		private MapCoordinates _location;
		private MapCoordinates _endLocation;
		private bool _pureHorizontalOrVertical;
		private Dir _dirBegin;
		private int _beginValue;
		private bool _hasBeginConditions;

		public BresenhamStepper(MapCoordinates start, MapCoordinates end, int? beginColumn = null, int? beginRow = null, int tally = -1)
		{
			if (start == end)
			{
				throw new RogueException("Points can't be equal in BresenhamStepper");
			}

			_location = start;
			_endLocation = end;

			DetermineStepDirection(start, end);
			if (tally != -1)
			{
				_tally = tally;
			}
			DetermineStartingConditions(beginRow, beginColumn);
		}

		private void DetermineStepDirection(MapCoordinates start, MapCoordinates end)
		{
			int spanRow = Math.Abs(start.Row - end.Row);
			int spanColumn = Math.Abs(start.Column - end.Column);

			_dirStep = spanRow > spanColumn ? Dir.Vert : Dir.Horiz;
			Dir dirOther = MapCoordinates.OtherDirection(_dirStep);
			_incLong = _incShort = new MapCoordinates();
			_incLong[_dirStep] = start[_dirStep] < end[_dirStep] ? 1 : -1;
			_incShort[dirOther] = start[dirOther] < end[dirOther] ? 1 : -1;
			_pureHorizontalOrVertical = start.Row == end.Row || start.Column == end.Column;
			_largeValue = Math.Max(spanColumn, spanRow);
			_smallValue = Math.Min(spanColumn, spanRow);
			_tally = _largeValue / 2;
		}

		private void DetermineStartingConditions(int? beginRow, int? beginColumn)
		{
			// Can't have both a start row and a start column
			if (beginColumn.HasValue && beginRow.HasValue)
			{
				throw new RogueException("Can't have starting values in both dimensions for a BresenhamStepper");
			}
			// Initialize _dirBegin to make ReSharper happy
			_dirBegin = Dir.Vert;
			_hasBeginConditions = false;

			// Did the user request a starting row?
			if (beginRow.HasValue)
			{
				// Set up for a vertical orientation
				_hasBeginConditions = true;
				_dirBegin = Dir.Vert;
				_beginValue = beginRow.Value;
			}
			// Did the user request a starting column?
			else if (beginColumn.HasValue)
			{
				// Set up for a horizontal orientation
				_hasBeginConditions = true;
				_dirBegin = Dir.Horiz;
				_beginValue = beginColumn.Value;
			}

			// If the caller requested starting conditions
			if (_hasBeginConditions)
			{
				// Sign from the start of Bresenham to begin value
				int signStartToBegin = Math.Sign(_location[_dirBegin] - _beginValue);

				// Sign from the end of Bresenham to the start
				int signStartToEnd = Math.Sign(_endLocation[_dirBegin] - _location[_dirBegin]);

				// Have the starting conditions been met already?
				if (signStartToBegin == 0)
				{
					// No need for starting conditions
					_hasBeginConditions = false;
				}
				// Are the starting conditions impossible to be met?
				else if (signStartToEnd == 0 || signStartToBegin == signStartToEnd)
				{
					// Throw an exception
					throw new RogueException("Specified a start value out of range in BresenhamStepper");
				}
			}
		}

		public IEnumerable<MapCoordinates> Steps
		{
			get
			{
				foreach (MapCoordinates location in StepsInternal.Where(location => !_hasBeginConditions || location[_dirBegin] == _beginValue))
				{
					_hasBeginConditions = false;
					yield return location;
				}
			}
		}

		public IEnumerable<MapCoordinates> StepsInternal
		{
			get
			{
				yield return _location;

				if (_pureHorizontalOrVertical)
				{
					while (true)
					{
						_location += _incLong;
						yield return _location;
					}
				}

				while (true)
				{
					_location += _incLong;
					_tally += _smallValue;

					if (_tally >= _largeValue)
					{
						_location += _incShort;
						_tally -= _largeValue;
					}
					yield return _location;
				}
			    // ReSharper disable once IteratorNeverReturns
			}
		}
	}
}
