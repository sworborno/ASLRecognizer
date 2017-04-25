import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;

import weka.classifiers.functions.MultilayerPerceptron;
import weka.core.Instances;
import weka.classifiers.trees.J48;


public class ServerASL 
{
	// 
	static final int PORT = 9999;
	
	static String trainFilePath = "C:\\Users\\Jiban Adhikary\\Documents\\Projects\\ASLRecognizer\\Letters\\train.arff";
	
	static Instances trainInstances = null; 
	static Instances testInstances = null; 
	static MultilayerPerceptron mlp = null;
	static String symbol = null; 
	static J48 decisionTree = null;
	
	UserInterfaceASL UI = new UserInterfaceASL();
	
	void buildModelWithMLP() throws Exception
	{
		BuildClassifier buildClassifier = new BuildClassifier();
		trainInstances = buildClassifier.loadTrainData(trainFilePath);
		mlp = buildClassifier.mlpClassifier(trainInstances);
		
		//testInstances = buildClassifier.loadTestData(testFilePath);
		//symbol = buildClassifier.evaluate(mlp, trainInstances, testInstances);
	}
	
	void buildModelwithDecisionTree() throws Exception
	{
		BuildClassifier buildClassifier = new BuildClassifier();
		trainInstances = buildClassifier.loadTrainData(trainFilePath);
		decisionTree = buildClassifier.decisionTreeTreeClassifier(trainInstances);
	}
	
	void makeGUI()
	{
		try {
            for (javax.swing.UIManager.LookAndFeelInfo info : javax.swing.UIManager.getInstalledLookAndFeels()) {
                if ("Nimbus".equals(info.getName())) {
                    javax.swing.UIManager.setLookAndFeel(info.getClassName());
                    break;
                }
            }
        } catch (ClassNotFoundException ex) {
            java.util.logging.Logger.getLogger(UserInterfaceASL.class.getName()).log(java.util.logging.Level.SEVERE, null, ex);
        } catch (InstantiationException ex) {
            java.util.logging.Logger.getLogger(UserInterfaceASL.class.getName()).log(java.util.logging.Level.SEVERE, null, ex);
        } catch (IllegalAccessException ex) {
            java.util.logging.Logger.getLogger(UserInterfaceASL.class.getName()).log(java.util.logging.Level.SEVERE, null, ex);
        } catch (javax.swing.UnsupportedLookAndFeelException ex) {
            java.util.logging.Logger.getLogger(UserInterfaceASL.class.getName()).log(java.util.logging.Level.SEVERE, null, ex);
        }
        //</editor-fold>

        /* Create and display the form */
        
        UI.jLabel2.setText("#");
        UI.setVisible(true);
        /*java.awt.EventQueue.invokeLater(new Runnable() {
            public void run() {
                
                //new UserInterface().setVisible(true);
                UI.jTextPane1.setText("H");
                
            }
        });*/
	}
	
	void startServer() throws Exception
	{
		//buildModelWithMLP(); 
		buildModelwithDecisionTree();
		System.out.println("Model has been built");
		
		makeGUI();
		System.out.println("GUI has been drawn");
		
		ServerSocket serverSocket = null;
        Socket socket = null;
        try 
        {
            serverSocket = new ServerSocket(PORT);
            System.out.println("Server socket created at port "+PORT);
            System.out.println("Waiting for client...");
        } 
        catch (IOException e) {
            e.printStackTrace();
        }
        while (true) {
            try 
            {
                socket = serverSocket.accept();
                System.out.println("Server socket has accepted a new client");
            } 
            catch (IOException e) {
                System.out.println("I/O error: " + e);
            }
            // new thread for a client
            new HandleThread(socket, trainInstances, mlp, decisionTree, UI).start();
        }
	}
	
		
}
