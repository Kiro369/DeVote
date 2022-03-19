import 'package:devote/models/people.dart';
import 'package:devote/widgets/InfoPage.dart';
import 'package:flutter/material.dart';
import 'package:syncfusion_flutter_charts/charts.dart';
import 'package:flutter/foundation.dart' show kIsWeb;

class PieChart extends StatelessWidget {
  final List<people> motrsh7;
  const PieChart({required this.motrsh7});


  _buildChild(BuildContext context,int index) => Container(
    height: MediaQuery.of(context).size.height / 1.7,
    width:MediaQuery.of(context).size.width / 2 ,
    child: InfoPage(index: index,motrsh7e: [
      people('عبد الفتاح السيسي', 6589305, '65%'),
      people(' موسي مصطفي موسي', 335555, '35%')
    ]),
  );


  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          height: MediaQuery.of(context).size.height / 4.5,
          width: kIsWeb
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
          width: MediaQuery.of(context).size.width / 4,
          child: Column(
            children: [
              kIsWeb
                  ? InkWell(
                      onTap: () {
                        showDialog(
                            context: context,
                            builder: (context) {
                              return Dialog(
                                elevation: 0,
                                backgroundColor: Colors.transparent,
                                child: _buildChild(context,0),
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
              ListTile(
                title: Text(
                  motrsh7[1].nickname,
                  textAlign: TextAlign.right,
                ),
                trailing: Icon(
                  Icons.circle,
                  color: const Color(0xffd82148),
                ),
              )
            ],
          ),
        )
      ],
    );
  }
}

