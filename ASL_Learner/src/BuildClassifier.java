import weka.core.Instances;
import weka.classifiers.Evaluation;
import weka.classifiers.functions.MultilayerPerceptron;
import weka.classifiers.trees.J48;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;



public class BuildClassifier {
	
	//"C:\\Users\\Jiban Adhikary\\Documents\\Projects\\ASLRecognizer\\Letters\\train.arff";
	
	//BufferedReader testDataReader = null;
	//Classifier classifier = null; 
	//MultilayerPerceptron mlp = null; 
	//Evaluation eval = null;
	
	
	public Instances loadTrainData(String trainFilePath) throws IOException
	{
		BufferedReader trainDataReader = new BufferedReader(new FileReader(trainFilePath));
        Instances trainInstances = new Instances(trainDataReader);
        trainInstances.setClassIndex(trainInstances.numAttributes() - 1);
        trainDataReader.close();    //loading training data
        System.out.println("Data have been read");
        return trainInstances; 
	}
	public Instances loadTestData(String testFilePath) throws IOException
	{
		BufferedReader testDataReader = new BufferedReader(new FileReader(testFilePath));
        Instances testInstances = new Instances(testDataReader);
        testInstances.setClassIndex(testInstances.numAttributes() - 1);
        testDataReader.close();    //loading training data
        System.out.println("Data have been read");
        return testInstances; 
	}
	
	public MultilayerPerceptron mlpClassifier(Instances trainInstances) throws Exception
	{
		MultilayerPerceptron mlp = new MultilayerPerceptron();
		mlp.setLearningRate(0.3);
        mlp.setMomentum(0.2);
        mlp.setTrainingTime(500);
        mlp.setHiddenLayers("a");
		mlp.buildClassifier(trainInstances);
		return mlp;
	}
	
	public J48 decisionTreeTreeClassifier(Instances trainInstances) throws Exception
	{
		J48 decisionTree = new J48();
		decisionTree.buildClassifier(trainInstances);
		return decisionTree;
	}
	
	public String evaluateMLP(MultilayerPerceptron mlp, Instances trainInstances, Instances testInstances) throws Exception
	{
		Evaluation eval = new Evaluation(trainInstances);
        eval.evaluateModelOnce(mlp,testInstances.firstInstance());
        String clsLabel = ""+ mlp.classifyInstance(testInstances.instance(0));
        return clsLabel;
	}
	
	public String evaluateDecisionTree(J48 decisionTree, Instances trainInstances, Instances testInstances) throws Exception
	{
		Evaluation eval = new Evaluation(trainInstances);
        eval.evaluateModelOnce(decisionTree,testInstances.firstInstance());
        String clsLabel = ""+ decisionTree.classifyInstance(testInstances.instance(0));
        return clsLabel;
	}
}
