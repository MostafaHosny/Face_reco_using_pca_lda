using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Accord.Math;
using Accord.Math.Decompositions;
using Accord.Statistics;
using FORMS = System.Windows.Forms;
using System.IO;
using Accord.Statistics.Analysis;
using System.ComponentModel;


namespace lda_pca_face_rec
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       // WriteableBitmap img ;
       // WriteableBitmap grayimg;
        public static int numberofimages, ImagePerClass, numOfClasses;
        public static List<double[]> mymat;
        public double[] totalmean;
        public double[][] ClassMeanTrain;
        public double[][] XB;
        public double[][] XW;
        PCA pca; 
        LDA lda;
        // decide var can decide which algorithem want to run pca = 0 , lda = 1 
        int decide = 1;
        EuclideanDistance ED = new EuclideanDistance();


        FileInfo[] info;
        BackgroundWorker trainWorker = new BackgroundWorker();
        BackgroundWorker testWorker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            ImagePerClass = 6;
         
            trainWorker.DoWork += new DoWorkEventHandler(Train_DoWork);
            trainWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Train_RunWorkerCompleted);
           
            testWorker.DoWork += new DoWorkEventHandler(Test_DoWork);
            testWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Test_RunWorkerCompleted);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var dialog = new System.Windows.Forms.FolderBrowserDialog();
            //System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            //if (result == FORMS.DialogResult.OK)
            //{
            //    DirectoryInfo dirInfo = new DirectoryInfo(dialog.SelectedPath.ToString());

            progressTraining.IsIndeterminate = true;
            btnTrain.IsEnabled = false;
            btnTest.IsEnabled = false;
        //    start();
            trainWorker.RunWorkerAsync(); 

           
        }

        void Train_DoWork(object sender, DoWorkEventArgs e)

         {
  
             start();
    

         }

        void Train_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
         {
             progressTraining.IsIndeterminate = false;
             btnTrain.IsEnabled = true;
             btnTest.IsEnabled = true;
         }

        void Test_DoWork(object sender, DoWorkEventArgs e)
        {

            double counter = 0, accurate = 0;
            for (int i = 6; i < numberofimages; i += 7)
            {
                counter++;
                var img = BitmapFactory.New(1, 1).FromStream(info[i].OpenRead());
                if (img.PixelWidth != 100 || img.PixelHeight != 150)
                {
                    img = img.Resize(100, 150, WriteableBitmapExtensions.Interpolation.Bilinear);
                }
                var mat = helper.imgtomat(img.ToByteArray());

                if (decide == 0)
                {
                    List<projectedTrainingMatrix> projectedTrainingSet = pca.getProjectedTrainingSet();

                    double[][] testCase = pca.getW().Transpose().Multiply(mat.Subtract(pca.getMeanMatrix()).ToArray());

                    String result = KNN.assignLabel(projectedTrainingSet.ToArray(), testCase, 3, ED);
                    if (int.Parse(result.Substring(1)) == ((i + 1) / 7))
                        accurate++;
                    else { }
                    //   MessageBox.Show(result);
                }
                else
                {

                    List<projectedTrainingMatrix> projectedTrainingSet = lda.getProjectedTrainingSet();

                    double[][] testCase = lda.getW().Transpose().Multiply(mat.Subtract(lda.getMeanMatrix()).ToArray());

                    String result = KNN.assignLabel(projectedTrainingSet.ToArray(), testCase, 3, ED);
                    if (int.Parse(result.Substring(1)) == ((i + 1) / 7))
                        accurate++;
                    else { }

                    //MessageBox.Show(result);
                    //    }

                }

            }
            MessageBox.Show("tested : " + counter + "\n accurate :" + accurate.ToString());

            //		


        }

        void Test_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressTraining.IsIndeterminate = false;
            btnTrain.IsEnabled = true;
            btnTest.IsEnabled = true;
        }
        void start()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"E:\Work\Work apps\Antony\IFI_DB");

            info = dirInfo.GetFiles("*.jpg");
            //info.Length
            numberofimages = info.Length;

            numOfClasses = numberofimages / ImagePerClass;
            mymat = new List<double[]>();
            List<string> filenames = new List<string>();
            for (int i = 0; i < numberofimages; i++)
            {
                if (info[i].Name.Substring(3, 2) != "07")
                {

                    filenames.Add(info[i].Name.Substring(0, 3));
                    var img = BitmapFactory.New(1, 1).FromStream(info[i].OpenRead());
                    if (img.PixelWidth != 100 || img.PixelHeight != 150)
                    {
                        img = img.Resize(100, 150, WriteableBitmapExtensions.Interpolation.Bilinear);
                    }
                    mymat.Add(helper.imgtomat(img.ToByteArray()));


                }
            }
            GC.Collect();
          if (decide == 0)
             {
                 pca = new PCA(mymat.ToArray(), filenames, filenames.Count);
             }
             else
             {
                 if (File.Exists(@"E:\Work\Work apps\lda pca face rec\outputFile.txt"))
                 {
                     ObjectToSerialize objectToSerialize = Serializer.DeSerializeObject(@"E:\Work\Work apps\lda pca face rec\outputFile.txt");
                     lda = objectToSerialize.Lda;
                   
                 }
                 else
                 {
                     lda = new LDA(mymat.ToArray(), filenames, filenames.Count);

                     ObjectToSerialize objectToSerialize = new ObjectToSerialize();
                     objectToSerialize.Lda = lda ;

                     Serializer serializer = new Serializer();
                     serializer.SerializeObject(@"E:\Work\Work apps\lda pca face rec\outputFile.txt", objectToSerialize);
                 }
             }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            progressTraining.IsIndeterminate = true;
            btnTrain.IsEnabled = false;
            btnTest.IsEnabled = false;
            testWorker.RunWorkerAsync();
        }

        //async void btnClicked()
        //{

          
        //    numberofimages = files.Count;
        //    numOfClasses =  numberofimages / ldahelper.ImagePerClass;
        //    mymat = new byte[ldahelper.numberofimages][,];
        //    for (int i = 0; i < ldahelper.numberofimages; i++)
        //    {

        //        var firstimg = await folder.GetFileAsync(files[i].Name);
        //        //   BitmapImage bmimg = new BitmapImage();
        //        var imgstream = await firstimg.OpenAsync(Windows.Storage.FileAccessMode.Read);

        //        var img = await BitmapFactory.New(1, 1).FromStream(imgstream);
        //        if (img.PixelWidth != 100 || img.PixelHeight != 150)
        //        {
        //            img = img.Resize(100, 150, WriteableBitmapExtensions.Interpolation.Bilinear);
        //        }

        //        //    grayimg = img.Gray();

        //        ldahelper.mymat[i] = image_loader.imgtomat(img.PixelBuffer.ToArray(), 100, 150);

        //        myimg.Source = img.Gray();

        //        //  int start6 = Environment.TickCount;


        //    }
        //    ldahelper.CalculateMeans();
        //    ldahelper.CalculateScatter();
        //    //    grayimg;
        // teted 84 
        // acurate 56
        //}


     
    }
}
