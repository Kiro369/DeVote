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
        title: Text('تفاصيل العملية',style: TextStyle(color:Colors.white),),
        centerTitle: true,
        backgroundColor: Colors.red[700],
      ),
      body: Container(
        decoration: BoxDecoration(
            image: DecorationImage(
              image: AssetImage("assets/a3.png"),
              fit: BoxFit.cover,
            ),
            color: Colors.red[700],),
        child: ListView(
          children: [
          ListTile(
            title: Text(hash,style: TextStyle(color: Colors.white),),
            subtitle: Text('Hash',style: TextStyle(color: Colors.white54),),
          ),
            ListTile(
              title: Text(elector,style: TextStyle(color: Colors.white),),
              subtitle: Text('From',style: TextStyle(color: Colors.white54),),
            ),
            ListTile(
              title: Text(elected,style: TextStyle(color: Colors.white),),
              subtitle: Text('To',style: TextStyle(color: Colors.white54),),
            ),
            ListTile(
              title: Text(dateTime,style: TextStyle(color: Colors.white),),
              subtitle: Text('DateTime',style: TextStyle(color: Colors.white54),),
            ),
            Center(child: Image.asset('assets/a4.png')),
          ],
        ),
      ),
    );
  }
}
