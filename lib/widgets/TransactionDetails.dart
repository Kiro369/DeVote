import 'package:flutter/material.dart';
class TransactionDetails extends StatelessWidget {
  final String hash;
  final String dateTime;
  final String elector;
  final String elected;

  const TransactionDetails({ required this.hash, required this.dateTime, required this.elector, required this.elected}) ;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        leading: Icon(Icons.arrow_back_ios_outlined,size:0,color: Colors.white,),
        title: Text('Transaction Details',style: TextStyle(color:Colors.white),),
        centerTitle: true,
        backgroundColor: Color(0xff26375f),
      ),
      body: ListView(
        children: [
        ListTile(
          title: Text('Transaction Hash:',style: TextStyle(color: Colors.black,fontWeight: FontWeight.bold),),
          subtitle: Text(hash,style: TextStyle(color: Colors.black),
         maxLines: 2,  overflow: TextOverflow.fade,
            softWrap: false, ),
        ),
          Divider(
            height: 2,
            color: Colors.grey,
          ),
          ListTile(
            title: Text('Timestamp:',style: TextStyle(color: Colors.black,fontWeight: FontWeight.bold),),
            subtitle: Text(dateTime,style: TextStyle(color: Colors.black),),
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

        ],
      ),
    );
  }
}
