import 'package:flutter/material.dart';
import 'package:animated_text_kit/animated_text_kit.dart';
class Amaken extends StatefulWidget {
  const Amaken({Key? key}) : super(key: key);

  @override
  _AmakenState createState() => _AmakenState();
}

class _AmakenState extends State<Amaken> {
  String dropdownValue='الفيوم';
  var School =[
    {'city': 'cairo', 'school': 'marymor2s'},
    {'city': 'cairo', 'school': 'Aled'},
  ];
  List<String> cities =['الفيوم','الاسماعيلية','بورسعيد','شمال سيناء','جنوب سيناء','كفر الشيخ','الجيزة','دمياط','المنوفية','الاسكندرية','القاهرة'];
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        actions: [
          Padding(
            padding: const EdgeInsets.only(right: 10),
            child: Icon(Icons.check_box_outlined,color: Colors.white,size: 30,),
          )
        ],
        centerTitle: true,
        title: Text('اماكن الانتخابات',style: TextStyle(color: Colors.white),),
        backgroundColor: Colors.red[800],
        bottom: PreferredSize(
            preferredSize: Size.fromHeight(40.0),
          child: Container(
            width: MediaQuery.of(context).size.width,
            color: Colors.yellow[700],
            height: 40.0,
            child: DefaultTextStyle(
              textAlign: TextAlign.right,
              style: const TextStyle(
                fontSize: 16.0,
                color: const Color(0xff000000),
              ),
              child: AnimatedTextKit(
               repeatForever: true,
                  animatedTexts:[
                    RotateAnimatedText('موعد الانتخابات الرئاسية 26-27 يوليو'),
                    RotateAnimatedText('تتواجد مكن الانتخابات في جميع انحاء الجمهورية'),
                    RotateAnimatedText('يرجي الذهاب للانتخابات ببطاقة الرقم القومي'),
                  ],
                  isRepeatingAnimation: true,
            ),
      ),
          ),
        ),
    ),
        body: ListView(
            children: [
              Center(
                child: DropdownButton<String>(
                 alignment: Alignment.topRight,
                  //hint: Text('اختر المحافظة',textAlign: TextAlign.right,),
                  value: dropdownValue,
                  onChanged: (String? newValue) {
                    setState(() {
                      dropdownValue = newValue!;
                      print(newValue);
                    });
                  },
                  items: cities.map((String value) {
                    return DropdownMenuItem<String>(
                      value: value,
                      child: Text(value,textAlign: TextAlign.right,),
                    );
                  }).toList(),
                ),
              ),
            ],
    ),
    );
  }
}
