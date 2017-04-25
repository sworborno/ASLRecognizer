
//Author Jiban Adhikary
//This is the main class file of the server
public class ClassifierASL {
	
	public static void main(String[] args) throws Exception {
		
        // Initiate the server
		ServerASL server = new ServerASL();
		System.out.println("Starting Server");
		server.startServer();
	}
	
}
