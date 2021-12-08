import 'package:devote/models/people.dart';
import 'package:devote/widgets/PieChart.dart';
import 'package:flutter/material.dart';


class Result extends StatefulWidget {
  const Result({Key? key}) : super(key: key);

  @override
  _ResultState createState() => _ResultState();
}

class _ResultState extends State<Result> {
  List<people> motrsh7en=[
    people('عبد الفتاح السيسي', 2000),
    people(' موسي مصطفي موسي', 200)
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
        PieChart(motrsh7: motrsh7en,),
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
