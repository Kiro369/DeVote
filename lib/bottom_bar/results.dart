import 'package:devote/models/people.dart';
import 'package:devote/widgets/PieChart.dart';
import 'package:flutter/material.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:syncfusion_flutter_maps/maps.dart';

class Result extends StatefulWidget {
  const Result({Key? key}) : super(key: key);

  @override
  _ResultState createState() => _ResultState();
}

class _ResultState extends State<Result> {

  List<people> motrsh7en=[
    people('عبد الفتاح السيسي', 65),
    people(' موسي مصطفي موسي', 35)
  ];
  late List<Model> data;
  late MapShapeSource _mapSource;
  @override
  void initState() {

    data = const <Model> [
      Model('Kafr-El-Sheikh', const Color(0xffd00a16), 'كفر الشيخ'),
      Model('Al Fayyum (Fayoum)', const Color(0xffd00a16), 'الفيوم'),
      Model('Al Bahr/Al Ahmar (RedSea)',const Color(0xffd00a16), 'البحر الاحمر'),
      Model('Dumyat (Damietta)', const Color(0xffd00a16), 'دمياط'),
      Model('Matruh',  const Color(0xffd00a16), 'مطروح'),
      Model('Al Daqahliyah (Dakahlia)', const Color(0xffd00a16), 'الدقهلية'),
      Model('As Ismailiyah (Ismailia)', const Color(0xff6ca0ff), 'الاسماعيلية'),
      Model('Aswan', const Color(0xff6ca0ff), 'اسوان'),
      Model('Asyiut',  const Color(0xff6ca0ff), 'اسيوط'),
      Model('Beni Suwayf (Beni-Suef)', const Color(0xffd00a16), 'بني سويف'),
      Model('Qina',  const Color(0xff6ca0ff), 'قنا'),
      Model('Suhaj',  const Color(0xffd00a16), 'سوهاج'),
      Model('Al Iskandariyah (Alex.)', const Color(0xff6ca0ff), 'الاسكندرية'),
      Model('Al Qahirah (Cairo)', const Color(0xff6ca0ff), 'القاهرة'),
      Model('As Suways (Suez)', const Color(0xff6ca0ff), 'السويس'),
      Model('Bur Said (Port Said)', const Color(0xffd00a16), 'بورسعيد'),
      Model('Al Buhayrah (Behera)', const Color(0xffd00a16), 'البحيرة'),
      Model('Al Wadi/Al Jadid', const Color(0xffd00a16), 'الوادي الجديد'),
      Model('Janub Sina (South Sinai)', const Color(0xffd00a16), 'جنوب سيناء'),
      Model('Shamal Sina (North Sinai)', const Color(0xff6ca0ff), 'شمال سيناء'),
      Model('Al Minufiyah (Menoufia)', const Color(0xff6ca0ff), 'المنوفية'),
      Model('Al Gharbiyah (Gharbia)',const Color(0xffd00a16), 'الغربية'),
      Model('Al Qalyubiyah (Kalyoubia)', const Color(0xffd00a16), 'القليوبية'),
      Model('Ash Sharqiyah (Sharkia)',const Color(0xffd00a16), 'الشرقية'),
      Model('Al Jizah (Giza)',const Color(0xffd00a16), 'الجيزة'),
      Model('Al Minya (Menia)', const Color(0xff6ca0ff), 'المنيا'),

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
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        centerTitle: true,
        actions: [
          Padding(
            padding: const EdgeInsets.only(right: 10),
            child: Icon(Icons.check_box_outlined,color: Colors.white,size: 30,),
          )
        ],
        title: Text('نتائج الانتخابات الرئاسية',style: TextStyle(color: Colors.white),),
        backgroundColor: Colors.red[800],
      ),
      body: ListView(
        children: [
          Padding(
            padding: const EdgeInsets.all(12.0),
            child: Text('النتيجة الاجمالية',textAlign:TextAlign.center,style: TextStyle(fontSize: 18,fontWeight: FontWeight.bold),),
          ),
        PieChart(motrsh7: motrsh7en,),
          Padding(
            padding: const EdgeInsets.all(12.0),
            child: Text('النتيجة علي الخريطة',
              textAlign:TextAlign.center,
              style: TextStyle(fontSize: 18,fontWeight: FontWeight.bold),),
          ),
          //Padding(
            //padding: const EdgeInsets.all(15.0),
            //child: Image.asset('assets/preview.png'),
          //),
          Container(
            height: MediaQuery.of(context).size.height/2.5,
              child: SfMaps(
                layers: <MapShapeLayer>[
                  MapShapeLayer(
                    source: _mapSource,
                    showDataLabels: true,
                 //   legend: const MapLegend(MapElement.shape),
                    tooltipSettings: MapTooltipSettings(
                        color: Colors.grey[700],
                        strokeColor: Colors.white,
                        strokeWidth: 2),
                    strokeColor: Colors.white,
                    strokeWidth: 0.5,
                    shapeTooltipBuilder: (BuildContext context, int index) {
                      return Padding(
                        padding: const EdgeInsets.all(8.0),
                        child: Text(
                          data[index].stateCode,
                          style: const TextStyle(color: Colors.white),
                        ),
                      );
                    },
                    dataLabelSettings: MapDataLabelSettings(
                        textStyle: TextStyle(
                            color: Colors.black,
                           // fontWeight: FontWeight.bold,
                            fontSize:7)),
                  ),
                ],
              ),

          ),
          Padding(
            padding: const EdgeInsets.all(12.0),
            child: Text('BlockChain Explorer',textAlign:TextAlign.center,style: TextStyle(fontSize: 16,fontWeight: FontWeight.bold),),
          ),

        ],
      ),
    );
  }
}
/// Collection of Australia state code data.
class Model {
  /// Initialize the instance of the [Model] class.
  const Model(this.state, this.color, this.stateCode);

  /// Represents the Australia state name.
  final String state;

  /// Represents the Australia state color.
  final Color color;

  /// Represents the Australia state code.
  final String stateCode;
}