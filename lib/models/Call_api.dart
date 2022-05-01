import 'dart:convert';

import 'package:devote/models/transaction.dart';
import 'package:http/http.dart' as http;

class callApi{
  final dynamic url;

  callApi(this.url);

  Future<Transaction> getTransaction() async {
    http.Response transactionFuture = await http.get(url);
    if (transactionFuture.statusCode == 200) {
      final jsonResponse = json.decode(transactionFuture.body);
      Transaction transacion = new Transaction.fromJson(jsonResponse);
      print(transactionFuture.body);
      print(transacion);
    ;
      return Transaction.fromJson(json.decode(transactionFuture.body));
    } else {
      throw Exception('Can not load data');
    }
  }
}
// var url =
//     Uri.https('devote-explorer-backend.herokuapp.com',
//         'transactions');