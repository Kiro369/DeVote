import 'dart:convert';

import 'package:devote/models/transaction.dart';
import 'package:http/http.dart' as http;

import 'block.dart';

class CallApi{
  final dynamic url;

  CallApi(this.url);
  Future<List<Result>> getTransaction() async {
    http.Response transactionFuture = await http.get(url);
    if (transactionFuture.statusCode == 200) {
      final jsonResponse = json.decode(transactionFuture.body);
      Transaction transacion = new Transaction.fromJson(jsonResponse);
      //Transaction.fromJson(json.decode(transactionFuture.body))
      return transacion.transaction;
    } else {
      throw Exception('Can not load data');
    }
  }
  Future<List<Results>> getBlocks() async {
    http.Response transactionFuture = await http.get(url);
    if (transactionFuture.statusCode == 200) {
      final jsonResponse = json.decode(transactionFuture.body);
      Block transacion = new Block.fromJson(jsonResponse);
      return transacion.blocks ;
    } else {
      throw Exception('Can not load data');
    }
  }
}
