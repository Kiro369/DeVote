class Transaction{
  final dynamic pagination;
  final List<Result> transaction;

  Transaction(this.pagination, this.transaction);
  factory Transaction.fromJson(Map<String,dynamic> json){
    var list = json['result'] as List;
    List<Result> result = list.map((i) => Result.fromJson(i)).toList();
    return Transaction(json['pagination'], result);
  }
}
class Result {
  final String hash;
  final int dateTime;
  final String elector;
  final String elected;
  final int blockheight;

  Result(this.hash,this.dateTime,  this.elector, this.elected, this.blockheight);

  factory Result.fromJson(Map<String, dynamic> json) {
    return Result(
        json['Hash'],
        json['Date'],
        json['Elector'],
        json['Elected'],
        json['BlockHeight']);
  }
}
