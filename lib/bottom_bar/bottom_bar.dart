import 'package:devote/bottom_bar/amaken.dart';
import 'package:devote/bottom_bar/motrsh7en.dart';
import 'package:devote/bottom_bar/results.dart';
import 'package:flutter/material.dart';
class Bottom_bar extends StatefulWidget {
  const Bottom_bar({Key? key}) : super(key: key);

  @override
  _Bottom_barState createState() => _Bottom_barState();
}

class _Bottom_barState extends State<Bottom_bar> {
  List pages = [Amaken(),Motrsh7en(),Result(),];
  int current = 2;
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      bottomNavigationBar: BottomNavigationBar(
        type: BottomNavigationBarType.fixed,
        backgroundColor: Colors.red[800],
        iconSize: 26,
        selectedItemColor: Colors.white,
        unselectedItemColor: Colors.white54,
        selectedFontSize: 14,
        unselectedFontSize: 12,
        currentIndex: current,
        onTap: (index){
          setState(() {
            current=index;
          });
        },
        items: [
          BottomNavigationBarItem(
              title: Text('الاماكن'),
              icon: Icon(Icons.location_on)),
          BottomNavigationBarItem(
              title: Text('المرشحون'),
              icon: Icon(Icons.people_alt_outlined)),
          BottomNavigationBarItem(
              title: Text('النتائج'),
              icon: Icon(Icons.insert_chart_outlined)),
        ],

      ),
      body: pages[current],
    );
  }
}
