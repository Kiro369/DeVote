import 'package:devote/models/Ip.dart';
import 'package:devote/widgets/shimmerLoading.dart';

import '../models/call_api.dart';
import '/models/transaction.dart';
import 'package:flutter/material.dart';

import 'TransactionDetails.dart';
import 'package:timeago/timeago.dart' as timeago;

class TransactionsList extends StatefulWidget {
  final List transactions;
  final bool more;

  TransactionsList(this.transactions,[this.more=true]);

  @override
  _TransactionsListState createState() => _TransactionsListState();
}

class _TransactionsListState extends State<TransactionsList> {
  List _foundsearch = [];
  late Future<List<Result>> trans;
  late List tansactionList;
  late Future<Transaction> all;

  callApi transactions = ip().network;

  Future<void> getlist() async {
    tansactionList = await trans;
  }

  void showmore(String prev) async {
    Uri myUri = Uri.parse(prev);
    Map<String, String> queryParameters = myUri.queryParameters;
    transactions = callApi(Uri.https(ip().authority,
        ip().unencodedpath, queryParameters));
    trans = transactions.getTransaction();
    all = transactions.transactionPagination();
    await getlist();
    setState(() {
      _foundsearch = tansactionList;
    });
  }

  void _runFilter(String enteredKeyword) {
    List results = [];
    if (enteredKeyword.isEmpty) {
      // if the search field is empty or only contains white-space, we'll display all users

      results = widget.transactions;
    } else {
      results = widget.transactions
          .where((user) =>
              user.hash.toLowerCase().contains(enteredKeyword.toLowerCase()))
          .toList();
      // we use the toLowerCase() method to make it case-insensitive
    }

    // Refresh the UI
    setState(() {
      _foundsearch = results;
    });
  }

  @override
  void initState() {
    trans = transactions.getTransaction();
    tansactionList = widget.transactions;
    all = transactions.transactionPagination();
    getlist();
    _foundsearch = tansactionList;
    super.initState();
  }



  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        centerTitle: true,
        backgroundColor:const Color(0xff26375f),
        title: const Text('Latest Transactions'),
        leading: const Icon(
          Icons.arrow_back_ios,
          size: 0,
          color: Color(0xff26375f),
        ),
      ),
      body: Padding(
        padding: const EdgeInsets.all(4.0),
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.all(8.0),
              child: TextField(
                onChanged: (value) => _runFilter(value),
                decoration: const InputDecoration(
                    hoverColor: Colors.black,
                    fillColor: Colors.black,
                    focusColor: Colors.black,
                    labelText: 'Search by Hash',
                    suffixIcon: Icon(Icons.search)),
              ),
            ),
            Expanded(
              child: _foundsearch.isNotEmpty
                  ? Padding(
                      padding: const EdgeInsets.all(15.0),
                      child: FutureBuilder<Transaction>(
                        key: UniqueKey(),
                        future: all,
                        builder: (context, snapshot) {
                          if (snapshot.hasData) {
                            return SingleChildScrollView(
                              physics: const ScrollPhysics(),
                              child: Column(
                                children: [
                                  ListView.builder(
                                    physics: const NeverScrollableScrollPhysics(),
                                    shrinkWrap: true,
                                    itemCount: _foundsearch.length,
                                    itemBuilder: (context, index) => InkWell(
                                      onTap: () => Navigator.of(context)
                                              .push( MaterialPageRoute(
                                              builder:
                                                  (BuildContext context) =>
                                                       TransactionDetails(
                                                block: _foundsearch[index]
                                                    .blockheight,
                                                elector: _foundsearch[index]
                                                    .elector,
                                                elected: _foundsearch[index]
                                                    .elected,
                                                hash:
                                                    _foundsearch[index].hash,
                                                dateTime: _foundsearch[index]
                                                    .dateTime,
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
                                                      text: _foundsearch[
                                                              index]
                                                          .elector,
                                                      style: const TextStyle(
                                                          color:
                                                              Colors.blue,
                                                          fontSize: 13)),
                                                ],
                                              ),
                                            ),
                                            subtitle: RichText(
                                              maxLines: 1,
                                              overflow:
                                                  TextOverflow.ellipsis,
                                              softWrap: false,
                                              text: TextSpan(
                                                text: 'To ',
                                                style:const TextStyle(
                                                    color: Colors.black,
                                                    fontSize: 13),
                                                children: <TextSpan>[
                                                  TextSpan(
                                                      text: _foundsearch[
                                                              index]
                                                          .elected,
                                                      style: const TextStyle(
                                                          color:
                                                              Colors.blue,
                                                          fontSize: 13)),
                                                ],
                                              ),
                                            ),
                                            leading: SizedBox(
                                                width:
                                                    MediaQuery.of(context)
                                                            .size
                                                            .width /
                                                        2.4,
                                                child: ListTile(
                                                  leading: CircleAvatar(
                                                    backgroundColor:
                                                        Colors.grey[300],
                                                    child:const Center(
                                                        child: Text(
                                                      'Tx',
                                                      style: TextStyle(
                                                          fontSize: 16,
                                                          color:
                                                              Colors.black),
                                                    )),
                                                  ),
                                                  title: Text(
                                                    _foundsearch[index]
                                                        .hash,
                                                    style: const TextStyle(
                                                        fontSize: 12),
                                                    maxLines: 1,
                                                    overflow: TextOverflow
                                                        .ellipsis,
                                                    softWrap: false,
                                                  ),
                                                  subtitle: Text(
                                                    timeago.format(DateTime
                                                        .fromMillisecondsSinceEpoch(
                                                            _foundsearch[
                                                                    index]
                                                                .dateTime)),
                                                    style: const TextStyle(
                                                        color:
                                                            Colors.black45,
                                                        fontSize: 10),
                                                    maxLines: 1,
                                                    overflow: TextOverflow
                                                        .ellipsis,
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
                                  widget.more ==true?Row(
                                    mainAxisAlignment:
                                        MainAxisAlignment.spaceBetween,
                                    children: [
                                      Padding(
                                        padding: const EdgeInsets.all(12.0),
                                        // ignore: deprecated_member_use
                                        child: TextButton.icon(
                                            onPressed: () => snapshot.data!
                                                        .pagination.prev ==
                                                    null
                                                ? null
                                                : showmore(snapshot
                                                    .data!.pagination.prev),

                                            label: const Text(
                                              'Back',
                                              style: TextStyle(
                                                  color: Colors.white,
                                                  fontSize: 16),
                                            ),
                                            icon: const Padding(
                                              padding: EdgeInsets.all(8.0),
                                              child: Icon(
                                                Icons.arrow_back_ios_rounded,
                                                color: Colors.white,
                                                size: 15,
                                              ),
                                            ),
                                           ),
                                      ),
                                      Padding(
                                        padding: const EdgeInsets.all(12.0),
                                        // ignore: deprecated_member_use
                                        child: TextButton.icon(
                                            onPressed: () => snapshot.data!
                                                        .pagination.next ==
                                                    null
                                                ? null
                                                : showmore(snapshot
                                                    .data!.pagination.next),
                                            label: const Padding(
                                              padding: EdgeInsets.all(8.0),
                                              child: Icon(
                                                Icons.navigate_next_rounded,
                                                size: 25,
                                                color: Colors.white,
                                              ),
                                            ),
                                            icon: const Text(
                                              'Next',
                                              style: TextStyle(
                                                  color: Colors.white,
                                                  fontSize: 16),
                                            ),
                                        ),
                                      ),
                                    ],
                                  ):const Text(''),
                                ],
                              ),
                            );
                          } else if (snapshot.hasError) {
                            return Text("${snapshot.error}");
                          } // spinner
                          return const Center(child:  shimmerLo(),);
                        },
                      ),
                    )
                  : const Center(
                      child: Text(
                        'No results found',
                        style: TextStyle(fontSize: 20),
                      ),
                    ),
            ),
          ],
        ),
      ),
    );
  }
}
