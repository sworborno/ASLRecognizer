import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.Socket;



//This is a client file in this application to check if the server is running correctly.
public class TestClientASL {

	public static void main(String[] args) throws IOException, Exception 
	{
		
		String data = "1,2.137793,0,84.25,46.53262,1,0.03281304,-0.5382621,-0.8421385,-0.07145109,-0.9973112,-0.01628303,0.07502378,-0.6512374,0.7551565,-0.08448222,-0.6265414,0.7747959,0.150245,-0.263298,-0.9529431,0,0,0,0,1,28.05521,36.92539,21.90141,19.57606,59.54183,55.88707,7.508076,25.10793,22.41219,?";
		Socket clientSocket = new Socket("localhost", 9999);
		DataOutputStream outToServer = new DataOutputStream(clientSocket.getOutputStream());
		BufferedReader inFromServer = new BufferedReader(new InputStreamReader(clientSocket.getInputStream()));
		
		outToServer.writeBytes(data + '\n');
		String result = inFromServer.readLine();
		System.out.println("FROM SERVER: " + result);
		clientSocket.close();
	}

}
