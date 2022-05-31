import 'dart:convert';

import 'package:devote/models/people.dart';

import '/models/transaction.dart';
import 'package:http/http.dart' as http;

import 'block.dart';
import 'mapModel.dart';

class CallApi {
  final dynamic url;

  CallApi(this.url);

  Future<List<Result>> getTransaction() async {
    http.Response transactionFuture = await http.get(url);
    if (transactionFuture.statusCode == 200) {
      final jsonResponse = json.decode(transactionFuture.body);
      Transaction transacion = Transaction.fromJson(jsonResponse);
      //Transaction.fromJson(json.decode(transactionFuture.body))
      return transacion.transaction;
    } else {
      throw Exception('Can not load data');
    }
  }

  Future<List<Result>> getTransactionByBlock() async {
    http.Response transactionFuture = await http.get(url);
    if (transactionFuture.statusCode == 200) {
      final jsonResponse = json.decode(transactionFuture.body);
      TransactionBlock transacion = TransactionBlock.fromJson(jsonResponse);
      //Transaction.fromJson(json.decode(transactionFuture.body))
      return transacion.transactions;
    } else {
      throw Exception('Can not load data');
    }
  }

  Future<List<Model>> getMapResults() async {
    http.Response transactionFuture = await http.get(url);
    if (transactionFuture.statusCode == 200) {
      final jsonResponse = json.decode(transactionFuture.body);
      MapModel transacion = MapModel.fromJson(jsonResponse);
      //Transaction.fromJson(json.decode(transactionFuture.body))
      return transacion.model;
    } else {
      throw Exception('Can not load data');
    }
  }

  Future<List<ResultsChart>> getPieResult() async {
    http.Response transactionFuture = await http.get(url);
    if (transactionFuture.statusCode == 200) {
      final jsonResponse = json.decode(transactionFuture.body);
      PieResult pieResult = PieResult.fromJson(jsonResponse);
      //Transaction.fromJson(json.decode(transactionFuture.body))
      return pieResult.pie;
    } else {
      throw Exception('Can not load data');
    }
  }

  Future<Transaction> transactionPagination() async {
    http.Response transactionFuture = await http.get(url);
    if (transactionFuture.statusCode == 200) {
      final jsonResponse = json.decode(transactionFuture.body);
      Transaction transacion = Transaction.fromJson(jsonResponse);
      //Transaction.fromJson(json.decode(transactionFuture.body))
      return transacion;
    } else {
      throw Exception('Can not load data');
    }
  }

  Future<List<Results>> getBlocks() async {
    http.Response transactionFuture = await http.get(url);
    if (transactionFuture.statusCode == 200) {
      final jsonResponse = json.decode(transactionFuture.body);
      Block transacion = Block.fromJson(jsonResponse);

      return transacion.blocks;
    } else {
      throw Exception('Can not load data');
    }
  }

  Future<Block> pagination() async {
    http.Response transactionFuture = await http.get(url);
    if (transactionFuture.statusCode == 200) {
      final jsonResponse = json.decode(transactionFuture.body);
      Block transacion = Block.fromJson(jsonResponse);
      return transacion;
    } else {
      throw Exception('Can not load data');
    }
  }

  Future<BlockHeight> getbyBlockHeight() async {
    http.Response transactionFuture = await http.get(url);
    if (transactionFuture.statusCode == 200) {
      final jsonResponse = json.decode(transactionFuture.body);
      BlockHeight blockHeight = BlockHeight.fromJson(jsonResponse);
      return blockHeight;
    } else {
      throw Exception('Can not load data');
    }
  }
}
