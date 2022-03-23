import 'package:devote/models/people.dart';
import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart' show kIsWeb;

class InfoPage extends StatelessWidget {
  final List<people> motrsh7e;
  final int index;

  const InfoPage({required this.motrsh7e, required this.index});

  @override
  Widget build(BuildContext context) {
    return Center(
        child: ListView(
      children: [
        kIsWeb
            ? Column(
                children: [
                  Center(
                    child: Padding(
                      padding: const EdgeInsets.all(8.0),
                      child: Container(
                        width: MediaQuery.of(context).size.width / 9,
                        height: MediaQuery.of(context).size.width / 9,
                        child: CircleAvatar(
                            backgroundColor: Colors.white,
                            backgroundImage:
                                AssetImage(motrsh7e[index].image.toString())),
                      ),
                    ),
                  ),
                  ListTile(
                    title: Text(
                      motrsh7e[index].name.toString(),
                      textAlign: TextAlign.right,
                      style: TextStyle(
                        color: kIsWeb?Colors.white:Colors.black,
                      ),
                    ),
                    subtitle: Text(
                      'الاسم',
                      textAlign: TextAlign.right,
                      style: TextStyle(
                        color: kIsWeb?Colors.grey[400]:Colors.black,
                      ),
                    ),
                  ),
                  ListTile(
                    title: Text(
                      motrsh7e[index].nickname.toString(),
                      textAlign: TextAlign.right,
                      style: TextStyle(
                        color: kIsWeb?Colors.white:Colors.black,
                      ),
                    ),
                    subtitle: Text(
                      'اسم الشهرة',
                      textAlign: TextAlign.right,
                      style: TextStyle(
                        color: kIsWeb?Colors.grey[400]:Colors.black,
                      ),
                    ),
                  ),
                ],
              )
            : Row(
                children: [
                  Container(
                    width: MediaQuery.of(context).size.width / 2.5,
                    height: MediaQuery.of(context).size.width / 3,
                    child: CircleAvatar(
                        backgroundColor: Colors.white,
                        backgroundImage:
                            AssetImage(motrsh7e[index].image.toString())),
                  ),
                  Padding(
                    padding: const EdgeInsets.all(8.0),
                    child: Container(
                      width: MediaQuery.of(context).size.width / 2,
                      child: Column(
                        children: [
                          ListTile(
                            title: Text(
                              motrsh7e[index].name.toString(),
                              textAlign: TextAlign.right,
                            ),
                            subtitle: Text(
                              'الاسم',
                              textAlign: TextAlign.right,
                            ),
                          ),
                          ListTile(
                            title: Text(
                              motrsh7e[index].nickname.toString(),
                              textAlign: TextAlign.right,
                            ),
                            subtitle: Text(
                              'اسم الشهرة',
                              textAlign: TextAlign.right,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
        ListTile(
          title: Text(
            motrsh7e[index].codestr.toString(),
            textAlign: TextAlign.right,
            style: TextStyle(
              color: kIsWeb?Colors.white:Colors.black,
            ),
          ),
          subtitle: Text(
            'الرمز',
            textAlign: TextAlign.right,
            style: TextStyle(
              color: kIsWeb?Colors.grey[400]:Colors.black,
            ),
          ),
          trailing: Icon(
            motrsh7e[index].code?.icon,
            color: kIsWeb?Colors.white: Color(0xff26375f),
            size: 30,
          ),
        ),
        Padding(
          padding: const EdgeInsets.all(10.0),
          child: ListTile(
            title: Text(
              motrsh7e[index].info.toString(),
              textAlign: TextAlign.right,
              style: TextStyle(
                color: kIsWeb?Colors.white:Colors.black,
              ),
            ),
            subtitle: Text(
              'معلومات عامة',
              textAlign: TextAlign.right,
              style: TextStyle(
                color: kIsWeb?Colors.grey[400]:Colors.black,
              ),
            ),
          ),
        ),
      ],
    ));
  }
}
