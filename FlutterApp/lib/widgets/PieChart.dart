import '/models/people.dart';
import '/widgets/InfoPage.dart';
import 'package:flutter/material.dart';
import 'package:syncfusion_flutter_charts/charts.dart';
import 'package:flutter/foundation.dart' show kIsWeb;

class PieChart extends StatefulWidget {

  final List<ResultsChart> motrsh7;

  const PieChart({required this.motrsh7});

  @override
  _PieChartState createState() => _PieChartState();
}

class _PieChartState extends State<PieChart> {


  static bool isSmallScreen(BuildContext context) {
    return MediaQuery.of(context).size.width < 1000;
  }

  static bool isLargeScreen(BuildContext context) {
    return MediaQuery.of(context).size.width > 1200;
  }

  _buildChild(BuildContext context, int index, Color color) => Container(
        height: MediaQuery.of(context).size.height / 1.2,
        width: isSmallScreen(context)
            ? MediaQuery.of(context).size.width / 1.3
            : MediaQuery.of(context).size.width / 2,
        decoration: BoxDecoration(
          color: color,
          borderRadius: const BorderRadius.all(Radius.circular(15)),
        ),
        child: InfoPage(index: index, motrsh7e: [
          People(
              'عبد الفتاح السيسي',
              296000,
              '65%',
              'عبد الفتاح سعد خليل ',
              'assets/cc.jpg',
              'نجمة',
              const Icon(
                Icons.star,
              ),
              'ولد في 19 نوفمبر 1954 في القاهرة ، متزوج وله 4 أبناء\n تخرج من الكليـة الحربيــة (بكالوريوس العلوم العسكرية) في إبريل 1977.\n ترقى الى رتبة فريق أول وعين قائدا عاما للقوات المسلحة ووزيراً للدفاع والإنتاج الحربي منذ 12 أغسطس 2012.'),
          People(
              'موسي مصطفي موسي',
              2400,
              '35%',
              'موسي مصطفي موسي محمد',
              'assets/moussa.jpg',
              'طائرة',
              const Icon(
                Icons.airplanemode_on_sharp,
              ),
              'ولد في إبريل 1952، متزوج ولديه ثلاثة أبناء وهو رجل أعمال ويرأس مجلس إدارة إحدى الشركات الكبرى في مصر.\n يرأس موسى حزب الغد وإتحاد القبائل العربية والتحالف السياسى المصرى الذي يضم 18 حزبًا سياسيًا.'),
        ]),
      );

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        SizedBox(
          height: MediaQuery.of(context).size.height / 4.5,
          width: kIsWeb && isLargeScreen(context)
              ? MediaQuery.of(context).size.width / 4
              : MediaQuery.of(context).size.width / 2.2,
          //Initialize the chart widget
          child: widget.motrsh7[0].result==0&&widget.motrsh7[1].result==0?Center(
              child: const Text('لم تبدأ \nعملية التصويت بعد',textAlign: TextAlign.center,)):SfCircularChart(
            tooltipBehavior: TooltipBehavior(
              header: 'الانتخابات الرئاسية 2018',
                format: '  point.x : point.y صوت ',
                enable: true,
                tooltipPosition: TooltipPosition.pointer),
            backgroundColor: Colors.white,
            palette:const [ Color(0xff26375f), Color(0xffd82148)],
            borderColor: Colors.white,
            onDataLabelRender: (DataLabelRenderArgs args) {
              double val=double.parse(args.text);
                double value =val /(widget.motrsh7[0].result+widget.motrsh7[1].result)*100;
                args.text = value.toStringAsFixed(0)+'%';
            },
            series: <CircularSeries<ResultsChart, String>>[
              PieSeries<ResultsChart, String>(
                  selectionBehavior: SelectionBehavior(enable: true),
                  explode: true,
                  dataSource: widget.motrsh7,
                  xValueMapper: (ResultsChart sales, _) => sales.nickname,
                  yValueMapper: (ResultsChart sales, _) => sales.result,
                   //dataLabelMapper: (people sales, _) =>sales.per,
                  name: 'الانتخابات الرئاسية',
                  dataLabelSettings: const DataLabelSettings(
                    isVisible: true,
                    labelPosition: ChartDataLabelPosition.inside,
                  ))
            ],
          ),
        ),
        SizedBox(
          width: kIsWeb && isLargeScreen(context)
              ? MediaQuery.of(context).size.width / 4
              : MediaQuery.of(context).size.width / 1.85,
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
                                child:
                                    _buildChild(context, 0, const Color(0xff26375f)),
                              );
                            });
                      },
                      child: ListTile(
                        title: Text(
                          widget.motrsh7[0].nickname,
                          textAlign: TextAlign.right,
                        ),
                        trailing: const Icon(
                          Icons.circle,
                          color:  Color(0xff26375f),
                        ),
                      ),
                    )
                  : ListTile(
                      title: Text(
                        widget.motrsh7[0].nickname,
                        textAlign: TextAlign.right,
                      ),
                      trailing: const Icon(
                        Icons.circle,
                        color:  Color(0xff26375f),
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
                                child:
                                    _buildChild(context, 1, const Color(0xffd82148)),
                              );
                            });
                      },
                      child: ListTile(
                        title: Text(
                          widget.motrsh7[1].nickname,
                          textAlign: TextAlign.right,
                        ),
                        trailing:const Icon(
                          Icons.circle,
                          color:  Color(0xffd82148),
                        ),
                      ),
                    )
                  : ListTile(
                      title: Text(
                        widget.motrsh7[1].nickname,
                        textAlign: TextAlign.right,
                      ),
                      trailing: const Icon(
                        Icons.circle,
                        color:  Color(0xffd82148),
                      ),
                    ),
            ],
          ),
        )
      ],
    );
  }
}
