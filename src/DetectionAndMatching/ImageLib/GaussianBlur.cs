using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLib
{
    public sealed class GaussianBlur  : Convolution
    {
        private double sigma = 1.4;
        private int size = 5;

        /// <summary>
        /// Gaussian sigma value, [0.5, 5.0].
        /// </summary>
        ///
        /// <remarks><para>Sigma value for Gaussian function used to calculate
        /// the kernel.</para>
        ///
        /// <para>Default value is set to <b>1.4</b>.</para>
        /// </remarks>
        ///
        public double Sigma
        {
            get { return sigma; }
            set
            {
                // get new sigma value
                sigma = Math.Max(0.5, Math.Min(5.0, value));
                sigma = value;
                // create filter
                CreateFilter();
            }
        }

        /// <summary>
        /// Kernel size, [3, 21].
        /// </summary>
        ///
        /// <remarks><para>Size of Gaussian kernel.</para>
        ///
        /// <para>Default value is set to <b>5</b>.</para>
        /// </remarks>
        ///
        public int Size
        {
            get { return size; }
            set
            {
                size = Math.Max(3, Math.Min(21, value | 1));
                CreateFilter();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianBlur"/> class.
        /// </summary>
        ///
        public GaussianBlur()
        {
            CreateFilter();
            base.ProcessAlpha = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianBlur"/> class.
        /// </summary>
        ///
        /// <param name="sigma">Gaussian sigma value.</param>
        ///
        public GaussianBlur(double sigma)
        {
            Sigma = sigma;
            base.ProcessAlpha = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianBlur"/> class.
        /// </summary>
        ///
        /// <param name="sigma">Gaussian sigma value.</param>
        /// <param name="size">Kernel size.</param>
        ///
        public GaussianBlur(double sigma, int size)
        {
            Sigma = sigma;
            Size = size;
            base.ProcessAlpha = true;
        }

        // Private members
        #region Private Members

        // Create Gaussian filter
        private void CreateFilter()
        {
            // create Gaussian function
            var gaus = new Gaussian(sigma);
            // create kernel
            double[,] kernel = gaus.Kernel2D(size);
            double min = kernel[0, 0];
            // integer kernel
            int[,] intKernel = new int[size, size];
            int divisor = 0;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    double v = kernel[i, j] / min;

                    if (v > ushort.MaxValue)
                    {
                        v = ushort.MaxValue;
                    }
                    intKernel[i, j] = (int)v;

                    // collect divisor
                    divisor += intKernel[i, j];
                }
            }

            // update filter
            this.Kernel = intKernel;
            this.Divisor = divisor;
        }
        #endregion
    }
}
