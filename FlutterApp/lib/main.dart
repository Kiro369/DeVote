import 'package:flutter/gestures.dart';

import '/bottom_bar/results.dart';
import 'package:flutter/material.dart';
import 'bottom_bar/bottom_bar.dart';
import 'package:flutter/foundation.dart' show kIsWeb;

import 'bottom_bar/results.dart';

void main() {

  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      scrollBehavior: MaterialScrollBehavior().copyWith(
        dragDevices: {PointerDeviceKind.mouse, PointerDeviceKind.touch, PointerDeviceKind.stylus, PointerDeviceKind.unknown},),
      color: Colors.grey[50],
      title: 'DeVote',
      home: kIsWeb?Result():Bottom_bar(),
      debugShowCheckedModeBanner: false,
    );
  }
}
//for release apk:flutter build apk --split-per-abi
//web flutter run -d chrome --web-renderer html

