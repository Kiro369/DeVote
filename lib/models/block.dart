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
final String merkleRoot;
final String hash;
final String prevHash;
final String miner;

  Results(this.blockHeight, this.transactions, this.time, this.merkleRoot, this.miner, this.hash, this.prevHash);
  factory Results.fromJson(Map<String,dynamic> json){
    return Results(json['Height'], json['nTx'], json['Timestamp'], json['MerkleRoot'], json['Miner'],json['Hash'],json['PrevHash']);
  }
}
/*ListTile(
            title: Padding(
              padding: const EdgeInsets.only(top: 8.0),
              child: const Text('PrevHash:',style: TextStyle(color: Colors.black,fontWeight: FontWeight.bold),),
            ),
            subtitle: Padding(
              padding: const EdgeInsets.only(bottom:  8.0,top: 8.0),
              child: Text(widget.prevhash,style: const TextStyle(color: Colors.black),),
            ),
          ),
* */