import 'package:flutter/material.dart';
import 'package:syncfusion_flutter_maps/maps.dart';
import 'package:flutter/foundation.dart' show kIsWeb;

import '../models/mapModel.dart';
class MapChart extends StatelessWidget {
   final List<Model> data;
   final MapShapeSource mapSource;

   MapChart({ required this.data, required this.mapSource}) ;
  static bool isLargeScreen(BuildContext context) {
    return MediaQuery.of(context).size.width > 1200;
  }

  @override
  Widget build(BuildContext context) {
    return SizedBox(
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
                          width: kIsWeb&&isLargeScreen(context)? MediaQuery.of(context).size.width/4.4:MediaQuery.of(context).size.width/2.1,
                          child: ListTile(
                            trailing: Icon(Icons.check_circle,color:  data[index].votes[0].result>data[index].votes[1].result?const Color(0xff26375f):Colors.white,size: 16,),
                            title:const Text(
                              'السيسي ',textAlign: TextAlign.right,
                              style:  TextStyle(color: Colors.black,fontSize: 12),
                            ),
                            leading: Padding(
                              padding: const EdgeInsets.only(top: 8.0),
                              child: Text(
                                ' ${data[index].votes[0].result} صوت ',textAlign: TextAlign.left,textDirection: TextDirection.rtl,
                                style:  const TextStyle(color: Colors.black,fontSize: 9,),
                              ),
                            ),
                          ),

                      ),
                      SizedBox(
                        width:kIsWeb&&isLargeScreen(context)? MediaQuery.of(context).size.width/4.4:MediaQuery.of(context).size.width/2.2,
                        child: ListTile(
                          trailing: Icon(Icons.check_circle,color: data[index].votes[1].result>data[index].votes[0].result?const Color(0xffd82148):Colors.white,size: 16,),
                          title: const Text(
                            'موسي ',textAlign: TextAlign.right,
                            style:  TextStyle(color: Colors.black,fontSize: 12),
                          ),
                          leading: Padding(
                            padding: const EdgeInsets.only(top: 8.0),
                            child: Text(
                              ' ${data[index].votes[1].result} صوت ',textAlign: TextAlign.left,textDirection: TextDirection.rtl,
                              style:  const TextStyle(color: Colors.black,fontSize: 9,),
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
