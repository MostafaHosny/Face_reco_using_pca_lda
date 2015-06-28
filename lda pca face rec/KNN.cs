using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lda_pca_face_rec
{
    public class KNN
    {
        public static String assignLabel(projectedTrainingMatrix[] trainingSet, double[][] testFace, int K, EuclideanDistance metric)
        {
            projectedTrainingMatrix[] neighbors = findKNN(trainingSet, testFace, K, metric);
            return classify(neighbors);
        }

        // testFace has been projected to the subspace
        static projectedTrainingMatrix[] findKNN(projectedTrainingMatrix[] trainingSet, double[][] testFace, int K, EuclideanDistance metric) {
		int NumOfTrainingSet = trainingSet.Length;
		
		// initialization
		projectedTrainingMatrix[] neighbors = new projectedTrainingMatrix[K];
		int i;
		for (i = 0; i < K; i++) {
			trainingSet[i].distance = metric.getDistance(trainingSet[i].matrix,
					testFace);
//			System.out.println("index: " + i + " distance: "
//					+ trainingSet[i].distance);
			neighbors[i] = trainingSet[i];
		}

		// go through the remaining records in the trainingSet to find K nearest
		// neighbors
		for (i = K; i < NumOfTrainingSet; i++) {
			trainingSet[i].distance = metric.getDistance(trainingSet[i].matrix,
					testFace);
//			System.out.println("index: " + i + " distance: "
//					+ trainingSet[i].distance);

			int maxIndex = 0;
			for (int j = 0; j < K; j++) {
				if (neighbors[j].distance > neighbors[maxIndex].distance)
					maxIndex = j;
			}

			if (neighbors[maxIndex].distance > trainingSet[i].distance)
				neighbors[maxIndex] = trainingSet[i];
		}
		return neighbors;
	}

        // get the class label by using neighbors
        static String classify(projectedTrainingMatrix[] neighbors)
        {
            Dictionary<String, Double> map = new Dictionary<String, Double>();
            int num = neighbors.Length;

            for (int index = 0; index < num; index++)
            {
                projectedTrainingMatrix temp = neighbors[index];
                String key = temp.label;
                if (!map.ContainsKey(key))
                    map.Add(key, 1 / temp.distance);
                else
                {
                    double value = map[key];
                    value += 1 / temp.distance;
                    map[key]= value;
                }
            }

            // Find the most likely label
            double maxSimilarity = 0;
            String returnLabel = "";
            List<String> labelSet = map.Keys.ToList();
           foreach(string label in labelSet)
           {
               // String label = it.next();
                double value = map[label];
                if (value > maxSimilarity)
                {
                    maxSimilarity = value;
                    returnLabel = label;
                }
            }

            return returnLabel;
        }
    }
}
