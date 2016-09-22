using System.Collections.Generic;
using System.Linq;

namespace CSRogue.Utilities
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   The model for the markov process. </summary>
    /// <remarks>
    ///     Modified from https://github.com/MagicMau/ProceduralNameGenerator which was a
    ///     C# implementation of http://www.samcodes.co.uk/project/markov-namegen/
    ///     Darrell, 9/21/2016.
    /// </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class MarkovModel
    {
        #region Private variables
        private readonly List<char> _alphabet;
        private readonly Dictionary<char, int> _charToAlphaIndex = new Dictionary<char, int>();
        private Dictionary<string, List<double>> _chains;
        private readonly int _order;
        private readonly double _smoothing;
        #endregion

        #region Constructor
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor for Markov model. </summary>
        /// <remarks>   Darrell, 9/21/2016. </remarks>
        /// <param name="trainingData"> List of names to train by. </param>
        /// <param name="order">        Order of the Markov process. </param>
        /// <param name="smoothing">    The smoothing paramater for the Markov process. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public MarkovModel(IList<string> trainingData, int order, double smoothing)
        {
            // Identify and sort the alphabet used in the training data
            var letters = new HashSet<char>();
            foreach (var word in trainingData)
            {
                foreach (var c in word)
                {
                    letters.Add(c);
                }
            }
            _alphabet = letters.OrderBy(c => c).ToList();
            _alphabet.Insert(0, '#');
            for (var i = 0; i < _alphabet.Count; i++)
            {
                _charToAlphaIndex[_alphabet[i]] = i;
            }

            _order = order;
            _smoothing = smoothing;
            Train(trainingData);
        }
        #endregion

        #region Character Generation
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Generates the next character from the previous _order characters. </summary>
        /// <remarks>   Darrell, 9/21/2016. </remarks>
        /// <param name="context">  The context. </param>
        /// <param name="rnd">      The random. </param>
        /// <returns>   A char. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal char Generate(string context)
        {
            List<double> chain;
            return _chains.TryGetValue(context, out chain)
                ? _alphabet[SelectIndex(chain)]
                : '\0';
        }

        private static int SelectIndex(IList<double> chain)
        {
            // chains are "reverse cumulative" - i.e., the first element is the
            // sum of everything in the set and the values get smaller from there.
            // We're going to search through the cumulative list until the value is
            // small enough to be less than our random number to get our random
            // next character.
            var rand = Rnd.GlobalNextDouble() * chain[0];

            for (var i = 0; i < chain.Count; i++)
            {
                if (rand > chain[i])
                    return i - 1;
            }

            return chain.Count - 1;
        }
        #endregion

        #region Training
        private void Train(IEnumerable<string> trainingData)
        {
            _chains = new Dictionary<string, List<double>>();
            foreach (var d in trainingData)
            {
                var data = new string('#', _order) + d + '#';
                for (var i = 0; i < data.Length - _order; i++)
                {
                    var nextChar = data[i + _order];
                    for (var iOrder = 0; iOrder < _order; iOrder++)
                    {
                        var context = data.Substring(i + iOrder, _order - iOrder);

                        List<double> chainList;

                        if (!_chains.TryGetValue(context, out chainList))
                        {
                            chainList = Enumerable.Repeat(_smoothing, _alphabet.Count).ToList();
                            _chains[context] = chainList;
                        }

                        chainList[_charToAlphaIndex[nextChar]] += 1;
                    }
                }
            }

            // We do the accumulation for the cumulative distribution down here
            foreach (var chain in _chains.Values)
            {
                var accum = 0.0;
                for (var iChar = chain.Count - 1; iChar >= 0; iChar--)
                {
                    chain[iChar] += accum;
                    accum = chain[iChar];
                }
            }
        }
        #endregion
    }
}