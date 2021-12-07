import 'package:flutter/material.dart';
class Amaken extends StatefulWidget {
  const Amaken({Key? key}) : super(key: key);

  @override
  _AmakenState createState() => _AmakenState();
}

class _AmakenState extends State<Amaken> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        centerTitle: true,
        title: Text('اماكن الانتخاب',style: TextStyle(color: Colors.white),),
        backgroundColor: Colors.red[800],
      ),
      body: Center(child: Text('Amaken')),
    );
  }
}
