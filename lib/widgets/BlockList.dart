import 'package:flutter/material.dart';
import 'BlockDetails.dart';

class BlockList extends StatefulWidget {
  final List blocks;

  BlockList( this.blocks);
  @override

  _BlockListState createState() => _BlockListState(blocks);
}

class _BlockListState extends State<BlockList> {

  List _foundsearch=[];
  final List blocks;

  _BlockListState(this.blocks);

  void _runFilter(String enteredKeyword) {
    List results = [];
    if (enteredKeyword.isEmpty) {
      // if the search field is empty or only contains white-space, we'll display all users

      results = blocks;
    } else {
      results = blocks
          .where((block) =>
          block.blockHeight.toString().toLowerCase().contains(enteredKeyword.toLowerCase()))
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
    _foundsearch=blocks;
    super.initState();
  }
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        centerTitle: true,
        backgroundColor:const Color(0xff26375f),
        title: const Text('Latest Blocks'),
        leading: const Icon(Icons.arrow_back_ios,size: 0),
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
                    labelText: 'Search by Block Height',
                    suffixIcon: Icon(Icons.search)),
              ),
            ),

            Expanded(
              child: _foundsearch.isNotEmpty
                  ? ListView.builder(
                itemCount: _foundsearch.length,
                itemBuilder: (context, index) => InkWell(
                  onTap: () => Navigator.of(context)
                      .push(MaterialPageRoute(
                    builder: (BuildContext context) =>
                     BlockDetails(
                      blockHeight: _foundsearch[index].blockHeight,
                      miner:_foundsearch[index].miner ,
                      time: _foundsearch[index].time ,
                      transactions: _foundsearch[index].transactions ,
                      transactionTime:_foundsearch[index].transactionTime  ,
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
                              style: const TextStyle(
                                  color: Colors.black,
                                  fontSize: 13),
                              children: <TextSpan>[
                                TextSpan(
                                    text:
                                    blocks[index].miner,
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
                              text: _foundsearch[index].transactions.toString(),
                              style: const TextStyle(
                                  color: Colors.blue, fontSize: 13),
                              children: <TextSpan>[
                                TextSpan(
                                    text: ' in ${_foundsearch[index].transactionTime}',
                                    style: const TextStyle(
                                        color: Colors.black45,
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
                                  child: const Center(
                                      child: Text(
                                        'Bk',
                                        style: TextStyle(
                                            fontSize: 16,
                                            color: Colors.black),
                                      )),
                                ),
                                title: Text(
                                  _foundsearch[index].blockHeight.toString(),
                                  style: const TextStyle(fontSize: 12),
                                  maxLines: 1,
                                  overflow: TextOverflow.ellipsis,
                                  softWrap: false,
                                ),
                                subtitle: Text(
                                  _foundsearch[index].time,
                                  style: const TextStyle(
                                      color: Colors.black45,
                                      fontSize: 10),
                                  maxLines: 1,
                                  overflow: TextOverflow.ellipsis,
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
