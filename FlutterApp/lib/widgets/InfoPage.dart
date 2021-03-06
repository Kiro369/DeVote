import '/models/people.dart';
import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart' show kIsWeb;

class InfoPage extends StatelessWidget {
  final List<People> motrsh7e;
  final int index;

   InfoPage({required this.motrsh7e, required this.index});
  static bool isLargeScreen(BuildContext context) {
    return MediaQuery.of(context).size.width > 1200;
  }
  static bool isSmallScreen(BuildContext context) {
    return MediaQuery.of(context).size.width < 1000;
  }

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
                      child: SizedBox(
                        width: isSmallScreen(context)?MediaQuery.of(context).size.width / 3.5:MediaQuery.of(context).size.width / 9,
                        height:isSmallScreen(context)?MediaQuery.of(context).size.width / 4: MediaQuery.of(context).size.width / 9,
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
                      style: const TextStyle(

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
                      style: const TextStyle(
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
                  SizedBox(
                    width: MediaQuery.of(context).size.width / 2.5,
                    height: MediaQuery.of(context).size.width / 3,
                    child: CircleAvatar(
                        backgroundColor: Colors.white,
                        backgroundImage:
                            AssetImage(motrsh7e[index].image.toString())),
                  ),
                  Padding(
                    padding: const EdgeInsets.all(8.0),
                    child: SizedBox(
                      width: MediaQuery.of(context).size.width / 2,
                      child: Column(
                        children: [
                          ListTile(
                            title: Text(
                              motrsh7e[index].name.toString(),
                              textAlign: TextAlign.right,
                            ),
                            subtitle: const Text(
                              'الاسم',
                              textAlign: TextAlign.right,
                            ),
                          ),
                          ListTile(
                            title: Text(
                              motrsh7e[index].nickname.toString(),
                              textAlign: TextAlign.right,
                            ),
                            subtitle: const Text(
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
            style: const TextStyle(
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
            color: kIsWeb?Colors.white: const Color(0xff26375f),
            size: 30,
          ),
        ),
        Padding(
          padding: const EdgeInsets.all(10.0),
          child: ListTile(
            title: Text(
              motrsh7e[index].info.toString(),
              textAlign: TextAlign.right,
              textDirection: TextDirection.rtl,
              style: const TextStyle(
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
