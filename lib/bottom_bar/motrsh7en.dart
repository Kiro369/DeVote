import 'package:devote/models/people.dart';
import 'package:devote/widgets/InfoPage.dart';
import 'package:flutter/material.dart';

class Motrsh7en extends StatefulWidget {
  const Motrsh7en({Key? key}) : super(key: key);

  @override
  _Motrsh7enState createState() => _Motrsh7enState();
}

class _Motrsh7enState extends State<Motrsh7en> {
  List<people> motrsh7e=[
    people(
        'عبد الفتاح السيسي'
        ,296000,'65%',
        'عبد الفتاح سعد خليل ',
      'assets/cc.jpg','نجمة',
      Icon(Icons.star,color: Colors.yellow[600],),
      'ولد في 19 نوفمبر 1954 في القاهرة ، متزوج وله 4 أبناء\n تخرج من الكليـة الحربيــة (بكالوريوس العلوم العسكرية) في إبريل 1977\n ترقى الى رتبة فريق أول وعين قائدا عاما للقوات المسلحة ووزيراً للدفاع والإنتاج الحربي منذ 12 أغسطس 2012'
    ),
    people('موسي مصطفي موسي'
        ,2400,'35%',
      'موسي مصطفي موسي محمد',
      'assets/moussa.jpg', 'طائرة',
      Icon(Icons.airplanemode_on_sharp),
      'ولد في إبريل 1952، متزوج ولديه ثلاثة أبناء\n وهو رجل أعمال ويرأس مجلس إدارة إحدى الشركات الكبرى في مصر\n يرأس موسى حزب الغد وإتحاد القبائل العربية والتحالف السياسى المصرى الذي يضم 18 حزبًا سياسيًا.'
    ),
  ];
  @override
  Widget build(BuildContext context) {
    return DefaultTabController(
      length: 2,
      initialIndex: 1,
      child: Scaffold(
        backgroundColor: Colors.white,
        appBar: AppBar(
          actions: [
            Padding(
              padding: const EdgeInsets.only(right: 10),
              child: Icon(Icons.check_box_outlined,color: Colors.white,size: 30,),
            ),
          ],
          centerTitle: true,
          title: Text('المرشحون للانتخابات',style: TextStyle(color: Colors.white),),
          backgroundColor: Color(0xff26375f),
          bottom: TabBar(
            indicatorColor:Color(0xfff1f2f4),
            tabs: <Widget>[
              Tab(text: 'المرشح الثاني'),
              Tab(text: 'المرشح الاول'),
            ],
          ),
        ),
        body: TabBarView(
            children: [InfoPage(motrsh7e: motrsh7e, index: 1),
              InfoPage(motrsh7e: motrsh7e, index: 0)],
      ),
      ),
    );
  }
}
