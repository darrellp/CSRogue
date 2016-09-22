using System;
using System.Collections.Generic;
using System.Linq;

namespace CSRogue.Utilities
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Provides procedural generation of words using high-order Markov chains Uses Katz's back-off
    ///     model - chooses the next character based on conditional probability given the last n-
    ///     characters (where model order = n) and backs down to lower order models when higher models
    ///     fail Uses a Dirichlet prior, which is like additive smoothing and raises the chances of a
    ///     "random" letter being picked instead of one that's trained in.
    /// </summary>
    /// <remarks>   Darrell, 9/21/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class NameGenerator
    {
        #region Private variables
        private readonly MarkovModel _model;
        #endregion

        #region Public properties
        public int Order { get; set; }
        public double Smoothing { get; set; }
        #endregion

        #region Constructor
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        /// <remarks>   Darrell, 9/21/2016. </remarks>
        /// <param name="trainingData"> training data for the generator, array of words. </param>
        /// <param name="order">
        ///     number of models to use, will be of orders up to and including
        ///     "order".
        /// </param>
        /// <param name="smoothing">    the dirichlet prior/additive smoothing "randomness" factor. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public NameGenerator(IList<string> trainingData, int order, double smoothing)
        {
            Order = order;
            Smoothing = smoothing;
            _model = new MarkovModel(trainingData, order, smoothing);
        }
        #endregion

        #region Generation
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Generates a word. </summary>
        ///
        /// <remarks>   Darrell, 9/21/2016. </remarks>
        ///
        /// <returns>   A generated string. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Generate(int minLength = 0, int maxLength = int.MaxValue)
        {
            string name;
            var maxTries = 1000;
            var tries = 0;
            do
            {
                name = new string('#', Order);
                var letter = GetLetter(name);
                while (letter != '#' && letter != '\0')
                {
                    name += letter;
                    letter = GetLetter(name);
                }
            } while ((name.Length - Order < minLength || name.Length > maxLength) && tries ++ < maxTries);
            return name.Substring(Order);
        }

        private char GetLetter(string name)
        {
            char letter;
            var context = name.Substring(name.Length - Order);
            do
            {
                letter = _model.Generate(context);
                context = context.Substring(1);
            } while (letter == '\0' && context != string.Empty);

            return letter;
        }
        #endregion
    }
}