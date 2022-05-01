import 'package:devote/widgets/BlockDetails.dart';
import 'package:devote/widgets/BlockList.dart';
import 'package:devote/widgets/TransactionsList.dart';
import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart' show kIsWeb;
import '../models/Call_api.dart';
import '../models/block.dart';
import '../models/transaction.dart';
import 'TransactionDetails.dart';


class BlockChain extends StatefulWidget {
  const BlockChain({Key? key}) : super(key: key);

  @override
  _BlockChainState createState() => _BlockChainState();
}

class _BlockChainState extends State<BlockChain> {
  late Future<Transaction> daata;

  @override
  initState() {
    super.initState();
    callApi network = callApi(Uri.https('devote-explorer-backend.herokuapp.com','transactions'));
    daata =  network.getTransaction();
  }


  List<Block> blocks = [
    Block(13124253, 678264, '22 hrs 46 mins ago', '2 min ago', 'skdoahodjk'),
    Block(1390344253, 22264, '3 hrs 46 mins ago', '4 min ago', 'aaafhodjk'),
    Block(24354253, 3333, '2 hrs 46 mins ago', '6 min ago', 'wdd;slkmpo'),
    Block(137887753, 4444, '5 hrs 46 mins ago', '8 min ago', 'sldjnkffjk'),
    Block(32422, 55555, '2 hrs 46 mins ago', '11 min ago', 'hayieh'),
    Block(543634345, 666666, '3 hrs 46 mins ago', '13 min ago', 'ldsfhhuopsgu'),
    Block(3453454534, 777777, '6 hrs 46 mins ago', '14 min ago', 'zsg;o;udh;i'),
    Block(43545345, 88888, '8 hrs 46 mins ago', '15 min ago', 'iosgherog'),
    Block(2323324, 99999, '7 hrs 46 mins ago', '16 min ago', 'lwdjknilau'),
    Block(8979878989, 456456545, '9 hrs 46 mins ago', '20 min ago',
        'jksefuklgwe;'),
    Block(76768768, 3453453, '10 hrs 46 mins ago', '22 min ago', 'nilefh9'),
    Block(80009, 6734344, '1 hrs 46 mins ago', '24 min ago', 'elfnp9eioq'),
  ];
  List<Result> transactions = [
    Result('e12234c401d858d2',
        '0x25dc3a4eargggrgedvjlxngluoxc2', 'C.c',11),
    Result('e12234c401d858d2',
        '0x25dc3a4eargggrgedvjlxngluoxc2', 'C.c',11,),

  ];

  _buildChild(BuildContext context, String miner, int transactions,
      int blockHeight, String time, String transactionTime) =>
      Container(
          height: MediaQuery
              .of(context)
              .size
              .height / 1.3,
          width: MediaQuery
              .of(context)
              .size
              .width / 2.8,
          decoration: const BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.all(Radius.circular(20)),
          ),
          child: BlockDetails(
            miner: miner,
            transactions: transactions,
            time: time,
            blockHeight: blockHeight,
            transactionTime: transactionTime,
          ));

  _buildtransaction(BuildContext context,  String elector,
      String hash, String elected,int block) =>
      Container(
          height: MediaQuery
              .of(context)
              .size
              .height / 1.15,
          width: MediaQuery
              .of(context)
              .size
              .width / 2.8,
          decoration: const BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.all(Radius.circular(20)),
          ),
          child: TransactionDetails(
            dateTime: 'dateTime',
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
                height: MediaQuery
                    .of(context)
                    .size
                    .height / 2.3,
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
                          'Latest Blocks',
                          style: TextStyle(
                              fontSize: 15, fontWeight: FontWeight.bold),
                        ),
                      ),
                      Divider(
                        height: 2,
                        color: Colors.grey,
                      ),
                      Expanded(
                        child: ListView.builder(
                          itemCount: 8,
                          itemBuilder: (context, index) =>
                              Container(
                                child: InkWell(
                                  onTap: () =>
                                  kIsWeb
                                      ? showDialog(
                                      context: context,
                                      builder: (context) {
                                        return Dialog(
                                          elevation: 1,
                                          backgroundColor: Colors.transparent,
                                          child: _buildChild(
                                            context,
                                            blocks[index].miner,
                                            blocks[index].transactions,
                                            blocks[index].blockHeight,
                                            blocks[index].time,
                                            blocks[index].transactionTime,
                                          ),
                                        );
                                      })
                                      : Navigator.of(context)
                                      .push(new MaterialPageRoute(
                                    builder: (BuildContext context) =>
                                    new BlockDetails(
                                      blockHeight: blocks[index].blockHeight,
                                      miner: blocks[index].miner,
                                      time: blocks[index].time,
                                      transactions:
                                      blocks[index].transactions,
                                      transactionTime:
                                      blocks[index].transactionTime,
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
                                                    text: blocks[index].miner,
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
                                              text: blocks[index]
                                                  .transactions
                                                  .toString(),
                                              style: TextStyle(
                                                  color: Colors.blue,
                                                  fontSize: 13),
                                              children: <TextSpan>[
                                                TextSpan(
                                                    text:
                                                    ' in ${blocks[index]
                                                        .transactionTime}',
                                                    style: TextStyle(
                                                        color: Colors.black45,
                                                        fontSize: 13)),
                                              ],
                                            ),
                                          ),
                                          leading: Container(
                                              width: kIsWeb
                                                  ? MediaQuery
                                                  .of(context)
                                                  .size
                                                  .width /
                                                  4
                                                  : MediaQuery
                                                  .of(context)
                                                  .size
                                                  .width /
                                                  2.4,
                                              child: ListTile(
                                                leading: CircleAvatar(
                                                  backgroundColor: Colors
                                                      .grey[300],
                                                  child: Center(
                                                      child: Text(
                                                        'Bk',
                                                        style: TextStyle(
                                                            fontSize: 16,
                                                            color: Colors
                                                                .black),
                                                      )),
                                                ),
                                                title: Text(
                                                  blocks[index]
                                                      .blockHeight
                                                      .toString(),
                                                  style: TextStyle(
                                                      fontSize: 12),
                                                  maxLines: 1,
                                                  overflow: TextOverflow
                                                      .ellipsis,
                                                  softWrap: false,
                                                ),
                                                subtitle: Text(
                                                  blocks[index].time,
                                                  style: TextStyle(
                                                      color: Colors.black45,
                                                      fontSize: 10),
                                                  maxLines: 1,
                                                  overflow: TextOverflow
                                                      .ellipsis,
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
                        ),
                      ),
                      Container(
                        width: MediaQuery
                            .of(context)
                            .size
                            .width,
                        child: Padding(
                          padding: const EdgeInsets.all(4.0),
                          child: FlatButton(
                            child: Text(
                              'View all blocks',
                              style:
                              TextStyle(color: Colors.black, fontSize: 14),
                            ),
                            onPressed: () =>
                                Navigator.of(context).push(
                                    new MaterialPageRoute(
                                        builder: (BuildContext context) =>
                                        new BlockList(blocks))),
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
                height: MediaQuery
                    .of(context)
                    .size
                    .height / 2.2,
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
                          child:
                          FutureBuilder<Transaction>(
                            future: daata,
                            builder: (context, snapshot) {
                              if (snapshot.hasData) {
                                return ListView.builder(
                                  itemCount: 8,
                                  itemBuilder: (context, index) =>  Container(
                                    child: InkWell(
                                      onTap: () =>
                                      kIsWeb
                                          ? showDialog(
                                          context: context,
                                          builder: (context) {
                                            return Dialog(
                                              elevation: 1,
                                              backgroundColor: Colors.transparent,
                                              child: _buildtransaction(
                                                context,
                                               // snapshot.data!.transaction[index].dateTime,
                                                snapshot.data!.transaction[index].elector,
                                                snapshot.data!.transaction[index].hash,
                                                snapshot.data!.transaction[index].elected,
                                                snapshot.data!.transaction[index].blockheight,
                                              ),
                                            );
                                          })
                                          : Navigator.of(context)
                                          .push(new MaterialPageRoute(
                                        builder: (BuildContext context) =>
                                        new TransactionDetails(
                                          elector: snapshot.data!.transaction[index].elector,
                                          elected: snapshot.data!.transaction[index].elected,
                                          hash: snapshot.data!.transaction[index].hash,
                                          block: snapshot.data!.transaction[index].blockheight,
                                          dateTime: 'snapshot.data!.transaction[0].dateTime',
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
                                                        text:
                                                        snapshot.data?.transaction[index].elector??'error',
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
                                                        text:
                                                        snapshot.data?.transaction[index].elected??'error',
                                                        style: TextStyle(
                                                            color: Colors.blue,
                                                            fontSize: 13)),
                                                  ],
                                                ),
                                              ),
                                              leading: Container(
                                                  width: kIsWeb
                                                      ? MediaQuery
                                                      .of(context)
                                                      .size
                                                      .width /
                                                      4
                                                      : MediaQuery
                                                      .of(context)
                                                      .size
                                                      .width /
                                                      2.4,
                                                  child: ListTile(
                                                    leading: CircleAvatar(
                                                      backgroundColor: Colors
                                                          .grey[300],
                                                      child: Center(
                                                          child: Text(
                                                            'Tx',
                                                            style: TextStyle(
                                                                fontSize: 16,
                                                                color: Colors
                                                                    .black),
                                                          )),
                                                    ),
                                                    title: Text(
                                                      snapshot.data?.transaction[index].hash??'error',
                                                      style: TextStyle(
                                                          fontSize: 12),
                                                      maxLines: 1,
                                                      overflow: TextOverflow
                                                          .ellipsis,
                                                      softWrap: false,
                                                    ),
                                                    subtitle: Text(
                                                      'error',
                                                      style: TextStyle(
                                                          color: Colors.black45,
                                                          fontSize: 10),
                                                      maxLines: 1,
                                                      overflow: TextOverflow
                                                          .ellipsis,
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
                              } else if (snapshot.hasError) {return Text("${snapshot.error}");
                              } // spinner
                              return Center(child: CircularProgressIndicator());
                            },
                          ),
                      ),
                      Container(
                        width: MediaQuery
                            .of(context)
                            .size
                            .width,
                        child: FlatButton(
                          child: Text(
                            'View all transactions',
                            style: TextStyle(color: Colors.black, fontSize: 14),
                          ),
                          onPressed: () =>
                              Navigator.of(context).push(
                                  new MaterialPageRoute(
                                      builder: (BuildContext context) =>
                                      new TransactionsList(transactions))),
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
