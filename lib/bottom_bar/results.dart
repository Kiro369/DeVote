import 'package:devote/models/people.dart';
import 'package:devote/widgets/MapChart.dart';
import 'package:devote/widgets/PieChart.dart';
import 'package:devote/widgets/blockchain.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
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
  List<people> motrsh7en = [
    people('عبد الفتاح السيسي', 6589305, '65%'),
    people(' موسي مصطفي موسي', 335555, '35%')
  ];
  late List<Model> data;
  late MapShapeSource _mapSource;
final Color color2=const Color(0xffd82148);
final Color color1=const Color(0xff26375f);

  late LinkedScrollControllerGroup _controllers;
  late ScrollController _letters;
  late ScrollController _numbers;

  @override
  void initState() {
    _controllers = LinkedScrollControllerGroup();
    _letters = _controllers.addAndGet();
    _numbers = _controllers.addAndGet();
    data =  <Model>[
      Model('Kafr-El-Sheikh', color1, 'كفر الشيخ'),
      Model('Al Fayyum (Fayoum)',color1, 'الفيوم'),
      Model('Al Bahr/Al Ahmar (RedSea)', color1, 'البحر الاحمر'),
      Model('Dumyat (Damietta)',color1, 'دمياط'),
      Model('Matruh', color1, 'مطروح'),
      Model('Al Daqahliyah (Dakahlia)',color1, 'الدقهلية'),
      Model('As Ismailiyah (Ismailia)', color2, 'الاسماعيلية'),
      Model('Aswan', color2, 'اسوان'),
      Model('Asyiut', color2, 'اسيوط'),
      Model('Beni Suwayf (Beni-Suef)', color2, 'بني سويف'),
      Model('Qina', color2, 'قنا'),
      Model('Suhaj', color1, 'سوهاج'),
      Model('Al Iskandariyah (Alex.)', color2, 'الاسكندرية'),
      Model('Al Qahirah (Cairo)', color2, 'القاهرة'),
      Model('As Suways (Suez)', color2, 'السويس'),
      Model('Bur Said (Port Said)',color1, 'بورسعيد'),
      Model('Al Buhayrah (Behera)', color1, 'البحيرة'),
      Model('Al Wadi/Al Jadid', color1, 'الوادي الجديد'),
      Model('Janub Sina (South Sinai)', color1, 'جنوب سيناء'),
      Model('Shamal Sina (North Sinai)',color2, 'شمال سيناء'),
      Model('Al Minufiyah (Menoufia)', color2, 'المنوفية'),
      Model('Al Gharbiyah (Gharbia)', color1, 'الغربية'),
      Model('Al Qalyubiyah (Kalyoubia)', color1, 'القليوبية'),
      Model('Ash Sharqiyah (Sharkia)', color1, 'الشرقية'),
      Model('Al Jizah (Giza)', color1, 'الجيزة'),
      Model('Al Minya (Menia)',color2, 'المنيا'),
    ];
    _mapSource = MapShapeSource.asset(
      'assets/egypt2.json',
      shapeDataField: 'adm2',
      dataCount: data.length,
      primaryValueMapper: (int index) => data[index].state,
      dataLabelMapper: (int index) => data[index].stateCode,
      shapeColorValueMapper: (int index) => data[index].color,
    );
    super.initState();
  }
  @override
  void dispose() {
    _letters.dispose();
    _numbers.dispose();
    super.dispose();
  }
  static bool isSmallScreen(BuildContext context) {
    return MediaQuery.of(context).size.width < 1000;
  }

  static bool isLargeScreen(BuildContext context) {
    return MediaQuery.of(context).size.width > 1200;
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
        leading:kIsWeb?Padding(
          padding: const EdgeInsets.only(left: 18.0),
          child: IconButton(onPressed: () =>showDialog(
              context: context,
              builder: (context) {
                return Dialog(
                  elevation: 1,
                  backgroundColor: Colors.transparent,
                  child:Container(
                      height: MediaQuery.of(context).size.height / 2.5,
                      width: MediaQuery.of(context).size.width / 3.2,
                      decoration: BoxDecoration(
                        color: const Color(0xff26375f),
                        borderRadius: BorderRadius.all(Radius.circular(15)),
                      ),child: Column(children: [
                        Center(child: Padding(
                          padding: const EdgeInsets.all(14.0),
                          child: Icon(Icons.warning,size: 50,color:const Color(0xfff3f3f5)),
                        )),
                        Padding(
                          padding: const EdgeInsets.all(15.0),
                          child: Text('يمكنك معرفة امكان مكن الانتخابات الالكترونية من خلال تطبيقنا الخاص DeVote علي GooglePlay او AppStore',style: TextStyle(
                            fontSize: 18,
                            color:const Color(0xfff3f3f5),
                          ),textDirection: TextDirection.rtl,),
                        ),
                      Center(
                        child: Padding(
                          padding: const EdgeInsets.only(top:14.0),
                          child: FlatButton(color: const Color(0xfff3f3f5),onPressed: ()=> Navigator.of(context)
                            .pop(), child: Text('موافق',style: TextStyle(color: Color(0xff26375f)),)),
                        ),
                      )
                      ],))
                );
              })
             , icon: Icon(Icons.location_on,size: 30,color: Colors.black,)),
        ):Text('') ,
      ),
      body: kIsWeb&&isLargeScreen(context)? Scrollbar(
        isAlwaysShown: true,
        scrollbarOrientation: ScrollbarOrientation.right,
        controller: _letters,
        child: ScrollConfiguration(
          behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false),
          child: Row(
            children: [
              SizedBox(width: MediaQuery.of(context).size.width/2.2,child: Padding(
                padding: const EdgeInsets.only(bottom: 8.0),
                child: BlockChain( _letters,),
              )),
              Container(
                width: MediaQuery.of(context).size.width/2,
                child: ListView(
                  controller: _numbers,
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
                  ],
                ),
              ),
            ],
          ),
        ),
      ):
      ListView(
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
          Padding(
            padding: const EdgeInsets.all(12.0),

            child: RaisedButton.icon(
              onPressed: () => Navigator.of(context).push( MaterialPageRoute(
                  builder: (BuildContext context) =>  BlockChain())),
              shape:const RoundedRectangleBorder(
                  borderRadius: BorderRadius.all(Radius.circular(10.0))),
              label:const Text(
                'متابعة العملية الانتخابية بشكل مباشر',
                style: TextStyle(color: Colors.white, fontSize: 16),
              ),
              icon: const Padding(
                padding:  EdgeInsets.all(8.0),
                child: Icon(
                  Icons.how_to_vote_rounded,
                  color: Colors.white,
                ),
              ),
              textColor: Colors.white,
              splashColor:const Color(0xff26375f),
              color:const Color(0xffd82148),
            ),
          ),
        ],
      ),
    );
  }
}
