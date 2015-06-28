using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Math.Decompositions;
using Accord.Statistics;
using System.IO;
using Accord.Statistics.Analysis;
using System.Windows;

namespace lda_pca_face_rec
{
    class PCA :FeatureExtraction
    {
        int width = 100, hight = 150;
        public PCA(double[][] trainingSet, List<string> labels, int numOfComponents) 
        {
          


		this.trainingSet = trainingSet;
		this.labels = labels;
		this.numOfComponents = numOfComponents;

		this.meanMatrix = Accord.Statistics.Tools.Mean(this.trainingSet,0);

		this.W = getFeature(this.trainingSet, this.numOfComponents);

		// Construct projectedTrainingMatrix
		this.projectedTrainingSet = new List<projectedTrainingMatrix>();
		for (int i = 0; i < trainingSet.Count(); i++) 
        {
           
			projectedTrainingMatrix ptm = new projectedTrainingMatrix(this.W.Transpose().Multiply(trainingSet[i].Subtract(meanMatrix).ToArray()),labels[i]);
			this.projectedTrainingSet.Add(ptm);
		}
	
        }
        private double[][] getFeature(double[][] input, int K) 
        {

            //var activationContext = Type.GetTypeFromProgID("matlab.application.single");
            //var matlab = (MLApp.MLApp)Activator.CreateInstance(activationContext);
            //matlab.Visible = 0;


		int i, j;

		int row = input[0].Length;
		int column = input.Length;
		double[][] X = new double[column][];

		for (i = 0; i < column; i++) {
            X[i] = input[i].Subtract(this.meanMatrix);
		}


       // matlab.PutWorkspaceData("X", "base", X.ToMatrix());
      //  MessageBox.Show(matlab.Execute("XTX=X'*X;"));
      //  MessageBox.Show(matlab.Execute("size(XTX)"));
     //   var t = matlab.GetVariable("XTX", "base");
    //    matlab.Quit();

        double[][] XT = X.Transpose();
        double[,] XTX = X.Multiply(XT).ToMatrix();
        var feature = new EigenvalueDecomposition(XTX);


		//EigenvalueDecomposition feature = XTX.eig();
        double[] d = feature.RealEigenvalues;

	//	assert d.length >= K : "number of eigenvalues is less than K";
        int[] indexes;
        d.StableSort(out indexes);
       indexes= indexes.Reverse().ToArray();
        indexes = indexes.Submatrix(0, K-1);

		double[][] eigenVectors = XT.Multiply(feature.Eigenvectors.ToArray());
		double[][] selectedEigenVectors = eigenVectors.Submatrix(0,
				eigenVectors.Length - 1, indexes);

		// normalize the eigenvectors
        row = selectedEigenVectors.Length;
        column = selectedEigenVectors[0].Length;
		for (i = 0; i < column; i++) {
			double temp = 0;
			for (j = 0; j < row; j++)
				temp += Math.Pow(selectedEigenVectors[j][ i], 2);
			temp = Math.Sqrt(temp);

			for (j = 0; j < row; j++) {
				selectedEigenVectors[j][ i]= selectedEigenVectors[j][ i] / temp;
			}
		}

		return selectedEigenVectors;

	}

        override public double[][] getW()
        {
            return this.W;
        }
        override public List<projectedTrainingMatrix> getProjectedTrainingSet()
        {
            return this.projectedTrainingSet;
        }
        public double[][] getTrainingSet()
        {
            return this.trainingSet;
        }
        override public double[] getMeanMatrix()
        {
            // TODO Auto-generated method stub
            return meanMatrix;
        }
        public double[][] reconstruct(int whichImage, int dimensions) 
        {
		
		double[][] afterPCA = this.projectedTrainingSet[whichImage].matrix.Submatrix(0, dimensions-1, 0, 0);
		double[][] eigen = this.getW();
		return eigen.Multiply(afterPCA).Add(this.getMeanMatrix().Reshape(hight,width).ToArray());
		
	}
    }
}
