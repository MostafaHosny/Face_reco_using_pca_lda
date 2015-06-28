using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Statistics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace lda_pca_face_rec
{
    [Serializable]
    public abstract class FeatureExtraction  
    {


         protected double[][] trainingSet;
         protected List<string> labels;
         protected   int numOfComponents;
         public   double[] meanMatrix;
            // Output
         protected   double[][] W;
         protected List<projectedTrainingMatrix> projectedTrainingSet;

            public abstract double[][] getW();

            public abstract List<projectedTrainingMatrix> getProjectedTrainingSet();

            public abstract double[] getMeanMatrix();
        }
    }

