import 'package:flutter/material.dart';
import 'package:syncfusion_flutter_maps/maps.dart';
import 'package:flutter/foundation.dart' show kIsWeb;

import '../models/mapModel.dart';
class MapChart extends StatelessWidget {
  late final List<Model> data;
  late final MapShapeSource mapSource;

   MapChart({ required this.data, required this.mapSource}) ;

  @override
  Widget build(BuildContext context) {
    return Container(
      height: kIsWeb? MediaQuery.of(context).size.height/1.8:MediaQuery.of(context).size.height/2.5,
      child: SfMaps(
        layers: <MapShapeLayer>[
          MapShapeLayer(
            source: mapSource,
            showDataLabels: true,
            //   legend: const MapLegend(MapElement.shape),
            tooltipSettings:const MapTooltipSettings(
                color: Colors.white,
                strokeColor: Color(0xfff1f2f4),
                strokeWidth: 2),
            strokeColor:const Color(0xfff1f2f4),
            strokeWidth: 0.5,
            shapeTooltipBuilder: (BuildContext context, int index) {
              return Padding(
                padding: const EdgeInsets.all(8.0),
                child: FittedBox(
                  child: Column(
                    children: [
                      Center(
                        child: Text(
                          data[index].stateCode,
                          style: const TextStyle(color: Colors.black),
                        ),
                      ),

                      SizedBox(
                        width: kIsWeb? MediaQuery.of(context).size.width/4.4:MediaQuery.of(context).size.width/2.2,
                        child: ListTile(
                          trailing: Icon(Icons.check_circle,color: const Color(0xff26375f),size: 16,),
                          title:  Text(
                            'السيسي ',textAlign: TextAlign.right,
                            style:  TextStyle(color: Colors.black,fontSize: 12),
                          ),
                          leading: Padding(
                            padding: const EdgeInsets.only(top: 8.0),
                            child: Text(
                              '873988937 صوت',textAlign: TextAlign.left,textDirection: TextDirection.rtl,
                              style:  TextStyle(color: Colors.black,fontSize: 9,),
                            ),
                          ),
                        ),
                      ),
                      SizedBox(
                        width:kIsWeb? MediaQuery.of(context).size.width/4.4:MediaQuery.of(context).size.width/2.2,
                        child: ListTile(
                          trailing: Icon(Icons.check_circle,color: const Color(
                              0xffffffff),size: 0,),
                          title:  Text(
                            'موسي ',textAlign: TextAlign.right,
                            style:  TextStyle(color: Colors.black,fontSize: 12),
                          ),
                          leading: Padding(
                            padding: const EdgeInsets.only(top: 8.0),
                            child: Text(
                              ' 98937 صوت',textAlign: TextAlign.left,textDirection: TextDirection.rtl,
                              style:  TextStyle(color: Colors.black,fontSize: 9,),
                            ),
                          ),
                        ),
                      ),

                    ],
                  ),
                ),
              );
            },
            dataLabelSettings: const MapDataLabelSettings(
                textStyle: TextStyle(
                    color:Color(0xfff1f2f4),
                    // fontWeight: FontWeight.bold,
                    fontSize:7)),
          ),
        ],
      ),

    );
  }
}
