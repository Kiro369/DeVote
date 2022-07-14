

import 'call_api.dart';

class ip {
  static String endpoint='devote-explorer-backend.herokuapp.com';
  //for transactions in blockchain page
  callApi network = callApi(
      Uri.https(endpoint, 'transactions'));

  //for blocks in blockchain page
  callApi block =
  callApi(Uri.https(endpoint, 'blocks'));

  //for transactionlist showmore fn
  String authority = endpoint;
  String unencodedpath = 'transactions';

  //for blocklist showmore fn
  String authority_block = endpoint;
  String unencodedpath_block = 'blocks';

  //for TransactionDetails show block height
  String authority_blockHe = endpoint;
  String unencodedpath_blockhe = 'blocks/block-height/';

  //for BlockDetails show block height
  String authority_Height = endpoint;
  String unencodedpath_height = 'transactions/tx-block-height/';

  //for results candidates and map
  callApi candidates = callApi(
      Uri.https(endpoint, 'candidates'));
  callApi model = callApi(
      Uri.https(endpoint, 'governorates'));
}
