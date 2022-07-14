

import 'call_api.dart';

class ip {
  //for transactions in blockchain page
  callApi network = callApi(
      Uri.https('devote-explorer-backend.herokuapp.com', 'transactions'));

  //for blocks in blockchain page
  callApi block =
  callApi(Uri.https('devote-explorer-backend.herokuapp.com', 'blocks'));

  //for transactionlist showmore fn
  String authority = 'devote-explorer-backend.herokuapp.com';
  String unencodedpath = 'transactions';

  //for blocklist showmore fn
  String authority_block = 'devote-explorer-backend.herokuapp.com';
  String unencodedpath_block = 'blocks';

  //for TransactionDetails show block height
  String authority_blockHe = 'devote-explorer-backend.herokuapp.com';
  String unencodedpath_blockhe = 'blocks/block-height/';

  //for BlockDetails show block height
  String authority_Height = 'devote-explorer-backend.herokuapp.com';
  String unencodedpath_height = 'transactions/tx-block-height/';

  //for results candidates and map
  callApi candidates = callApi(
      Uri.https('devote-explorer-backend.herokuapp.com', 'candidates'));
  callApi model = callApi(
      Uri.https('devote-explorer-backend.herokuapp.com', 'governorates'));
}
