import 'package:devote/models/people.dart';
import 'package:devote/widgets/InfoPage.dart';
import 'package:flutter/material.dart';
import 'package:syncfusion_flutter_charts/charts.dart';
import 'package:flutter/foundation.dart' show kIsWeb;

class PieChart extends StatelessWidget {
  final List<people> motrsh7;
  const PieChart({required this.motrsh7});
  static bool isSmallScreen(BuildContext context) {
    return MediaQuery.of(context).size.width < 1000;
  }

  static bool isLargeScreen(BuildContext context) {
    return MediaQuery.of(context).size.width > 1200;
  }



  _buildChild(BuildContext context,int index,Color color) => Container(
    height: MediaQuery.of(context).size.height / 1.2,
    width:isSmallScreen(context)?MediaQuery.of(context).size.width / 1.3:MediaQuery.of(context).size.width / 2,
    decoration: BoxDecoration(
      color: color,
      borderRadius: BorderRadius.all(Radius.circular(15)),
    ),
    child: InfoPage(index: index,motrsh7e:[
      people(
          'عبد الفتاح السيسي'
          ,296000,'65%',
          'عبد الفتاح سعد خليل ',
          'assets/cc.jpg','نجمة',
          Icon(Icons.star,),
          'ولد في 19 نوفمبر 1954 في القاهرة ، متزوج وله 4 أبناء\n تخرج من الكليـة الحربيــة (بكالوريوس العلوم العسكرية) في إبريل 1977\n ترقى الى رتبة فريق أول وعين قائدا عاما للقوات المسلحة ووزيراً للدفاع والإنتاج الحربي منذ 12 أغسطس 2012'
      ),
      people('موسي مصطفي موسي'
          ,2400,'35%',
          'موسي مصطفي موسي محمد',
          'assets/moussa.jpg', 'طائرة',
          const Icon(Icons.airplanemode_on_sharp,),
          'ولد في إبريل 1952، متزوج ولديه ثلاثة أبناء\n وهو رجل أعمال ويرأس مجلس إدارة إحدى الشركات الكبرى في مصر\n يرأس موسى حزب الغد وإتحاد القبائل العربية والتحالف السياسى المصرى الذي يضم 18 حزبًا سياسيًا.'
      ),
    ]),
  );


  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          height: MediaQuery.of(context).size.height / 4.5,
          width: kIsWeb&&isLargeScreen(context)
              ? MediaQuery.of(context).size.width / 4
              : MediaQuery.of(context).size.width / 2,
          //Initialize the chart widget
          child: SfCircularChart(
            tooltipBehavior: TooltipBehavior(
                enable: true, tooltipPosition: TooltipPosition.pointer),
            backgroundColor: Colors.white,
            palette: [const Color(0xff26375f), const Color(0xffd82148)],
            borderColor: Colors.white,
            onDataLabelRender: (DataLabelRenderArgs args) {
              double value = double.parse(args.text);
              args.text = value.toStringAsFixed(0);
            },
            series: <CircularSeries<people, String>>[
              PieSeries<people, String>(
                  selectionBehavior: SelectionBehavior(enable: true),
                  explode: true,
                  dataSource: motrsh7,
                  xValueMapper: (people sales, _) => sales.nickname,
                  yValueMapper: (people sales, _) => sales.result,
                  //  dataLabelMapper: (people sales, _) =>sales.per,
                  name: 'الانتخابات الرئاسية',
                  dataLabelSettings: DataLabelSettings(
                    isVisible: true,
                    labelPosition: ChartDataLabelPosition.outside,
                  ))
            ],
          ),
        ),
        Container(
          width: kIsWeb&&isLargeScreen(context) ?MediaQuery.of(context).size.width / 4:MediaQuery.of(context).size.width / 2,
          child: Column(
            children: [
              kIsWeb
                  ? InkWell(
                      onTap: () {
                        showDialog(
                            context: context,
                            builder: (context) {
                              return Dialog(
                                elevation: 1,
                                backgroundColor: Colors.transparent,
                                child: _buildChild(context,0, Color(0xff26375f)),
                              );
                            });
                      },
                      child: ListTile(
                        title: Text(
                          motrsh7[0].nickname,
                          textAlign: TextAlign.right,
                        ),
                        trailing: Icon(
                          Icons.circle,
                          color: const Color(0xff26375f),
                        ),
                      ),
                    )
                  : ListTile(
                      title: Text(
                        motrsh7[0].nickname,
                        textAlign: TextAlign.right,
                      ),
                      trailing: Icon(
                        Icons.circle,
                        color: const Color(0xff26375f),
                      ),
                    ),
              kIsWeb
                  ? InkWell(
                onTap: () {
                  showDialog(
                      context: context,
                      builder: (context) {
                        return Dialog(
                          elevation: 1,
                          backgroundColor: Colors.transparent,
                          child: _buildChild(context,1, Color(0xffd82148)),
                        );
                      });
                },
                child: ListTile(
                  title: Text(
                    motrsh7[1].nickname,
                    textAlign: TextAlign.right,
                  ),
                  trailing: Icon(
                    Icons.circle,
                    color: const Color(0xffd82148),
                  ),
                ),
              ):ListTile(
                title: Text(
                  motrsh7[1].nickname,
                  textAlign: TextAlign.right,
                ),
                trailing: Icon(
                  Icons.circle,
                  color: const Color(0xffd82148),
                ),
              ),
            ],
          ),
        )
      ],
    );
  }
}

