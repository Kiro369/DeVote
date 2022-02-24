import 'package:devote/models/people.dart';
import 'package:devote/widgets/MapChart.dart';
import 'package:devote/widgets/PieChart.dart';
import 'package:devote/widgets/TransactionDetails.dart';
import 'package:devote/widgets/blockchain.dart';
import 'package:flutter/material.dart';
import 'package:syncfusion_flutter_maps/maps.dart';

import '../models/mapModel.dart';
import '../models/transaction.dart';

class Result extends StatefulWidget {
  const Result({Key? key}) : super(key: key);

  @override
  _ResultState createState() => _ResultState();
}

class _ResultState extends State<Result> {
  List<Container> explorer = [];
  List<Transaction> transactions = [
    Transaction('c401d858d2', '22 hrs 46 mins ago',
        '0x25dc3a4eargggrgedvjlxngluoxc2', 'C.c'),
    Transaction('0x8c401d858d2', '2 hrs 46 mins ago',
        '0ecsgrfdgsergrgfjilzxdglsuixgh9r2', 'C.c'),
  ];

  _blockchain() {
    for (var i = 0; i < transactions.length; i++) {
      final transcation = transactions[i];
      final hash = transcation.hash;
      final date = transcation.dateTime;
      final elector = transcation.elector;
      final elected = transcation.elected;

      explorer.add(Container(
        child: Padding(
          padding: const EdgeInsets.all(4.0),
          child: ListTile(
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(10),
              side: BorderSide(color: Colors.black),
            ),
            leading: Text(
              date,
              style: TextStyle(
                fontSize: 10,
              ),
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
            ),
            title: Text(elector),
            subtitle: Text(elected),
            trailing: Text(hash),
            onTap: () => Navigator.of(context).push(new MaterialPageRoute(
              builder: (BuildContext context) => new TransactionDetails(
                elector: elector,
                elected: elected,
                hash: hash,
                dateTime: date,
              ),
            )),
          ),
        ),
      ));
    }
  }

  List<people> motrsh7en = [
    people('عبد الفتاح السيسي', 6589305, '65%'),
    people(' موسي مصطفي موسي', 335555, '35%')
  ];
  late List<Model> data;
  late MapShapeSource _mapSource;

  @override
  void initState() {
    data = const <Model>[
      Model('Kafr-El-Sheikh', Color(0xffd00a16), 'كفر الشيخ'),
      Model('Al Fayyum (Fayoum)', Color(0xffd00a16), 'الفيوم'),
      Model('Al Bahr/Al Ahmar (RedSea)', Color(0xffd00a16), 'البحر الاحمر'),
      Model('Dumyat (Damietta)', Color(0xffd00a16), 'دمياط'),
      Model('Matruh', Color(0xffd00a16), 'مطروح'),
      Model('Al Daqahliyah (Dakahlia)', Color(0xffd00a16), 'الدقهلية'),
      Model('As Ismailiyah (Ismailia)', Color(0xff6ca0ff), 'الاسماعيلية'),
      Model('Aswan', Color(0xff6ca0ff), 'اسوان'),
      Model('Asyiut', Color(0xff6ca0ff), 'اسيوط'),
      Model('Beni Suwayf (Beni-Suef)', Color(0xffd00a16), 'بني سويف'),
      Model('Qina', Color(0xff6ca0ff), 'قنا'),
      Model('Suhaj', Color(0xffd00a16), 'سوهاج'),
      Model('Al Iskandariyah (Alex.)', Color(0xff6ca0ff), 'الاسكندرية'),
      Model('Al Qahirah (Cairo)', Color(0xff6ca0ff), 'القاهرة'),
      Model('As Suways (Suez)', Color(0xff6ca0ff), 'السويس'),
      Model('Bur Said (Port Said)', Color(0xffd00a16), 'بورسعيد'),
      Model('Al Buhayrah (Behera)', Color(0xffd00a16), 'البحيرة'),
      Model('Al Wadi/Al Jadid', Color(0xffd00a16), 'الوادي الجديد'),
      Model('Janub Sina (South Sinai)', Color(0xffd00a16), 'جنوب سيناء'),
      Model('Shamal Sina (North Sinai)', Color(0xff6ca0ff), 'شمال سيناء'),
      Model('Al Minufiyah (Menoufia)', Color(0xff6ca0ff), 'المنوفية'),
      Model('Al Gharbiyah (Gharbia)', Color(0xffd00a16), 'الغربية'),
      Model('Al Qalyubiyah (Kalyoubia)', Color(0xffd00a16), 'القليوبية'),
      Model('Ash Sharqiyah (Sharkia)', Color(0xffd00a16), 'الشرقية'),
      Model('Al Jizah (Giza)', Color(0xffd00a16), 'الجيزة'),
      Model('Al Minya (Menia)', Color(0xff6ca0ff), 'المنيا'),
    ];
    _mapSource = MapShapeSource.asset(
      'assets/egypt2.json',
      shapeDataField: 'adm2',
      dataCount: data.length,
      primaryValueMapper: (int index) => data[index].state,
      dataLabelMapper: (int index) => data[index].stateCode,
      shapeColorValueMapper: (int index) => data[index].color,
    );
    _blockchain();
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        centerTitle: true,
        actions: const [
          Padding(
            padding: EdgeInsets.only(right: 10),
            child: Icon(
              Icons.check_box_outlined,
              color: Colors.white,
              size: 30,
            ),
          )
        ],
        title: const Text(
          'نتائج الانتخابات الرئاسية',
          style: TextStyle(color: Colors.white),
        ),
        backgroundColor: Colors.red[800],
      ),
      body: ListView(
        children: [
          const Padding(
            padding: EdgeInsets.all(12.0),
            child: Text(
              'النتيجة الاجمالية',
              textAlign: TextAlign.center,
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
          ),
          PieChart(
            motrsh7: motrsh7en,
          ),
          const Padding(
            padding: EdgeInsets.all(12.0),
            child: Text(
              'النتيجة علي الخريطة',
              textAlign: TextAlign.center,
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
          ),
          MapChart(data: data, mapSource: _mapSource),
          /* const Padding(
            padding: EdgeInsets.all(12.0),
            child: Text(
              'متابعة العملية الانتخابية',
              textAlign: TextAlign.center,
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
            ),
          ),
          const Text(
            '( BlockChain Explorer )',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontSize: 12,
            ),
          ),
          Column(children: explorer),*/
          Padding(
            padding: const EdgeInsets.all(12.0),
            child: RaisedButton.icon(
              onPressed: () => Navigator.of(context).push(new MaterialPageRoute(
                  builder: (BuildContext context) => new BlockChain())),
              shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.all(Radius.circular(10.0))),
              label: Text(
                'متابعة العملية الانتخابية بشكل مباشر',
                style: TextStyle(color: Colors.white, fontSize: 16),
              ),
              icon: Padding(
                padding: const EdgeInsets.all(8.0),
                child: Icon(
                  Icons.how_to_vote_rounded,
                  color: Colors.white,
                ),
              ),
              textColor: Colors.white,
              splashColor: Colors.red,
              color: Color(0xff6ca0ff),
            ),
          ),
        ],
      ),
    );
  }
}
