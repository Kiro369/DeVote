import 'package:devote/models/people.dart';

class MapModel {
  final List<Model> model;

  MapModel(this.model);

  factory MapModel.fromJson(Map<String, dynamic> json) {
    var list = json['result'] as List;
    List<Model> result = list.map((i) => Model.fromJson(i)).toList();
    return MapModel(result);
  }
}

class Model {
  final String state;
  final int color;

  final String stateCode;
  final List<ResultsChart> votes;

  const Model(this.state, this.color, this.stateCode, this.votes);

  factory Model.fromJson(Map<String, dynamic> json) {
    var list = json['Votes'] as List;
    List<ResultsChart> result =
        list.map((i) => ResultsChart.fromJson(i)).toList();
    return Model(
        json['EnglishName'], json['Color'], json['ArabicName'], result);
  }
}
