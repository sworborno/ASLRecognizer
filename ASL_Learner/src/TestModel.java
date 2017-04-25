import java.io.BufferedReader;
import java.io.FileReader;

import weka.classifiers.Evaluation;
import weka.classifiers.functions.MultilayerPerceptron;
import weka.core.Instances;

public class TestModel {

	public static void main(String[] args) throws Exception {
		System.out.println("Weka Tool");

		BufferedReader dataReader = new BufferedReader(new FileReader("C:\\Users\\Jiban Adhikary\\Documents\\Projects\\ASLRecognizer\\Letters\\train.arff"));
		Instances train = new Instances(dataReader);
		train.setClassIndex(train.numAttributes() -1);
		dataReader.close();    //loading training data

		System.out.println("Data have been read");

		BufferedReader treader = new BufferedReader(new FileReader("C:\\Users\\Jiban Adhikary\\Documents\\Projects\\ASLRecognizer\\Letters\\test.arff"));
		Instances test = new Instances(treader);
		test.setClassIndex(test.numAttributes() -1);
		treader.close();       //loading testing data

		//Classifier cls = new MultilayerPerceptron();
		//cls.buildClassifier(train); 

		MultilayerPerceptron mlp = new MultilayerPerceptron();
		//Setting Parameters
		mlp.setLearningRate(0.3);
		mlp.setMomentum(0.2);
		mlp.setTrainingTime(500);
		mlp.setHiddenLayers("a");
		mlp.buildClassifier(train);

		System.out.println("Data have been trained");



		//Evaluation of model
		Evaluation eval = new Evaluation(train);
		eval.evaluateModelOnce(mlp,test.firstInstance());



		for (int i = 0; i < test.numInstances(); i++) 
		{
			double clsLabel = mlp.classifyInstance(test.instance(i));
			System.out.println(clsLabel);
			//test.instance(i).setClassValue(clsLabel);
		}

		//System.out.println(eval.getClass());


		//Evaluation eval = new Evaluation(train);
		//eval.evaluateModelOnce(cls,test);

		//System.out.println(eval.toMatrixString("\nConfusion Matrix\n========\n"));

	}

}
