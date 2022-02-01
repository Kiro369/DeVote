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
    people('عبد الفتاح السيسي', 55),
    people(' موسي مصطفي موسي', 30)
  ];
  late List<Model> data;
  late MapShapeSource _mapSource;
  @override
  void initState() {

    data = const <Model> [
      Model('Kafr-El-Sheikh', Color.fromRGBO(155, 239, 216, 1.0), 'كفر الشيخ'),
      Model('Al Fayyum (Fayoum)', Colors.red, 'الفيوم'),
      Model('Al Bahr/Al Ahmar (RedSea)',Color.fromRGBO(32, 109, 04, 1.0), 'البحر الاحمر'),
      Model('Dumyat (Damietta)', Color.fromRGBO(171, 56, 224, 0.75), 'دمياط'),
      Model('Matruh', Color.fromRGBO(126, 247, 74, 0.75), 'مطروح'),
      Model('Al Daqahliyah (Dakahlia)', Color.fromRGBO(79, 60, 201, 0.7), 'الدهلقية'),
      Model('As Ismailiyah (Ismailia)', Color.fromRGBO(99, 164, 230, 1), 'الاسماعيلية'),
      Model('Aswan', Colors.teal, 'اسوان'),
      Model('Asyiut', Colors.teal, 'اسيوط'),
      Model('Beni Suwayf (Beni-Suef)', Colors.teal, 'بني سويف'),
      Model('Qina', Colors.teal, 'قنا'),
      Model('Suhaj', Colors.teal, 'سوهاج'),
      Model('Al Iskandariyah (Alex.)', Colors.teal, 'الاسكندرية'),
      Model('Al Qahirah (Cairo)', Colors.teal, 'القاهرة'),
      Model('As Suways (Suez)', Colors.teal, 'السويس'),
      Model('Bur Said (Port Said)', Colors.teal, 'بورسعيد'),
      Model('Al Buhayrah (Behera)', Colors.teal, 'البحيرة'),
      Model('Al Wadi/Al Jadid', Colors.teal, 'الوادي الجديد'),
      Model('Janub Sina (South Sinai)', Colors.teal, 'جنوب سيناء'),
      Model('Shamal Sina (North Sinai)', Colors.teal, 'شمال سيناء'),
      Model('Al Minufiyah (Menoufia)', Colors.teal, 'المنوفية'),
      Model('Al Gharbiyah (Gharbia)', Colors.teal, 'الغربية'),
      Model('Al Qalyubiyah (Kalyoubia)', Colors.teal, 'القليوبية'),
      Model('Ash Sharqiyah (Sharkia)', Colors.red, 'الشرقية'),
      Model('Al Jizah (Giza)', Colors.red, 'الجيزة'),
      Model('Al Minya (Menia)', Colors.teal, 'المنيا'),

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
            height: MediaQuery.of(context).size.height/2,
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
                          fontWeight: FontWeight.bold,
                          fontSize:8)),
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