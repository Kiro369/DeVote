import 'package:flutter/material.dart';
import 'bottom_bar/bottom_bar.dart';

void main() {
  runApp( MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'DeVote',
      theme: ThemeData(
        primarySwatch: Colors.blue,
      ),
      home: Bottom_bar(),
      debugShowCheckedModeBanner: false,
    );
  }
}

