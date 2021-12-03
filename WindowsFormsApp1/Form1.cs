using java.io;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using weka.core;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        const int percentSplit = 66;

        static weka.classifiers.Classifier Bestmodel = null;

        static weka.classifiers.Classifier J48model = null;
        static weka.classifiers.Classifier RandomForestmodel = null;
        static weka.classifiers.Classifier RandomTreemodel = null;
        static weka.classifiers.Classifier NaiveBayesmodel = null;
        static weka.classifiers.Classifier _1NNmodel = null;
        static weka.classifiers.Classifier _3NNmodel = null;
        static weka.classifiers.Classifier _5NNmodel = null;
        static weka.classifiers.Classifier _7NNmodel = null;
        static weka.classifiers.Classifier _9NNmodel = null;
        static weka.classifiers.Classifier LogRegmodel = null;
        static weka.classifiers.Classifier MLPmodel = null;
        static weka.classifiers.Classifier SVMmodel = null;

        static List<Object> newValues;
        static string bestAlgorithm;
        static int x = 20;
        static int y = 20;
        static Instances insts;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            //Select file and get filename
            openFileDialog1.Filter = "Dataset files(*.arff;*.csv)|*.arff;*.csv|All Files(*.*)|*.*";
            openFileDialog1.Title = "Please select a dataset";
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if(dialogResult == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                textBoxFileName.Text = fileName;
                Classification(fileName);
            }
        }

        private void Classification(string fileName)
        {
            newValues = new List<object>();
            insts = new Instances(new FileReader(fileName));

            double maxVal = findBestAlgorithm(insts);
            string maxValue = String.Format("{0:0.00}", maxVal);
            labelBestSolution.Text = bestAlgorithm + " is the most succesful algorithm for this data set(%" + maxValue + ")";
            
        }

        private double findBestAlgorithm(Instances insts)
        {
            Cursor.Current = Cursors.WaitCursor;
            richTextBox1.Text += "Testing J48 Algorithm...";
            double maxValue = J48classifyTest(insts);
            Bestmodel = J48model;
            bestAlgorithm = "Desicion Tree(J48)";

            richTextBox1.Text += "\n Testing Naive Bayes Algorithm...";
            double nbValue = NaiveBayesclassifyTest(insts);
            if (nbValue > maxValue)
            {
                maxValue = nbValue;
                Bestmodel = NaiveBayesmodel;
                bestAlgorithm = "Naive Bayes";
            }

            richTextBox1.Text += "\n Testing Random Forest Algorithm...";
            double ranForValue = RandomForestmodelclassifyTest(insts);
            if (ranForValue > maxValue)
            {
                ranForValue = nbValue;
                Bestmodel = RandomForestmodel;
                bestAlgorithm = "Random Forest";
            }

            richTextBox1.Text += "\n Testing Random Tree Algorithm...";
            double ranTreeValue = RandomTreemodelmodelclassifyTest(insts);
            if (ranTreeValue > maxValue)
            {
                maxValue = ranTreeValue;
                Bestmodel = RandomTreemodel;
                bestAlgorithm = "Random Tree";
            }

            richTextBox1.Text += "\n Testing 1-Nearest Neighbor Algorithm...";
            double _1NNValue = _1NNmodelclassifyTest(insts);
            if (_1NNValue > maxValue)
            {
                maxValue = _1NNValue;
                Bestmodel = _1NNmodel;
                bestAlgorithm = "1-Nearest Neighbour";
            }

            richTextBox1.Text += "\n Testing 3-Nearest Neighbor Algorithm...";
            double _3NNValue = _3NNmodelclassifyTest(insts);
            if (_3NNValue > maxValue)
            {
                maxValue = _3NNValue;
                Bestmodel = _3NNmodel;
                bestAlgorithm = "3-Nearest Neighbour";
            }

            richTextBox1.Text += "\n Testing 5-Nearest Neighbor Algorithm...";
            double _5NNValue = _5NNmodelclassifyTest(insts);
            if (_5NNValue > maxValue)
            {
                maxValue = _5NNValue;
                Bestmodel = _5NNmodel;
                bestAlgorithm = "5-Nearest Neighbour";
            }

            richTextBox1.Text += "\n Testing 7-Nearest Neighbor Algorithm...";
            double _7NNValue = _7NNmodelclassifyTest(insts);
            if (_7NNValue > maxValue)
            {
                maxValue = _7NNValue;
                Bestmodel = _7NNmodel;
                bestAlgorithm = "7-Nearest Neighbour";
            }

            richTextBox1.Text += "\n Testing 9-Nearest Neighbor Algorithm...";
            double _9NNValue = _9NNmodelclassifyTest(insts);
            if (_9NNValue > maxValue)
            {
                maxValue = _9NNValue;
                Bestmodel = _9NNmodel;
                bestAlgorithm = "9-Nearest Neighbour";
            }

            richTextBox1.Text += "\n Testing Logistic Regression Algorithm...";
            double logRegValue = LogRegmodelclassifyTest(insts);
            if (logRegValue > maxValue)
            {
                maxValue = logRegValue;
                Bestmodel = LogRegmodel;
                bestAlgorithm = "Logistic Regression";
            }

            richTextBox1.Text += "\n Testing Artificial Neural Network(MLP) Algorithm...";
            double mlpValue = MLPmodelclassifyTest(insts);
            if (mlpValue > maxValue)
            {
                maxValue = mlpValue;
                Bestmodel = MLPmodel;
                bestAlgorithm = "Artificial Neural Network(MLP)";
            }

            richTextBox1.Text += "\n Testing Support Vector Machine Algorithm...";
            double svmValue = SVMmodelclassifyTest(insts);
            if (svmValue > maxValue)
            {
                maxValue = svmValue;
                Bestmodel = SVMmodel;
                bestAlgorithm = "Support Vector Machine";
            }
            Cursor.Current = Cursors.Default;
            return maxValue;
        }
        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        #region Classification Tests
        public static double J48classifyTest(Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                J48model = new weka.classifiers.trees.J48();

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                Instances train = new Instances(insts, 0, trainSize);

                J48model.buildClassifier(train);


                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    Instance currentInst = insts.instance(i);
                    double predictedClass = J48model.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double RandomForestmodelclassifyTest(Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                RandomForestmodel = new weka.classifiers.trees.RandomForest();

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                Instances train = new Instances(insts, 0, trainSize);

                RandomForestmodel.buildClassifier(train);


                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    Instance currentInst = insts.instance(i);
                    double predictedClass = RandomForestmodel.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double RandomTreemodelmodelclassifyTest(Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                RandomTreemodel = new weka.classifiers.trees.RandomTree();

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                Instances train = new Instances(insts, 0, trainSize);

                RandomTreemodel.buildClassifier(train);


                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    Instance currentInst = insts.instance(i);
                    double predictedClass = RandomTreemodel.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double NaiveBayesclassifyTest(Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                NaiveBayesmodel = new weka.classifiers.bayes.NaiveBayes();

                weka.filters.Filter myDiscretize = new weka.filters.unsupervised.attribute.Discretize();
                myDiscretize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myDiscretize);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                Instances train = new Instances(insts, 0, trainSize);

                NaiveBayesmodel.buildClassifier(train);

                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    Instance currentInst = insts.instance(i);
                    double predictedClass = NaiveBayesmodel.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double _1NNmodelclassifyTest(Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                _1NNmodel = new weka.classifiers.lazy.IBk(1);

                weka.filters.Filter myNominalToBinary = new weka.filters.unsupervised.attribute.NominalToBinary();
                myNominalToBinary.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNominalToBinary);

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                Instances train = new Instances(insts, 0, trainSize);

                _1NNmodel.buildClassifier(train);

                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    Instance currentInst = insts.instance(i);
                    double predictedClass = _1NNmodel.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double _3NNmodelclassifyTest(Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                _3NNmodel = new weka.classifiers.lazy.IBk(3);

                weka.filters.Filter myNominalToBinary = new weka.filters.unsupervised.attribute.NominalToBinary();
                myNominalToBinary.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNominalToBinary);

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                Instances train = new Instances(insts, 0, trainSize);

                _3NNmodel.buildClassifier(train);

                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    Instance currentInst = insts.instance(i);
                    double predictedClass = _3NNmodel.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double _5NNmodelclassifyTest(Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                _5NNmodel = new weka.classifiers.lazy.IBk(5);

                weka.filters.Filter myNominalToBinary = new weka.filters.unsupervised.attribute.NominalToBinary();
                myNominalToBinary.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNominalToBinary);

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                Instances train = new Instances(insts, 0, trainSize);

                _5NNmodel.buildClassifier(train);

                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    Instance currentInst = insts.instance(i);
                    double predictedClass = _5NNmodel.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double _7NNmodelclassifyTest(Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                _7NNmodel = new weka.classifiers.lazy.IBk(7);

                weka.filters.Filter myNominalToBinary = new weka.filters.unsupervised.attribute.NominalToBinary();
                myNominalToBinary.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNominalToBinary);

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                Instances train = new Instances(insts, 0, trainSize);

                _7NNmodel.buildClassifier(train);

                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    Instance currentInst = insts.instance(i);
                    double predictedClass = _7NNmodel.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double _9NNmodelclassifyTest(Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                _9NNmodel = new weka.classifiers.lazy.IBk(9);

                weka.filters.Filter myNominalToBinary = new weka.filters.unsupervised.attribute.NominalToBinary();
                myNominalToBinary.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNominalToBinary);

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                Instances train = new Instances(insts, 0, trainSize);

                _9NNmodel.buildClassifier(train);

                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    Instance currentInst = insts.instance(i);
                    double predictedClass = _9NNmodel.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double LogRegmodelclassifyTest(Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                LogRegmodel = new weka.classifiers.functions.Logistic();

                weka.filters.Filter myNominalToBinary = new weka.filters.unsupervised.attribute.NominalToBinary();
                myNominalToBinary.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNominalToBinary);

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                Instances train = new Instances(insts, 0, trainSize);

                LogRegmodel.buildClassifier(train);

                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    Instance currentInst = insts.instance(i);
                    double predictedClass = LogRegmodel.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double MLPmodelclassifyTest(Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                MLPmodel = new weka.classifiers.functions.MultilayerPerceptron();

                weka.filters.Filter myNominalToBinary = new weka.filters.unsupervised.attribute.NominalToBinary();
                myNominalToBinary.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNominalToBinary);

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                Instances train = new Instances(insts, 0, trainSize);

                MLPmodel.buildClassifier(train);

                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    Instance currentInst = insts.instance(i);
                    double predictedClass = MLPmodel.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double SVMmodelclassifyTest(Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                SVMmodel = new weka.classifiers.functions.SMO();

                weka.filters.Filter myNominalToBinary = new weka.filters.unsupervised.attribute.NominalToBinary();
                myNominalToBinary.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNominalToBinary);

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                Instances train = new Instances(insts, 0, trainSize);

                SVMmodel.buildClassifier(train);

                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    Instance currentInst = insts.instance(i);
                    double predictedClass = SVMmodel.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }
        #endregion
    }
}
