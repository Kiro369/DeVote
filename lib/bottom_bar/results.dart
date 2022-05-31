import 'dart:async';
import 'package:timer_builder/timer_builder.dart';

import '../models/Call_api.dart';
import '/models/people.dart';
import '/widgets/MapChart.dart';
import '/widgets/PieChart.dart';
import '/widgets/blockchain.dart';
import 'package:flutter/material.dart';
import 'package:linked_scroll_controller/linked_scroll_controller.dart';
import 'package:syncfusion_flutter_maps/maps.dart';
import '../models/mapModel.dart';
import 'package:flutter/foundation.dart' show kIsWeb;

class Result extends StatefulWidget {
  const Result({Key? key}) : super(key: key);

  @override
  _ResultState createState() => _ResultState();
}

class _ResultState extends State<Result> {
  late Future<List<ResultsChart>> pie;
  late Future<List<Model>> map;
  late DateTime update;
  List<ResultsChart> motrsh7en = [
    ResultsChart(
      '65%',
      'عبد الفتاح السيسي',
      1,
    ),
    ResultsChart(
      '35%',
      ' موسي مصطفي موسي',
      1,
    )
  ];
  late List<Model> data;
  late MapShapeSource _mapSource;
  late LinkedScrollControllerGroup _controllers;
  late ScrollController _letters;
  late ScrollController _numbers;

  @override
  void initState() {
    update=DateTime.now().add(const Duration(seconds: 10));
    CallApi network = CallApi(
        Uri.https('devote-explorer-backend.herokuapp.com', 'candidates'));
    pie = network.getPieResult();
    CallApi model = CallApi(
        Uri.https('devote-explorer-backend.herokuapp.com', 'governorates'));
    map = model.getMapResults();
    Timer.periodic(const Duration(seconds: 15), (Timer t) => mapResult());
    Timer.periodic(const Duration(seconds: 5), (Timer t) => pieResult());
    data = [
      const Model('Al Iskandariyah (Alex.)', 0xffb4b4b4, 'الاسكندرية', []),
      const Model('Aswan', 0xffb4b4b4, 'اسوان', []),
      const Model('Asyiut', 0xffb4b4b4, 'اسيوط', []),
      const Model('Al Buhayrah (Behera)', 0xffb4b4b4, 'البحيرة', []),
      const Model('Beni Suwayf (Beni-Suef)', 0xffb4b4b4, 'بني سويف', []),
      const Model('Al Qahirah (Cairo)', 0xffb4b4b4, 'القاهرة', []),
      const Model('Al Daqahliyah (Dakahlia)', 0xffb4b4b4, 'الدقهلية', []),
      const Model('Dumyat (Damietta)', 0xffb4b4b4, 'دمياط', []),
      const Model('Al Fayyum (Fayoum)', 0xffb4b4b4, 'الفيوم', []),
      const Model('Al Gharbiyah (Gharbia)', 0xffb4b4b4, 'الغربية', []),
      const Model('Al Jizah (Giza)', 0xffb4b4b4, 'الجيزة', []),
      const Model('As Ismailiyah (Ismailia)', 0xffb4b4b4, 'الاسماعيلية', []),
      const Model('Kafr-El-Sheikh', 0xffb4b4b4, 'كفر الشيخ', []),
      const Model('Luxor', 0xffb4b4b4, 'الاقصر', []),
      const Model('Matruh', 0xffb4b4b4, 'مطروح', []),
      const Model('Al Minya (Menia)', 0xffb4b4b4, 'المنيا', []),
      const Model('Al Minufiyah (Menoufia)', 0xffb4b4b4, 'المنوفية', []),
      const Model('Al Wadi/Al Jadid', 0xffb4b4b4, 'الوادي الجديد', []),
      const Model('Shamal Sina (North Sinai)', 0xffb4b4b4, 'شمال سيناء', []),
      const Model('Bur Said (Port Said)', 0xffb4b4b4, 'بورسعيد', []),
      const Model('Al Qalyubiyah (Kalyoubia)', 0xffb4b4b4, 'القليوبية', []),
      const Model('Qina', 0xffb4b4b4, 'قنا', []),
      const Model('Al Bahr/Al Ahmar (RedSea)', 0xffb4b4b4, 'البحر الاحمر', []),
      const Model('Ash Sharqiyah (Sharkia)', 0xffb4b4b4, 'الشرقية', []),
      const Model('Suhaj', 0xffb4b4b4, 'سوهاج', []),
      const Model('Janub Sina (South Sinai)', 0xffb4b4b4, 'جنوب سيناء', []),
      const Model('As Suways (Suez)', 0xffb4b4b4, 'السويس', []),
    ];
    _getlist();

    _controllers = LinkedScrollControllerGroup();
    _letters = _controllers.addAndGet();
    _numbers = _controllers.addAndGet();

    _mapSource = MapShapeSource.asset(
      'assets/egypt2.json',
      shapeDataField: 'adm2',
      dataCount: data.length,
      primaryValueMapper: (int index) => data[index].state,
      dataLabelMapper: (int index) => data[index].stateCode,
      shapeColorValueMapper: (int index) => Color(data[index].color),
    );
    super.initState();
  }

  _getlist() async {
    motrsh7en = await pie;
    data = await map;
  }

  @override
  void dispose() {
    _letters.dispose();
    _numbers.dispose();
    super.dispose();
  }

  static bool isLargeScreen(BuildContext context) {
    return MediaQuery.of(context).size.width > 1200;
  }

  Widget mapResult() {
    Widget maps = FutureBuilder(
      future: map,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.done) {
          if (snapshot.hasData) {
            return MapChart(data: data, mapSource: _mapSource);
          } else if (snapshot.hasError) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(30.0),
                child: Image.asset(
                  "assets/gif1.gif",
                  height: MediaQuery.of(context).size.height / 2.5,
                  // width: 125.0,
                ),
              ),
            );
          }
        }
        return Center(
          child: Padding(
            padding: const EdgeInsets.all(30.0),
            child: Image.asset(
              "assets/gif1.gif",
              height: MediaQuery.of(context).size.height / 2.5,
              // width: 125.0,
            ),
          ),
        );
      },
    );
    return maps;
  }

  Widget pieResult() {
    Widget charts = FutureBuilder(
        future: pie,
        builder: (context, snapshot) {
          return PieChart(
            motrsh7: motrsh7en,
          );
        });
    return charts;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        centerTitle: true,
        //  leading: ,
        actions: const [
          Padding(
            padding: EdgeInsets.only(right: 18),
            child: Icon(
              Icons.check_box_outlined,
              color: Colors.black,
              size: 30,
            ),
          )
        ],
        title: const Text(
          'نتائج الانتخابات الرئاسية',
          style: TextStyle(color: Colors.black),
        ),
        backgroundColor: Colors.white,
        leading: kIsWeb
            ? Padding(
                padding: const EdgeInsets.only(left: 18.0),
                child: IconButton(
                    onPressed: () => showDialog(
                        context: context,
                        builder: (context) {
                          return Dialog(
                              elevation: 1,
                              backgroundColor: Colors.transparent,
                              child: Container(
                                  height:
                                      MediaQuery.of(context).size.height / 2.5,
                                  width:
                                      MediaQuery.of(context).size.width / 3.2,
                                  decoration: const BoxDecoration(
                                    color: Color(0xff26375f),
                                    borderRadius:
                                        BorderRadius.all(Radius.circular(15)),
                                  ),
                                  child: Column(
                                    children: [
                                      const Center(
                                          child: Padding(
                                        padding: EdgeInsets.all(14.0),
                                        child: Icon(Icons.warning,
                                            size: 50, color: Color(0xfff3f3f5)),
                                      )),
                                      const Padding(
                                        padding: EdgeInsets.all(15.0),
                                        child: Text(
                                          'يمكنك معرفة امكان مكن الانتخابات الالكترونية من خلال تطبيقنا الخاص DeVote علي GooglePlay او AppStore',
                                          style: TextStyle(
                                            fontSize: 18,
                                            color: Color(0xfff3f3f5),
                                          ),
                                          textDirection: TextDirection.rtl,
                                        ),
                                      ),
                                      Center(
                                        child: Padding(
                                          padding:
                                              const EdgeInsets.only(top: 14.0),
                                          // ignore: deprecated_member_use
                                          child: FlatButton(
                                              color: const Color(0xfff3f3f5),
                                              onPressed: () =>
                                                  Navigator.of(context).pop(),
                                              child: const Text(
                                                'موافق',
                                                style: TextStyle(
                                                    color: Color(0xff26375f)),
                                              )),
                                        ),
                                      )
                                    ],
                                  )));
                        }),
                    icon: const Icon(
                      Icons.location_on,
                      size: 30,
                      color: Colors.black,
                    )),
              )
            : const Text(''),
      ),
      body: kIsWeb && isLargeScreen(context)
          ? Scrollbar(
              thumbVisibility: true,
              scrollbarOrientation: ScrollbarOrientation.right,
              controller: _letters,
              child: ScrollConfiguration(
                behavior:
                    ScrollConfiguration.of(context).copyWith(scrollbars: false),
                child: Row(
                  children: [
                    SizedBox(
                        width: MediaQuery.of(context).size.width / 2.2,
                        child: Padding(
                          padding: const EdgeInsets.only(bottom: 8.0),
                          child: BlockChain(
                            _letters,
                          ),
                        )),
                    SizedBox(
                      width: MediaQuery.of(context).size.width / 2,
                      child: ListView(
                        controller: _numbers,
                        children: [
                          const Padding(
                            padding: EdgeInsets.all(12.0),
                            child: Text(
                              'النتيجة الاجمالية',
                              textAlign: TextAlign.center,
                              style: TextStyle(
                                  fontSize: 18, fontWeight: FontWeight.bold),
                            ),
                          ),
                          TimerBuilder.scheduled([update], builder: (context){ return pieResult();}),
                          const Padding(
                            padding: EdgeInsets.all(12.0),
                            child: Text(
                              'النتيجة علي الخريطة',
                              textAlign: TextAlign.center,
                              style: TextStyle(
                                  fontSize: 18, fontWeight: FontWeight.bold),
                            ),
                          ),
                          //MapChart(data: data, mapSource: _mapSource),
                          mapResult(),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            )
          : ListView(
              children: [
                const Padding(
                  padding: EdgeInsets.all(12.0),
                  child: Text(
                    'النتيجة الاجمالية',
                    textAlign: TextAlign.center,
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                ),
                TimerBuilder.scheduled([update], builder: (context){ return pieResult();}),
                const Padding(
                  padding: EdgeInsets.all(12.0),
                  child: Text(
                    'النتيجة علي الخريطة',
                    textAlign: TextAlign.center,
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                ),
                TimerBuilder.scheduled([update], builder: (context){ return mapResult();}),
                Padding(
                  padding: const EdgeInsets.all(12.0),
                  // ignore: deprecated_member_use
                  child: RaisedButton.icon(
                    onPressed: () => Navigator.of(context).push(
                        MaterialPageRoute(
                            builder: (BuildContext context) => BlockChain())),
                    shape: const RoundedRectangleBorder(
                        borderRadius: BorderRadius.all(Radius.circular(10.0))),
                    label: const Text(
                      'متابعة العملية الانتخابية بشكل مباشر',
                      style: TextStyle(color: Colors.white, fontSize: 16),
                    ),
                    icon: const Padding(
                      padding: EdgeInsets.all(8.0),
                      child: Icon(
                        Icons.how_to_vote_rounded,
                        color: Colors.white,
                      ),
                    ),
                    textColor: Colors.white,
                    splashColor: const Color(0xff26375f),
                    color: const Color(0xffd82148),
                  ),
                ),
              ],
            ),
    );
  }
}
