//   Description:       Implementation of 3D Perlin Noise after Rene Shulte's implementation which is after Ken Perlin's reference implementation.
//
using System;

namespace CSRogue.Utilities
{
    /// <summary>
    /// Implementation of 3D Perlin Noise after Ken Perlin's reference implementation.
    /// </summary>
    public class PerlinNoise3D
    {
        #region Fields
        private const int PermLength = 256;
        private readonly int[] _perm;
        private double _max = 1.0;
        private int _octaves = 2;
        private double _persistence = 0.9;
        #endregion

        #region Properties
        public double Frequency { get; set; }

        public double Persistence
        {
            get
            {
                return _persistence;
            }
            set
            {
                _persistence = value;
                CalculateMax();
            }
        }

        public int Octaves
        {
            get
            {
                return _octaves;
            }
            set
            {
                _octaves = value;
                CalculateMax();
            }
        }
        #endregion

        #region Contructors
        public PerlinNoise3D()
        {
            _perm = new int[PermLength * 2];
            InitNoiseFunctions();

            // Default values
            Frequency = 0.023;

            CalculateMax();
        }
        #endregion

        #region Methods
        private void CalculateMax()
        {
            _max = (1.0 - Math.Pow(Persistence, _octaves)) / (1.0 - Persistence);
        }

        public void InitNoiseFunctions()
        {
            // Fill empty
            for (int i = 0; i < PermLength; i++)
            {
                _perm[i] = i;
            }

            // Generate random numbers
            for (int i = 0; i < PermLength - 1; i++)
            {
                int j = Rnd.GlobalNext(PermLength - i);
                int tmp = _perm[i];
                _perm[i] = _perm[PermLength + i] = _perm[i + j];
                _perm[i + j] = tmp;
            }
        }

        public double ComputePositive(double x, double y, double z)
        {
            // Map to range 0..1
            return (ComputeRaw(x, y, z) + 1.0) / 2.0;
        }

        public double ComputeTurbulence(double x, double y, double z)
        {
            // Return absolute value
            return Math.Abs(ComputeRaw(x, y, z));
        }

        public double ComputeRaw(double x, double y, double z)
        {
            double noise = 0;
            double amp = 1.0;
            double freq = Frequency;
            for (int i = 0; i < Octaves; i++)
            {
                noise += Noise(x * freq, y * freq, z * freq) * amp;
                freq *= 2;                                // octave is the double of the previous frequency
                amp *= Persistence;
            }
            return noise / _max;
        }

        private double Noise(double x, double y, double z)
        {
            // Find unit cube that contains point
            int iX = (int)Math.Floor(x);
            int iY = (int)Math.Floor(y);
            int iZ = (int)Math.Floor(z);

            // Find relative x, y, z of the point in the cube.
            x -= iX;
            y -= iY;
            z -= iZ;

            // Compute fade curves for each of x, y, z
            double u = Fade(x);
            double v = Fade(y);
            double w = Fade(z);

            iX &= 0xff;
            iY &= 0xff;
            iZ &= 0xff;

            // Hash coordinates of the 8 cube corners
            int a = _perm[iX] + iY;
            int aa = _perm[a] + iZ;
            int ab = _perm[a + 1] + iZ;
            int b = _perm[iX + 1] + iY;
            int ba = _perm[b] + iZ;
            int bb = _perm[b + 1] + iZ;

            // And add blended results from 8 corners of cube.
            return Lerp(w,
                Lerp(v,
                    Lerp(u, Grad(_perm[aa], x, y, z),
                        Grad(_perm[ba], x - 1, y, z)),
                    Lerp(u, Grad(_perm[ab], x, y - 1, z),
                        Grad(_perm[bb], x - 1, y - 1, z))),
                Lerp(v,
                    Lerp(u, Grad(_perm[aa + 1], x, y, z - 1),
                        Grad(_perm[ba + 1], x - 1, y, z - 1)),
                    Lerp(u, Grad(_perm[ab + 1], x, y - 1, z - 1),
                        Grad(_perm[bb + 1], x - 1, y - 1, z - 1))));
        }

        private static double Fade(double t)
        {
            // Smooth interpolation parameter
            return (t * t * t * (t * (t * 6 - 15) + 10));
        }

        private static double Lerp(double alpha, double a, double b)
        {
            // Linear interpolation
            return (a + alpha * (b - a));
        }

        private static double Grad(int hashCode, double x, double y, double z)
        {
            // Convert lower 4 bits of hash code into 12 gradient directions
            int h = hashCode & 0xf;
            double u = h < 8 ? x : y;
            double v = h < 4 ? y : h == 12 || h == 14 ? x : z;
            return (((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v));
        }
        #endregion
    }
}
