import 'package:devote/widgets/BlockDetails.dart';
import 'package:devote/widgets/BlockList.dart';
import 'package:devote/widgets/TransactionsList.dart';
import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart' show kIsWeb;
import '../models/Call_api.dart';
import '../models/block.dart';
import '../models/transaction.dart';
import 'TransactionDetails.dart';
import 'package:timeago/timeago.dart' as timeago;

class BlockChain extends StatefulWidget {
  const BlockChain({Key? key}) : super(key: key);

  @override
  _BlockChainState createState() => _BlockChainState();
}

class _BlockChainState extends State<BlockChain> {
  late Future<List<Result>> daata;
  late Future<List<Results>> blockks;
  late List transactionList;
  late List blockList;

  @override
  initState() {
    super.initState();
    CallApi network = CallApi(
        Uri.https('devote-explorer-backend.herokuapp.com', 'transactions'));
    CallApi block = CallApi(
        Uri.https('devote-explorer-backend.herokuapp.com', 'blocks'));
    daata = network.getTransaction();
    blockks = block.getBlocks();
    getlist();
  }

  void getlist() async {
    transactionList = await daata;
    blockList = await blockks;
  }


  _buildChild(BuildContext context, String miner, int transactions,
          int blockHeight, int time,String merkleRoot,String hash,String prevHash) =>
      Container(
          height: MediaQuery.of(context).size.height / 1.3,
          width: MediaQuery.of(context).size.width / 2.8,
          decoration: const BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.all(Radius.circular(20)),
          ),
          child: BlockDetails(
            hash: hash,
            merkleRoot: merkleRoot,
            prevhash: prevHash,
            miner: miner,
            transactions: transactions,
            time: time,
            blockHeight: blockHeight,
          ));

  _buildtransaction(BuildContext context,int date, String elector, String hash,
          String elected, int block) =>
      Container(
          height: MediaQuery.of(context).size.height / 1.15,
          width: MediaQuery.of(context).size.width / 2.8,
          decoration: const BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.all(Radius.circular(20)),
          ),
          child: TransactionDetails(
            dateTime: date,
            hash: hash,
            elected: elected,
            elector: elector,
            block: block,
          ));

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        backgroundColor: kIsWeb ? Colors.white : Colors.grey[50],
        appBar: kIsWeb
            ? AppBar(
                leading: const Icon(Icons.arrow_back, color: Colors.white),
                centerTitle: true,
                elevation: 0,
                backgroundColor: Colors.white,
                title: Column(children: [
                  const Text(
                    "متابعة العملية الانتخابية",
                    style: TextStyle(color: Colors.black),
                  ),
                  GestureDetector(
                    child: const Text(
                      'BlockChain Explorer',
                      style: TextStyle(fontSize: 12, color: Colors.black45),
                    ),
                  )
                ]),
              )
            : AppBar(
                actions: const [
                  Padding(
                    padding: EdgeInsets.all(8.0),
                    child: Icon(Icons.how_to_vote_rounded,
                        size: 30, color: Colors.white),
                  ),
                ],
                leading: const Icon(
                  Icons.arrow_back_ios_outlined,
                  size: 0,
                ),
                centerTitle: true,
                backgroundColor: const Color(0xff26375f),
                title: Column(children: [
                  const Text(
                    "متابعة العملية الانتخابية",
                  ),
                  GestureDetector(
                    child: const Text(
                      'BlockChain Explorer',
                      style: TextStyle(fontSize: 12, color: Colors.white54),
                    ),
                  )
                ]),
              ),
        body: ListView(
          children: [
            Padding(
              padding: const EdgeInsets.all(8.0),
              child: Container(
                height: MediaQuery.of(context).size.height / 2.3,
                decoration: BoxDecoration(
                  color: Colors.white54,
                  border: Border.all(color: Colors.grey, width: 0.5),
                  borderRadius: const BorderRadius.all(Radius.circular(20.0)),
                ),
                child: Expanded(
                  child: Column(
                    children: [
                      const ListTile(
                        title: Text(
                          'Latest Blocks',
                          style: TextStyle(
                              fontSize: 15, fontWeight: FontWeight.bold),
                        ),
                      ),
                      const Divider(
                        height: 2,
                        color: Colors.grey,
                      ),
                      Expanded(
                        child: FutureBuilder<List<Results>>(
                          future: blockks,
                          builder: (context, snapshot) {
                            if (snapshot.hasData) {
                              return ListView.builder(
                                itemCount: 8,
                                itemBuilder: (context, index) => Container(
                                  child: InkWell(
                                    onTap: () => kIsWeb
                                        ? showDialog(
                                            context: context,
                                            builder: (context) {
                                              return Dialog(
                                                elevation: 1,
                                                backgroundColor:
                                                    Colors.transparent,
                                                child: _buildChild(
                                                  context,
                                                  snapshot.data![index].miner,
                                                  snapshot.data![index].transactions,
                                                  snapshot.data![index].blockHeight,
                                                  snapshot.data![index].time,
                                                  snapshot.data![index].merkleRoot,
                                                  snapshot.data![index].hash,
                                                  snapshot.data![index].prevHash,
                                                ),
                                              );
                                            })
                                        : Navigator.of(context)
                                            .push( MaterialPageRoute(
                                            builder: (BuildContext context) =>
                                                 BlockDetails(
                                              blockHeight:
                                              snapshot.data![index].blockHeight,
                                              miner: snapshot.data![index].miner,
                                              time: snapshot.data![index].time,
                                              transactions:
                                              snapshot.data![index].transactions,
                                              merkleRoot:
                                              snapshot.data![index].merkleRoot,
                                                  prevhash: snapshot.data![index].prevHash,
                                                  hash: snapshot.data![index].hash,
                                            ),
                                          )),
                                    child: Expanded(
                                      child: Column(
                                        children: [
                                          ListTile(
                                            horizontalTitleGap: 3,
                                            title: RichText(
                                              maxLines: 1,
                                              overflow: TextOverflow.fade,
                                              softWrap: false,
                                              text: TextSpan(
                                                text: 'Miner ',
                                                style: TextStyle(
                                                    color: Colors.black,
                                                    fontSize: 13),
                                                children: <TextSpan>[
                                                  TextSpan(
                                                      text: snapshot.data?[index].miner??
                                                          'error',
                                                      style: TextStyle(
                                                          color: Colors.blue,
                                                          fontSize: 13)),
                                                ],
                                              ),
                                            ),
                                            subtitle: RichText(
                                              maxLines: 1,
                                              overflow: TextOverflow.ellipsis,
                                              softWrap: false,
                                              text: TextSpan(
                                                text: snapshot.data?[index]
                                                    .transactions
                                                    .toString()??
                                                    'error',
                                                style: TextStyle(
                                                    color: Colors.blue,
                                                    fontSize: 13),
                                                children: <TextSpan>[
                                                  TextSpan(
                                                      text:
                                                          ' txns',
                                                      style: TextStyle(
                                                          color: Colors.blue,
                                                          fontSize: 13)),
                                                ],
                                              ),
                                            ),
                                            leading: Container(
                                                width: kIsWeb
                                                    ? MediaQuery.of(context)
                                                            .size
                                                            .width /
                                                        4
                                                    : MediaQuery.of(context)
                                                            .size
                                                            .width /
                                                        2.4,
                                                child: ListTile(
                                                  leading: CircleAvatar(
                                                    backgroundColor:
                                                        Colors.grey[300],
                                                    child: Center(
                                                        child: Text(
                                                      'Bk',
                                                      style: TextStyle(
                                                          fontSize: 16,
                                                          color: Colors.black),
                                                    )),
                                                  ),
                                                  title: Text(
                                                    snapshot.data?[index]
                                                        .blockHeight
                                                        .toString()??
                                                        'error',
                                                    style:
                                                        TextStyle(fontSize: 12),
                                                    maxLines: 1,
                                                    overflow:
                                                        TextOverflow.ellipsis,
                                                    softWrap: false,
                                                  ),
                                                  subtitle: Text(
                                                    timeago.format(DateTime.fromMillisecondsSinceEpoch(snapshot.data![index].time)),
                                                    style: TextStyle(
                                                        color: Colors.black45,
                                                        fontSize: 10),
                                                    maxLines: 1,
                                                    overflow:
                                                        TextOverflow.ellipsis,
                                                    softWrap: false,
                                                  ),
                                                )),
                                          ),
                                          Divider(
                                            height: 2,
                                            color: Colors.grey,
                                          ),
                                        ],
                                      ),
                                    ),
                                  ),
                                ),
                              );
                            } else if (snapshot.hasError) {
                              return Text("${snapshot.error}");
                            } // spinner
                            return Center(child: CircularProgressIndicator());
                          },
                        ),
                      ),
                      Container(
                        width: MediaQuery.of(context).size.width,
                        child: Padding(
                          padding: const EdgeInsets.all(4.0),
                          child: FlatButton(
                            child: Text(
                              'View all blocks',
                              style:
                                  TextStyle(color: Colors.black, fontSize: 14),
                            ),
                            onPressed: () => Navigator.of(context).push(
                                new MaterialPageRoute(
                                    builder: (BuildContext context) =>
                                        new BlockList(blockList))),
                            textColor: Colors.white,
                            splashColor: Color(0xff6ca0ff),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.all(8.0),
              child: Container(
                height: MediaQuery.of(context).size.height / 2.2,
                decoration: BoxDecoration(
                  color: Colors.white54,
                  border: Border.all(color: Colors.grey, width: 0.5),
                  borderRadius: BorderRadius.all(Radius.circular(20.0)),
                ),
                child: Expanded(
                  child: Column(
                    children: [
                      ListTile(
                        title: Text(
                          'Latest Transactions',
                          style: TextStyle(
                              fontSize: 15, fontWeight: FontWeight.bold),
                        ),
                      ),
                      Divider(
                        height: 2,
                        color: Colors.grey,
                      ),
                      Expanded(
                        child: FutureBuilder<List<Result>>(
                          future: daata,
                          builder: (context, snapshot) {
                            if (snapshot.hasData) {
                              return ListView.builder(
                                itemCount: 8,
                                itemBuilder: (context, index) => Container(
                                  child: InkWell(
                                    onTap: () => kIsWeb
                                        ? showDialog(
                                            context: context,
                                            builder: (context) {
                                              return Dialog(
                                                elevation: 1,
                                                backgroundColor:
                                                    Colors.transparent,
                                                child: _buildtransaction(
                                                  context,
                                                   snapshot.data![index].dateTime,
                                                  snapshot.data![index].elector,
                                                  snapshot.data![index].hash,
                                                  snapshot.data![index].elected,
                                                  snapshot
                                                      .data![index].blockheight,
                                                ),
                                              );
                                            })
                                        : Navigator.of(context)
                                            .push(new MaterialPageRoute(
                                            builder: (BuildContext context) =>
                                                new TransactionDetails(
                                              elector:
                                                  snapshot.data![index].elector,
                                              elected:
                                                  snapshot.data![index].elected,
                                              hash: snapshot.data![index].hash,
                                              block: snapshot
                                                  .data![index].blockheight,
                                              dateTime:
                                                  snapshot.data![index].dateTime,
                                            ),
                                          )),
                                    child: Expanded(
                                      child: Column(
                                        children: [
                                          ListTile(
                                            horizontalTitleGap: 3,
                                            title: RichText(
                                              maxLines: 1,
                                              overflow: TextOverflow.fade,
                                              softWrap: false,
                                              text: TextSpan(
                                                text: 'From ',
                                                style: TextStyle(
                                                    color: Colors.black,
                                                    fontSize: 13),
                                                children: <TextSpan>[
                                                  TextSpan(
                                                      text: snapshot
                                                              .data?[index]
                                                              .elector ??
                                                          'error',
                                                      style: TextStyle(
                                                          color: Colors.blue,
                                                          fontSize: 13)),
                                                ],
                                              ),
                                            ),
                                            subtitle: RichText(
                                              maxLines: 1,
                                              overflow: TextOverflow.ellipsis,
                                              softWrap: false,
                                              text: TextSpan(
                                                text: 'To ',
                                                style: TextStyle(
                                                    color: Colors.black,
                                                    fontSize: 13),
                                                children: <TextSpan>[
                                                  TextSpan(
                                                      text: snapshot
                                                              .data?[index]
                                                              .elected ??
                                                          'error',
                                                      style: TextStyle(
                                                          color: Colors.blue,
                                                          fontSize: 13)),
                                                ],
                                              ),
                                            ),
                                            leading: Container(
                                                width: kIsWeb
                                                    ? MediaQuery.of(context)
                                                            .size
                                                            .width /
                                                        4
                                                    : MediaQuery.of(context)
                                                            .size
                                                            .width /
                                                        2.4,
                                                child: ListTile(
                                                  leading: CircleAvatar(
                                                    backgroundColor:
                                                        Colors.grey[300],
                                                    child: Center(
                                                        child: Text(
                                                      'Tx',
                                                      style: TextStyle(
                                                          fontSize: 16,
                                                          color: Colors.black),
                                                    )),
                                                  ),
                                                  title: Text(
                                                    snapshot.data?[index]
                                                            .hash ??
                                                        'error',
                                                    style:
                                                        TextStyle(fontSize: 12),
                                                    maxLines: 1,
                                                    overflow:
                                                        TextOverflow.ellipsis,
                                                    softWrap: false,
                                                  ),
                                                  subtitle: Text(
                                                    timeago.format( DateTime.fromMillisecondsSinceEpoch(snapshot.data![index].dateTime))
                                                      ,
                                                    style: TextStyle(
                                                        color: Colors.black45,
                                                        fontSize: 10),
                                                    maxLines: 1,
                                                    overflow:
                                                        TextOverflow.ellipsis,
                                                    softWrap: false,
                                                  ),
                                                )),
                                          ),
                                          Divider(
                                            height: 2,
                                            color: Colors.grey,
                                          ),
                                        ],
                                      ),
                                    ),
                                  ),
                                ),
                              );
                            } else if (snapshot.hasError) {
                              return Text("${snapshot.error}");
                            } // spinner
                            return Center(child: CircularProgressIndicator());
                          },
                        ),
                      ),
                      Container(
                        width: MediaQuery.of(context).size.width,
                        child: FlatButton(
                          child: Text(
                            'View all transactions',
                            style: TextStyle(color: Colors.black, fontSize: 14),
                          ),
                          onPressed: () => Navigator.of(context).push(
                              new MaterialPageRoute(
                                  builder: (BuildContext context) =>
                                      new TransactionsList(transactionList))),
                          textColor: Colors.white,
                          splashColor: Color(0xff6ca0ff),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            )
          ],
        ));
  }
}
