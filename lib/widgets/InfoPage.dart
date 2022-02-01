import 'package:devote/models/people.dart';
import 'package:flutter/material.dart';
class InfoPage extends StatelessWidget {
  final List<people> motrsh7e;
  final int index;

  const InfoPage({ required this.motrsh7e, required this.index}) ;

  @override
  Widget build(BuildContext context) {
    return Center(child:ListView(
      children: [
        Row(
          children: [
            Container(
              width: MediaQuery.of(context).size.width/2.5,
              height:  MediaQuery.of(context).size.width/3,
              child: CircleAvatar(
                  backgroundImage:AssetImage(motrsh7e[index].image.toString())),
            ),
            Padding(
              padding: const EdgeInsets.all(8.0),
              child: Container(
                width: MediaQuery.of(context).size.width/2,
                child: Column(
                  children: [
                    ListTile(
                      title: Text(motrsh7e[index].name.toString(),textAlign: TextAlign.right,),
                      subtitle: Text('الاسم',textAlign: TextAlign.right,),
                    ),
                    ListTile(
                      title: Text(motrsh7e[index].nickname.toString(),textAlign: TextAlign.right,),
                      subtitle: Text('اسم الشهرة',textAlign: TextAlign.right,),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),

        ListTile(
          title: Text(motrsh7e[index].codestr.toString(),textAlign: TextAlign.right,),
          subtitle: Text('الرمز',textAlign: TextAlign.right,),
          trailing: Icon(motrsh7e[index].code?.icon,color: Colors.yellow[700],size: 30,),
        ),
        Padding(
          padding: const EdgeInsets.all(10.0),
          child: ListTile(
            title: Text(motrsh7e[index].info.toString(),textAlign: TextAlign.right,),
            subtitle: Text('معلومات عامة',textAlign: TextAlign.right,),
          ),
        ),
      ],
    )
    );
  }
}
