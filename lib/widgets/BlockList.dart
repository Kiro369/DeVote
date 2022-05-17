import 'package:flutter/material.dart';
import '../models/Call_api.dart';
import '../models/block.dart';
import 'BlockDetails.dart';

import 'package:timeago/timeago.dart' as timeago;

class BlockList extends StatefulWidget {
  final List blocks;

  BlockList(this.blocks);

  @override
  _BlockListState createState() => _BlockListState();
}

class _BlockListState extends State<BlockList> {
  List _foundsearch = [];
  late Future<List<Results>> blockks;
  late List blockList;
  late Future<Block> all;
  CallApi block = CallApi(Uri.https('devote-explorer-backend.herokuapp.com',
      'blocks'));

  Future<void> getlist() async {
    blockList = await blockks;
  }

  void showmore(String prev) async {

    Uri myUri = Uri.parse(prev);
    Map<String, String> queryParameters = myUri.queryParameters;
    block = CallApi(Uri.https('devote-explorer-backend.herokuapp.com',
        'blocks',queryParameters));
    blockks = block.getBlocks();
    all=block.pagination();
    await getlist();
    setState(() {

      _foundsearch = blockList;
      print(prev);
    });

  }

  void _runFilter(String enteredKeyword) {
    List results = [];
    if (enteredKeyword.isEmpty) {
      // if the search field is empty or only contains white-space, we'll display all users

      results = blockList;
    } else {
      results = blockList
          .where((block) => block.blockHeight
              .toString()
              .toLowerCase()
              .contains(enteredKeyword.toLowerCase()))
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
    blockks = block.getBlocks();
    blockList = widget.blocks;
    all=block.pagination();
    getlist();
    _foundsearch = blockList;
    super.initState();
  }



  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        centerTitle: true,
        backgroundColor: //kIsWeb?Colors.white:
            const Color(0xff26375f),
        title: const Text(
          'Latest Blocks',
          style: TextStyle(
              color: //kIsWeb?Colors.black:
                  Colors.white),
        ),
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
                    labelText: 'Search by Block Height',
                    suffixIcon: Icon(Icons.search)),
              ),
            ),
            Expanded(
              child: _foundsearch.isNotEmpty
                  ? Padding(
                      padding: const EdgeInsets.all(15.0),
                      child: FutureBuilder<Block>(
                        key: UniqueKey(),
                        future: all,
                        builder: (context, snapshot) {
                          if (snapshot.hasData) {
                            return SingleChildScrollView(
                              physics: ScrollPhysics(),
                              child: Column(
                                children: [
                                  ListView.builder(
                                    physics: NeverScrollableScrollPhysics(),
                                    shrinkWrap: true,
                                    //key: UniqueKey(),
                                    itemCount: _foundsearch.length,
                                    itemBuilder: (context, index) => InkWell(
                                      onTap: () =>Navigator.of(context)
                                              .push(MaterialPageRoute(
                                              builder: (BuildContext context) =>
                                                  BlockDetails(
                                                blockHeight:
                                                _foundsearch[index].blockHeight,
                                                miner: _foundsearch[index].miner,
                                                time:  _foundsearch[index].time,
                                                transactions:
                                                _foundsearch[index].transactions,
                                                merkleRoot:
                                                _foundsearch[index].merkleRoot,
                                                hash:  _foundsearch[index].hash,
                                                prevhash:
                                                _foundsearch[index].prevHash,
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
                                                      text:
                                                      _foundsearch[index].miner,
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
                                                text:  _foundsearch[index]
                                                    .transactions
                                                    .toString(),
                                                style: const TextStyle(
                                                    color: Colors.blue,
                                                    fontSize: 13),
                                                children: <TextSpan>[
                                                  TextSpan(
                                                      text: ' txns',
                                                      style: const TextStyle(
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
                                                    _foundsearch[index]
                                                        .blockHeight
                                                        .toString(),
                                                    style: const TextStyle(
                                                        fontSize: 12),
                                                    maxLines: 1,
                                                    overflow: TextOverflow.ellipsis,
                                                    softWrap: false,
                                                  ),
                                                  subtitle: Text(
                                                    timeago.format(DateTime
                                                        .fromMillisecondsSinceEpoch(
                                                        _foundsearch[index]
                                                                .time)),
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
                                  Row(
                                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                    children: [
                                      Padding(
                                        padding: const EdgeInsets.all(12.0),
                                        child: RaisedButton.icon(
                                            onPressed: () => snapshot.data!.pagination.prev==null?null:showmore(snapshot.data!.pagination.prev),
                                            shape: const RoundedRectangleBorder(
                                                borderRadius:
                                                BorderRadius.all(Radius.circular(10.0))),
                                            label: const Text(
                                              'Back',
                                              style: TextStyle(color: Colors.white, fontSize: 16),
                                            ),
                                            icon: const Padding(
                                              padding: EdgeInsets.all(8.0),
                                              child: Icon(
                                                Icons.arrow_back_ios_rounded,
                                                color: Colors.white,
                                                size: 15,
                                              ),
                                            ),
                                            textColor: Colors.white,
                                            color: Colors.blue),
                                      ),
                                      Padding(
                                        padding: const EdgeInsets.all(12.0),
                                        child: RaisedButton.icon(
                                            onPressed: () =>snapshot.data!.pagination.next==null?null: showmore(snapshot.data!.pagination.next),
                                            shape: const RoundedRectangleBorder(
                                                borderRadius:
                                                BorderRadius.all(Radius.circular(10.0))),
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
                                              style: TextStyle(color: Colors.white, fontSize: 16),
                                            ),
                                            textColor: Colors.white,
                                            color: Colors.blue),
                                      ),
                                    ],
                                  ),
                                ],
                              ),
                            );
                          } else if (snapshot.hasError) {
                            return Text("${snapshot.error}");
                          } // spinner
                          return Center(child: CircularProgressIndicator());
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
