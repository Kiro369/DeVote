import 'package:flutter/material.dart';

import '../models/transaction.dart';
import 'TransactionDetails.dart';
class TransactionsList extends StatefulWidget {
  final List transactions;

   TransactionsList( this.transactions);
@override
_TransactionsListState createState() => _TransactionsListState(transactions);
}

class _TransactionsListState extends State<TransactionsList> {

  List _foundsearch=[];
 final List transactions;

  _TransactionsListState(this.transactions);

void _runFilter(String enteredKeyword) {
    List results = [];
    if (enteredKeyword.isEmpty) {
      // if the search field is empty or only contains white-space, we'll display all users

      results = transactions;
    } else {
      results = transactions
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
    _foundsearch=transactions;
    super.initState();
  }
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        centerTitle: true,
        backgroundColor: Colors.red[800],
        title: Text('Latest Transactions'),
        leading: Icon(Icons.arrow_back_ios,size: 0),
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
                  ? ListView.builder(
                itemCount: _foundsearch.length,
                itemBuilder: (context, index) => Container(
                  child: InkWell(
                    onTap: () => Navigator.of(context)
                        .push(new MaterialPageRoute(
                      builder: (BuildContext context) =>
                      new TransactionDetails(
                        elector: _foundsearch[index].elector,
                        elected: _foundsearch[index].elected,
                        hash: _foundsearch[index].hash,
                        dateTime: _foundsearch[index].dateTime,
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
                                      _foundsearch[index].elector,
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
                                      _foundsearch[index].elected,
                                      style: TextStyle(
                                          color: Colors.blue,
                                          fontSize: 13)),
                                ],
                              ),
                            ),
                            leading: Container(
                                width: MediaQuery.of(context)
                                    .size
                                    .width /
                                    2.4,
                                child: ListTile(
                                  leading: CircleAvatar(
                                    backgroundColor: Colors.grey[300],
                                    child: Center(
                                        child: Text(
                                          'Tx',
                                          style: TextStyle(
                                              fontSize: 16,
                                              color: Colors.black),
                                        )),
                                  ),
                                  title: Text(
                                    _foundsearch[index].hash,
                                    style: TextStyle(fontSize: 12),
                                    maxLines: 1,
                                    overflow: TextOverflow.ellipsis,
                                    softWrap: false,
                                  ),
                                  subtitle: Text(
                                    _foundsearch[index].dateTime,
                                    style: TextStyle(
                                        color: Colors.black45,
                                        fontSize: 10),
                                    maxLines: 1,
                                    overflow: TextOverflow.ellipsis,
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
