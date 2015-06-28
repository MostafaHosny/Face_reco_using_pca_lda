using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace lda_pca_face_rec
{
    [Serializable()]
    public class projectedTrainingMatrix :ISerializable
    {
      public  double[][] matrix;
      public String label;
      public double distance = 0;

      public projectedTrainingMatrix(SerializationInfo info, StreamingContext ctxt)
   {
       this.matrix = (double[][])info.GetValue("Matrix",typeof( double[][]));
      this.label = (string)info.GetValue("Label",typeof(string));
      this.distance = (double)info.GetValue("Distance", typeof(double));
  
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
   {
       info.AddValue("Matrix", this.matrix);
       info.AddValue("Label", this.label);
       info.AddValue("Distance", this.distance);
   }

        public projectedTrainingMatrix(double[][] m, String l)
        {
            this.matrix = m;
            this.label = l;
        }
    }
}
