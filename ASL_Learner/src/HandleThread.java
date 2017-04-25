import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.PrintStream;
import java.net.Socket;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.List;
import java.io.RandomAccessFile;
import java.io.PrintWriter;

import weka.classifiers.functions.MultilayerPerceptron;
import weka.classifiers.trees.J48;
import weka.core.Instances;

public class HandleThread extends Thread
{
	protected Socket socket;
	protected Instances trainInstances; 
	protected MultilayerPerceptron mlp;
	protected J48 decisionTree;
	static String testFilePath = "C:\\Users\\Jiban Adhikary\\Documents\\Projects\\ASLRecognizer\\Letters\\test_inst.arff";
	int position = 41;
	
	protected UserInterfaceASL UI;
	
	
	Instances testInstances = null; 
	
    public HandleThread(Socket clientSocket, Instances trainInstances, MultilayerPerceptron mlp, J48 decisionTree, UserInterfaceASL UI) {
        this.socket = clientSocket;
        this.trainInstances = trainInstances;
        this.decisionTree = decisionTree;
        this.mlp = mlp;
        this.UI = UI;
    }
    
    public void writeDataInTestFile(String data)
    {
    	try{
    		PrintWriter writer = new PrintWriter(testFilePath, "UTF-8");
    		String relationName = "@RELATION test";
    		String attribute = "@ATTRIBUTE";
    		String attributeClass = "@ATTRIBUTE class {A,B,C,D,E,F,I,L,R,U,V,W,X,Y}";
    		String dataStart = "@DATA";
    		
    		writer.println(relationName);
    		for(int i = 1; i < 36; i++)
    		{
    			writer.println(attribute+" f"+i+" REAL");
    		}
    		writer.println(attributeClass);
    		writer.println(dataStart);
    		writer.println(data);
    		writer.close();
    	}
    	
    	catch(Exception e)
    	{
    		System.out.println("Exception "+ e);
    	}
    }
    
    public String recognizeSymbolMLP() throws Exception
    {
    	BuildClassifier buildClassifier = new BuildClassifier();
    	testInstances = buildClassifier.loadTestData(testFilePath);
    	String symbol = buildClassifier.evaluateMLP(mlp, trainInstances, testInstances);
    	return symbol;
    }
    
    public String recognizeSymbolDecisionTree() throws Exception
    {
    	BuildClassifier buildClassifier = new BuildClassifier();
    	testInstances = buildClassifier.loadTestData(testFilePath);
    	String symbol = buildClassifier.evaluateDecisionTree(decisionTree, trainInstances, testInstances);
    	return symbol;
    }
    
    
    public void run()
    {
    	InputStream inputStream = null;
    	BufferedReader bufferReader = null;
        DataOutputStream outputStream = null;
        
        try
        {
        	System.out.println("Reading input stream...");
        	inputStream = socket.getInputStream();
        	bufferReader = new BufferedReader(new InputStreamReader(inputStream));
        	outputStream = new DataOutputStream(socket.getOutputStream());
        }
        catch(IOException ex)
        {
        	return; 
        }
        
        
        String line;
        try 
        {
        	System.out.println("Reading input");
			line = bufferReader.readLine();
			if(line!=null)
			{
				
				System.out.println("Data received, sending ack to client");
				outputStream.flush();
				System.out.println(line);
				
				
				writeDataInTestFile(line);
				
				
				String sign = null; 
				try {
					//sign = recognizeSymbolMLP(); // Call function to recognize a symbol by mlp
					sign = recognizeSymbolDecisionTree(); //call function to recognize a symbol using decision tree
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				
				String guiOutput = null;
				
				if(sign.equals("0.0"))
					guiOutput = ""+ 'A';
				else if(sign.equals("1.0"))
					guiOutput = ""+ 'B';
				else if(sign.equals("2.0"))
					guiOutput = ""+ 'C';
				else if(sign.equals("3.0"))
					guiOutput = ""+ 'D';
				else if(sign.equals("4.0"))
					guiOutput = ""+ 'E';
				else if(sign.equals("5.0"))
					guiOutput = ""+ 'F';
				else if(sign.equals("6.0"))
					guiOutput = ""+ 'I';
				else if(sign.equals("7.0"))
					guiOutput = ""+ 'L';
				else if(sign.equals("8.0"))
					guiOutput = ""+ 'R';
				else if(sign.equals("9.0"))
					guiOutput = ""+ 'U';
				else if(sign.equals("10.0"))
					guiOutput = ""+ 'V';
				else if(sign.equals("11.0"))
					guiOutput = ""+ 'W';
				else if(sign.equals("12.0"))
					guiOutput = ""+ 'X';
				else if(sign.equals("13.0"))
					guiOutput = ""+ 'Y';
				
				
				System.out.println("Letter recognized as: "+guiOutput);
				outputStream.writeBytes("Letter recognized as: "+guiOutput);
				UI.jLabel2.setText(guiOutput);
				socket.close();
				
				/*UserInterfaceASL aslgui = new UserInterfaceASL();
				aslgui.setSign(sign);
				aslgui.drawGUI();*/
				
			}
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return;
		}
    }
}
