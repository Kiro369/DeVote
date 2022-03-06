import 'package:flutter/material.dart';
import 'package:animated_text_kit/animated_text_kit.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:location/location.dart';
import 'package:url_launcher/url_launcher.dart';
class Amaken extends StatefulWidget {
  const Amaken({Key? key}) : super(key: key);

  @override
  _AmakenState createState() => _AmakenState();
}

class _AmakenState extends State<Amaken> {

  late LatLng location ;
 // late BitmapDescriptor icon;

  final Set<Marker> markers = new Set(); //markers for google map
  static const LatLng showLocation = const LatLng(30.033333, 31.233334);
 var locations =[
    {'name':'Cairo','location':LatLng(30.033333, 31.233334)},
   {'name':'Sohag','location':LatLng(26.549999	,31.700001)},
   {'name':'6 october ','location':LatLng(29.952654,30.921919)},
   {'name':'Giza ','location':LatLng(	30.013056,	31.208853)},
   {'name':'Alex ','location':LatLng(31.205753	,29.924526)},
  ];
 /* getIcons() async {
    var icon = await BitmapDescriptor.fromAssetImage(
        ImageConfiguration(devicePixelRatio: 3.2),
        "assets/images/markeruser.png");
    setState(() {
      this.icon = icon;
    });
  }*/

  Set<Marker> getmarkers() { //markers to place on map
    for(var i=0;i<locations.length;i++){
      final title=locations[i]['name'];
      final loc=locations[i]['location'];
        markers.add(Marker(//add first marker
          markerId: MarkerId(showLocation.toString()),
          position:  loc as LatLng, //position of marker
          infoWindow: InfoWindow( //popup info
            title:title.toString(),
            // snippet: 'My Custom Subtitle',
          ),

          icon: BitmapDescriptor.defaultMarker, //Icon for Marker
        ));
    }
    return markers;
  }
  @override
  void initState() {
    super.initState();
    location= LatLng(30.033333, 31.233334);
    //getIcons();
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
        body: Column(
            children: [
           //   FlatButton(onPressed: _getlocation,child: Text('Current Location'),),
              location ==null? Center(child: Text('Wait to get your location !'))
              : Center(
                         child: Container(
                           height: MediaQuery.of(context).size.height*0.76,
                           child: GoogleMap(
                             markers: getmarkers(),
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
