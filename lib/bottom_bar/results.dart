import 'package:flutter/material.dart';
import 'package:syncfusion_flutter_charts/charts.dart';

class Result extends StatefulWidget {
  const Result({Key? key}) : super(key: key);

  @override
  _ResultState createState() => _ResultState();
}

class _ResultState extends State<Result> {
  List<_people> motrsh7en=[
    _people('عبد الفتاح السيسي', 2000),
    _people('موسي مصطفي', 200)
  ];
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
          Row(
            children: [
               Container(
                height: MediaQuery.of(context).size.height/4.5,
                width: MediaQuery.of(context).size.width/2,
                //Initialize the chart widget
                child: SfCircularChart(
                    backgroundColor: Colors.white,
                    palette: [
                   const Color(0xffd00a16),
                    const Color(0xff6ca0ff)
                         ],
                    borderColor: Colors.white,
                    onDataLabelRender: (DataLabelRenderArgs args){
                      double value = double.parse(args.text);
                      args.text = value.toStringAsFixed(0);
                    },
                    series: <CircularSeries<_people, String>>[
                      PieSeries<_people, String>(
                          selectionBehavior: SelectionBehavior(enable: true),
                          explode: true,
                          dataSource: motrsh7en,
                          xValueMapper: (_people sales, _) => sales.name,
                          yValueMapper: (_people sales, _) => sales.result,
                          name: 'Sales',
                          dataLabelSettings: DataLabelSettings(isVisible: true,)
                      )
                    ]),
              ),
              Container(

                width: MediaQuery.of(context).size.width/2,
                child: Column(

                  children: [
                    ListTile(
                      title: Text('عبد الفتاح السيسي',textAlign: TextAlign.right,),
                      trailing: Icon(Icons.circle,color: const Color(0xffd00a16) ,),
                    ),
                    ListTile(
                      title: Text(' موسي مصطفي موسي',textAlign: TextAlign.right,),
                      trailing: Icon(Icons.circle,color: const Color(0xff6ca0ff) ,),
                    )
                  ],
                ),
              )
            ],),
          Padding(
            padding: const EdgeInsets.all(12.0),
            child: Text('النتيجة علي الخريطة',textAlign:TextAlign.center,style: TextStyle(fontSize: 18,fontWeight: FontWeight.bold),),
          ),
          Padding(
            padding: const EdgeInsets.all(15.0),
            child: Image.asset('assets/preview.png'),
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
class _people {
  _people(this.name, this.result);

  final String name;
  final double result;
}