import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'TransactionDetails.dart';
import 'package:timeago/timeago.dart' as timeago;
class TransactionsList extends StatefulWidget {

  final List transactions;

   TransactionsList( this.transactions);
@override
_TransactionsListState createState() => _TransactionsListState();
}

class _TransactionsListState extends State<TransactionsList> {
  List _foundsearch=[];

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
    _foundsearch=widget.transactions;
    super.initState();
  }
  _buildChild(BuildContext context,int block,int date, String elector,
       String hash, String elected) =>
      Container(
          height: MediaQuery.of(context).size.height / 1.15,
          width: MediaQuery.of(context).size.width / 2.8,
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.all(Radius.circular(20)),
          ),
          child: TransactionDetails(
            block: block,
            dateTime:date ,
            hash:hash ,
            elected: elected,
            elector: elector,

          ));
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        centerTitle: true,
        backgroundColor: Color(0xff26375f),
        title: Text('Latest Transactions'),
        leading: Icon(Icons.arrow_back_ios,size: 0,color: Color(0xff26375f),),
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
                    child: ListView.builder(
                itemCount: _foundsearch.length,
                itemBuilder: (context, index) => Container(
                    child: InkWell(
                      onTap: () =>  kIsWeb
                          ? showDialog(
                          context: context,
                          builder: (context) {
                            return Dialog(
                              elevation: 1,
                              backgroundColor: Colors.transparent,
                              child: _buildChild(
                                context,
                                _foundsearch[index].blockheight,
                                _foundsearch[index].dateTime,
                                _foundsearch[index].elector,
                                _foundsearch[index].hash,
                                _foundsearch[index].elected,
                              ),
                            );
                          })
                          :Navigator.of(context)
                          .push(new MaterialPageRoute(
                        builder: (BuildContext context) =>
                        new TransactionDetails(
                          block:  _foundsearch[index].blockheight,
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
                                      timeago.format(DateTime.fromMillisecondsSinceEpoch(_foundsearch[index].dateTime)),
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
