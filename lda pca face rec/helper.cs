using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lda_pca_face_rec
{
    class helper
    {
        public static double[] imgtomat(byte[] myimg)
        {
            double[] mymat = new double[myimg.Length/4];

            int temp;

            for (int i = 0; i < myimg.Length; i += 4)
            {
                temp = i / 4;
                //0.21 R + 0.71 G + 0.07 B
             //   mymat[temp] = (int)((0.07 * (double)myimg[i]) + (0.71 * (double)myimg[i + 1]) + (0.21 * (double)myimg[i + 2]));
                mymat[temp] = (int)( ((double)myimg[i] + (double)myimg[i + 1]+ (double)myimg[i + 2])/3 );
           
            }
            return mymat;
        }
    }
}
