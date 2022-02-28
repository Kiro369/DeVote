import 'package:flutter/material.dart';
class BlockDetails extends StatelessWidget {
  final String miner;
  final  int blockHeight;
  final int transactions;
  final String time;
  final String transactionTime;

  const BlockDetails({ required this.miner,required this.transactions,required this.blockHeight,required this.time,required this.transactionTime}) ;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        leading: Icon(Icons.arrow_back_ios_outlined,size:0,color: Colors.white,),
        title: Text('Block Details',style: TextStyle(color:Colors.white),),
        centerTitle: true,
        backgroundColor: Colors.red[700],
      ),
      body: ListView(
        children: [
          ListTile(
            title: Text(miner,style: TextStyle(color: Colors.blue),),
            subtitle: Text('Miner',style: TextStyle(color: Colors.black45),),
          ),
          ListTile(
            title: Text(time,style: TextStyle(color: Colors.blue),),
            subtitle: Text('Time',style: TextStyle(color: Colors.black45),),
          ),
          ListTile(
            title: Text(blockHeight.toString(),style: TextStyle(color: Colors.blue),),
            subtitle: Text('Block Height',style: TextStyle(color: Colors.black45),),
          ),
          ListTile(
            title: Text(transactions.toString(),style: TextStyle(color: Colors.blue),),
            subtitle: Text('Transactions',style: TextStyle(color: Colors.black45),),
          ),
          ListTile(
            title: Text(transactionTime,style: TextStyle(color: Colors.blue),),
            subtitle: Text('Transaction Time',style: TextStyle(color: Colors.black45),),
          ),
        ],
      ),
    );
  }
}
