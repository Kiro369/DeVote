import 'package:flutter/material.dart';
import 'package:syncfusion_flutter_maps/maps.dart';

import '../models/mapModel.dart';
class MapChart extends StatelessWidget {
  late final List<Model> data;
  late final MapShapeSource mapSource;

   MapChart({ required this.data, required this.mapSource}) ;

  @override
  Widget build(BuildContext context) {
    return Container(
      height: MediaQuery.of(context).size.height/2.5,
      child: SfMaps(
        layers: <MapShapeLayer>[
          MapShapeLayer(
            source: mapSource,
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
                child: FittedBox(
                  child: Column(
                    children: [
                      Text(
                        data[index].stateCode,
                        style: const TextStyle(color: Colors.white),
                      ),
                      Text(
                        'السيسي 873988937 صوت',textAlign: TextAlign.right,
                        style: const TextStyle(color: Colors.white),
                      ),
                      Text(
                        'موسي 24%',textAlign: TextAlign.right,
                        style: const TextStyle(color: Colors.white),
                      ),
                    ],
                  ),
                ),
              );
            },
            dataLabelSettings: MapDataLabelSettings(
                textStyle: TextStyle(
                    color: Colors.black,
                    // fontWeight: FontWeight.bold,
                    fontSize:7)),
          ),
        ],
      ),

    );
  }
}
