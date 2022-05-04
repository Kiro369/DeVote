import 'package:devote/models/Call_api.dart';
import 'package:devote/models/transaction.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import 'TransactionsList.dart';

class BlockDetails extends StatefulWidget {
  final String miner;
  final int blockHeight;
  final int transactions;
  final int time;
  final String merkleRoot;
  final String hash;
  final String prevhash;

  // ignore: use_key_in_widget_constructors
  const BlockDetails(
      {required this.miner,
      required this.transactions,
      required this.blockHeight,
      required this.time,
      required this.merkleRoot,
      required this.hash,
      required this.prevhash});

  @override
  _BlockDetailsState createState() => _BlockDetailsState();
}

class _BlockDetailsState extends State<BlockDetails> {
  late Future<List<Result>> daata;
  late List transactionList;

  @override
  initState() {
    super.initState();

    CallApi network = CallApi(Uri.https('devote-explorer-backend.herokuapp.com',
        'transactions/tx-block-height/${widget.blockHeight}'));
    daata = network.getTransaction();

    //block();
  }


  void block() async {
    transactionList = await daata;
    Navigator.push(
      context,
      MaterialPageRoute(
          builder: (context) => TransactionsList(transactionList)),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        leading: const Icon(
          Icons.arrow_back_ios_outlined,
          size: 0,
          color: Color(0xff26375f),
        ),
        title: const Text(
          'Block Details',
          style: TextStyle(color: Colors.white),
        ),
        centerTitle: true,
        backgroundColor: const Color(0xff26375f),
      ),
      body: ListView(
        children: [
          ListTile(
            title: const Padding(
              padding: EdgeInsets.only(top: 8.0),
              child: Text(
                'Block Height:',
                style:
                    TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
              ),
            ),
            subtitle: Padding(
              padding: const EdgeInsets.only(top: 8.0),
              child: Text(
                widget.blockHeight.toString(),
                style: const TextStyle(color: Colors.black),
              ),
            ),
          ),
          const Divider(
            height: 2,
            color: Colors.grey,
          ),

          ListTile(
            title: const Text(
              'Timestamp:',
              style:
                  TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
            ),
            subtitle: Padding(
              padding: const EdgeInsets.only(top: 8.0),
              child: Text(
                DateTime.fromMillisecondsSinceEpoch(widget.time).toString(),
                style: const TextStyle(color: Colors.black),
              ),
            ),
          ),
          const Divider(
            height: 2,
            color: Colors.grey,
          ),
          ListTile(
            title: const Padding(
              padding: EdgeInsets.only(top: 8.0),
              child: Text(
                'MerkleRoot:',
                style:
                    TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
              ),
            ),
            subtitle: Padding(
              padding: const EdgeInsets.only(top: 8.0, bottom: 8.0),
              child: Text(
                widget.merkleRoot,
                style: const TextStyle(color: Colors.black),
              ),
            ),
          ),
          const Divider(
            height: 2,
            color: Colors.grey,
          ),
          ListTile(
            title: const Text(
              'Transactions:',
              style:
                  TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
            ),
            subtitle: Padding(
              padding: const EdgeInsets.only(top: 8.0),
              child: InkWell(
                  onTap: () => block(),
                  child: Text(
                    widget.transactions.toString(),
                    style: const TextStyle(color: Colors.blue),
                  )),
            ),
          ),
          const Divider(
            height: 2,
            color: Colors.grey,
          ),
          ListTile(
            title: const Padding(
              padding: EdgeInsets.only(top: 8.0),
              child: Text(
                'Hash:',
                style:
                    TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
              ),
            ),
            subtitle: Padding(
              padding: const EdgeInsets.only(top: 8.0, bottom: 8.0),
              child: Text(
                widget.hash,
                style: const TextStyle(color: Colors.black),
              ),
            ),
          ),
          const Divider(
            height: 2,
            color: Colors.grey,
          ),
          ListTile(
            title: const Text(
              'Mined by:',
              style:
                  TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
            ),
            subtitle: Padding(
              padding: const EdgeInsets.only(top: 8.0),
              child: RichText(
                maxLines: 2,
                overflow: TextOverflow.fade,
                softWrap: false,
                text: TextSpan(
                  text: widget.miner,
                  style: const TextStyle(color: Colors.black, fontSize: 13),
                ),
              ),
            ),
          ),
          kIsWeb
              ? Center(
                  child: SizedBox(
                      height: 180,
                      width: 180,
                      child: Image.asset('assets/a4.png',
                          color: const Color(0xff26375f))),
                )
              : const Text(''),
        ],
      ),
    );
  }
}
