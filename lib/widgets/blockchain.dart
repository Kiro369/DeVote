import 'package:flutter/material.dart';

import '../models/transaction.dart';
import 'TransactionDetails.dart';

class BlockChain extends StatefulWidget {
  const BlockChain({Key? key}) : super(key: key);

  @override
  _BlockChainState createState() => _BlockChainState();
}

class _BlockChainState extends State<BlockChain> {
  List<Container> explorer = [];
  List<Transaction> transactions = [
    Transaction('c401d858d2', '22 hrs 46 mins ago',
        '0x25dc3a4eargggrgedvjlxngluoxc2', 'C.c'),
    Transaction('0x8c401d858d2', '2 hrs 46 mins ago',
        '0ecsgrfdgsergrgfjilzxdglsuixgh9r2', 'C.c'),
    Transaction('c401d858d2', '22 hrs 46 mins ago',
        '0x25dc3a4eargggrgedvjlxngluoxc2', 'C.c'),
    Transaction('0x8c401d858d2', '2 hrs 46 mins ago',
        '0ecsgrfdgsergrgfjilzxdglsuixgh9r2', 'C.c'),
    Transaction('c401d858d2', '22 hrs 46 mins ago',
        '0x25dc3a4eargggrgedvjlxngluoxc2', 'C.c'),
    Transaction('0x8c401d858d2', '2 hrs 46 mins ago',
        '0ecsgrfdgsergrgfjilzxdglsuixgh9r2', 'C.c'),
  ];

  _blockchain() {
    for (var i = 0; i < transactions.length; i++) {
      final transcation = transactions[i];
      final hash = transcation.hash;
      final date = transcation.dateTime;
      final elector = transcation.elector;
      final elected = transcation.elected;

      explorer.add(Container(
        child: Padding(
          padding: const EdgeInsets.all(4.0),
          child: Container(
            decoration: BoxDecoration(
                image: DecorationImage(
                  image: AssetImage("assets/a1.png"),
                  fit: BoxFit.cover,
                ),
                color: Colors.red[700],
                borderRadius: BorderRadius.all(Radius.circular(15.0))),
            child: Padding(
              padding: const EdgeInsets.all(5.0),
              child: InkWell(
                onTap: () => Navigator.of(context).push(new MaterialPageRoute(
                  builder: (BuildContext context) => new TransactionDetails(
                    elector: elector,
                    elected: elected,
                    hash: hash,
                    dateTime: date,
                  ),
                )),
                child: Column(
                  children: [
                    ListTile(
                      title: Text(hash, style: TextStyle(color: Colors.white)),
                      subtitle:
                          Text('Hash', style: TextStyle(color: Colors.white54)),
                    ),
                    Align(
                        alignment: Alignment.centerLeft,
                        child: Padding(
                          padding: const EdgeInsets.all(8.0),
                          child: Text('From: $elector',
                              textAlign: TextAlign.end,
                              style: TextStyle(color: Colors.white)),
                        )),
                    Align(
                        alignment: Alignment.centerLeft,
                        child: Padding(
                          padding: const EdgeInsets.all(8.0),
                          child: Text(
                            'To: $elected',
                            textAlign: TextAlign.start,
                            style: TextStyle(color: Colors.white),
                          ),
                        )),
                    Align(
                      child: Padding(
                        padding: const EdgeInsets.all(8.0),
                        child: Text(date,
                            style: TextStyle(
                              color: Colors.white,
                            )),
                      ),
                      alignment: Alignment.bottomRight,
                    )
                  ],
                ),
              ),
            ),
          ),
        ),
      ));
    }
  }

  @override
  void initState() {
    _blockchain();
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
          actions: [
            Padding(
              padding: const EdgeInsets.all(8.0),
              child: Icon(Icons.how_to_vote_rounded, size: 30, color: Colors.white),
            ),
          ],
          leading: Icon(
            Icons.arrow_back_ios_outlined,
            size: 0,

          ),
          centerTitle: true,
          backgroundColor: Colors.red[700],
          title: Column(children: [
            Text(
              "متابعة العملية الانتخابية",
            ),
            GestureDetector(
              child: Text(
                'BlockChain Explorer',
                style: TextStyle(fontSize: 12, color: Colors.white54),
              ),
            )
          ]),
        ),
        body: ListView(
          children: [
            ClipRect(
              child: Banner(
                message:'شرح العملية' ,
              location: BannerLocation.topEnd,
          color: Colors.red,
          child: Container(
          child: Padding(
          padding: const EdgeInsets.all(4.0),
      child: Container(
          decoration: BoxDecoration(
                image: DecorationImage(
                  image: AssetImage("assets/a2.png"),
                  fit: BoxFit.cover,
                ),
                color: Colors.yellow[700],
                borderRadius: BorderRadius.all(Radius.circular(15.0))),
          child: Padding(
              padding: const EdgeInsets.all(5.0),
              child: Column(
                  children: [
                    ListTile(
                    
                      title: Text('رقم العملية', style: TextStyle(color: Colors.black)),
                      subtitle:
                      Text('Hash', style: TextStyle(color: Colors.black45)),
                    ),
                    Align(
                        alignment: Alignment.centerLeft,
                        child: Padding(
                          padding: const EdgeInsets.all(8.0),
                          child: Text('From: الناخب',
                              textAlign: TextAlign.end,
                              style: TextStyle(color: Colors.black)),
                        )),
                    Align(
                        alignment: Alignment.centerLeft,
                        child: Padding(
                          padding: const EdgeInsets.all(8.0),
                          child: Text(
                            'To: الرئيس المنتخب',
                            textAlign: TextAlign.start,
                            style: TextStyle(color: Colors.black),
                          ),
                        )),
                    Align(
                      child: Padding(
                        padding: const EdgeInsets.all(8.0),
                        child: Text('توقيت العملية',
                            style: TextStyle(
                              color: Colors.black45,
                            )),
                      ),
                      alignment: Alignment.bottomRight,
                    )
                  ],
                ),
              ),
          ),
      ),
    ),
        ),
            ),
            Column(children: explorer),
          ],
        ));
  }
}
