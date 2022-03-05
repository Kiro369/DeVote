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
        leading:const Icon(Icons.arrow_back_ios_outlined,size:0,color: Colors.white,),
        title: const Text('Block Details',style: TextStyle(color:Colors.white),),
        centerTitle: true,
        backgroundColor: const Color(0xff26375f),
      ),
      body: ListView(
        children: [
          ListTile(
            title: const Text('Block Height:',style: TextStyle(color: Colors.black,fontWeight: FontWeight.bold),),
            subtitle: Text(blockHeight.toString(),style: const TextStyle(color: Colors.black),),
          ),
          const Divider(
            height: 2,
            color: Colors.grey,
          ),
          ListTile(
            title:const Text('Timestamp:',style: TextStyle(color: Colors.black,fontWeight: FontWeight.bold),),
            subtitle: Text(time,style: const TextStyle(color: Colors.black),),
          ),
          const Divider(
            height: 2,
            color: Colors.grey,
          ),
          ListTile(
            title:const Text('Transactions',style: TextStyle(color: Colors.black,fontWeight: FontWeight.bold),),
            subtitle: Text(transactions.toString(),style:const TextStyle(color: Colors.black),),
          ),
          const   Divider(
            height: 2,
            color: Colors.grey,
          ),
          ListTile(
            title:const Text('Mined by:',style: TextStyle(color: Colors.black,fontWeight: FontWeight.bold),),
            subtitle: RichText(
              maxLines: 2,
              overflow: TextOverflow.fade,
              softWrap: false,
              text: TextSpan(
                text: miner,
                style:const TextStyle(
                    color: Colors.blue, fontSize: 13),
                children: <TextSpan>[
                  TextSpan(
                      text: ' in $transactionTime',
                      style:const TextStyle(
                          color: Colors.black,
                          fontSize: 13)),
                ],
              ),
            ),),

        ],
      ),
    );
  }
}
