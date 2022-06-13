import '/bottom_bar/amaken.dart';
import '/bottom_bar/motrsh7en.dart';
import '/bottom_bar/results.dart';
import 'package:flutter/material.dart';

// ignore: camel_case_types
class Bottom_bar extends StatefulWidget {
  const Bottom_bar({Key? key}) : super(key: key);

  @override
  _Bottom_barState createState() => _Bottom_barState();
}

// ignore: camel_case_types
class _Bottom_barState extends State<Bottom_bar> {
  //list of pages and initial home page (current)
  List pages = [
    const Amaken(),
    const Motrsh7en(),
    const Result(),
  ];
  int current = 2;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      bottomNavigationBar: BottomNavigationBar(
        type: BottomNavigationBarType.fixed,
        backgroundColor: Colors.white,
        iconSize: 26,
        selectedItemColor: Colors.black,
        unselectedItemColor: Colors.black45,
        selectedFontSize: 14,
        unselectedFontSize: 12,
        currentIndex: current,
        onTap: (index) {
          setState(() {
            current = index;
          });
        },
        items: const [
          BottomNavigationBarItem(
              label: 'المواقع', icon: Icon(Icons.location_on)),
          BottomNavigationBarItem(
              label: 'المرشحون', icon: Icon(Icons.people_alt_outlined)),
          BottomNavigationBarItem(
              label: 'النتائج', icon: Icon(Icons.insert_chart_outlined)),
        ],
      ),
      body: pages[current],
    );
  }
}
