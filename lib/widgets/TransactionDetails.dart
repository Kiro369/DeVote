import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
class TransactionDetails extends StatelessWidget {
  final String hash;
  final int dateTime;
  final String elector;
  final String elected;
  final int block;

  const TransactionDetails({ required this.hash, required this.dateTime, required this.elector, required this.elected, required this.block}) ;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        leading: Icon(Icons.arrow_back_ios_outlined,size:0,color: Color(0xff26375f),),
        title: Text('Transaction Details',style: TextStyle(color:Colors.white),),
        centerTitle: true,
        backgroundColor: Color(0xff26375f),
      ),
      body: ListView(
        children: [
        Padding(
          padding: const EdgeInsets.only(top: 8.0),
          child: ListTile(

            title: Text('Transaction Hash:',style: TextStyle(color: Colors.black,fontWeight: FontWeight.bold),),
            subtitle: Padding(
              padding: const EdgeInsets.only(top: 8.0),
              child: Text(hash,style: TextStyle(color: Colors.black),
           maxLines: 2,  overflow: TextOverflow.fade,
                softWrap: false, ),
            ),
          ),
        ),
          Divider(
            height: 2,
            color: Colors.grey,
          ),
          ListTile(
            title: Text('Timestamp:',style: TextStyle(color: Colors.black,fontWeight: FontWeight.bold),),
            subtitle: Padding(
              padding: const EdgeInsets.only(top:8.0),
              child: Text(DateTime.fromMillisecondsSinceEpoch(dateTime).toString(),style: TextStyle(color: Colors.black),),
            ),
          ),
          Divider(
            height: 2,
            color: Colors.grey,
          ),
          ListTile(
            title:  Text('From:',style: TextStyle(color: Colors.black,fontWeight:FontWeight.bold),),
            subtitle: Text(elector,style: TextStyle(color: Colors.blue),  overflow: TextOverflow.fade,
              softWrap: false,),
          ),
          ListTile(
            title:Text('To:',style: TextStyle(color: Colors.black,fontWeight: FontWeight.bold),),
            subtitle: Text(elected,style: TextStyle(color: Colors.blue),  overflow: TextOverflow.fade,
              softWrap: false,),
          ),
          Divider(
            height: 2,
            color: Colors.grey,
          ),
          ListTile(
            title:Text('Block:',style: TextStyle(color: Colors.black,fontWeight: FontWeight.bold),),
            subtitle: Padding(
              padding: const EdgeInsets.only(top: 8.0),
              child: Text(block.toString(),style: TextStyle(color: Colors.black),  overflow: TextOverflow.fade,
                softWrap: false,),
            ),
          ),
          kIsWeb? Center(
            child: Container(
                height: 180,
                width:180,child: Image.asset('assets/a4.png',color: const Color(0xff26375f))),
          ):Text(''),

        ],
      ),
    );
  }
}
