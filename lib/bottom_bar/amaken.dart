import 'package:flutter/material.dart';
import 'package:animated_text_kit/animated_text_kit.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:location/location.dart';
class Amaken extends StatefulWidget {
  const Amaken({Key? key}) : super(key: key);

  @override
  _AmakenState createState() => _AmakenState();
}

class _AmakenState extends State<Amaken> {

  late LatLng location ;

  Future<void> _getlocation() async{
    final locdata= await Location.instance.getLocation();
  final  lat= locdata.latitude;
    final  long=locdata.longitude;
    setState(() {
      location = LatLng(lat!, long!);
      print(locdata);
    });
  }
  @override
  void initState() {
    super.initState();
    location= LatLng(30.033333, 31.233334);
    _getlocation();
  }
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        actions: [
          Padding(
            padding: const EdgeInsets.only(right: 10),
            child: Icon(Icons.check_box_outlined,color: Colors.white,size: 30,),
          )
        ],
        centerTitle: true,
        title: Text('اماكن الانتخابات',style: TextStyle(color: Colors.white),),
        backgroundColor: Color(0xff26375f),
        bottom: PreferredSize(
            preferredSize: Size.fromHeight(40.0),
          child: Container(
            width: MediaQuery.of(context).size.width,
            color: Color(0xfff1f2f4),
            height: 40.0,
            child: DefaultTextStyle(
              textAlign: TextAlign.right,
              style: const TextStyle(
                fontSize: 16.0,
                color: const Color(0xff000000),
              ),
              child: AnimatedTextKit(
               repeatForever: true,
                  animatedTexts:[
                    RotateAnimatedText('موعد الانتخابات الرئاسية 26-27 يوليو'),
                    RotateAnimatedText('تتواجد مكن الانتخابات في جميع انحاء الجمهورية'),
                    RotateAnimatedText('يرجي الذهاب للانتخابات ببطاقة الرقم القومي'),
                  ],
                  isRepeatingAnimation: true,
            ),
      ),
          ),
        ),
    ),
        body: ListView(
            children: [
              FlatButton(onPressed: _getlocation,child: Text('Current Location'),),
              location ==null? Center(child: Text('Wait to get your location !'))
              : Center(
                         child: Container(
                           height: MediaQuery.of(context).size.height/2,
                           child: GoogleMap(
                             myLocationButtonEnabled: true,
                              myLocationEnabled: true,
                   // onMapCreated: _onMapCreated,
                    initialCameraPosition: CameraPosition(
                       target: location,
                       zoom: 14,
                      ),
                    ),),)
            ],
    ),
    );
  }
}
