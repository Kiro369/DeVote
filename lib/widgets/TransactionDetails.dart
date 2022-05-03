import 'package:devote/models/block.dart';
import 'package:devote/widgets/BlockDetails.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import '../models/Call_api.dart';

class TransactionDetails extends StatefulWidget {
  final String hash;
  final int dateTime;
  final String elector;
  final String elected;
  final int block;

  const TransactionDetails(
      {required this.hash,
      required this.dateTime,
      required this.elector,
      required this.elected,
      required this.block});

  @override
  _TransactionDetailsState createState() => _TransactionDetailsState();
}

class _TransactionDetailsState extends State<TransactionDetails> {
  late Future<List<Results>> blockks;
  @override
  initState() {
    super.initState();

    CallApi block = CallApi(
        Uri.https('devote-explorer-backend.herokuapp.com', 'blocks'));

    blockks = block.getBlocks();

  }

  void block(String miner,int transactions,int blockHeight,int time,String merkleRoot,String hash,String prevHash) async {

    Navigator.push(
      context,
      MaterialPageRoute(
          builder: (context) => BlockDetails(
              miner: miner,
              transactions: transactions,
              blockHeight: blockHeight,
              time: time,
              merkleRoot: merkleRoot,
              hash: hash,
              prevhash:prevHash)),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        leading: Icon(
          Icons.arrow_back_ios_outlined,
          size: 0,
          color: Color(0xff26375f),
        ),
        title: Text(
          'Transaction Details',
          style: TextStyle(color: Colors.white),
        ),
        centerTitle: true,
        backgroundColor: Color(0xff26375f),
      ),
      body: ListView(
        children: [
          Padding(
            padding: const EdgeInsets.only(top: 8.0),
            child: ListTile(
              title: Text(
                'Transaction Hash:',
                style:
                    TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
              ),
              subtitle: Padding(
                padding: const EdgeInsets.only(top: 8.0, bottom: 8.0),
                child: Text(
                  widget.hash,
                  style: TextStyle(color: Colors.black),
                ),
              ),
            ),
          ),
          Divider(
            height: 2,
            color: Colors.grey,
          ),
          ListTile(
            title: Text(
              'Timestamp:',
              style:
                  TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
            ),
            subtitle: Padding(
              padding: const EdgeInsets.only(top: 8.0),
              child: Text(
                DateTime.fromMillisecondsSinceEpoch(widget.dateTime).toString(),
                style: TextStyle(color: Colors.black),
              ),
            ),
          ),
          Divider(
            height: 2,
            color: Colors.grey,
          ),
          ListTile(
            title: Text(
              'From:',
              style:
                  TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
            ),
            subtitle: Text(
              widget.elector,
              style: TextStyle(color: Colors.black),
              overflow: TextOverflow.fade,
              softWrap: false,
            ),
          ),
          ListTile(
            title: Text(
              'To:',
              style:
                  TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
            ),
            subtitle: Text(
              widget.elected,
              style: TextStyle(color: Colors.black),
              overflow: TextOverflow.fade,
              softWrap: false,
            ),
          ),
          Divider(
            height: 2,
            color: Colors.grey,
          ),
          FutureBuilder<List<Results>>(
            future: blockks,
            builder: (context, snapshot) {
              if (snapshot.hasData) {
                return ListTile(
                  title: Text(
                    'Block:',
                    style: TextStyle(
                        color: Colors.black, fontWeight: FontWeight.bold),
                  ),
                  subtitle: Padding(
                    padding: const EdgeInsets.only(top: 8.0),
                    child: InkWell(
                      onTap: () => block(snapshot.data![0].miner,
                        snapshot.data![0].transactions,
                        snapshot.data![0].blockHeight,
                        snapshot.data![0].time,
                        snapshot.data![0].merkleRoot,
                        snapshot.data![0].hash,
                        snapshot.data![0].prevHash,),
                      child: Text(
                        widget.block.toString(),
                        style: TextStyle(color: Colors.blue),
                        overflow: TextOverflow.fade,
                        softWrap: false,
                      ),
                    ),
                  ),
                );
              } else if (snapshot.hasError) {
                return CircularProgressIndicator(color: Colors.grey[50],);
              } // spinner
              return  ListTile(
                title: Text(
                  'Block:',
                  style: TextStyle(
                      color: Colors.black, fontWeight: FontWeight.bold),
                ),
                subtitle: Padding(
                  padding: const EdgeInsets.only(top: 8.0),
                  child: Text(
                    widget.block.toString(),
                    style: TextStyle(color: Colors.black),
                    overflow: TextOverflow.fade,
                    softWrap: false,
                  ),
                ),
              );
            },
          ),
          kIsWeb
              ? Center(
                  child: Container(
                      height: 180,
                      width: 180,
                      child: Image.asset('assets/a4.png',
                          color: const Color(0xff26375f))),
                )
              : Text(''),
        ],
      ),
    );
  }
}
