class Block{
  final dynamic pagination;
  final List<Results> blocks;

  Block(this.pagination, this.blocks);
  factory Block.fromJson(Map<String,dynamic> json){
    var list = json['result'] as List;
    List<Results> result = list.map((i) => Results.fromJson(i)).toList();
    return Block(json['pagination'], result);
  }
  
}
class Results{
final  int blockHeight;
final int transactions;
final int time;
final String transactionTime;
final String miner;

  Results(this.blockHeight, this.transactions, this.time, this.transactionTime, this.miner);
  factory Results.fromJson(Map<String,dynamic> json){
    return Results(json['Height'], json['nTx'], json['Timestamp'], json['MerkleRoot'], json['Miner']);
  }
}