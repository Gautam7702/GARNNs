import socket

def start_server():
    # Set the host and port to listen on
    host = '127.0.0.1'
    port = 12345

    # Create a socket object
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    # Bind the socket to a specific address and port
    server_socket.bind((host, port))

    # Listen for incoming connections (max 5 clients in the queue)
    server_socket.listen(5)

    print(f"Server listening on {host}:{port}")

    while True:
        # Wait for a connection from a client
        client_socket, client_address = server_socket.accept()
        print(f"Accepted connection from {client_address}")

        # Handle the client's request
        handle_client(client_socket)

def handle_client(client_socket):
    try:
        # Process the data and get the output
        output = findRoadPercentage("./Snapshot.png")
        # Send the output back to the client
        client_socket.send(output.encode('utf-8'))


        print("Connection closed")
    except Exception as e:
        print(f"Error handling client: {str(e)}")
    finally:
        client_socket.close()

def findRoadPercentage(filePath):
    return "100"

if __name__ == "__main__":
    start_server()
