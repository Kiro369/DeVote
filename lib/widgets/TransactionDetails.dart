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
          title: Text(hash,style: TextStyle(color: Colors.blue),),
          subtitle: Text('Hash',style: TextStyle(color: Colors.black45),),
        ),
          ListTile(
            title: Text(elector,style: TextStyle(color: Colors.blue),),
            subtitle: Text('From',style: TextStyle(color: Colors.black45),),
          ),
          ListTile(
            title: Text(elected,style: TextStyle(color: Colors.blue),),
            subtitle: Text('To',style: TextStyle(color: Colors.black45),),
          ),
          ListTile(
            title: Text(dateTime,style: TextStyle(color: Colors.blue),),
            subtitle: Text('DateTime',style: TextStyle(color: Colors.black45),),
          ),

        ],
      ),
    );
  }
}
