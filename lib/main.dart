import 'package:devote/bottom_bar/results.dart';
import 'package:flutter/gestures.dart';
import 'package:flutter/material.dart';
import 'bottom_bar/bottom_bar.dart';
import 'package:flutter/foundation.dart' show kIsWeb;

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      //scrollBehavior: MaterialScrollBehavior().copyWith(
        //dragDevices: {PointerDeviceKind.mouse, PointerDeviceKind.touch, PointerDeviceKind.stylus, PointerDeviceKind.unknown},),

      title: 'DeVote',
      home: kIsWeb?Result():Bottom_bar(),
      debugShowCheckedModeBanner: false,
    );
  }
}

