using Accord.Math.Decompositions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Statistics;
using System.IO;
using Accord.Statistics.Analysis;
using System.Windows;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace lda_pca_face_rec
{
    [Serializable()]
   public class LDA :FeatureExtraction , ISerializable
    {

         public LDA(SerializationInfo info, StreamingContext ctxt)
   {
      this.labels = (List<string>)info.GetValue("Labels", typeof(List<string>));
      this.meanMatrix = (double[])info.GetValue("MeanMatrix",typeof(double[]));
      this.numOfComponents = (int)info.GetValue("NumOfComponents", typeof(int));
      this.projectedTrainingSet = (List<projectedTrainingMatrix>)info.GetValue("ProjectedTrainingSet", typeof(List<projectedTrainingMatrix>));
      this.trainingSet = (double[][])info.GetValue("TrainingSet",typeof(double[][]));
      this.W = (double[][])info.GetValue("W", typeof(double[][]));
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
   {
      info.AddValue("Labels", this.labels);
      info.AddValue("MeanMatrix", this.meanMatrix);
      info.AddValue("NumOfComponents", this.numOfComponents);
      info.AddValue("ProjectedTrainingSet", this.projectedTrainingSet);
      info.AddValue("TrainingSet", this.trainingSet);
      info.AddValue("W", this.W);
   }
       public LDA(double[][] trainingSet, List<string> labels,int numOfComponents)
       {
		int n = trainingSet.Length; // sample size
		HashSet<String> tempSet = new HashSet<String>(labels);
		int c = tempSet.Count; // class size
		
		// process in PCA
		PCA pca = new PCA(trainingSet, labels, n - c);

		// classify
		double[][] meanTotal = new double[n - c][];
        for (int i = 0; i < n - c; i++)
        {
            meanTotal[i] = new double[1];
        }
        Dictionary<String, List<double[]>> dict = new Dictionary<String, List<double[]>>();
		List<projectedTrainingMatrix> pcaTrain = pca.getProjectedTrainingSet();
		for (int i = 0; i < pcaTrain.Count; i++) {
			String key = pcaTrain[i].label;
           meanTotal= meanTotal.Add(pcaTrain[i].matrix);
			if (!dict.ContainsKey(key)) {
                List<double[]> temp = new List<double[]>();
               
               temp.Add(pcaTrain[i].matrix.Transpose()[0]);
				dict.Add(key, temp);
			} else {
                List<double[]> temp = dict[key];
                temp.Add(pcaTrain[i].matrix.Transpose()[0]);
				dict[key]= temp;
			}
		}
		meanTotal.ToMatrix().Multiply((double) 1 / n);

		// calculate Sw, Sb
		double[][] Sw = new double[n - c][];
        double[][] Sb = new double[n - c][];
        for (int i = 0; i < n - c; i++)
        {
            Sw[i] = new double[n-c];
            Sb[i] = new double[n-c];
        }
        List<String> labelSet = dict.Keys.ToList();
            foreach(string label in labelSet)
           {
               List<double[]> tempMatrix = dict[label];
               double[][] matrixWithinThatClass = tempMatrix.ToArray();
               double[] meanOfCurrentClass = Accord.Statistics.Tools.Mean(matrixWithinThatClass);
			for (int i = 0; i < matrixWithinThatClass.Length; i++) {
				double[][] temp1 = matrixWithinThatClass[i].ToArray().Subtract(meanOfCurrentClass.ToArray());
				temp1 = temp1.Multiply(temp1.Transpose());
				Sw =Sw.Add(temp1);
			}

			double[][] temp = meanOfCurrentClass.ToArray().Subtract(meanTotal);
			temp = temp.Multiply(temp.Transpose()).ToMatrix().Multiply((double)matrixWithinThatClass.Length).ToArray();
			Sb=Sb.Add(temp);
		}

		// calculate the eigenvalues and vectors of Sw^-1 * Sb
		double [][] targetForEigen = Sw.Inverse().Multiply(Sb);
        var feature = new EigenvalueDecomposition(targetForEigen.ToMatrix());

        double[] d = feature.RealEigenvalues;
        int[] indexes;
        d.StableSort(out indexes);
        indexes = indexes.Reverse().ToArray();
      
        indexes = indexes.Submatrix(0, c - 1);

           //int[] indexes = getIndexesOfKEigenvalues(d, c - 1);

        double[][] eigenVectors = feature.Eigenvectors.ToArray();
		double[][] selectedEigenVectors = eigenVectors.Submatrix(0,	eigenVectors.Length - 1, indexes);

		this.W = pca.getW().Multiply(selectedEigenVectors);

		// Construct projectedTrainingMatrix
		this.projectedTrainingSet = new List<projectedTrainingMatrix>();
		 for (int i = 0; i < trainingSet.Length; i++) {
            projectedTrainingMatrix ptm = new projectedTrainingMatrix(this.W.Transpose().Multiply(trainingSet[i].Subtract(pca.meanMatrix).ToArray()), labels[i]);
			this.projectedTrainingSet.Add(ptm);
		}
		this.meanMatrix= pca.meanMatrix;
        GC.Collect();
	}
    override public double[][] getW() {
		return this.W;
	}

	override public List<projectedTrainingMatrix> getProjectedTrainingSet() {
		return this.projectedTrainingSet;
	}
	
	override public double[] getMeanMatrix() {
		// TODO Auto-generated method stub
		return meanMatrix;
	}
    }
}
