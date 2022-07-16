# DeVote
A Decentralized E-Voting Project using Distributed Ledger Technology (DLT) and Deep Learning.

# Description
Applying DLTs to E-Voting is not easy and is a challenging task, and we love challenge.
* We are using Blockchain as an example of DLTs (for educational purposes).
* Our Blockchain uses LRN (Least Random Number) Consensus, in which all Blockchain nodes at time X-1 broadcast
    a generated random number, and at time X, the node with least random number gets to add the next block.
* Our blockchain is a permissioned blockchain, that uses a private network that his its own DNS Seeder and is secured with AES encryption.
* The AES key is exchanged over the network securely using Elliptic-curve Diffie-Hellman Key Exchange. 
* All stored information is hashed with Argon2id hashing algorithm.
* All the data is stored using LevelDB.
* The packets sent over the network are encoded with Google Protobuf.

# Installation

First you gotta run the DNS Seeder, preferably on a static IP machine, you need to open a port for the DNS Seeder (any number)
you can adjust the port number in Program.cs in DNSSeeder project
```
private static readonly int Port = your_port;
```

then in Program.cs in DeVote project
adjust the following line:
```
DNSSeeder.AsynchronousClient seederClient = new(your_ip, your_port);
```

you need to open another port for the Node itself (DeVote), you can adjust the port number in Program.cs in DeVote project.
```
var server = new Server(4269);
```
After setting up the environment (steps below) adjust the paths appsettings.json in DeVote Project.
This should be enough to get your Blockchain network up and running.
Then to get the Flutter App running, you should configure the API, you can find how to configure it here
[DeVote API](https://github.com/Kiro369/DeVote/blob/master/Explorer/ReadME.md)
after you get the API running, adjust the API endpoint in 
```
FlutterApp/lib/models/Ip.dart
```
to get the flutter app running.

To get the Desktop app fully running you need to setup the Python/DeepLearning environment, that can be found here.
[DeVote AI Environment](https://github.com/Kiro369/DeVote/blob/master/Recognition/README.md)


# Tribute
This project is dedicated to the loving memory of our late dear friend and precious team member Ahmed Al-M'aandi whom his beautiful soul passed away and heartbreakingly left our world before this project could come to light. This is for you Ahmed, we hope you're proud of us, we will forever miss you, till we see you again.
