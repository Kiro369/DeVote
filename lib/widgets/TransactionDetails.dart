import '/models/block.dart';
import '/widgets/BlockDetails.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import '../models/Call_api.dart';
import 'blockchain.dart';

class TransactionDetails extends StatefulWidget {
  final String hash;
  final int dateTime;
  final String elector;
  final String elected;
  final int block;

  // ignore: use_key_in_widget_constructors
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
  late Future<BlockHeight> blockks;

  @override
  initState() {
    super.initState();
print(widget.block.toString());
    CallApi block =
        CallApi(Uri.https('devote-explorer-backend.herokuapp.com', 'blocks/block-height/${widget.block}'));
    blockks = block.getbyBlockHeight() ;
  }
  static bool isLargeScreen(BuildContext context) {
    return MediaQuery.of(context).size.width > 1200;
  }


  void block(String miner, int transactions, int blockHeight, int time,
      String merkleRoot, String hash, String prevHash) async {
    Navigator.pushReplacement(
      context,
      MaterialPageRoute(
          builder: (context) => BlockDetails(
              miner: miner,
              transactions: transactions,
              blockHeight: blockHeight,
              time: time,
              merkleRoot: merkleRoot,
              hash: hash,
              prevhash: prevHash)),
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
          'Transaction Details',
          style: TextStyle(color: Colors.white),
        ),
        centerTitle: true,
        backgroundColor: const Color(0xff26375f),
      ),
      body: kIsWeb&&isLargeScreen(context)? Row(
        children: [
          Padding(
            padding: const EdgeInsets.all(20.0),
            child: Container(
              width: MediaQuery.of(context).size.width/1.5,
              child: ListView(
                children: [
                  Padding(
                    padding: const EdgeInsets.only(top: 8.0),
                    child: ListTile(
                      title: const Text(
                        'Transaction Hash:',
                        style:
                        TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
                      ),
                      subtitle: Padding(
                        padding: const EdgeInsets.only(top: 8.0, bottom: 8.0),
                        child: Text(
                          widget.hash,
                          style: const TextStyle(color: Colors.black),
                        ),
                      ),
                    ),
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
                        DateTime.fromMillisecondsSinceEpoch(widget.dateTime).toString(),
                        style: const TextStyle(color: Colors.black),
                      ),
                    ),
                  ),

                  ListTile(
                    title: const Text(
                      'From:',
                      style:
                      TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
                    ),
                    subtitle: Text(
                      widget.elector,
                      style: const TextStyle(color: Colors.black),
                      overflow: TextOverflow.fade,
                      softWrap: false,
                    ),
                  ),
                  ListTile(
                    title: const Text(
                      'To:',
                      style:
                      TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
                    ),
                    subtitle: Text(
                      widget.elected,
                      style: const TextStyle(color: Colors.black),
                      overflow: TextOverflow.fade,
                      softWrap: false,
                    ),
                  ),
                  FutureBuilder<BlockHeight>(
                    future: blockks,
                    builder: (context, snapshot) {
                      if (snapshot.hasData) {
                        return ListTile(
                          title: const Text(
                            'Block:',
                            style: TextStyle(
                                color: Colors.black, fontWeight: FontWeight.bold),
                          ),
                          subtitle: Padding(
                            padding: const EdgeInsets.only(top: 8.0),
                            child: GestureDetector(
                              onTap: () => block(
                                snapshot.data!.block.miner,
                                snapshot.data!.block.transactions,
                                snapshot.data!.block.blockHeight,
                                snapshot.data!.block.time,
                                snapshot.data!.block.merkleRoot,
                                snapshot.data!.block.hash,
                                snapshot.data!.block.prevHash,
                              ),
                              child: Text(
                                widget.block.toString(),
                                style: const TextStyle(color: Colors.blue),
                                overflow: TextOverflow.fade,
                                softWrap: false,
                              ),
                            ),
                          ),
                        );
                      } else if (snapshot.hasError) {
                        return Text(snapshot.error.toString());
                      } // spinner
                      return ListTile(
                        title: const Text(
                          'Block:',
                          style: TextStyle(
                              color: Colors.black, fontWeight: FontWeight.bold),
                        ),
                        subtitle: Padding(
                          padding: const EdgeInsets.only(top: 8.0),
                          child: Text(
                            widget.block.toString(),
                            style: const TextStyle(color: Colors.black),
                            overflow: TextOverflow.fade,
                            softWrap: false,
                          ),
                        ),
                      );
                    },
                  ),
                ],
              ),
            ),
          ),
          Container(
            width: MediaQuery.of(context).size.width/3.5,
            child: Column(
              children: [
                Container(
                    height: 300,
                    width: 300,
                    child: Image.asset('assets/a4.png',
                        color: const Color(0xff26375f))),
              ],
            ),
          )
        ],
      ):
      ListView(
        children: [
          Padding(
            padding: const EdgeInsets.only(top: 8.0),
            child: ListTile(
              title: const Text(
                'Transaction Hash:',
                style:
                    TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
              ),
              subtitle: Padding(
                padding: const EdgeInsets.only(top: 8.0, bottom: 8.0),
                child: Text(
                  widget.hash,
                  style: const TextStyle(color: Colors.black),
                ),
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
                DateTime.fromMillisecondsSinceEpoch(widget.dateTime).toString(),
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
              'From:',
              style:
                  TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
            ),
            subtitle: Text(
              widget.elector,
              style: const TextStyle(color: Colors.black),
              overflow: TextOverflow.fade,
              softWrap: false,
            ),
          ),
          ListTile(
            title: const Text(
              'To:',
              style:
                  TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
            ),
            subtitle: Text(
              widget.elected,
              style: const TextStyle(color: Colors.black),
              overflow: TextOverflow.fade,
              softWrap: false,
            ),
          ),
          const Divider(
            height: 2,
            color: Colors.grey,
          ),
          FutureBuilder<BlockHeight>(
            future: blockks,
            builder: (context, snapshot) {
              if (snapshot.hasData) {
                return ListTile(
                  title: const Text(
                    'Block:',
                    style: TextStyle(
                        color: Colors.black, fontWeight: FontWeight.bold),
                  ),
                  subtitle: Padding(
                    padding: const EdgeInsets.only(top: 8.0),
                    child: InkWell(
                      onTap: () => block(
                        snapshot.data!.block.miner,
                        snapshot.data!.block.transactions,
                        snapshot.data!.block.blockHeight,
                        snapshot.data!.block.time,
                        snapshot.data!.block.merkleRoot,
                        snapshot.data!.block.hash,
                        snapshot.data!.block.prevHash,
                      ),
                      child: Text(
                        widget.block.toString(),
                        style: const TextStyle(color: Colors.blue),
                        overflow: TextOverflow.fade,
                        softWrap: false,
                      ),
                    ),
                  ),
                );
              } else if (snapshot.hasError) {
                return Text(snapshot.error.toString());
              } // spinner
              return ListTile(
                title: const Text(
                  'Block:',
                  style: TextStyle(
                      color: Colors.black, fontWeight: FontWeight.bold),
                ),
                subtitle: Padding(
                  padding: const EdgeInsets.only(top: 8.0),
                  child: Text(
                    widget.block.toString(),
                    style: const TextStyle(color: Colors.black),
                    overflow: TextOverflow.fade,
                    softWrap: false,
                  ),
                ),
              );
            },
          ),
        ],
      ),
    );
  }
}
