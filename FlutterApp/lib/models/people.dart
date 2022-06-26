import 'package:flutter/cupertino.dart';

class People {
  final String nickname;
  final Icon? code;
  final String? codestr;
  final String? info;
  final String? image;
  final String? name;
  final num result;
  final String id;

  People(
    this.nickname,
    this.result,
    this.id, [
    this.name,
    this.image,
    this.codestr,
    this.code,
    this.info,
  ]);
}

class PieResult {
  final List<ResultsChart> pie;

  PieResult(this.pie);

  factory PieResult.fromJson(Map<String, dynamic> json) {
    var list = json['result'] as List;
    List<ResultsChart> result =
        list.map((i) => ResultsChart.fromJson(i)).toList();
    return PieResult(result);
  }
}

class ResultsChart {
  final String id;
  final String nickname;
  final num result;

  ResultsChart(this.id, this.nickname, this.result);

  factory ResultsChart.fromJson(Map<String, dynamic> json) {
    return ResultsChart(
        json['ID'] ?? '', json['Name'] ?? '', json['NoVotes'] ?? '');
  }
}
