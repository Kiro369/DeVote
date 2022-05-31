

import 'package:devote/widgets/shimmerLoading.dart';

import '/widgets/BlockDetails.dart';
import '/widgets/BlockList.dart';
import '/widgets/TransactionsList.dart';
import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart' show kIsWeb;
import '../models/Call_api.dart';
import '../models/block.dart';
import '../models/transaction.dart';
import 'TransactionDetails.dart';
import 'package:timeago/timeago.dart' as timeago;

class BlockChain extends StatefulWidget {
    final ScrollController? scrollController;
    static String id ='blockChain';
   BlockChain( [this.scrollController]);

  @override
  _BlockChainState createState() => _BlockChainState();
}

class _BlockChainState extends State<BlockChain> {
  late Future<List<Result>> daata;
  late Future<List<Results>> blockks;
  late Future<Block> all_blocks;
  late Future<Transaction> all_transactions;
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
    all_blocks=block.pagination();
    all_transactions=network.transactionPagination();
    getlist();
  }

  void getlist() async {
    transactionList = await daata;
    blockList = await blockks;
  }

  static bool isLargeScreen(BuildContext context) {
    return MediaQuery.of(context).size.width > 1200;
  }


  @override
  Widget build(BuildContext context) {
    return Scaffold(
        backgroundColor: kIsWeb ? Colors.white : Colors.grey[50],
        appBar: kIsWeb&&isLargeScreen(context)
            ? null
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
                  color:  Color(0xff26375f),
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
          controller: kIsWeb&&isLargeScreen(context) ? widget.scrollController:ScrollController(),
          children: [
            kIsWeb&&isLargeScreen(context)?
            Padding(
              padding: const EdgeInsets.all(12.0),
              child: Column(children: [
                const Text(
                  "متابعة العملية الانتخابية",
                  style: TextStyle(color: Colors.black,fontSize: 18, fontWeight: FontWeight.bold),
                ),
                GestureDetector(
                  child: const Text(
                    'BlockChain Explorer',
                    style: TextStyle(fontSize: 13, color: Colors.black45),
                  ),
                )
              ]),
            ):const Text('',style:  TextStyle(fontSize: 0),),
            Padding(
              padding: const EdgeInsets.all(8.0),
              child: Container(
                height: MediaQuery.of(context).size.height / 2.3,
                decoration: BoxDecoration(
                  color: Colors.white54,
                  border: Border.all(color: Colors.grey, width: 0.5),
                  borderRadius: const BorderRadius.all(Radius.circular(20.0)),
                ),
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
                      child: FutureBuilder<Block>(
                        future: all_blocks,
                        builder: (context, snapshot) {
                          if (snapshot.hasData) {
                            return ScrollConfiguration(
                              behavior: ScrollConfiguration.of(context).copyWith(scrollbars: true),
                              child: ListView.builder(
                                  controller: ScrollController(),
                                itemCount: 8,
                                itemBuilder: (context, index) => InkWell(
                                  onTap: () =>  Navigator.of(context)
                                          .push( MaterialPageRoute(
                                          builder: (BuildContext context) =>
                                               BlockDetails(
                                            blockHeight:
                                            snapshot.data!.blocks[index].blockHeight,
                                            miner: snapshot.data!.blocks[index].miner,
                                            time: snapshot.data!.blocks[index].time,
                                            transactions:
                                            snapshot.data!.blocks[index].transactions,
                                            merkleRoot:
                                            snapshot.data!.blocks[index].merkleRoot,
                                                prevhash: snapshot.data!.blocks[index].prevHash,
                                                hash: snapshot.data!.blocks[index].hash,
                                          ),
                                        )),
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
                                            style: const TextStyle(
                                                color: Colors.black,
                                                fontSize: 13),
                                            children: <TextSpan>[
                                              TextSpan(
                                                  text: snapshot.data?.blocks[index].miner??
                                                      'error',
                                                  style: const TextStyle(
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
                                            text: snapshot.data?.blocks[index]
                                                .transactions
                                                .toString()??
                                                'error',
                                            style: const TextStyle(
                                                color: Colors.blue,
                                                fontSize: 13),
                                            children: const <TextSpan>[
                                               TextSpan(
                                                  text:
                                                      ' txns',
                                                  style: TextStyle(
                                                      color: Colors.blue,
                                                      fontSize: 13)),
                                            ],
                                          ),
                                        ),
                                        leading: SizedBox(
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
                                                child: const Center(
                                                    child: Text(
                                                  'Bk',
                                                  style: TextStyle(
                                                      fontSize: 16,
                                                      color: Colors.black),
                                                )),
                                              ),
                                              title: Text(
                                                snapshot.data?.blocks[index]
                                                    .blockHeight
                                                    .toString()??
                                                    'error',
                                                style:
                                                    const TextStyle(fontSize: 12),
                                                maxLines: 1,
                                                overflow:
                                                    TextOverflow.ellipsis,
                                                softWrap: false,
                                              ),
                                              subtitle: Text(
                                                timeago.format(DateTime.fromMillisecondsSinceEpoch(snapshot.data!.blocks[index].time)),
                                                style: const TextStyle(
                                                    color: Colors.black45,
                                                    fontSize: 10),
                                                maxLines: 1,
                                                overflow:
                                                    TextOverflow.ellipsis,
                                                softWrap: false,
                                              ),
                                            )),
                                      ),
                                      const Divider(
                                        height: 2,
                                        color: Colors.grey,
                                      ),
                                    ],
                                  ),
                                ),
                              ),
                            );
                          } else if (snapshot.hasError) {
                            return Center(
                              child: Padding(
                                padding: const EdgeInsets.all(30.0),
                                child: Image.asset(
                                  'assets/no_internet.gif',
                                  height: MediaQuery.of(context).size.height / 2.5,
                                  // width: 125.0,
                                ),
                              ),
                            );
                          } // spinner
                          return  Center(child:  ShimmerLo(),);
                        },
                      ),
                    ),
                    SizedBox(
                      width: MediaQuery.of(context).size.width,
                      child: Padding(
                        padding: const EdgeInsets.all(4.0),
                        // ignore: deprecated_member_use
                        child: FlatButton(
                          child: const Text(
                            'View all blocks',
                            style:
                                TextStyle(color: Colors.black, fontSize: 14),
                          ),
                          // ignore: unnecessary_null_comparison
                          onPressed: () =>blockList==null?null: Navigator.of(context).push(
                               MaterialPageRoute(
                                  builder: (BuildContext context) =>
                                       BlockList(blockList))),
                          textColor: Colors.white,
                          splashColor: Colors.white,
                          hoverColor: Colors.white,
                        ),
                      ),
                    ),
                  ],
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
                  borderRadius: const BorderRadius.all( Radius.circular(20.0)),
                ),
                child: Column(
                  children: [
                    const ListTile(
                      title: Text(
                        'Latest Transactions',
                        style: TextStyle(
                            fontSize: 15, fontWeight: FontWeight.bold),
                      ),
                    ),
                    const Divider(
                      height: 2,
                      color: Colors.grey,
                    ),
                    Expanded(
                      child: FutureBuilder<Transaction>(
                        future: all_transactions,
                        builder: (context, snapshot) {
                          if (snapshot.hasData) {
                            return ScrollConfiguration(
                              behavior: ScrollConfiguration.of(context).copyWith(scrollbars: true),
                              child: ListView.builder(
                                controller: ScrollController(),
                                itemCount: 8,
                                itemBuilder: (context, index) => InkWell(
                                  onTap: () =>  Navigator.of(context)
                                          .push( MaterialPageRoute(
                                          builder: (BuildContext context) =>
                                               TransactionDetails(
                                            elector:
                                                snapshot.data!.transaction[index].elector,
                                            elected:
                                                snapshot.data!.transaction[index].elected,
                                            hash: snapshot.data!.transaction[index].hash,
                                            block: snapshot
                                                .data!.transaction[index].blockheight,
                                            dateTime:
                                                snapshot.data!.transaction[index].dateTime,
                                          ),
                                        )),
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
                                            style: const TextStyle(
                                                color: Colors.black,
                                                fontSize: 13),
                                            children: <TextSpan>[
                                              TextSpan(
                                                  text: snapshot
                                                          .data?.transaction[index]
                                                          .elector ??
                                                      'error',
                                                  style: const TextStyle(
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
                                            style: const TextStyle(
                                                color: Colors.black,
                                                fontSize: 13),
                                            children: <TextSpan>[
                                              TextSpan(
                                                  text: snapshot
                                                          .data?.transaction[index]
                                                          .elected ??
                                                      'error',
                                                  style: const TextStyle(
                                                      color: Colors.blue,
                                                      fontSize: 13)),
                                            ],
                                          ),
                                        ),
                                        leading: SizedBox(
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
                                                child: const Center(
                                                    child: Text(
                                                  'Tx',
                                                  style: TextStyle(
                                                      fontSize: 16,
                                                      color: Colors.black),
                                                )),
                                              ),
                                              title: Text(
                                                snapshot.data?.transaction[index]
                                                        .hash ??
                                                    'error',
                                                style:
                                                    const TextStyle(fontSize: 12),
                                                maxLines: 1,
                                                overflow:
                                                    TextOverflow.ellipsis,
                                                softWrap: false,
                                              ),
                                              subtitle: Text(
                                                timeago.format( DateTime.fromMillisecondsSinceEpoch(snapshot.data!.transaction[index].dateTime))
                                                  ,
                                                style:const  TextStyle(
                                                    color: Colors.black45,
                                                    fontSize: 10),
                                                maxLines: 1,
                                                overflow:
                                                    TextOverflow.ellipsis,
                                                softWrap: false,
                                              ),
                                            )),
                                      ),
                                      const Divider(
                                        height: 2,
                                        color: Colors.grey,
                                      ),
                                    ],
                                  ),
                                ),
                              ),
                            );
                          } else if (snapshot.hasError) {
                            return Center(
                              child: Padding(
                                padding: const EdgeInsets.all(30.0),
                                child: Image.asset(
                                  'assets/no_internet.gif',
                                  height: MediaQuery.of(context).size.height / 2.5,
                                  // width: 125.0,
                                ),
                              ),
                            );
                          } // spinner
                          return Center(child:  ShimmerLo(),);
                        },
                      ),
                    ),
                    SizedBox(
                      width: MediaQuery.of(context).size.width,
                      // ignore: deprecated_member_use
                      child: FlatButton(
                        child: const Text(
                          'View all transactions',
                          style: TextStyle(color: Colors.black, fontSize: 14),
                        ),
                        // ignore: unnecessary_null_comparison
                        onPressed: () => transactionList==null?null:Navigator.of(context).push(
                            MaterialPageRoute(
                                builder: (BuildContext context) =>
                                     TransactionsList(transactionList))),
                        textColor: Colors.white,
                        splashColor: Colors.white,
                        hoverColor:  Colors.white,
                      ),
                    ),
                  ],
                ),
              ),
            )
          ],
        ));
  }
}
