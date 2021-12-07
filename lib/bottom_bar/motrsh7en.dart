import 'package:flutter/material.dart';
class Motrsh7en extends StatefulWidget {
  const Motrsh7en({Key? key}) : super(key: key);

  @override
  _Motrsh7enState createState() => _Motrsh7enState();
}

class _Motrsh7enState extends State<Motrsh7en> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        centerTitle: true,
        title: Text('المترشحون للانتخاب',style: TextStyle(color: Colors.white),),
        backgroundColor: Colors.red[800],
      ),
      body: Center(child: Text('Motrsh7en')),
    );
  }
}
